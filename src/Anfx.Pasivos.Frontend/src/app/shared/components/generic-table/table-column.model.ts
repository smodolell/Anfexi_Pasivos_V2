export type SortDirection = 'asc' | 'desc';

export interface TableColumn {
  /** Propiedad del objeto — soporta dot-notation: 'address.city' */
  key: string;
  /** Texto del encabezado */
  header: string;
  /** Tipo de dato para formateo automático */
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
   * Ancho máximo de la celda en px.
   * El texto largo se trunca con ellipsis y el valor completo aparece como tooltip.
   */
  maxWidth?: number;
}

export interface TableSortEvent {
  column: string;
  direction: SortDirection;
}

export interface TableAction {
  /** Identificador emitido al padre en el evento actionCalled */
  id: string;
  /** Texto del tooltip */
  label: string;
  /** Clase de ícono FontAwesome 5, ej: 'fas fa-pen', 'fas fa-trash-alt' */
  icon: string;
  /** Variante semántica del botón — mapea a las clases globales .edit-btn, .delete-btn, etc. */
  variant?: 'edit' | 'delete' | 'config' | 'info' | 'warning';
  /** Clase CSS personalizada para el botón */
  btnClass?: string;
  /** Función opcional que deshabilita el botón fila a fila */
  // eslint-disable-next-line @typescript-eslint/no-explicit-any
  disabledFn?: (row: any) => boolean;
  
}

// eslint-disable-next-line @typescript-eslint/no-explicit-any
export interface TableActionEvent<T = any> {
  action: string;
  row: T;
}
