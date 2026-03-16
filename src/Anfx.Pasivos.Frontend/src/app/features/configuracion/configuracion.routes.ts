import { Routes } from '@angular/router';
import { AdminLayoutComponent } from '../../layout/admin-layout.component';

export const configuracionRoutes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      {
        path: 'fondeador',
        data: { title: 'Fondeadores' },
        loadComponent: () =>
          import('../../pages/configuracion/fondeador/fondeador-list.component').then(
            (m) => m.FondeadorListComponent
          ),
      },
      {
        path: 'fondeador/new',
        data: { title: 'Nuevo Fondeador' },
        loadComponent: () =>
          import('../../pages/configuracion/fondeador/fondeador-form.component').then(
            (m) => m.FondeadorFormComponent
          ),
      },
      {
        path: 'fondeador/edit/:id',
        data: { title: 'Editar Fondeador' },
        loadComponent: () =>
          import('../../pages/configuracion/fondeador/fondeador-form.component').then(
            (m) => m.FondeadorFormComponent
          ),
      },
      {
        path: 'tipo-credito',
        data: { title: 'Tipos de Crédito' },
        loadComponent: () =>
          import('../../pages/configuracion/tipo-credito/tipo-credito-list.component').then(
            (m) => m.TipoCreditoListComponent
          ),
      },
      {
        path: 'tipo-credito/new',
        data: { title: 'Nuevo Tipo de Crédito' },
        loadComponent: () =>
          import('../../pages/configuracion/tipo-credito/tipo-credito-form.component').then(
            (m) => m.TipoCreditoFormComponent
          ),
      },
      {
        path: 'tipo-credito/edit/:id',
        data: { title: 'Editar Tipo de Crédito' },
        loadComponent: () =>
          import('../../pages/configuracion/tipo-credito/tipo-credito-form.component').then(
            (m) => m.TipoCreditoFormComponent
          ),
      },
      {
        path: 'tipo-tabla-amortiza',
        data: { title: 'Tablas de Amortización' },
        loadComponent: () =>
          import('../../pages/configuracion/tipo-tabla-amortiza/tipo-tabla-amortiza-list.component').then(
            (m) => m.TipoTablaAmortizaListComponent
          ),
      },
      {
        path: 'tipo-tabla-amortiza/new',
        data: { title: 'Nueva Tabla de Amortización' },
        loadComponent: () =>
          import('../../pages/configuracion/tipo-tabla-amortiza/tipo-tabla-amortiza-form.component').then(
            (m) => m.TipoTablaAmortizaFormComponent
          ),
      },
      {
        path: 'tipo-tabla-amortiza/edit/:id',
        data: { title: 'Editar Tabla de Amortización' },
        loadComponent: () =>
          import('../../pages/configuracion/tipo-tabla-amortiza/tipo-tabla-amortiza-form.component').then(
            (m) => m.TipoTablaAmortizaFormComponent
          ),
      },
    ],
  },
];

export default configuracionRoutes;
