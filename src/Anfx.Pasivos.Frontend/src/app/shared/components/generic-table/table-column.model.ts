export type SortDirection = 'asc' | 'desc';

export interface TableColumn {
  /** Propiedad del objeto, soporta dot-notation: 'address.city' */
  key: string;
  /** Texto del encabezado */
  header: string;
  /** Tipo de dato para formateo */
  type?: 'text' | 'date' | 'boolean' | 'number' | 'currency' | 'percent';
  /** Ocultar columna sin quitarla de la definición */
  visible?: boolean;
  /** Habilita ordenamiento server-side en esta columna */
  sortable?: boolean;
  /**
   * Ocultar la columna en pantallas menores al breakpoint indicado.
   * 'sm' < 576px | 'md' < 768px | 'lg' < 992px | 'xl' < 1200px
   */
  hideBelow?: 'sm' | 'md' | 'lg' | 'xl';
  /**
   * Ancho máximo del contenido de la celda (en px).
   * El texto largo se trunca con ellipsis y muestra el valor completo en tooltip.
   */
  maxWidth?: number;
}

export interface TableSortEvent {
  column: string;
  direction: SortDirection;
}

export interface TableAction {
  /** Identificador emitido al padre cuando se hace clic */
  id: string;
  /** Texto del tooltip */
  label: string;
  /** Clase de ícono FontAwesome, ej: 'fa-solid fa-edit' */
  icon: string;
  /** Clase CSS del botón Bootstrap, ej: 'btn-primary', 'btn-danger' */
  btnClass: string;
}

export interface TableActionEvent<T = any> {
  action: string;
  row: T;
}
