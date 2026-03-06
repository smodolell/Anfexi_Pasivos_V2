# ColoniaAutoComplete Component

Componente reutilizable para selección de colonias con autocompletado de código postal.

## Características

- **Autocompletado de código postal**: Busca códigos postales mientras el usuario escribe
- **Carga automática de colonias**: Al seleccionar un código postal, carga automáticamente las colonias
- **Carga por ID**: Si se proporciona un `coloniaId`, carga todos los datos automáticamente
- **Integración con formularios**: Implementa `ControlValueAccessor` para trabajar con `FormControl`
- **Validaciones**: Soporte para campos requeridos
- **Estados de solo lectura**: Estado y municipio son de solo lectura

## Uso

### 1. Importar el componente

```typescript
import { ColoniaAutoCompleteComponent } from './shared/components/colonia-auto-complete/colonia-auto-complete.component';

@Component({
  imports: [ColoniaAutoCompleteComponent],
  // ...
})
```

### 2. Usar en el template

```html
<app-colonia-auto
  [required]="true"
  [codigoPostalRequired]="true"
  labelCodigoPostal="Código Postal"
  labelEstado="Estado"
  labelMunicipio="Municipio"
  labelColonia="Colonia"
  formControlName="direccion">
</app-colonia-auto>
```

### 3. En el FormGroup

```typescript
this.form = this.fb.group({
  direccion: [null] // El componente manejará el modelo ColoniaModel
});
```

## Inputs

| Input | Tipo | Requerido | Descripción |
|-------|------|-----------|-------------|
| `required` | `boolean` | No | Si el campo colonia es requerido |
| `codigoPostalRequired` | `boolean` | No | Si el campo código postal es requerido |
| `labelCodigoPostal` | `string` | No | Etiqueta para código postal (default: 'Código Postal') |
| `labelEstado` | `string` | No | Etiqueta para estado (default: 'Estado') |
| `labelMunicipio` | `string` | No | Etiqueta para municipio (default: 'Municipio') |
| `labelColonia` | `string` | No | Etiqueta para colonia (default: 'Colonia') |

## Modelo de Datos

```typescript
interface ColoniaModel {
  codigoPostal: string;
  estado: string;
  municipio: string;
  coloniaId: number | null;
}
```

## Servicios Requeridos

El componente utiliza los siguientes métodos del `ColoniaService`:

- `getCodigosPostales(codigoPostal: string)` - Busca códigos postales
- `getColoniasByCodigoPostal(codigoPostal: string)` - Obtiene colonias por código postal
- `getColoniasById(id: number)` - Obtiene datos completos por ID de colonia

## Flujo de Funcionamiento

1. **Usuario escribe código postal**: Se activa el autocompletado después de 3 caracteres
2. **Selecciona código postal**: Se cargan automáticamente estado, municipio y colonias
3. **Selecciona colonia**: Se actualiza el modelo con todos los datos
4. **Carga por ID**: Si se proporciona un `coloniaId`, se cargan todos los datos automáticamente

## Estilos

El componente utiliza clases de Bootstrap 5 y es completamente responsive.
