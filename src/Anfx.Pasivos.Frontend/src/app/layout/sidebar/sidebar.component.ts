import {
  ChangeDetectionStrategy, Component, DestroyRef,
  OnInit, input, output, signal, inject
} from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterModule, Router, NavigationEnd } from '@angular/router';
import { filter } from 'rxjs';
import { MenuService } from '../../services/menu.service';
import { MenuItem } from 'src/types/menu.model';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [RouterModule],
  templateUrl: './sidebar.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class SidebarComponent implements OnInit {
  /** Rol del usuario autenticado — provisto por AdminLayoutComponent */
  readonly role       = input.required<string>();
  /** Indica si el sidebar está en modo mini (colapsado) */
  readonly isMini         = input<boolean>(true);
  /** Indica si el overlay móvil está visible (clase show-sidebar en body) */
  readonly isMobileNavOpen = input<boolean>(false);
  readonly navItemSelected = output<void>();
  readonly menuItems = signal<MenuItem[]>([]);
  readonly openId    = signal<string | null>(null);

  private readonly router      = inject(Router);
  private readonly menuService = inject(MenuService);
  private readonly destroyRef  = inject(DestroyRef);

  ngOnInit(): void {
    this.menuService.getMenuForRole(this.role())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(items => {
        this.menuItems.set(items);
        // Una vez cargados los items, sincroniza con la ruta actual
        this.openId.set(this.getIdFromUrl(this.router.url));
      });

    // Sincroniza el submenú abierto con la ruta activa en cada navegación
    this.router.events
      .pipe(
        filter((e): e is NavigationEnd => e instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(e => this.openId.set(this.getIdFromUrl(e.urlAfterRedirects)));
  }

  toggle(id: string): void {
    this.openId.set(this.openId() === id ? null : id);
  }

  isOpen(id: string): boolean {
    return this.openId() === id;
  }

  private getIdFromUrl(url: string): string | null {
    const item = this.menuItems().find(m =>
      m.children?.some(child => url.startsWith(child.route))
    );
    return item?.id ?? null;
  }
}
