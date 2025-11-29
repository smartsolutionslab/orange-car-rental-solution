/**
 * Reservation search filters
 */
import type { ISODateString, SortOrder, PageNumber, PageSize } from '../common';
import type { CustomerId } from './customer-id.type';
import type { ReservationStatus } from './reservation-status.enum';
import type { ReservationSortField } from './reservation-sort-field.enum';

export type ReservationSearchFilters = {
  readonly customerId?: CustomerId;
  readonly status?: ReservationStatus;
  readonly pickupDateFrom?: ISODateString;
  readonly pickupDateTo?: ISODateString;
  readonly sortBy?: ReservationSortField;
  readonly sortOrder?: SortOrder;
  readonly pageNumber?: PageNumber;
  readonly pageSize?: PageSize;
};
