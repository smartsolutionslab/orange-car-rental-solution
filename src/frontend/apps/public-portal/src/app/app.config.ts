import {
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
  APP_INITIALIZER,
  LOCALE_ID,
} from '@angular/core';
import type { ApplicationConfig, EnvironmentProviders, Provider } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, withInterceptors, HttpClient } from '@angular/common/http';
import {
  provideKeycloak,
  withAutoRefreshToken,
  AutoRefreshTokenService,
  UserActivityService,
  includeBearerTokenInterceptor,
  createInterceptorCondition,
  INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
} from 'keycloak-angular';
import type { IncludeBearerTokenCondition } from 'keycloak-angular';
import { API_CONFIG } from '@orange-car-rental/shared';
import { provideI18n } from '@orange-car-rental/i18n';
import { ConfigService } from './services/config.service';
import { initializeApp } from './initializers/config.initializer';
import { environment } from '../environments/environment';

import { routes } from './app.routes';

const urlCondition = createInterceptorCondition<IncludeBearerTokenCondition>({
  urlPattern: /^(https?:\/\/[^/]+)?(\/api)(\/.*)?$/i,
});

/**
 * Returns Keycloak providers only if a valid Keycloak URL is configured.
 * This allows E2E tests to run without a Keycloak server.
 */
function getKeycloakProviders(): (Provider | EnvironmentProviders)[] {
  // Skip Keycloak initialization if no URL is configured (e.g., in E2E tests)
  if (!environment.keycloak.url) {
    console.log('Keycloak URL not configured - skipping authentication');
    return [];
  }

  return [
    provideKeycloak({
      config: {
        url: environment.keycloak.url,
        realm: environment.keycloak.realm,
        clientId: environment.keycloak.clientId,
      },
      initOptions: {
        onLoad: 'check-sso',
        silentCheckSsoRedirectUri: `${window.location.origin}/assets/silent-check-sso.html`,
        checkLoginIframe: false,
      },
      features: [
        withAutoRefreshToken({
          onInactivityTimeout: 'logout',
          sessionTimeout: 300000,
        }),
      ],
      providers: [AutoRefreshTokenService, UserActivityService],
    }),
  ];
}

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withFetch(), withInterceptors([includeBearerTokenInterceptor])),
    ...provideI18n(),
    { provide: LOCALE_ID, useValue: 'de-DE' },
    { provide: API_CONFIG, useExisting: ConfigService },
    {
      provide: INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
      useValue: [urlCondition],
    },
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [HttpClient, ConfigService],
      multi: true,
    },
    ...getKeycloakProviders(),
  ],
};
