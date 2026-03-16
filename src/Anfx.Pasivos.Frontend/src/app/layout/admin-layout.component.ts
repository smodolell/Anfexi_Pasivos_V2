import {
  Component, OnInit, OnDestroy,
  inject, Renderer2, ChangeDetectorRef, ViewEncapsulation,
} from '@angular/core';
import { RouterOutlet, Router, RouterModule, NavigationEnd, ActivatedRoute } from '@angular/router';
import { CommonModule, DOCUMENT } from '@angular/common';
import { Subject, switchMap, takeUntil, filter } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { LayoutService } from '../services/layout.service';
import { MenuService } from '../services/menu.service';
import { MenuItem } from '../shared/models/menu-item.model';
import { TopbarComponent } from '../shared/components/topbar/topbar.component';
import { SidebarNavComponent } from '../shared/components/sidebar-nav/sidebar-nav.component';
import { FooterComponent } from '../shared/components/footer/footer.component';

@Component({
  selector: 'app-admin-layout',
  standalone: true,
  imports: [RouterOutlet, CommonModule, RouterModule, TopbarComponent, SidebarNavComponent, FooterComponent],
  templateUrl: './admin-layout.component.html',
  styleUrls: ['./admin-layout.component.scss'],
  encapsulation: ViewEncapsulation.None,
})
export class AdminLayoutComponent implements OnInit, OnDestroy {
  currentTitle = '';
  menuItems: MenuItem[] = [];

  private readonly renderer      = inject(Renderer2);
  private readonly document      = inject(DOCUMENT);
  private readonly cdr           = inject(ChangeDetectorRef);
  private readonly destroy$      = new Subject<void>();
  private readonly authService   = inject(AuthService);
  private readonly router        = inject(Router);
  private readonly activatedRoute = inject(ActivatedRoute);
  private readonly layoutService = inject(LayoutService);
  private readonly menuService   = inject(MenuService);

  ngOnInit(): void {
    // Menú reactivo: se recarga cada vez que cambia el usuario (ej: re-login con otro rol)
    this.authService.currentUser$.pipe(
      switchMap(user => this.menuService.getMenuForUser(user)),
      takeUntil(this.destroy$),
    ).subscribe(items => {
      this.menuItems = items;
      this.cdr.detectChanges();
    });

    // Título de página
    this.layoutService.title$.pipe(takeUntil(this.destroy$)).subscribe(title => {
      this.currentTitle = title;
      this.cdr.detectChanges();
    });

    // Auto-título desde route data
    this.router.events.pipe(
      filter(e => e instanceof NavigationEnd),
      takeUntil(this.destroy$),
    ).subscribe(() => {
      const title = this.extractTitle(this.activatedRoute);
      if (title) this.layoutService.setTitle(title);
    });

    if (globalThis.window !== undefined) {
      this.addBodyClasses();
      this.loadScriptsSequentially([
        'assets/dist/js/perfect-scrollbar.jquery.min.js',
        'assets/dist/js/waves.js',
        'assets/dist/js/custom.min.js',
      ]);
    }
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private extractTitle(route: ActivatedRoute): string | null {
    let current = route;
    while (current.firstChild) current = current.firstChild;
    return current.snapshot.data['title'] ?? null;
  }

  private addBodyClasses(): void {
    const body = this.document.body;
    if (body) {
      this.renderer.addClass(body, 'skin-blue');
      this.renderer.addClass(body, 'fixed-layout');
      this.renderer.addClass(body, 'mini-sidebar');
    }
  }

  private loadScriptsSequentially(srcs: string[], index = 0): void {
    if (index >= srcs.length) return;
    const script = this.renderer.createElement('script');
    this.renderer.setAttribute(script, 'src', srcs[index]);
    this.renderer.setAttribute(script, 'type', 'text/javascript');
    script.onload = () => this.loadScriptsSequentially(srcs, index + 1);
    this.renderer.appendChild(this.document.body, script);
  }
}
