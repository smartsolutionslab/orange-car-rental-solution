import { Injectable, inject } from '@angular/core';
import type { CanActivateFn } from '@angular/router';
import { KeycloakService } from 'keycloak-angular';

@Injectable({
  providedIn: 'root'
})
export class AuthGuardService {
  constructor(
    private keycloakService: KeycloakService
  ) {}

  async canActivate(): Promise<boolean> {
    const authenticated = await this.keycloakService.isLoggedIn();

    if (!authenticated) {
      // Redirect to login
      await this.keycloakService.login({
        redirectUri: window.location.origin + window.location.pathname
      });
      return false;
    }

    return true;
  }
}

/**
 * Auth guard for protecting routes
 */
export const authGuard: CanActivateFn = async () => {
  const authGuardService = inject(AuthGuardService);
  return authGuardService.canActivate();
};
