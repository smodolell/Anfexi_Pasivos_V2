import { Environment } from './environment.interface';

export const environment: Environment = {
  production: true,
  apiUrl:     'http://172.22.147.81/ApiPasivos/api',
  menuApiUrl: 'assets/menu.json',
  app: { name: 'Pasivos', version: '0.0.1' },
  company:    'ANFEXI TECHNOLOGIES',
};
