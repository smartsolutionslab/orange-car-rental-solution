import { ApplicationConfig, provideZoneChangeDetection, LOCALE_ID, APP_INITIALIZER } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptorsFromDi } from '@angular/common/http';
import { KeycloakService } from 'keycloak-angular';
import { initializeKeycloak } from './initializers/keycloak.initializer';

import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withFetch(), withInterceptorsFromDi()),
    { provide: LOCALE_ID, useValue: 'de-DE' },
    KeycloakService,
    {
      provide: APP_INITIALIZER,
      useFactory: initializeKeycloak,
      deps: [KeycloakService],
      multi: true
    }
  ]
};
