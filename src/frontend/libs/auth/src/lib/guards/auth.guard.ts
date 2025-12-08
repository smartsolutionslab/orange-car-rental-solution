import { inject } from '@angular/core';
import { Router } from '@angular/router';
import type { CanActivateFn } from '@angular/router';
import Keycloak from 'keycloak-js';
import { logError } from '@orange-car-rental/util';

/**
 * Authentication guard for protecting routes
 * Redirects to Keycloak login if user is not authenticated
 */
export const authGuard: CanActivateFn = async () => {
  const keycloak = inject(Keycloak);
  const router = inject(Router);

  try {
    const authenticated = keycloak.authenticated ?? false;

    if (!authenticated) {
      await keycloak.login({
        redirectUri: window.location.origin + window.location.pathname
      });
      return false;
    }

    return true;
  } catch (error) {
    logError('AuthGuard', 'Auth guard error', error);
    router.navigate(['/login']);
    return false;
  }
};
