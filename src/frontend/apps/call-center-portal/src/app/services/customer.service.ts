import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Customer, CustomerSearchQuery, CustomerSearchResult, UpdateCustomerRequest } from './customer.model';
import { ConfigService } from './config.service';

/**
 * Service for accessing the Customers API
 * Handles customer lookup and management operations
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
   * Get customer by ID
   * @param id Customer ID
   * @returns Observable of customer details
   */
  getCustomer(id: string): Observable<Customer> {
    return this.http.get<Customer>(`${this.apiUrl}/${id}`);
  }

  /**
   * Search customers with optional filters
   * @param query Search query parameters
   * @returns Observable of search results with customers and pagination
   */
  searchCustomers(query?: CustomerSearchQuery): Observable<CustomerSearchResult> {
    let params = new HttpParams();

    if (query) {
      if (query.email) params = params.set('email', query.email);
      if (query.phoneNumber) params = params.set('phoneNumber', query.phoneNumber);
      if (query.lastName) params = params.set('lastName', query.lastName);
      if (query.pageNumber !== undefined) params = params.set('pageNumber', query.pageNumber.toString());
      if (query.pageSize !== undefined) params = params.set('pageSize', query.pageSize.toString());
    }

    return this.http.get<CustomerSearchResult>(this.apiUrl, { params });
  }

  /**
   * Update customer information
   * @param id Customer ID
   * @param request Update customer request
   * @returns Observable of updated customer
   */
  updateCustomer(id: string, request: UpdateCustomerRequest): Observable<Customer> {
    return this.http.put<Customer>(`${this.apiUrl}/${id}`, request);
  }
}
