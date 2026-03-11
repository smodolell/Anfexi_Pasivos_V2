import {
  ApplicationConfig,
  importProvidersFrom,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';

import { routes } from './app.routes';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { API_AUTH_URL, API_CATALOGO_URL, API_COTIZADOR_URL, API_SISTEMA_URL } from './api.config';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { provideApi } from '@api/provide-api';
import { provideHighcharts } from 'highcharts-angular';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHighcharts(),
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(withFetch(), withInterceptors([AuthInterceptor])),
    provideApi({
      basePath:
        process.env['NODE_ENV'] === 'production'
          ? 'https://dev.anfexi.com/profuturo/backend'
          : 'https://localhost:7223',
      withCredentials: true,
      // Si necesitas configuración adicional
    }),
    {
      provide: API_AUTH_URL,
      useValue:
        process.env['NODE_ENV'] === 'production'
          ? 'https://dev.anfexi.com/profuturo/backend/auth/api'
          : 'https://localhost:7223/api',
    },
    {
      provide: API_CATALOGO_URL,
      useValue:
        process.env['NODE_ENV'] === 'production'
          ? 'https://dev.anfexi.com/profuturo/backend/catalogo/api'
          : 'https://localhost:7223/api',
    },
    {
      provide: API_SISTEMA_URL,
      useValue:
        process.env['NODE_ENV'] === 'production'
          ? 'https://dev.anfexi.com/profuturo/backend/sistema/api'
          : 'https://localhost:7223/api',
    },
    {
      provide: API_COTIZADOR_URL,
      useValue:
        process.env['NODE_ENV'] === 'production'
          ? 'https://dev.anfexi.com/profuturo/backend/cotizador/api'
          : 'https://localhost:7223/api',
    },
    provideClientHydration(withEventReplay()),
  ],
};
