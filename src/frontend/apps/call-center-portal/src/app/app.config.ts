import { provideZoneChangeDetection, LOCALE_ID } from '@angular/core';
import type { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors } from '@angular/common/http';
import {
  provideKeycloak,
  withAutoRefreshToken,
  AutoRefreshTokenService,
  UserActivityService,
  includeBearerTokenInterceptor,
  createInterceptorCondition,
  INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG
} from 'keycloak-angular';
import type { IncludeBearerTokenCondition } from 'keycloak-angular';
import { environment } from '../environments/environment';

import { routes } from './app.routes';

const urlCondition = createInterceptorCondition<IncludeBearerTokenCondition>({
  urlPattern: /^(https?:\/\/[^/]+)?(\/api)(\/.*)?$/i
});

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withFetch(), withInterceptors([includeBearerTokenInterceptor])),
    { provide: LOCALE_ID, useValue: 'de-DE' },
    {
      provide: INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
      useValue: [urlCondition]
    },
    provideKeycloak({
      config: {
        url: environment.keycloak.url,
        realm: environment.keycloak.realm,
        clientId: environment.keycloak.clientId
      },
      initOptions: {
        onLoad: 'check-sso',
        silentCheckSsoRedirectUri: `${window.location.origin}/assets/silent-check-sso.html`,
        checkLoginIframe: false
      },
      features: [
        withAutoRefreshToken({
          onInactivityTimeout: 'logout',
          sessionTimeout: 300000
        })
      ],
      providers: [AutoRefreshTokenService, UserActivityService]
    })
  ]
};
