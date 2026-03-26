import { Routes } from '@angular/router';
import { AdminLayoutComponent } from '../../layout/admin-layout.component';

export const operacionesRoutes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      {
        path: 'asociar-contrato',
        data: { title: 'Asociar Contrato Activo a Pasivo' },
        loadComponent: () =>
          import('../../pages/operaciones/asociar-contrato/asociar-contrato.component').then(
            (m) => m.AsociarContratoComponent
          ),
      },
      {
        path: 'anticipo',
        data: { title: 'Anticipo a Contrato Pasivo' },
        loadComponent: () =>
          import('../../pages/operaciones/anticipo/anticipo.component').then(
            (m) => m.AnticipoComponent
          ),
      },
      {
        path: 'caja-manual',
        data: { title: 'Caja Manual' },
        loadComponent: () =>
          import('../../pages/operaciones/caja-manual/caja-manual.component').then(
            (m) => m.CajaManualComponent
          ),
      },
      {
        path: 'cargo-adicional',
        data: { title: 'Cargo Adicional' },
        loadComponent: () =>
          import('../../pages/operaciones/cargo-adicional/cargo-adicional.component').then(
            (m) => m.CargoAdicionalComponent
          ),
      },
      {
        path: 'contratos-pasivos',
        data: { title: 'Contratos Pasivos' },
        loadComponent: () =>
          import('../../pages/operaciones/contratos-pasivos/contratos-pasivos-list.component').then(
            (m) => m.ContratosPasivosListComponent
          ),
      },
      {
        path: 'contratos-pasivos/nuevo',
        data: { title: 'Nuevo Contrato Pasivo' },
        loadComponent: () =>
          import('../../pages/operaciones/contratos-pasivos/contrato-form.component').then(
            (m) => m.ContratoFormComponent
          ),
      },
      {
        path: 'contratos-pasivos/edit/:id',
        data: { title: 'Editar Contrato Pasivo' },
        loadComponent: () =>
          import('../../pages/operaciones/contratos-pasivos/contrato-form.component').then(
            (m) => m.ContratoFormComponent
          ),
      },
      {
        path: 'contratos-pasivos/view/:id',
        data: { title: 'Detalle de Contrato Pasivo' },
        loadComponent: () =>
          import('../../pages/operaciones/contratos-pasivos/contrato-view.component').then(
            (m) => m.ContratoViewComponent
          ),
      },
      {
        path: 'nuevo-contrato',
        data: { title: 'Nuevo Contrato' },
        loadComponent: () =>
          import('../../pages/operaciones/nuevo-contrato/nuevo-contrato.component').then(
            (m) => m.NuevoContratoComponent
          ),
      },
    ],
  },
];

export default operacionesRoutes;
