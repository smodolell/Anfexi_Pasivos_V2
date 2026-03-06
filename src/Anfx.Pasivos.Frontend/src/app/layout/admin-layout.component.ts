import { Component, OnInit, OnDestroy, inject, Renderer2, Inject, ChangeDetectorRef } from '@angular/core';
import { RouterOutlet, Router, RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService, User } from '../services/auth.service';
import { DOCUMENT } from '@angular/common';
import { LayoutService } from '../services/layout.service';
import { ViewEncapsulation } from '@angular/core';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterModule],
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AdminLayoutComponent implements OnInit, OnDestroy {
  isSidebarCollapsed = false;
  currentUser: User | null = null;
  currentTitle: string = 'Title';
  showLogoutModal = false;

  private renderer = inject(Renderer2);
  private document = inject(DOCUMENT);
  private cdr = inject(ChangeDetectorRef);
  
  private cssLink: HTMLLinkElement | null = null;
  private jsScript: HTMLScriptElement | null = null;


  constructor(
    private authService: AuthService,
    private router: Router,
    private layoutService: LayoutService
  ) {}

  ngOnInit() {
    // Suscribirse al usuario actual
    this.authService.currentUser$.subscribe(user => {
      this.currentUser = user;
    });

    // Suscribirse al título del layout
    this.layoutService.title$.subscribe(title => {
      this.currentTitle = title;
      // Forzar la detección de cambios
      this.cdr.detectChanges();
    });

    // Solo ejecutar en el navegador
    if (typeof window !== 'undefined') {
      this.loadExternalResources();
      this.addBodyClasses();
    }
  }

  ngOnDestroy() {
    // Cleanup si es necesario
  }

  logout() {
    this.showLogoutModal = true;
    // Forzar detección de cambios para asegurar que el modal se renderice
    this.cdr.detectChanges();
  }

  confirmLogout() {
    this.authService.logout();
    this.showLogoutModal = false;
  }

  cancelLogout() {
    this.showLogoutModal = false;
  }

  onModalKeydown(event: KeyboardEvent) {
    // Manejar tecla Escape para cerrar el modal
    if (event.key === 'Escape') {
      this.cancelLogout();
    }
  }

  // Verificar si el usuario tiene un rol específico
  hasRole(role: string): boolean {
    return this.authService.hasRole(role);
  }

  private addBodyClasses() {
    // Agregar clases específicas del login-layout al body
    if (this.document.body) {
      this.renderer.addClass(this.document.body, 'skin-blue');
      this.renderer.addClass(this.document.body, 'fixed-layout');
      this.renderer.addClass(this.document.body, 'mini-sidebar');
    }
  }

  private loadExternalResources() {

    // Cargar JavaScript específico del admin-layout
    this.jsScript = this.renderer.createElement('script');
    this.renderer.setAttribute(this.jsScript, 'src', 'assets/dist/js/perfect-scrollbar.jquery.min.js');
    this.renderer.setAttribute(this.jsScript, 'type', 'text/javascript');
    this.renderer.appendChild(this.document.body, this.jsScript);

    this.jsScript = this.renderer.createElement('script');
    this.renderer.setAttribute(this.jsScript, 'src', 'assets/dist/js/waves.js');
    this.renderer.setAttribute(this.jsScript, 'type', 'text/javascript');
    this.renderer.appendChild(this.document.body, this.jsScript);

    this.jsScript = this.renderer.createElement('script');
    this.renderer.setAttribute(this.jsScript, 'src', 'assets/dist/js/sidebarmenu.js');
    this.renderer.setAttribute(this.jsScript, 'type', 'text/javascript');
    this.renderer.appendChild(this.document.body, this.jsScript);

    this.jsScript = this.renderer.createElement('script');
    this.renderer.setAttribute(this.jsScript, 'src', 'assets/dist/js/custom.min.js');
    this.renderer.setAttribute(this.jsScript, 'type', 'text/javascript');
    this.renderer.appendChild(this.document.body, this.jsScript);

  }  
  
}
