/**
 * Price calculation request matching backend CalculatePriceQuery
 */
import type { ISODateString } from '../common';
import type { CategoryCode } from '../vehicle';
import type { LocationCode } from '../location';

export type PriceCalculationRequest = {
  readonly categoryCode: CategoryCode;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly locationCode?: LocationCode;
};
