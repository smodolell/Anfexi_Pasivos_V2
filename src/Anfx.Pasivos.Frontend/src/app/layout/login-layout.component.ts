import { Component, OnInit, OnDestroy, inject, Renderer2, Inject } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { DOCUMENT } from '@angular/common';

@Component({
  selector: 'app-login-layout',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './login-layout.component.html',
  styleUrls: ['./login-layout.component.scss']
})
export class LoginLayoutComponent implements OnInit, OnDestroy {
  private renderer = inject(Renderer2);
  private document = inject(DOCUMENT);
  
  private cssLink: HTMLLinkElement | null = null;
  private jsScript: HTMLScriptElement | null = null;

  ngOnInit() {
    // Solo ejecutar en el navegador
    if (typeof window !== 'undefined') {
      this.loadExternalResources();
      this.addBodyClasses();
    }
  }

  ngOnDestroy() {
    // Solo ejecutar en el navegador
    if (typeof window !== 'undefined') {
      this.cleanupExternalResources();
      this.removeBodyClasses();
    }
  }

  private loadExternalResources() {
    
    this.cssLink = this.renderer.createElement('link');
    this.renderer.setAttribute(this.cssLink, 'rel', 'stylesheet');
    this.renderer.setAttribute(this.cssLink, 'href', 'assets/dist/css/pages/other-pages.css');
    this.renderer.appendChild(this.document.head, this.cssLink);

    // Cargar CSS específico del login-layout
    this.cssLink = this.renderer.createElement('link');
    this.renderer.setAttribute(this.cssLink, 'rel', 'stylesheet');
    this.renderer.setAttribute(this.cssLink, 'href', 'assets/dist/css/pages/login-register-lock.css');
    this.renderer.appendChild(this.document.head, this.cssLink);

    // Cargar JavaScript específico del login-layout
    this.jsScript = this.renderer.createElement('script');
    this.renderer.setAttribute(this.jsScript, 'src', 'assets/js/login-layout.js');
    this.renderer.setAttribute(this.jsScript, 'type', 'text/javascript');
    this.renderer.appendChild(this.document.body, this.jsScript);
  }

  private addBodyClasses() {
    // Agregar clases específicas del login-layout al body
    if (this.document.body) {
      this.renderer.addClass(this.document.body, 'skin-default');
      this.renderer.addClass(this.document.body, 'card-no-border');
    }
  }

  private removeBodyClasses() {
    // Remover clases específicas del login-layout del body
    if (this.document.body) {
      this.renderer.removeClass(this.document.body, 'skin-default');
      this.renderer.removeClass(this.document.body, 'card-no-border');
    }
  }

  private cleanupExternalResources() {
    // Limpiar CSS
    if (this.cssLink && this.cssLink.parentNode) {
      this.renderer.removeChild(this.document.head, this.cssLink);
    }

    // Limpiar JavaScript
    if (this.jsScript && this.jsScript.parentNode) {
      this.renderer.removeChild(this.document.body, this.jsScript);
    }

    // Limpiar el manager de JavaScript si existe
    if (typeof window !== 'undefined' && (window as any).loginLayoutManager) {
      (window as any).loginLayoutManager.destroy();
      delete (window as any).loginLayoutManager;
    }
  }
}
