import { inject, Injectable } from "@angular/core";
import Keycloak from "keycloak-js";
import type { KeycloakProfile } from "keycloak-js";
import { logError } from "@orange-car-rental/util";

/**
 * Creates a mock Keycloak instance for use when Keycloak is not configured.
 * This allows the application to run in E2E test mode without a Keycloak server.
 */
function createMockKeycloak(): Keycloak {
  return {
    authenticated: false,
    token: undefined,
    refreshToken: undefined,
    tokenParsed: undefined,
    realmAccess: { roles: [] },
    init: () => Promise.resolve(false),
    login: () => Promise.resolve(),
    logout: () => Promise.resolve(),
    updateToken: () => Promise.resolve(false),
    loadUserProfile: () => Promise.resolve({}),
  } as unknown as Keycloak;
}

/**
 * Base authentication service using Keycloak
 * Provides common authentication methods that can be used directly
 * or extended by portal-specific auth services.
 *
 * When Keycloak is not available (e.g., in E2E tests), this service
 * falls back to a mock implementation that reports user as not authenticated.
 */
@Injectable({
  providedIn: "root",
})
export class BaseAuthService {
  protected readonly keycloak: Keycloak = inject(Keycloak, { optional: true }) ?? createMockKeycloak();

  /**
   * Check if user is authenticated
   */
  isAuthenticated(): boolean {
    return this.keycloak.authenticated ?? false;
  }

  /**
   * Get user profile from Keycloak
   */
  async getUserProfile(): Promise<KeycloakProfile | null> {
    try {
      if (this.isAuthenticated()) {
        return await this.keycloak.loadUserProfile();
      }
      return null;
    } catch (error) {
      logError("BaseAuthService", "Error loading user profile", error);
      return null;
    }
  }

  /**
   * Get user roles from Keycloak realm access
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
   * Get access token, refreshing if needed
   */
  async getToken(): Promise<string> {
    await this.keycloak.updateToken(30);
    return this.keycloak.token ?? "";
  }

  /**
   * Login user via Keycloak redirect
   */
  login(): void {
    this.keycloak.login();
  }

  /**
   * Logout user and redirect to origin
   */
  logout(): void {
    this.keycloak.logout({ redirectUri: window.location.origin });
  }

  /**
   * Get username from token
   */
  getUsername(): string {
    return this.keycloak.tokenParsed?.["preferred_username"] ?? "";
  }

  /**
   * Call center agent roles
   */
  private readonly agentRoles = [
    "call-center-agent",
    "call-center-supervisor",
    "admin",
  ] as const;

  /**
   * Check if user is a call center agent (has any agent role)
   */
  isCallCenterAgent(): boolean {
    return this.agentRoles.some((role) => this.hasRole(role));
  }

  /**
   * Check if user is a customer (not an agent)
   */
  isCustomer(): boolean {
    return this.isAuthenticated() && !this.isCallCenterAgent();
  }

  /**
   * Get appropriate redirect URL based on user roles after login
   * @param returnUrl - Optional return URL from query params
   */
  getPostLoginRedirect(returnUrl?: string | null): string {
    // If there's a specific return URL (not root or login), use it
    if (returnUrl && returnUrl !== "/" && returnUrl !== "/login") {
      return returnUrl;
    }

    // Role-based default redirect
    if (this.isCallCenterAgent()) {
      return "/admin";
    }

    return "/my-bookings";
  }
}
