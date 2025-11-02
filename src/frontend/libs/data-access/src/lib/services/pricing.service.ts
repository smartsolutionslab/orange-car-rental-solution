import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PriceCalculation, PriceCalculationRequest } from '../models/pricing.model';
import { ConfigService } from './config.service';

/**
 * Service for accessing the Pricing API
 * Handles automatic price calculation for vehicle rentals
 */
@Injectable({
  providedIn: 'root'
})
export class PricingService {
  private readonly http = inject(HttpClient);
  private readonly configService = inject(ConfigService);

  private get apiUrl(): string {
    return `${this.configService.apiUrl}/api/pricing`;
  }

  /**
   * Calculate rental price based on vehicle category, dates, and location
   * @param request Price calculation request
   * @returns Observable of price calculation with detailed breakdown
   */
  calculatePrice(request: PriceCalculationRequest): Observable<PriceCalculation> {
    return this.http.post<PriceCalculation>(`${this.apiUrl}/calculate`, request);
  }
}
