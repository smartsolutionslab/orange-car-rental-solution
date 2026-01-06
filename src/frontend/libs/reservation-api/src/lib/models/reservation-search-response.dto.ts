/**
 * Paginated search results
 */
import type {
  PageNumber,
  PageSize,
  TotalCount,
  TotalPages,
} from "@orange-car-rental/shared";
import type { Reservation } from "./reservation.dto";

export type ReservationSearchResponse = {
  readonly items: Reservation[];
  readonly totalCount: TotalCount;
  readonly pageNumber: PageNumber;
  readonly pageSize: PageSize;
  readonly totalPages: TotalPages;
};
