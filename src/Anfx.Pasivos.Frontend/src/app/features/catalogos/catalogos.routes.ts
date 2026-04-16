import { Routes } from '@angular/router';
import { AdminLayoutComponent } from '../../layout/admin-layout.component';

export const catalogosRoutes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      {
        path: 'colonias',
        data: { title: 'Colonias' },
        loadComponent: () =>
          import('../../pages/catalogos/colonia/colonia-list.component').then(
            (m) => m.ColoniaListComponent,
          ),
      },
      {
        path: 'bancos',
        data: { title: 'Bancos' },
        loadComponent: () =>
          import('../../pages/catalogos/bancos/bancos-list.component').then(
            (m) => m.BancosListComponent,
          ),
      },
      {
        path: 'bancos/new',
        data: { title: 'Nuevo Banco' },
        loadComponent: () =>
          import('../../pages/catalogos/bancos/banco-form.component').then(
            (m) => m.BancoFormComponent,
          ),
      },
      {
        path: 'bancos/edit/:id',
        data: { title: 'Editar Banco' },
        loadComponent: () =>
          import('../../pages/catalogos/bancos/banco-form.component').then(
            (m) => m.BancoFormComponent,
          ),
      },
      {
        path: 'cuentas-bancarias',
        data: { title: 'Cuentas Bancarias' },
        loadComponent: () =>
          import('../../pages/catalogos/cuentas-bancarias/cuentas-bancarias-list.component').then(
            (m) => m.CuentasBancariasListComponent,
          ),
      },
      {
        path: 'cuentas-bancarias/new',
        data: { title: 'Nueva Cuenta Bancaria' },
        loadComponent: () =>
          import('../../pages/catalogos/cuentas-bancarias/cuenta-bancaria-form.component').then(
            (m) => m.CuentaBancariaFormComponent,
          ),
      },
      {
        path: 'cuentas-bancarias/edit/:id',
        data: { title: 'Editar Cuenta Bancaria' },
        loadComponent: () =>
          import('../../pages/catalogos/cuentas-bancarias/cuenta-bancaria-form.component').then(
            (m) => m.CuentaBancariaFormComponent,
          ),
      },
      {
        path: 'estatus-contrato',
        data: { title: 'Estatus de Contrato' },
        loadComponent: () =>
          import('../../pages/catalogos/estatus-contrato/estatus-contrato-list.component').then(
            (m) => m.EstatusContratoListComponent,
          ),
      },
      {
        path: 'estatus-contrato/new',
        data: { title: 'Nuevo Estatus de Contrato' },
        loadComponent: () =>
          import('../../pages/catalogos/estatus-contrato/estatus-contrato-form.component').then(
            (m) => m.EstatusContratoFormComponent,
          ),
      },
      {
        path: 'estatus-contrato/edit/:id',
        data: { title: 'Editar Estatus de Contrato' },
        loadComponent: () =>
          import('../../pages/catalogos/estatus-contrato/estatus-contrato-form.component').then(
            (m) => m.EstatusContratoFormComponent,
          ),
      },
      {
        path: 'tipo-pago',
        data: { title: 'Tipos de Pago' },
        loadComponent: () =>
          import('../../pages/catalogos/tipo-pago/tipo-pago-list.component').then(
            (m) => m.TipoPagoListComponent,
          ),
      },
      {
        path: 'tipo-direccion',
        data: { title: 'Tipos de Dirección' },
        loadComponent: () =>
          import('../../pages/catalogos/tipodireccion/tipodireccion-list.component').then(
            (m) => m.TipoDireccionListComponent,
          ),
      },
      {
        path: 'tasas',
        data: { title: 'Tasas' },
        loadComponent: () =>
          import('../../pages/catalogos/tasas/tasas.component').then(
            (m) => m.TasasComponent,
          ),
      },
    ],
  },
];

export default catalogosRoutes;
