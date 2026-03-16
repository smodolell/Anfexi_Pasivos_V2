import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';

import { routes } from './app.routes';
import { API_AUTH_URL, API_CATALOGO_URL, API_COTIZADOR_URL, API_SISTEMA_URL } from './api.config';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { provideApi } from '@api/provide-api';
import { provideHighcharts } from 'highcharts-angular';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHighcharts(),
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideRouter(routes),
    provideHttpClient(withFetch(), withInterceptors([AuthInterceptor])),
    provideApi({
      basePath: environment.apiBaseUrl,
      withCredentials: true,
    }),
    { provide: API_AUTH_URL,      useValue: environment.apiAuthUrl      },
    { provide: API_CATALOGO_URL,  useValue: environment.apiCatalogoUrl  },
    { provide: API_SISTEMA_URL,   useValue: environment.apiSistemaUrl   },
    { provide: API_COTIZADOR_URL, useValue: environment.apiCotizadorUrl },
  ],
};
