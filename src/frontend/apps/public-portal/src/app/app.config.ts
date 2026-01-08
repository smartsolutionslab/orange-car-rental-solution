import {
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
  APP_INITIALIZER,
  LOCALE_ID,
} from '@angular/core';
import type { ApplicationConfig, EnvironmentProviders, Provider } from '@angular/core';
import { provideRouter } from '@angular/router';
import type { HttpInterceptorFn } from '@angular/common/http';
import { provideHttpClient, withFetch, withInterceptors, HttpClient } from '@angular/common/http';
import { signal } from '@angular/core';
import {
  provideKeycloak,
  withAutoRefreshToken,
  AutoRefreshTokenService,
  UserActivityService,
  includeBearerTokenInterceptor,
  createInterceptorCondition,
  INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
  KEYCLOAK_EVENT_SIGNAL,
  KeycloakEventType,
} from 'keycloak-angular';
import type { IncludeBearerTokenCondition, KeycloakEvent } from 'keycloak-angular';
import Keycloak from 'keycloak-js';
import { API_CONFIG } from '@orange-car-rental/shared';
import { provideI18n } from '@orange-car-rental/i18n';
import { ConfigService } from './services/config.service';
import { initializeApp } from './initializers/config.initializer';
import { environment } from '../environments/environment';

import { routes } from './app.routes';

// Check if Keycloak is enabled
const isKeycloakEnabled = !!environment.keycloak.url;

const urlCondition = createInterceptorCondition<IncludeBearerTokenCondition>({
  urlPattern: /^(https?:\/\/[^/]+)?(\/api)(\/.*)?$/i,
});

/**
 * Returns HTTP interceptors. Only includes bearer token interceptor when Keycloak is enabled.
 */
function getHttpInterceptors(): HttpInterceptorFn[] {
  if (!isKeycloakEnabled) {
    return [];
  }
  return [includeBearerTokenInterceptor];
}

/**
 * Mock Keycloak class for E2E tests.
 * This satisfies the DI requirement without connecting to a real Keycloak server.
 * Using a class instead of factory to ensure it's available at class initialization time.
 */
class MockKeycloak {
  authenticated = false;
  token: string | undefined = undefined;
  refreshToken: string | undefined = undefined;
  tokenParsed: unknown = undefined;
  realmAccess = { roles: [] as string[] };
  init() { return Promise.resolve(false); }
  login() { return Promise.resolve(); }
  logout() { return Promise.resolve(); }
  updateToken() { return Promise.resolve(false); }
  loadUserProfile() { return Promise.resolve({}); }
}

/**
 * Returns Keycloak providers only if a valid Keycloak URL is configured.
 * This allows E2E tests to run without a Keycloak server.
 * When disabled, provides a mock Keycloak to satisfy DI requirements.
 */
function getKeycloakProviders(): (Provider | EnvironmentProviders)[] {
  // Skip Keycloak initialization if no URL is configured (e.g., in E2E tests)
  if (!isKeycloakEnabled) {
    console.log('Keycloak URL not configured - skipping authentication');
    // Provide mock instances to satisfy DI for services that inject Keycloak or keycloak-angular tokens
    // This includes mock providers for keycloak-angular internal services that may be tree-shaken in
    return [
      {
        provide: Keycloak,
        useClass: MockKeycloak,
      },
      {
        provide: KEYCLOAK_EVENT_SIGNAL,
        useValue: signal<KeycloakEvent>({ type: KeycloakEventType.KeycloakAngularNotInitialized }),
      },
      {
        provide: INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
        useValue: [],
      },
      // Mock keycloak-angular services that might be instantiated
      {
        provide: AutoRefreshTokenService,
        useValue: {
          start: (): void => { /* no-op for E2E */ },
          stop: (): void => { /* no-op for E2E */ },
        },
      },
      {
        provide: UserActivityService,
        useValue: {
          startMonitoring: (): void => { /* no-op for E2E */ },
          stopMonitoring: (): void => { /* no-op for E2E */ },
          lastActivitySignal: signal(Date.now()),
        },
      },
    ];
  }

  return [
    {
      provide: INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
      useValue: [urlCondition],
    },
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
    provideHttpClient(withFetch(), withInterceptors(getHttpInterceptors())),
    ...provideI18n(),
    { provide: LOCALE_ID, useValue: 'de-DE' },
    { provide: API_CONFIG, useExisting: ConfigService },
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [HttpClient, ConfigService],
      multi: true,
    },
    ...getKeycloakProviders(),
  ],
};
