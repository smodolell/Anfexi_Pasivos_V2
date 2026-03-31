import { Routes } from '@angular/router';
import { AdminLayoutComponent } from '../../layout/admin-layout.component';

export const procesosRoutes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      {
        path: 'vencimiento',
        data: { title: 'Procesa Vencimientos de Contratos Pasivos' },
        loadComponent: () =>
          import('../../pages/procesos/vencimiento/vencimiento.component').then(
            (m) => m.VencimientoComponent
          ),
      },
      {
        path: 'moratorios',
        data: { title: 'Proceso de Moratorios' },
        loadComponent: () =>
          import('../../pages/procesos/moratorios/moratorios.component').then(
            (m) => m.MoratoriosComponent
          ),
      },
    ],
  },
];

export default procesosRoutes;
