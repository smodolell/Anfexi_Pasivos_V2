import { Routes } from '@angular/router';
import { AdminLayoutComponent } from '../../layout/admin-layout.component';

export const adminRoutes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: '', redirectTo: '/admin/reportes/dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        data: { title: 'Dashboard' },
        loadComponent: () =>
          import('../../pages/reportes/dashboard/dashboard.component').then(
            (m) => m.DashboardComponent,
          ),
      },
      {
        path: 'reportes/dashboard',
        data: { title: 'Monitor de Cartera Pasiva' },
        loadComponent: () =>
          import('../../pages/reportes/dashboard/dashboard.component').then(
            (m) => m.DashboardComponent,
          ),
      },
      {
        path: 'usuarios',
        data: { title: 'Administración de Usuarios' },
        loadComponent: () =>
          import('../../pages/sistema/usuario/usuario-list.component').then(
            (m) => m.UsuarioListComponent,
          ),
      },
      {
        path: 'roles',
        data: { title: 'Roles' },
        loadComponent: () =>
          import('../../pages/sistema/rol/rol-list.component').then(
            (m) => m.RolListComponent,
          ),
      },
      {
        path: 'empresas',
        data: { title: 'Empresas' },
        loadComponent: () =>
          import('../../pages/sistema/empresa/empresa-list.component').then(
            (m) => m.EmpresaListComponent,
          ),
      },
      {
        path: 'profile',
        data: { title: 'Mi Perfil' },
        loadComponent: () =>
          import('../../pages/admin/profile/profile.component').then(
            (m) => m.ProfileComponent,
          ),
      },
      {
        path: 'contratos/info-general',
        data: { title: 'Información General del Contrato Pasivo' },
        loadComponent: () =>
          import('../../pages/contratos/info-general/info-general-contrato-pasivo.component').then(
            (m) => m.InfoGeneralContratoPasivoComponent,
          ),
      },
    ],
  },
];

export default adminRoutes;
