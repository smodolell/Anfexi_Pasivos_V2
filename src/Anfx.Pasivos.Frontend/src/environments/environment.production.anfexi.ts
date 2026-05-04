import { Environment } from './environment.interface';

export const environment: Environment = {
  production: true,
  apiUrl:     'https://dev.anfexi.com/profuturo/pasivos/backend/api',
  menuApiUrl: 'assets/menu.json',
  app: { name: 'Pasivos', version: '0.0.1' },
  company:    'ANFEXI TECHNOLOGIES',
};
