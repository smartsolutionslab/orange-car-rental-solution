import { Injectable } from '@angular/core';
import { KeycloakService } from 'keycloak-angular';
import type { KeycloakProfile } from 'keycloak-js';
import { logError } from '@orange-car-rental/util';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private keycloakService: KeycloakService) {}

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    return this.keycloakService.isLoggedIn();
  }

  /**
   * Get user profile
   */
  async getUserProfile(): Promise<KeycloakProfile | null> {
    try {
      if (this.isAuthenticated()) {
        return await this.keycloakService.loadUserProfile();
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
    return this.keycloakService.getUserRoles();
  }

  /**
   * Check if user has specific role
   */
  hasRole(role: string): boolean {
    return this.keycloakService.isUserInRole(role);
  }

  /**
   * Get access token
   */
  getToken(): Promise<string> {
    return this.keycloakService.getToken();
  }

  /**
   * Login user
   */
  login(): void {
    this.keycloakService.login();
  }

  /**
   * Logout user
   */
  logout(): void {
    this.keycloakService.logout(window.location.origin);
  }

  /**
   * Get username
   */
  getUsername(): string {
    return this.keycloakService.getUsername();
  }
}
