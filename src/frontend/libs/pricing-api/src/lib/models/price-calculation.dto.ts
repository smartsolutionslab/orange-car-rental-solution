/**
 * Price calculation result matching backend PriceCalculationResult
 * All prices include German VAT (19%)
 */
import type { Currency, Price } from '@orange-car-rental/shared';
import type { CategoryCode, DailyRate } from '@orange-car-rental/vehicle-api';
import type { TotalDays } from '@orange-car-rental/reservation-api';
import type { VatRate } from './vat-rate.type';

export type PriceCalculation = {
  readonly categoryCode: CategoryCode;
  readonly totalDays: TotalDays;
  readonly dailyRateNet: DailyRate;
  readonly dailyRateGross: DailyRate;
  readonly totalPriceNet: Price;
  readonly totalPriceGross: Price;
  readonly vatAmount: Price;
  readonly vatRate: VatRate;
  readonly currency: Currency;
};
