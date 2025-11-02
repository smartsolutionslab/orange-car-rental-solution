/**
 * Price calculation request matching backend CalculatePriceQuery
 */
export interface PriceCalculationRequest {
  categoryCode: string;
  pickupDate: string; // ISO 8601 format
  returnDate: string; // ISO 8601 format
  locationCode?: string;
}

/**
 * Price calculation result matching backend PriceCalculationResult
 * All prices include German VAT (19%)
 */
export interface PriceCalculation {
  categoryCode: string;
  totalDays: number;
  dailyRateNet: number;
  dailyRateGross: number;
  totalPriceNet: number;
  totalPriceGross: number;
  vatAmount: number;
  vatRate: number;
  currency: string;
}
