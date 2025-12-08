import { inject, Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { BaseAuthService } from '@orange-car-rental/auth';
import { environment } from '../../environments/environment';
import { logError } from '@orange-car-rental/util';
import type { RegisterData, TokenResponse } from '../types';
export type { RegisterData } from '../types';

/**
 * Authentication service for public portal
 * Extends BaseAuthService with additional methods for:
 * - Direct login with email/password
 * - User registration
 * - Password reset
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseAuthService {
  private readonly http = inject(HttpClient);

  private keycloakUrl = environment.keycloak?.url || 'http://localhost:9080';
  private realm = environment.keycloak?.realm || 'orange-car-rental';
  private clientId = environment.keycloak?.clientId || 'public-portal';

  /**
   * Login user with email and password using Keycloak REST API
   * @param email User email
   * @param password User password
   * @param rememberMe Whether to remember the user
   */
  async loginWithPassword(email: string, password: string, _rememberMe = false): Promise<void> {
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
        this.http.post<TokenResponse>(tokenUrl, body.toString(), { headers })
      );

      // Store tokens and reinitialize Keycloak
      if (response.access_token) {
        await this.keycloak.init({
          token: response.access_token,
          refreshToken: response.refresh_token,
          checkLoginIframe: false
        });
      }
    } catch (error: unknown) {
      logError('AuthService', 'Login error', error);
      throw error;
    }
  }

  /**
   * Register a new user
   * @param userData User registration data
   */
  async register(userData: RegisterData): Promise<void> {
    try {
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
      await this.loginWithPassword(userData.email, userData.password, false);
    } catch (error: unknown) {
      logError('AuthService', 'Registration error', error);
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
    } catch (error: unknown) {
      logError('AuthService', 'Password reset error', error);
      throw error;
    }
  }
}
