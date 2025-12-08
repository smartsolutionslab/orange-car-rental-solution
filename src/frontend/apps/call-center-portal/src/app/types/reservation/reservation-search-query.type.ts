/**
 * Reservation search query (extended for call-center)
 */
import type { CustomerId } from '@orange-car-rental/reservation-api';
import type { VehicleId, CategoryCode } from '@orange-car-rental/vehicle-api';
import type { ReservationStatus, ReservationSortField } from '@orange-car-rental/reservation-api';
import type { ISODateString, Price, SortOrder } from '@orange-car-rental/shared';
import type { LocationCode } from '@orange-car-rental/location-api';

export type ReservationSearchQuery = {
  readonly customerId?: CustomerId;
  readonly vehicleId?: VehicleId;
  readonly status?: ReservationStatus;
  readonly pickupDateFrom?: ISODateString;
  readonly pickupDateTo?: ISODateString;
  readonly locationCode?: LocationCode;
  readonly categoryCode?: CategoryCode;
  readonly minPrice?: Price;
  readonly maxPrice?: Price;
  readonly sortBy?: ReservationSortField;
  readonly sortOrder?: SortOrder;
  readonly pageNumber?: number;
  readonly pageSize?: number;
};
