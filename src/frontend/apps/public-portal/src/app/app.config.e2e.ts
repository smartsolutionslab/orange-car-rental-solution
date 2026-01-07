import {
  provideBrowserGlobalErrorListeners,
  provideZoneChangeDetection,
  APP_INITIALIZER,
  LOCALE_ID,
  InjectionToken,
  ErrorHandler,
} from '@angular/core';
import type { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withFetch, HttpClient } from '@angular/common/http';
import { signal } from '@angular/core';
import { API_CONFIG } from '@orange-car-rental/shared';
import { provideI18n } from '@orange-car-rental/i18n';
import { ConfigService } from './services/config.service';
import { initializeApp } from './initializers/config.initializer';
// Import the actual Keycloak class from keycloak-js to use as the DI token
// keycloak-js doesn't have the class field initializer issues that keycloak-angular has
import Keycloak from 'keycloak-js';

import { routes } from './app.routes';

/**
 * Mock Keycloak class for E2E tests.
 * This satisfies the DI requirement without connecting to a real Keycloak server.
 */
class MockKeycloak {
  authenticated = false;
  token: string | undefined = undefined;
  refreshToken: string | undefined = undefined;
  tokenParsed: unknown = undefined;
  realmAccess = { roles: [] as string[] };
  init() {
    return Promise.resolve(false);
  }
  login() {
    return Promise.resolve();
  }
  logout() {
    return Promise.resolve();
  }
  updateToken() {
    return Promise.resolve(false);
  }
  loadUserProfile() {
    return Promise.resolve({});
  }
}

// Create tokens that match keycloak-angular's tokens
// We can't import from keycloak-angular as it causes DI issues in E2E
const KEYCLOAK_EVENT_SIGNAL = new InjectionToken('Keycloak Events Signal');
const INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG = new InjectionToken(
  'Include the bearer token when explicitly defined int the URL pattern condition',
);

/**
 * Custom error handler that logs detailed error information for debugging
 */
class E2EErrorHandler implements ErrorHandler {
  handleError(error: unknown): void {
    console.error('E2E Error Handler - Full Error:', error);
    if (error instanceof Error) {
      console.error('E2E Error Handler - Message:', error.message);
      console.error('E2E Error Handler - Stack:', error.stack);
    }
    // Also check for nested errors
    const anyError = error as { rejection?: unknown; ngOriginalError?: unknown };
    if (anyError.rejection) {
      console.error('E2E Error Handler - Rejection:', anyError.rejection);
    }
    if (anyError.ngOriginalError) {
      console.error('E2E Error Handler - ngOriginalError:', anyError.ngOriginalError);
    }
  }
}

console.log('Using E2E app config (no Keycloak)');

export const appConfig: ApplicationConfig = {
  providers: [
    provideBrowserGlobalErrorListeners(),
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withFetch()),
    ...provideI18n(),
    { provide: LOCALE_ID, useValue: 'de-DE' },
    { provide: API_CONFIG, useExisting: ConfigService },
    {
      provide: APP_INITIALIZER,
      useFactory: initializeApp,
      deps: [HttpClient, ConfigService],
      multi: true,
    },
    // Custom error handler for debugging
    {
      provide: ErrorHandler,
      useClass: E2EErrorHandler,
    },
    // Mock Keycloak providers for E2E
    {
      provide: Keycloak,
      useClass: MockKeycloak,
    },
    {
      provide: KEYCLOAK_EVENT_SIGNAL,
      useValue: signal({ type: 'KeycloakAngularNotInitialized' }),
    },
    {
      provide: INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
      useValue: [],
    },
  ],
};
