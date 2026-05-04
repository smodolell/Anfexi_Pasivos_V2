import { Environment } from './environment.interface';

export const environment: Environment = {
  production: false,
  apiUrl:     'http://localhost:5056/api',
  menuApiUrl: 'assets/menu.json',
  app: { name: 'Pasivos', version: '0.0.1' },
  company:    'ANFEXI TECHNOLOGIES',
};
