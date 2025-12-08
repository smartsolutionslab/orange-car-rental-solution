import { Injectable } from '@angular/core';
import { BaseAuthService } from '@orange-car-rental/auth';

/**
 * Authentication service for call-center portal
 * Extends BaseAuthService with all common Keycloak authentication methods
 */
@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseAuthService {
  // All methods inherited from BaseAuthService:
  // - isAuthenticated()
  // - getUserProfile()
  // - getUserRoles()
  // - hasRole(role)
  // - getToken()
  // - login()
  // - logout()
  // - getUsername()
}
