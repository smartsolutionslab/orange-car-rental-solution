import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import {
  BaseCustomerService,
  type CustomerProfile,
  type UpdateCustomerProfileRequest,
  type CustomerId,
} from '@orange-car-rental/customer-api';

/**
 * Customer Service for the public portal
 * Extends BaseCustomerService with self-service profile operations
 */
@Injectable({
  providedIn: 'root',
})
export class CustomerService extends BaseCustomerService {
  /**
   * Get current user's profile
   * Uses the authenticated user's customer ID from the JWT token
   * @returns Observable of customer profile
   */
  getMyProfile(): Observable<CustomerProfile> {
    return this.http.get<CustomerProfile>(`${this.customersUrl}/profile`);
  }

  /**
   * Update customer profile
   * @param customerId Customer ID
   * @param profile Updated profile data
   * @returns Observable of updated customer profile
   */
  updateCustomerProfile(
    customerId: CustomerId,
    profile: UpdateCustomerProfileRequest,
  ): Observable<CustomerProfile> {
    return this.http.put<CustomerProfile>(`${this.customersUrl}/${customerId}/profile`, profile);
  }

  /**
   * Update current user's profile
   * @param profile Updated profile data
   * @returns Observable of updated customer profile
   */
  updateMyProfile(profile: UpdateCustomerProfileRequest): Observable<CustomerProfile> {
    return this.http.put<CustomerProfile>(`${this.customersUrl}/profile`, profile);
  }
}
