/**
 * Price calculation request matching backend CalculatePriceQuery
 */
import type { ISODateString } from "@orange-car-rental/shared";
import type { CategoryCode } from "@orange-car-rental/vehicle-api";
import type { LocationCode } from "@orange-car-rental/location-api";

export type PriceCalculationRequest = {
  readonly categoryCode: CategoryCode;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly locationCode?: LocationCode;
};
