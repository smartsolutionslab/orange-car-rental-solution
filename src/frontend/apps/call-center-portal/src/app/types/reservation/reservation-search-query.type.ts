/**
 * Reservation search query (extended for call-center)
 */
import type {
  CustomerId,
  VehicleId,
  ReservationStatus,
  ISODateString,
  LocationCode,
  CategoryCode,
  Price,
  ReservationSortField,
  SortOrder,
} from '@orange-car-rental/data-access';

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
