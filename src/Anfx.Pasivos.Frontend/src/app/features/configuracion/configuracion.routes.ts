import { Routes } from '@angular/router';
import { AdminLayoutComponent } from '../../layout/admin-layout.component';

export const configuracionRoutes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      {
        path: 'fondeador',
        loadComponent: () =>
          import('../../pages/configuracion/fondeador/fondeador-list.component').then(
            (m) => m.FondeadorListComponent
          ),
      },
      {
        path: 'fondeador/new',
        loadComponent: () =>
          import('../../pages/configuracion/fondeador/fondeador-form.component').then(
            (m) => m.FondeadorFormComponent
          ),
      },
      {
        path: 'fondeador/edit/:id',
        loadComponent: () =>
          import('../../pages/configuracion/fondeador/fondeador-form.component').then(
            (m) => m.FondeadorFormComponent
          ),
      },
      {
        path: 'tipo-credito',
        loadComponent: () =>
          import('../../pages/configuracion/tipo-credito/tipo-credito-list.component').then(
            (m) => m.TipoCreditoListComponent
          ),
      },
      {
        path: 'tipo-credito/new',
        loadComponent: () =>
          import('../../pages/configuracion/tipo-credito/tipo-credito-form.component').then(
            (m) => m.TipoCreditoFormComponent
          ),
      },
      {
        path: 'tipo-credito/edit/:id',
        loadComponent: () =>
          import('../../pages/configuracion/tipo-credito/tipo-credito-form.component').then(
            (m) => m.TipoCreditoFormComponent
          ),
      },
      {
        path: 'tipo-tabla-amortiza',
        loadComponent: () =>
          import('../../pages/configuracion/tipo-tabla-amortiza/tipo-tabla-amortiza-list.component').then(
            (m) => m.TipoTablaAmortizaListComponent
          ),
      },
      {
        path: 'tipo-tabla-amortiza/new',
        loadComponent: () =>
          import('../../pages/configuracion/tipo-tabla-amortiza/tipo-tabla-amortiza-form.component').then(
            (m) => m.TipoTablaAmortizaFormComponent
          ),
      },
      {
        path: 'tipo-tabla-amortiza/edit/:id',
        loadComponent: () =>
          import('../../pages/configuracion/tipo-tabla-amortiza/tipo-tabla-amortiza-form.component').then(
            (m) => m.TipoTablaAmortizaFormComponent
          ),
      },
    ],
  },
];

export default configuracionRoutes;
