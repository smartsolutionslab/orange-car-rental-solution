/**
 * Customer search query
 * Matches backend SearchCustomersRequest
 */
import type { EmailAddress, PhoneNumber, ISODateString } from '@orange-car-rental/shared';

export interface CustomerSearchQuery {
  readonly searchTerm?: string; // Searches in both firstName and lastName
  readonly email?: EmailAddress;
  readonly phoneNumber?: PhoneNumber;
  readonly status?: string;
  readonly city?: string;
  readonly postalCode?: string;
  readonly minAge?: number;
  readonly maxAge?: number;
  readonly licenseExpiringWithinDays?: number;
  readonly registeredFrom?: ISODateString;
  readonly registeredTo?: ISODateString;
  readonly sortBy?: string;
  readonly sortDescending?: boolean;
  readonly pageNumber?: number;
  readonly pageSize?: number;
}
