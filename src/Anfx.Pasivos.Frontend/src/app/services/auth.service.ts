import { Injectable, Inject } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import { Router } from '@angular/router';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { API_AUTH_URL } from '../api.config';

export interface User {
  id: number;
  nombreCompleto: string;
  email: string;
  usuarioNombre: string;
  role: 'Webmaster' | 'Admin' | 'User';
}

export interface LoginCredentials {
  email: string;
  contrasenia: string;
  recuerdame?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<User | null>(null);
  public currentUser$ = this.currentUserSubject.asObservable();
  
  private isAuthenticatedSubject = new BehaviorSubject<boolean>(false);
  public isAuthenticated$ = this.isAuthenticatedSubject.asObservable();

  constructor(
    private router: Router,
    private http: HttpClient,
    @Inject(API_AUTH_URL) private readonly apiBaseUrl: string
  ) {
    // Inicializar con valores por defecto
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
    
    // Verificar autenticación almacenada
    this.checkStoredAuth();
  }

  // Verificar si hay autenticación almacenada
  private checkStoredAuth(): void {
    // Verificar si estamos en el navegador
    if (typeof window !== 'undefined' && typeof localStorage !== 'undefined') {
      console.log('Verificando autenticación almacenada...');
      
      // Solo verificar localStorage (siempre guardamos ahí)
      const storedUser = localStorage.getItem('currentUser');
      const storedToken = localStorage.getItem('authToken');
      
      console.log('Datos encontrados en localStorage:', { 
        hasUser: !!storedUser, 
        hasToken: !!storedToken
      });
      
      if (storedUser && storedToken) {
        try {
          const user = JSON.parse(storedUser);
          
          // Verificar si el token ha expirado
          if (this.isTokenExpired(storedToken)) {
            console.log('Token expirado, limpiando autenticación');
            this.clearAuth();
            return;
          }
          
          this.currentUserSubject.next(user);
          this.isAuthenticatedSubject.next(true);
          
          // Configurar listener para sincronización entre pestañas
          this.setupStorageListener();
          
          console.log(`✅ Autenticación restaurada desde localStorage para usuario:`, user.usuarioNombre);
        } catch (error) {
          console.error('Error al parsear datos de usuario:', error);
          this.clearAuth();
        }
      } else {
        console.log('No se encontraron datos de autenticación en localStorage');
        // Configurar listener de todas formas para futuras sincronizaciones
        this.setupStorageListener();
      }
    }
  }

  // Login del usuario
  async login(credentials: LoginCredentials): Promise<{ success: boolean; message: string; user?: User, status?: string, errors?: string[] }> {
    try {
      const url = `${this.apiBaseUrl}/auth/login`;
      type ApiResponse = { token: string; user: User; message?: string, errors?: string[] };
      const res = await firstValueFrom(this.http.post<ApiResponse>(url, credentials));

      const user = res.user;
      const token = res.token;

      // Almacenar datos de autenticación solo en el navegador
      if (typeof window !== 'undefined') {
        // Limpiar cualquier autenticación previa
        this.clearAuth();
        
        // Siempre guardar en localStorage para persistencia entre pestañas
        localStorage.setItem('currentUser', JSON.stringify(user));
        localStorage.setItem('authToken', token);
        console.log('Sesión guardada en localStorage (persistente)');
        
        // Configurar listener para sincronización entre pestañas
        this.setupStorageListener();
      }

      // Actualizar estado
      this.currentUserSubject.next(user);
      this.isAuthenticatedSubject.next(true);

      return { success: true, message: res.message || 'Login exitoso', user };
    } catch (err) {
      const error = err as HttpErrorResponse;
      const message = error.name == 'HttpErrorResponse' ? 'No se pudo conectar con el servidor.' : error.error?.message || error.statusText || 'Error en el servidor';
      return { success: false, message, errors: error.error.errors || [] };
    }
  }

  // Logout del usuario
  logout(): void {
    this.notifyOtherTabs();
    this.clearAuth();
    this.router.navigate(['/auth/login']);
  }

  // Limpiar datos de autenticación
  private clearAuth(): void {
    // Limpiar solo si estamos en el navegador
    if (typeof window !== 'undefined') {
      localStorage.removeItem('currentUser');
      localStorage.removeItem('authToken');
      console.log('Datos de autenticación limpiados de localStorage');
    }
    
    this.currentUserSubject.next(null);
    this.isAuthenticatedSubject.next(false);
  }

  // Obtener usuario actual
  getCurrentUser(): User | null {
    return this.currentUserSubject.value;
  }

  // Verificar si está autenticado
  isAuthenticated(): boolean {
    return this.isAuthenticatedSubject.value;
  }

  // Forzar verificación de autenticación (útil para guards)
  async checkAuthentication(): Promise<boolean> {
    if (typeof window !== 'undefined') {
      const token = this.getAuthToken();
      if (token && !this.isTokenExpired(token)) {
        // Si hay token válido pero no está autenticado, restaurar
        if (!this.isAuthenticated()) {
          this.checkStoredAuth();
        }
        return this.isAuthenticated();
      } else {
        // Token inválido o expirado
        this.clearAuth();
        return false;
      }
    }
    return false;
  }

  // Verificar si es admin
  isAdmin(): boolean {
    const user = this.getCurrentUser();
    return user?.role === 'Admin' || user?.role === 'Webmaster';
  }

  // Verificar si tiene rol específico
  hasRole(role: string): boolean {
    const user = this.getCurrentUser();
    return user?.role === role;
  }

  // Obtener token de autenticación
  getAuthToken(): string | null {
    // Solo acceder a storage si estamos en el navegador
    if (typeof window !== 'undefined') {
      return localStorage.getItem('authToken');
    }
    return null;
  }

  // Verificar si el token ha expirado
  isTokenExpired(token?: string): boolean {
    const tokenToCheck = token || this.getAuthToken();
    if (!tokenToCheck) return true;
    
    try {
      // Decodificar JWT (parte del payload)
      const parts = tokenToCheck.split('.');
      if (parts.length !== 3) return true;
      
      const payload = JSON.parse(atob(parts[1]));
      const currentTime = Math.floor(Date.now() / 1000);
      
      // Verificar si el token ha expirado
      if (payload.exp && payload.exp < currentTime) {
        return true;
      }
      
      return false;
    } catch (error) {
      console.error('Error al verificar expiración del token:', error);
      return true;
    }
  }

  // Renovar autenticación
  refreshAuth(): void {
    if (this.isAuthenticated() && !this.isTokenExpired()) {
      // En un caso real, harías una llamada para renovar el token
      console.log('Autenticación renovada');
    } else {
      this.clearAuth();
    }
  }

  // Configurar listener para sincronización entre pestañas
  private setupStorageListener(): void {
    if (typeof window !== 'undefined') {
      // Escuchar cambios en el storage para sincronizar entre pestañas
      window.addEventListener('storage', (event) => {
        if (event.key === 'currentUser' || event.key === 'authToken') {
          if (event.newValue === null) {
            // Token o usuario eliminado en otra pestaña
            this.clearAuth();
            this.router.navigate(['/auth/login']);
          } else if (event.key === 'currentUser' && event.newValue) {
            // Usuario actualizado en otra pestaña
            try {
              const user = JSON.parse(event.newValue);
              this.currentUserSubject.next(user);
              this.isAuthenticatedSubject.next(true);
            } catch (error) {
              console.error('Error al sincronizar usuario:', error);
            }
          }
        }
      });

      // Escuchar mensajes de otras pestañas
      window.addEventListener('message', (event) => {
        if (event.data && event.data.type === 'AUTH_LOGOUT') {
          this.clearAuth();
          this.router.navigate(['/auth/login']);
        }
      });
    }
  }

  // Notificar a otras pestañas sobre logout
  private notifyOtherTabs(): void {
    if (typeof window !== 'undefined') {
      // Enviar mensaje a otras pestañas
      window.localStorage.setItem('auth_logout', Date.now().toString());
      window.localStorage.removeItem('auth_logout');
    }
  }
}
