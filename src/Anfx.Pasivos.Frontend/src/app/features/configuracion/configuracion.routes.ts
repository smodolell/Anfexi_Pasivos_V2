import { Routes } from '@angular/router';
import { FondeadorPage } from './pages/fondeador-page/fondeador-page';
import { TipoTablaAmortizaPage } from './pages/tipo-tabla-amortiza-page/tipo-tabla-amortiza-page';
import { AdminLayoutComponent } from '../../layout/admin-layout.component';

export const configuracionRoutes: Routes = [
  {
    path: '',
    component:AdminLayoutComponent,
    children: [
      { path: 'fondeador', component: FondeadorPage },
      { path: 'tipo-tabla-amortiza', component: TipoTablaAmortizaPage },
    ],
  },
];

export default configuracionRoutes;
