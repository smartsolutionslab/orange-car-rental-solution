/**
 * Price calculation result matching backend PriceCalculationResult
 * All prices include German VAT (19%)
 */
import type { Currency, Price } from '../common';
import type { CategoryCode, DailyRate } from '../vehicle';
import type { TotalDays } from '../reservation';
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
