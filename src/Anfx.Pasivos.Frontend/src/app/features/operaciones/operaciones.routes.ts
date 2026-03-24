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
    ],
  },
];

export default operacionesRoutes;
