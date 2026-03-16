import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, shareReplay } from 'rxjs';
import { MenuItem } from '../shared/models/menu-item.model';
import { User } from './auth.service';

/**
 * MenuService — Fuente única de verdad para la navegación lateral.
 *
 * Flujo actual : carga assets/menu.json (mock de API), filtra por rol en cliente.
 * Flujo futuro : cambiar MENU_URL a '/api/menu' cuando el backend esté listo.
 *               Si el servidor devuelve ítems ya filtrados por usuario,
 *               eliminar filterByRole() y simplificar getMenuForUser().
 */
@Injectable({ providedIn: 'root' })
export class MenuService {
  private readonly http = inject(HttpClient);

  // ── Swap futuro: cambiar esta URL a '/api/menu' ─────────────
  private readonly MENU_URL = 'assets/menu.json';

  // Cache: el JSON se descarga una sola vez por sesión
  private readonly allItems$ = this.http.get<MenuItem[]>(this.MENU_URL).pipe(
    shareReplay(1),
  );

  // ── API pública ──────────────────────────────────────────────

  /**
   * Retorna los ítems de menú visibles para el usuario dado.
   * El filtrado es en cliente; cuando el backend filtre por rol,
   * solo eliminar el map() y retornar allItems$ directamente.
   */
  getMenuForUser(user: User | null): Observable<MenuItem[]> {
    return this.allItems$.pipe(
      map(items => this.filterAndSort(items, user?.role)),
    );
  }

  // ── Privados ─────────────────────────────────────────────────

  private filterAndSort(items: MenuItem[], role?: string): MenuItem[] {
    return items
      .filter(item => this.canAccess(item, role))
      .sort((a, b) => (a.order ?? 99) - (b.order ?? 99))
      .map(item => ({
        ...item,
        children: item.children
          ? this.filterAndSort(item.children, role)
          : undefined,
      }))
      .filter(item => !item.children || item.children.length > 0);
  }

  private canAccess(item: MenuItem, role?: string): boolean {
    if (!item.roles || item.roles.length === 0) return true;
    return !!role && item.roles.includes(role);
  }
}
