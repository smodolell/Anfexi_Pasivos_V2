import {
  Component, OnInit, OnDestroy,
  inject, Renderer2, ChangeDetectorRef, ViewEncapsulation
} from '@angular/core';
import { RouterOutlet, Router, RouterModule, NavigationEnd, ActivatedRoute } from '@angular/router';
import { CommonModule, DOCUMENT } from '@angular/common';
import { Subject, takeUntil, filter } from 'rxjs';
import { AuthService, User } from '../services/auth.service';
import { LayoutService } from '../services/layout.service';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterModule],
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class AdminLayoutComponent implements OnInit, OnDestroy {
  currentUser: User | null = null;
  currentTitle = 'Title';
  showLogoutModal = false;
  activeMenu: string | null = null;

  private readonly renderer = inject(Renderer2);
  private readonly document = inject(DOCUMENT);
  private readonly cdr = inject(ChangeDetectorRef);
  private readonly destroy$ = new Subject<void>();

  constructor(
    private readonly authService: AuthService,
    private readonly router: Router,
    private readonly activatedRoute: ActivatedRoute,
    private readonly layoutService: LayoutService
  ) {}

  ngOnInit() {
    this.authService.currentUser$.pipe(takeUntil(this.destroy$)).subscribe(user => {
      this.currentUser = user;
    });

    this.layoutService.title$.pipe(takeUntil(this.destroy$)).subscribe(title => {
      this.currentTitle = title;
      this.cdr.detectChanges();
    });

    // Auto-título desde route data en cada navegación
    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd),
      takeUntil(this.destroy$)
    ).subscribe(() => {
      const title = this.extractTitle(this.activatedRoute);
      if (title) this.layoutService.setTitle(title);
    });

    if (globalThis.window !== undefined) {
      this.loadExternalResources();
      this.addBodyClasses();
    }
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  logout() {
    this.showLogoutModal = true;
    this.cdr.detectChanges();
  }

  confirmLogout() {
    this.authService.logout();
    this.showLogoutModal = false;
  }

  cancelLogout() {
    this.showLogoutModal = false;
  }

  toggleMenu(menuName: string) {
    this.activeMenu = this.activeMenu === menuName ? null : menuName;
  }

  onModalKeydown(event: KeyboardEvent) {
    if (event.key === 'Escape') this.cancelLogout();
  }

  hasRole(role: string): boolean {
    return this.authService.hasRole(role);
  }

  /** Recorre el árbol de rutas hijo para encontrar el data.title más profundo. */
  private extractTitle(route: ActivatedRoute): string | null {
    let current = route;
    while (current.firstChild) current = current.firstChild;
    return current.snapshot.data['title'] ?? null;
  }

  private addBodyClasses() {
    if (this.document.body) {
      this.renderer.addClass(this.document.body, 'skin-blue');
      this.renderer.addClass(this.document.body, 'fixed-layout');
      this.renderer.addClass(this.document.body, 'mini-sidebar');
    }
  }

  private loadExternalResources() {
    const scripts = [
      'assets/dist/js/perfect-scrollbar.jquery.min.js',
      'assets/dist/js/waves.js',
      'assets/dist/js/custom.min.js',
    ];
    this.loadScriptsSequentially(scripts);
  }

  private loadScriptsSequentially(srcs: string[], index = 0) {
    if (index >= srcs.length) return;
    const script = this.renderer.createElement('script');
    this.renderer.setAttribute(script, 'src', srcs[index]);
    this.renderer.setAttribute(script, 'type', 'text/javascript');
    script.onload = () => this.loadScriptsSequentially(srcs, index + 1);
    this.renderer.appendChild(this.document.body, script);
  }
}
