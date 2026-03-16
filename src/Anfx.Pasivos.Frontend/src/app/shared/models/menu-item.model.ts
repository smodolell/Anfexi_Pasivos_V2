/**
 * Modelo de ítem de menú.
 * Diseñado para soportar tanto definición estática como carga desde API/DB.
 */
export interface MenuItem {
  /** Identificador único del ítem (usado como key de tracking y accordion) */
  id: string;

  /** Texto visible en el menú */
  label: string;

  /** Clase FontAwesome, ej: 'fa-solid fa-chart-line' */
  icon: string;

  /** Ruta Angular para ítems hoja. Omitir en grupos con children. */
  route?: string;

  /**
   * Roles que pueden ver este ítem.
   * undefined o arreglo vacío = visible para todos los roles.
   * Ej: ['Admin', 'Webmaster']
   */
  roles?: string[];

  /** Ítems hijos (submenu). Si está presente, el ítem actúa como grupo colapsable. */
  children?: MenuItem[];

  /** Orden de renderizado. Menor número = primero. Útil cuando viene de DB. */
  order?: number;

  /** Deshabilita el ítem sin ocultarlo */
  disabled?: boolean;
}
