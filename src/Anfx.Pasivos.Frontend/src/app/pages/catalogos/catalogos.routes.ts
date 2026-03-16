import { Routes } from '@angular/router';

export const catalogosRoutes: Routes = [
  {
    path: 'catalogos',
    children: [
      { path: '**', redirectTo: 'catalogos' },
    ],
  },
];

export default catalogosRoutes;
