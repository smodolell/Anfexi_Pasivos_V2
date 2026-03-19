import { Routes } from '@angular/router';
import { AdminLayoutComponent } from './layout/admin-layout.component';
import { LoginLayoutComponent } from './layout/login-layout.component';
import { authGuard } from './guards/auth.guard';

export const routes: Routes = [
  // Ruta raíz → login
  { path: '', redirectTo: '/auth/login', pathMatch: 'full' },

  // Rutas con layout de administración - PROTEGIDAS
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: '/admin/reportes/dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        data: { title: 'Dashboard' },
        loadComponent: () =>
          import('./pages/reportes/dashboard/dashboard.component').then((m) => m.DashboardComponent),
      },
      {
        path: 'reportes/dashboard',
        data: { title: 'Monitor de Cartera Pasiva' },
        loadComponent: () =>
          import('./pages/reportes/dashboard/dashboard.component').then((m) => m.DashboardComponent),
      },
      {
        path: 'usuarios',
        data: { title: 'Administración de Usuarios' },
        loadComponent: () =>
          import('./pages/sistema/usuario/usuario-list.component').then(
            (m) => m.UsuarioListComponent,
          ),
      },
      {
        path: 'roles',
        data: { title: 'Roles' },
        loadComponent: () =>
          import('./pages/sistema/rol/rol-list.component').then((m) => m.RolListComponent),
      },
      {
        path: 'empresas',
        data: { title: 'Empresas' },
        loadComponent: () =>
          import('./pages/sistema/empresa/empresa-list.component').then(
            (m) => m.EmpresaListComponent,
          ),
      },
      {
        path: 'profile',
        data: { title: 'Mi Perfil' },
        loadComponent: () =>
          import('./pages/admin/profile/profile.component').then((m) => m.ProfileComponent),
      },
      {
        path: 'contratos/info-general',
        data: { title: 'Información General del Contrato Pasivo' },
        loadComponent: () =>
          import('./pages/contratos/info-general/info-general-contrato-pasivo.component').then(
            (m) => m.InfoGeneralContratoPasivoComponent,
          ),
      },
    ],
  },

  // Configuración - PROTEGIDA
  {
    path: 'configuracion',
    canActivate: [authGuard],
    loadChildren: () => import('./features/configuracion/configuracion.routes'),
  },

  // Operaciones - PROTEGIDAS
  {
    path: 'operaciones',
    canActivate: [authGuard],
    loadChildren: () => import('./features/operaciones/operaciones.routes'),
  },

  // Procesos - PROTEGIDOS
  {
    path: 'procesos',
    canActivate: [authGuard],
    loadChildren: () => import('./features/procesos/procesos.routes'),
  },

  // Catálogos - PROTEGIDOS
  {
    path: 'catalogos',
    component: AdminLayoutComponent,
    canActivate: [authGuard],
    children: [
      {
        path: 'colonias',
        data: { title: 'Colonias' },
        loadComponent: () =>
          import('./pages/catalogos/colonia/colonia-list.component').then(
            (m) => m.ColoniaListComponent,
          ),
      },
      {
        path: 'bancos',
        data: { title: 'Bancos' },
        loadComponent: () =>
          import('./pages/catalogos/bancos/bancos-list.component').then(
            (m) => m.BancosListComponent,
          ),
      },
      {
        path: 'bancos/new',
        data: { title: 'Nuevo Banco' },
        loadComponent: () =>
          import('./pages/catalogos/bancos/banco-form.component').then(
            (m) => m.BancoFormComponent,
          ),
      },
      {
        path: 'bancos/edit/:id',
        data: { title: 'Editar Banco' },
        loadComponent: () =>
          import('./pages/catalogos/bancos/banco-form.component').then(
            (m) => m.BancoFormComponent,
          ),
      },
      {
        path: 'cuentas-bancarias',
        data: { title: 'Cuentas Bancarias' },
        loadComponent: () =>
          import('./pages/catalogos/cuentas-bancarias/cuentas-bancarias-list.component').then(
            (m) => m.CuentasBancariasListComponent,
          ),
      },
      {
        path: 'cuentas-bancarias/new',
        data: { title: 'Nueva Cuenta Bancaria' },
        loadComponent: () =>
          import('./pages/catalogos/cuentas-bancarias/cuenta-bancaria-form.component').then(
            (m) => m.CuentaBancariaFormComponent,
          ),
      },
      {
        path: 'cuentas-bancarias/edit/:id',
        data: { title: 'Editar Cuenta Bancaria' },
        loadComponent: () =>
          import('./pages/catalogos/cuentas-bancarias/cuenta-bancaria-form.component').then(
            (m) => m.CuentaBancariaFormComponent,
          ),
      },
      {
        path: 'estatus-contrato',
        data: { title: 'Estatus de Contrato' },
        loadComponent: () =>
          import('./pages/catalogos/estatus-contrato/estatus-contrato-list.component').then(
            (m) => m.EstatusContratoListComponent,
          ),
      },
      {
        path: 'estatus-contrato/new',
        data: { title: 'Nuevo Estatus de Contrato' },
        loadComponent: () =>
          import('./pages/catalogos/estatus-contrato/estatus-contrato-form.component').then(
            (m) => m.EstatusContratoFormComponent,
          ),
      },
      {
        path: 'estatus-contrato/edit/:id',
        data: { title: 'Editar Estatus de Contrato' },
        loadComponent: () =>
          import('./pages/catalogos/estatus-contrato/estatus-contrato-form.component').then(
            (m) => m.EstatusContratoFormComponent,
          ),
      },
      {
        path: 'tipo-pago',
        data: { title: 'Tipos de Pago' },
        loadComponent: () =>
          import('./pages/catalogos/tipo-pago/tipo-pago-list.component').then(
            (m) => m.TipoPagoListComponent,
          ),
      },
    ],
  },

  // Autenticación
  {
    path: 'auth',
    component: LoginLayoutComponent,
    children: [
      { path: '', redirectTo: '/auth/login', pathMatch: 'full' },
      {
        path: 'login',
        loadComponent: () =>
          import('./pages/auth/login/login.component').then((m) => m.LoginComponent),
      },
    ],
  },

  { path: 'logout', redirectTo: '/auth/login', pathMatch: 'full' },
  {
    path: 'unauthorized',
    loadComponent: () =>
      import('./pages/auth/unauthorized/unauthorized.component').then((m) => m.UnauthorizedComponent),
  },
];
