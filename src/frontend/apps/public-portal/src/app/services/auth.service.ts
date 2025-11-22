import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { KeycloakService } from 'keycloak-angular';
import { KeycloakProfile } from 'keycloak-js';
import { firstValueFrom } from 'rxjs';
import { environment } from '../../environments/environment';

export interface RegisterData {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  acceptMarketing: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private keycloakUrl = environment.keycloak?.url || 'http://localhost:9080';
  private realm = environment.keycloak?.realm || 'orange-car-rental';
  private clientId = environment.keycloak?.clientId || 'public-portal';

  constructor(
    private keycloakService: KeycloakService,
    private http: HttpClient
  ) {}

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
      console.error('Error loading user profile', error);
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
   * Login user with redirect to Keycloak (legacy method)
   */
  loginRedirect(): void {
    this.keycloakService.login();
  }

  /**
   * Login user with email and password using Keycloak REST API
   * @param email User email
   * @param password User password
   * @param rememberMe Whether to remember the user
   */
  async login(email: string, password: string, rememberMe: boolean = false): Promise<void> {
    try {
      const tokenUrl = `${this.keycloakUrl}/realms/${this.realm}/protocol/openid-connect/token`;

      const body = new URLSearchParams();
      body.set('client_id', this.clientId);
      body.set('username', email);
      body.set('password', password);
      body.set('grant_type', 'password');

      const headers = new HttpHeaders({
        'Content-Type': 'application/x-www-form-urlencoded'
      });

      const response = await firstValueFrom(
        this.http.post<any>(tokenUrl, body.toString(), { headers })
      );

      // Store tokens
      if (response.access_token) {
        // Initialize Keycloak with the obtained token
        await this.keycloakService.init({
          config: {
            url: this.keycloakUrl,
            realm: this.realm,
            clientId: this.clientId
          },
          initOptions: {
            token: response.access_token,
            refreshToken: response.refresh_token,
            checkLoginIframe: false
          }
        });
      }
    } catch (error: any) {
      console.error('Login error:', error);
      throw error;
    }
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

  /**
   * Register a new user
   * @param userData User registration data
   */
  async register(userData: RegisterData): Promise<void> {
    try {
      const registrationUrl = `${this.keycloakUrl}/realms/${this.realm}/users`;

      const userPayload = {
        username: userData.email,
        email: userData.email,
        firstName: userData.firstName,
        lastName: userData.lastName,
        enabled: true,
        emailVerified: false,
        credentials: [{
          type: 'password',
          value: userData.password,
          temporary: false
        }],
        attributes: {
          phoneNumber: [userData.phoneNumber],
          dateOfBirth: [userData.dateOfBirth],
          acceptMarketing: [userData.acceptMarketing.toString()]
        }
      };

      // Use Keycloak registration endpoint (requires realm setting: registrationAllowed=true)
      const registerUrl = `${this.keycloakUrl}/realms/${this.realm}/protocol/openid-connect/registrations`;

      const headers = new HttpHeaders({
        'Content-Type': 'application/json'
      });

      await firstValueFrom(
        this.http.post(registerUrl, userPayload, { headers })
      );

      // After successful registration, log the user in
      await this.login(userData.email, userData.password, false);
    } catch (error: any) {
      console.error('Registration error:', error);
      throw error;
    }
  }

  /**
   * Send password reset email
   * @param email User email address
   */
  async resetPassword(email: string): Promise<void> {
    try {
      // Keycloak requires admin credentials for password reset
      // In production, this should be handled by a backend API endpoint
      // For now, we'll use the Keycloak forgot-password endpoint
      const resetUrl = `${this.keycloakUrl}/realms/${this.realm}/login-actions/reset-credentials`;

      const headers = new HttpHeaders({
        'Content-Type': 'application/x-www-form-urlencoded'
      });

      const body = new URLSearchParams();
      body.set('username', email);

      await firstValueFrom(
        this.http.post(resetUrl, body.toString(), { headers })
      );
    } catch (error: any) {
      console.error('Password reset error:', error);
      throw error;
    }
  }
}
