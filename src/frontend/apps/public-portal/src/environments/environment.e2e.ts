/**
 * E2E Test Environment
 *
 * This environment is used for Playwright E2E tests. It disables Keycloak
 * to allow tests to run without an authentication server.
 */
export const environment = {
  production: false,
  e2e: true,
  apiUrl: 'http://localhost:5000',
  keycloak: {
    url: '', // Empty URL will skip Keycloak initialization
    realm: 'orange-car-rental',
    clientId: 'public-portal',
  },
};
