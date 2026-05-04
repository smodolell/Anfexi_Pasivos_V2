import { Routes } from '@angular/router';
import { AdminLayoutComponent } from '../../layout/admin-layout.component';

export const reportesRoutes: Routes = [
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: '', redirectTo: 'configuracion', pathMatch: 'full' },
      {
        path: 'configuracion',
        data: { title: 'Configuración de Reportes' },
        loadComponent: () =>
          import('./configuracion/reporte-list.component').then(m => m.ReporteListComponent),
      },
      {
        path: 'configuracion/new',
        data: { title: 'Nuevo Reporte' },
        loadComponent: () =>
          import('./configuracion/reporte-form.component').then(m => m.ReporteFormComponent),
      },
      {
        path: 'configuracion/edit/:id',
        data: { title: 'Editar Reporte' },
        loadComponent: () =>
          import('./configuracion/reporte-form.component').then(m => m.ReporteFormComponent),
      },
      {
        path: 'configuracion/:reporteId/parametros/:id',
        data: { title: 'Editar Parámetro' },
        loadComponent: () =>
          import('./configuracion/parametro-form.component').then(m => m.ParametroFormComponent),
      },
      {
        path: 'ejecutar',
        data: { title: 'Ejecutar Reporte' },
        loadComponent: () =>
          import('./ejecutar/reporte-ejecutar.component').then(m => m.ReporteEjecutarComponent),
      },
      {
        path: 'historial',
        data: { title: 'Historial de Reportes' },
        loadComponent: () =>
          import('./historial/reporte-historial.component').then(m => m.ReporteHistorialComponent),
      },
    ],
  },
];

export default reportesRoutes;
