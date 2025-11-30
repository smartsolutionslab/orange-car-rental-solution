import { inject } from '@angular/core';
import type { CanActivateFn } from '@angular/router';
import Keycloak from 'keycloak-js';

/**
 * Auth guard for protecting routes
 * Uses the new Keycloak direct injection
 */
export const authGuard: CanActivateFn = async () => {
  const keycloak = inject(Keycloak);
  const authenticated = keycloak.authenticated ?? false;

  if (!authenticated) {
    await keycloak.login({
      redirectUri: window.location.origin + window.location.pathname
    });
    return false;
  }

  return true;
};
