import { Routes } from '@angular/router';
import { EstatusContratoPage } from './pages/estatus-contrato-page/estatus-contrato-page';
import { TipoCreditoPage } from './pages/tipo- credito-page/tipo- credito-page';
import { TipoPagoPage } from './pages/tipo-pago-page/tipo-pago-page';
import { BancoPage } from './pages/banco-page/banco-page';
import { CuentaBancariaPage } from './pages/cuenta-bancaria-page/cuenta-bancaria-page';

export const catalogosRoutes: Routes = [
  {
    path: 'catalogos',
    children: [
      { path: 'estatus-contrato', component: EstatusContratoPage },
      { path: 'tipo-credito', component: TipoCreditoPage },
      { path: 'tipo-pago', component: TipoPagoPage },
      { path: 'banco', component: BancoPage },
      { path: 'cuenta-bancaria', component: CuentaBancariaPage },
      { path: '**', redirectTo: 'catalogos' },
    ],
  },
];

export default catalogosRoutes;
