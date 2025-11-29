/**
 * Customer search query
 */
import type { EmailAddress, PhoneNumber } from '@orange-car-rental/data-access';

export type CustomerSearchQuery = {
  readonly email?: EmailAddress;
  readonly phoneNumber?: PhoneNumber;
  readonly lastName?: string;
  readonly pageNumber?: number;
  readonly pageSize?: number;
};
