import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { CustomerProfile, UpdateCustomerProfileRequest } from './customer.model';

/**
 * Customer Service
 * Handles customer profile operations
 */
@Injectable({
  providedIn: 'root'
})
export class CustomerService {
  private readonly apiUrl = environment.apiUrl || 'http://localhost:5000';

  constructor(private http: HttpClient) {}

  /**
   * Get customer profile by ID
   * @param customerId Customer ID
   * @returns Observable of customer profile
   */
  getCustomerProfile(customerId: string): Observable<CustomerProfile> {
    return this.http.get<CustomerProfile>(`${this.apiUrl}/api/customers/${customerId}`);
  }

  /**
   * Get current user's profile
   * Uses the authenticated user's customer ID from the JWT token
   * @returns Observable of customer profile
   */
  getMyProfile(): Observable<CustomerProfile> {
    return this.http.get<CustomerProfile>(`${this.apiUrl}/api/customers/profile`);
  }

  /**
   * Update customer profile
   * @param customerId Customer ID
   * @param profile Updated profile data
   * @returns Observable of updated customer profile
   */
  updateCustomerProfile(customerId: string, profile: UpdateCustomerProfileRequest): Observable<CustomerProfile> {
    return this.http.put<CustomerProfile>(`${this.apiUrl}/api/customers/${customerId}/profile`, profile);
  }

  /**
   * Update current user's profile
   * @param profile Updated profile data
   * @returns Observable of updated customer profile
   */
  updateMyProfile(profile: UpdateCustomerProfileRequest): Observable<CustomerProfile> {
    return this.http.put<CustomerProfile>(`${this.apiUrl}/api/customers/profile`, profile);
  }
}
