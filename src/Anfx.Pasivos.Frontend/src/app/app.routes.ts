import { Routes } from '@angular/router';
import { LoginLayoutComponent } from './layout/login-layout.component';
import { OktaCallbackComponent } from '@okta/okta-angular';
import { authGuard } from './guards/auth.guard';
import { noAuthGuard } from './guards/no-auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/auth/login', pathMatch: 'full' },

  // Ruta de callback de Okta — maneja el redirect tras autenticación
  { path: 'login/callback', component: OktaCallbackComponent },

  // Ruta de callback post-logout de Okta — limpia la sesión y redirige al login
  {
    path: 'logout/callback',
    loadComponent: () =>
      import('./pages/auth/logout-callback/logout-callback.component').then(
        (m) => m.LogoutCallbackComponent,
      ),
  },

  {
    path: 'admin',
    canActivate: [authGuard],
    loadChildren: () => import('./features/admin/admin.routes'),
  },
  {
    path: 'configuracion',
    canActivate: [authGuard],
    loadChildren: () => import('./pages/configuracion/configuracion.routes'),
  },
  {
    path: 'operaciones',
    canActivate: [authGuard],
    loadChildren: () => import('./pages/operaciones/operaciones.routes'),
  },
  {
    path: 'procesos',
    canActivate: [authGuard],
    loadChildren: () => import('./pages/procesos/procesos.routes'),
  },
  {
    path: 'catalogos',
    canActivate: [authGuard],
    loadChildren: () => import('./features/catalogos/catalogos.routes'),
  },

  {
    path: 'auth',
    component: LoginLayoutComponent,
    canActivate: [noAuthGuard],
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
      import('./pages/auth/unauthorized/unauthorized.component').then(
        (m) => m.UnauthorizedComponent,
      ),
  },
  {
    path: '**',
    loadComponent: () =>
      import('./pages/auth/not-found/not-found.component').then(
        (m) => m.NotFoundComponent,
      ),
  },
];
