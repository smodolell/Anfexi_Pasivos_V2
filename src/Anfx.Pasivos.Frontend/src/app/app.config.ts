import {
  ApplicationConfig,
  provideBrowserGlobalErrorListeners,
  provideZonelessChangeDetection,
} from '@angular/core';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import { provideToastr } from 'ngx-toastr';

import { routes } from './app.routes';
import { API_AUTH_URL, API_CATALOGO_URL, API_COTIZADOR_URL, API_SISTEMA_URL, MENU_API_URL } from './api.config';
import { AuthInterceptor } from './interceptors/auth.interceptor';
import { provideApi } from '@api/provide-api';
import { provideHighcharts } from 'highcharts-angular';
import { environment } from '../environments/environment';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHighcharts(),
    provideBrowserGlobalErrorListeners(),
    provideZonelessChangeDetection(),
    provideAnimationsAsync(),
    provideToastr({
      timeOut:          3500,
      positionClass:    'toast-top-right',
      preventDuplicates: true,
      progressBar:      true,
      closeButton:      true,
    }),
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
    { provide: MENU_API_URL,     useValue: environment.menuApiUrl },
  ],
};
