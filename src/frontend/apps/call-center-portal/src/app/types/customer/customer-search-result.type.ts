/**
 * Customer search result
 */
import type { Customer } from './customer.type';

export type CustomerSearchResult = {
  readonly customers: Customer[];
  readonly totalCount: number;
  readonly pageNumber: number;
  readonly pageSize: number;
  readonly totalPages: number;
  readonly hasPreviousPage?: boolean;
  readonly hasNextPage?: boolean;
};
