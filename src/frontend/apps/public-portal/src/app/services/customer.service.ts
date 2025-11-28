import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CustomerProfile, UpdateCustomerProfileRequest } from './customer.model';
import { CustomerId } from './reservation.model';
import { ConfigService } from './config.service';

/**
 * Customer Service
 * Handles customer profile operations
 */
@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly http = inject(HttpClient);
  private readonly configService = inject(ConfigService);

  private get apiUrl(): string {
    return `${this.configService.apiUrl}/api/customers`;
  }

  /**
   * Get customer profile by ID
   * @param customerId Customer ID
   * @returns Observable of customer profile
   */
  getCustomerProfile(customerId: CustomerId): Observable<CustomerProfile> {
    return this.http.get<CustomerProfile>(`${this.apiUrl}/${customerId}`);
  }

  /**
   * Get current user's profile
   * Uses the authenticated user's customer ID from the JWT token
   * @returns Observable of customer profile
   */
  getMyProfile(): Observable<CustomerProfile> {
    return this.http.get<CustomerProfile>(`${this.apiUrl}/profile`);
  }

  /**
   * Update customer profile
   * @param customerId Customer ID
   * @param profile Updated profile data
   * @returns Observable of updated customer profile
   */
  updateCustomerProfile(customerId: CustomerId, profile: UpdateCustomerProfileRequest): Observable<CustomerProfile> {
    return this.http.put<CustomerProfile>(`${this.apiUrl}/${customerId}/profile`, profile);
  }

  /**
   * Update current user's profile
   * @param profile Updated profile data
   * @returns Observable of updated customer profile
   */
  updateMyProfile(profile: UpdateCustomerProfileRequest): Observable<CustomerProfile> {
    return this.http.put<CustomerProfile>(`${this.apiUrl}/profile`, profile);
  }
}
