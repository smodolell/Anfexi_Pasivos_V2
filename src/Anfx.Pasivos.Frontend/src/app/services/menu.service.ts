import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, shareReplay, map } from 'rxjs';
import { MenuItem, MenuChild } from '../../types/menu.model';
import { MENU_API_URL } from '../api.config';

@Injectable({ providedIn: 'root' })
export class MenuService {
  private readonly http       = inject(HttpClient);
  private readonly menuApiUrl = inject(MENU_API_URL);

  /**
   * Caché compartida del JSON completo — se carga una sola vez por sesión.
   * shareReplay(1) retiene el último valor y lo reutiliza en nuevas suscripciones.
   */
  private readonly allItems$ = this.http
    .get<MenuItem[]>(this.menuApiUrl)
    .pipe(shareReplay(1));

  /**
   * Devuelve los items de menú visibles para el rol indicado.
   * Hoy apunta a assets/menu.json; cuando el backend esté listo,
   * solo hay que cambiar MENU_API_URL en environment.ts → sin tocar este servicio.
   */
  getMenuForRole(role: string): Observable<MenuItem[]> {
  return this.allItems$.pipe(
    map(items =>
      items
        .filter(item => item.roles?.includes(role) ?? true)
        .map(item => ({
          ...item,
          children: (item.children ?? [])
            .filter((child: MenuChild) => child.roles?.includes(role) ?? true)
        }))
        .filter(item => item.children.length > 0)
    )
  );
}
}
