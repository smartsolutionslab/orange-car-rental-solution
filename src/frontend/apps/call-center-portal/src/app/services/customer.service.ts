import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { switchMap } from 'rxjs/operators';
import { HttpParamsBuilder } from '@orange-car-rental/shared';
import { BaseCustomerService } from '@orange-car-rental/customer-api';
import type {
  Customer,
  CustomerSearchQuery,
  CustomerSearchResult,
  UpdateCustomerRequest,
} from '../types';

/**
 * Service for accessing the Customers API
 * Extends BaseCustomerService with call-center specific operations
 */
@Injectable({
  providedIn: 'root',
})
export class CustomerService extends BaseCustomerService {
  /**
   * Get customer by ID
   * @param id Customer ID
   * @returns Observable of customer details
   */
  getCustomer(id: string): Observable<Customer> {
    return this.http.get<Customer>(`${this.customersUrl}/${id}`);
  }

  /**
   * Search customers with optional filters
   * @param query Search query parameters
   * @returns Observable of search results with customers and pagination
   */
  searchCustomers(query?: CustomerSearchQuery): Observable<CustomerSearchResult> {
    const params = HttpParamsBuilder.fromObject(query);
    return this.http.get<CustomerSearchResult>(`${this.customersUrl}/search`, { params });
  }

  /**
   * Update customer information (profile and license)
   * @param id Customer ID
   * @param request Update customer request
   * @returns Observable of updated customer
   */
  updateCustomer(id: string, request: UpdateCustomerRequest): Observable<Customer> {
    // Split into two calls: profile update and license update
    const profileRequest = {
      profile: {
        firstName: request.firstName,
        lastName: request.lastName,
        phoneNumber: request.phoneNumber,
      },
      address: {
        street: request.street,
        city: request.city,
        postalCode: request.postalCode,
        country: request.country,
      },
    };

    const licenseRequest = {
      licenseNumber: request.licenseNumber,
      issueCountry: request.licenseIssueCountry,
      issueDate: request.licenseIssueDate,
      expiryDate: request.licenseExpiryDate,
    };

    // Update profile first, then license
    return this.http.put<void>(`${this.customersUrl}/${id}/profile`, profileRequest).pipe(
      switchMap(() => this.http.put<void>(`${this.customersUrl}/${id}/license`, licenseRequest)),
      switchMap(() => this.getCustomer(id)),
    );
  }
}
