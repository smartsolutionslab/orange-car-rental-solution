import type { Provider, EnvironmentProviders } from '@angular/core';
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

/**
 * Keycloak configuration options
 */
export type KeycloakConfig = {
  readonly url: string;
  readonly realm: string;
  readonly clientId: string;
};

/**
 * Options for Keycloak provider
 */
export type KeycloakProviderOptions = {
  /**
   * Keycloak server configuration
   */
  readonly config: KeycloakConfig;
  /**
   * URL pattern to include bearer token for (regex)
   * @default /^(https?:\/\/[^/]+)?(\/api)(\/.*)?$/i
   */
  readonly bearerTokenUrlPattern?: RegExp;
  /**
   * Session timeout in milliseconds for auto-refresh
   * @default 300000 (5 minutes)
   */
  readonly sessionTimeout?: number;
  /**
   * What happens on inactivity timeout
   * @default 'logout'
   */
  readonly onInactivityTimeout?: 'login' | 'logout' | 'none';
};

/**
 * Default URL pattern for API endpoints
 */
const DEFAULT_API_URL_PATTERN = /^(https?:\/\/[^/]+)?(\/api)(\/.*)?$/i;

/**
 * Creates Keycloak providers with common configuration
 * Reduces boilerplate in app.config.ts files
 *
 * @param options Keycloak configuration options
 * @returns Array of providers for Keycloak integration
 */
export function provideKeycloakAuth(options: KeycloakProviderOptions): (Provider | EnvironmentProviders)[] {
  const urlPattern = options.bearerTokenUrlPattern ?? DEFAULT_API_URL_PATTERN;
  const sessionTimeout = options.sessionTimeout ?? 300000;
  const onInactivityTimeout = options.onInactivityTimeout ?? 'logout';

  const urlCondition = createInterceptorCondition<IncludeBearerTokenCondition>({
    urlPattern
  });

  return [
    {
      provide: INCLUDE_BEARER_TOKEN_INTERCEPTOR_CONFIG,
      useValue: [urlCondition]
    },
    provideKeycloak({
      config: {
        url: options.config.url,
        realm: options.config.realm,
        clientId: options.config.clientId
      },
      initOptions: {
        onLoad: 'check-sso',
        silentCheckSsoRedirectUri: `${window.location.origin}/assets/silent-check-sso.html`,
        checkLoginIframe: false
      },
      features: [
        withAutoRefreshToken({
          onInactivityTimeout,
          sessionTimeout
        })
      ],
      providers: [AutoRefreshTokenService, UserActivityService]
    })
  ];
}

/**
 * Re-export the bearer token interceptor for use with provideHttpClient
 */
export { includeBearerTokenInterceptor };
