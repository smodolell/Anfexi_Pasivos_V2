import { configuracionRoutes } from './features/configuracion/configuracion.routes';
import { CuentaBancariaForm } from './features/catalogos/components/cuenta-bancaria-form/cuenta-bancaria-form';
import { Routes } from '@angular/router';
import { LayoutComponent } from './layout/layout.component';
import { AdminLayoutComponent } from './layout/admin-layout.component';
import { LoginLayoutComponent } from './layout/login-layout.component';
import { AuthGuard } from './guards/auth.guard';
import { BancoFormComponent } from './pages/catalogos/bancos/banco-form.component';
import { CuentaBancariaFormComponent } from './pages/catalogos/cuentas-bancarias/cuenta-bancaria-form.component';

export const routes: Routes = [
  // Rutas con layout principal (como Master Page)
  {
    path: '',
    component: LayoutComponent,
    children: [{ path: '', redirectTo: '/auth/login', pathMatch: 'full' }],
  },

  // Rutas con layout de administración - PROTEGIDAS
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [AuthGuard], // Requiere autenticación Y rol de admin
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
            (m) => m.InfoGeneralContratoPasivoComponent
          ),
      },
    ],
  },
  {
    path: 'configuracion',
    loadChildren: () => import('./features/configuracion/configuracion.routes'),
  },
  {
    path: 'catalogos',
    component: AdminLayoutComponent,
    canActivate: [AuthGuard], // Requiere autenticación Y rol de admin
    children: [
      // Catálogos
      {
        path: 'tiposdirecciones',
        data: { title: 'Tipos de Dirección' },
        loadComponent: () =>
          import('./pages/catalogos/tipodireccion/tipodireccion-list.component').then(
            (m) => m.TipoDireccionListComponent,
          ),
      },
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
        path: 'estatus-contrato/edit:id',
        data: { title: 'Editar Estatus de Contrato' },
        loadComponent: () =>
          import('./pages/catalogos/estatus-contrato/estatus-contrato-form.component').then(
            (m) => m.EstatusContratoFormComponent,
          ),
      },
    ],
  },
  // Rutas con layout de administración - PROTEGIDAS
  {
    path: 'admin',
    component: AdminLayoutComponent,
    canActivate: [AuthGuard],
    children: [
      { path: '', redirectTo: '/admin/reportes/dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        data: { title: 'Dashboard de Administración' },
        loadComponent: () =>
          import('./pages/admin/dashboard/dashboard.component').then((m) => m.DashboardComponent),
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
    ],
  },

  // Rutas con layout de login/autenticación
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

  // Ruta de logout
  { path: 'logout', redirectTo: '/auth/login', pathMatch: 'full' },

  // Ruta para usuarios no autenticados que intenten acceder a rutas protegidas
  { path: 'unauthorized', redirectTo: '/auth/login', pathMatch: 'full' },
];
