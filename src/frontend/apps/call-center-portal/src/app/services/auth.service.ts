import { inject, Injectable } from '@angular/core';
import Keycloak from 'keycloak-js';
import type { KeycloakProfile } from 'keycloak-js';
import { logError } from '@orange-car-rental/util';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly keycloak = inject(Keycloak);

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    return this.keycloak.authenticated ?? false;
  }

  /**
   * Get user profile
   */
  async getUserProfile(): Promise<KeycloakProfile | null> {
    try {
      if (this.isAuthenticated()) {
        return await this.keycloak.loadUserProfile();
      }
      return null;
    } catch (error) {
      logError('AuthService', 'Error loading user profile', error);
      return null;
    }
  }

  /**
   * Get user roles
   */
  getUserRoles(): string[] {
    return this.keycloak.realmAccess?.roles ?? [];
  }

  /**
   * Check if user has specific role
   */
  hasRole(role: string): boolean {
    return this.getUserRoles().includes(role);
  }

  /**
   * Get access token
   */
  async getToken(): Promise<string> {
    await this.keycloak.updateToken(30);
    return this.keycloak.token ?? '';
  }

  /**
   * Login user
   */
  login(): void {
    this.keycloak.login();
  }

  /**
   * Logout user
   */
  logout(): void {
    this.keycloak.logout({ redirectUri: window.location.origin });
  }

  /**
   * Get username
   */
  getUsername(): string {
    return this.keycloak.tokenParsed?.['preferred_username'] ?? '';
  }
}
