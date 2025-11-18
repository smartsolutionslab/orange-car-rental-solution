import { KeycloakService } from 'keycloak-angular';

/**
 * Initialize Keycloak with configuration
 */
export function initializeKeycloak(keycloak: KeycloakService) {
  return () =>
    keycloak.init({
      config: {
        url: 'http://localhost:8080',
        realm: 'orange-car-rental',
        clientId: 'call-center-portal'
      },
      initOptions: {
        onLoad: 'check-sso',
        silentCheckSsoRedirectUri:
          window.location.origin + '/assets/silent-check-sso.html',
        checkLoginIframe: false
      },
      bearerExcludedUrls: ['/assets']
    });
}
