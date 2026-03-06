import { Injectable } from '@angular/core';
import { CanActivate, Router, UrlTree } from '@angular/router';
import { Observable, of } from 'rxjs';
import { AuthService } from '../services/auth.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  
  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean | UrlTree> {
    // Verificar autenticación de forma síncrona primero
    const isCurrentlyAuthenticated = this.authService.isAuthenticated();
    
    if (isCurrentlyAuthenticated) {
      console.log('AuthGuard - Usuario ya autenticado');
      return of(true);
    }

    // Verificar si hay token en storage
    const token = this.authService.getAuthToken();
    if (!token) {
      console.log('AuthGuard - No hay token, redirigiendo a login');
      return of(this.router.createUrlTree(['/auth/login']));
    }

    console.log('AuthGuard - Token encontrado, verificando autenticación...');

    // Forzar verificación de autenticación
    return new Observable(observer => {
      this.authService.checkAuthentication().then(isAuthenticated => {
        console.log('AuthGuard - Verificación completada:', isAuthenticated);
        if (isAuthenticated) {
          observer.next(true);
        } else {
          observer.next(this.router.createUrlTree(['/auth/login']));
        }
        observer.complete();
      }).catch(error => {
        console.error('AuthGuard - Error en verificación:', error);
        observer.next(this.router.createUrlTree(['/auth/login']));
        observer.complete();
      });
    });
  }
}
