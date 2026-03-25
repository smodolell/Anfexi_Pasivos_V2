// import {
//   Component, OnInit, inject,
//   signal, ChangeDetectionStrategy, DestroyRef, output, input,
// } from '@angular/core';
// import { CommonModule } from '@angular/common';
// import { RouterModule, Router, NavigationEnd } from '@angular/router';
// import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
// import { filter, merge, of } from 'rxjs';
// import { MenuItem } from 'src/types/menu.model';
// @Component({
//   selector: 'app-sidebar-nav',
//   standalone: true,
//   imports: [CommonModule, RouterModule],
//   templateUrl: './sidebar-nav.component.html',
//   styleUrl: './sidebar-nav.component.scss',
//   changeDetection: ChangeDetectionStrategy.OnPush,
// })
// export class SidebarNavComponent implements OnInit {
//   /** Ítems de menú ya filtrados por rol. Provistos por el layout padre. */
//   items           = input<MenuItem[]>([]);
//   isMini          = input<boolean>(true);
//   isMobileNavOpen = input<boolean>(false);

//   /** Emite cuando el usuario hace clic en un enlace hijo — el layout padre cierra el overlay mobile */
//   navItemSelected = output<void>();

//   private readonly router     = inject(Router);
//   private readonly destroyRef = inject(DestroyRef);

//   /** Id del grupo actualmente expandido (accordion) */
//   readonly activeGroup = signal<string | null>(null);

//   ngOnInit(): void {
//     // Auto-expande el grupo que contiene la ruta activa,
//     // tanto al cargar como en cada navegación posterior.
//     merge(of(null), this.router.events.pipe(filter(e => e instanceof NavigationEnd)))
//       .pipe(takeUntilDestroyed(this.destroyRef))
//       .subscribe(() => this.syncActiveGroup());
//   }

//   toggleGroup(id: string): void {
//     this.activeGroup.update(v => (v === id ? null : id));
//   }

//   onNavItemSelected(): void {
//     this.navItemSelected.emit();
//   }

//   /** True si algún hijo del grupo coincide con la URL actual. */
//   isGroupActive(item: MenuItem): boolean {
//     const url = this.router.url;
//     return item.children?.some(c => c.route && url.startsWith(c.route)) ?? false;
//   }

//   isSubmenuOpen(item: MenuItem): boolean {
//     return (!this.isMini() || this.isMobileNavOpen()) && this.activeGroup() === item.id;
//   }

//   private syncActiveGroup(): void {
//     const url = this.router.url;
//     const active = this.items().find(item =>
//       item.children?.some(c => c.route && url.startsWith(c.route)),
//     );
//     if (active) this.activeGroup.set(active.id);
//   }
// }
