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
  readonly menuItems  = signal<MenuItem[]>([]);
  readonly openIndex  = signal<number>(-1);

  private readonly router      = inject(Router);
  private readonly menuService = inject(MenuService);
  private readonly destroyRef  = inject(DestroyRef);

  ngOnInit(): void {
    this.menuService.getMenuForRole(this.role())
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(items => this.menuItems.set(items));

    // Sincroniza el submenú abierto con la ruta activa
    this.openIndex.set(this.getIndexFromUrl(this.router.url));
    this.router.events
      .pipe(
        filter((e): e is NavigationEnd => e instanceof NavigationEnd),
        takeUntilDestroyed(this.destroyRef)
      )
      .subscribe(e => this.openIndex.set(this.getIndexFromUrl(e.urlAfterRedirects)));
  }

  toggle(index: number): void {
    this.openIndex.set(this.openIndex() === index ? -1 : index);
  }

  isOpen(index: number): boolean {
    return this.openIndex() === index;
  }

  private getIndexFromUrl(url: string): number {
    const items = this.menuItems();
    const found = items.findIndex(item => url.startsWith(item.routePrefix));
    return found;
  }
}
