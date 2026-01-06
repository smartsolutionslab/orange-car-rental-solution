/**
 * Generic paginated response type
 */
import type { TotalCount } from "./total-count.type";
import type { PageNumber } from "./page-number.type";
import type { PageSize } from "./page-size.type";
import type { TotalPages } from "./total-pages.type";

export type PaginatedResponse<T> = {
  readonly items: T[];
  readonly totalCount: TotalCount;
  readonly pageNumber: PageNumber;
  readonly pageSize: PageSize;
  readonly totalPages: TotalPages;
  readonly hasPreviousPage?: boolean;
  readonly hasNextPage?: boolean;
};
