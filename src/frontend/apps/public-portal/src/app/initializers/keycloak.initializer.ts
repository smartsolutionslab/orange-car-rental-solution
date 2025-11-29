import { KeycloakService } from 'keycloak-angular';
import { environment } from '../../environments/environment';

/**
 * Initialize Keycloak with configuration
 * Gracefully handles when Keycloak is not available
 */
export function initializeKeycloak(keycloak: KeycloakService): () => Promise<void> {
  return async () => {
    try {
      await keycloak.init({
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
        bearerExcludedUrls: ['/assets'],
        enableBearerInterceptor: false
      });
    } catch (error) {
      console.warn('Keycloak initialization failed - continuing without auth:', error);
      // Allow app to continue even if Keycloak is not available
    }
  };
}
