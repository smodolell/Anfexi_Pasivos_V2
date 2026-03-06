export interface SortingParameter {
  column: string;
  desc: boolean; // La propiedad 'desc' determina el tipo de ordenamiento: si su valor es 'true', se aplica un orden descendente; si es 'false', el orden será ascendente.
}