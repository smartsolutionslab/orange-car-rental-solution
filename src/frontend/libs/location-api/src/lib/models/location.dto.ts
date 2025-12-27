/**
 * Location model representing a rental location
 */
import type { PostalCode } from '@orange-car-rental/shared';
import type { LocationCode } from './location-code.type';
import type { LocationName } from './location-name.type';
import type { CityName } from './city-name.type';
import type { StreetAddress } from './street-address.type';
import type { FullAddress } from './full-address.type';
import type { LocationStatus } from './location-status.enum';

export type Location = {
  readonly code: LocationCode;
  readonly name: LocationName;
  readonly street: StreetAddress;
  readonly city: CityName;
  readonly postalCode: PostalCode;
  readonly fullAddress: FullAddress;
  readonly status?: LocationStatus;
};
