/**
 * Reservation search result
 */
import type { Reservation } from './reservation.type';

export type ReservationSearchResult = {
  readonly reservations: Reservation[];
  readonly totalCount: number;
  readonly pageNumber: number;
  readonly pageSize: number;
  readonly totalPages: number;
  readonly hasPreviousPage?: boolean;
  readonly hasNextPage?: boolean;
};
