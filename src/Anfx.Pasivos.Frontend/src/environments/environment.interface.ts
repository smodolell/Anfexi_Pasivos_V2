export interface Environment {
  production: boolean;
  apiUrl: string;
  menuApiUrl: string;
  app: { name: string; version: string };
  company: string;
}
