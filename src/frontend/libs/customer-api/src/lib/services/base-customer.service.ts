import { Injectable, inject } from "@angular/core";
import { HttpClient } from "@angular/common/http";
import { Observable } from "rxjs";
import type { CustomerProfile } from "../models";
import type { CustomerId } from "@orange-car-rental/reservation-api";
import { API_CONFIG } from "@orange-car-rental/shared";

/**
 * Base service for accessing the Customer API
 * Contains common customer operations shared across all applications
 *
 * Applications must provide API_CONFIG token with their ConfigService:
 * @example
 * providers: [{ provide: API_CONFIG, useExisting: ConfigService }]
 */
@Injectable({
  providedIn: "root",
})
export class BaseCustomerService {
  protected readonly http = inject(HttpClient);
  protected readonly config = inject(API_CONFIG);

  protected get customersUrl(): string {
    return `${this.config.apiUrl}/api/customers`;
  }

  /**
   * Get customer profile by ID
   * @param customerId Customer ID
   * @returns Observable of customer profile
   */
  getCustomerProfile(customerId: CustomerId): Observable<CustomerProfile> {
    return this.http.get<CustomerProfile>(`${this.customersUrl}/${customerId}`);
  }
}
