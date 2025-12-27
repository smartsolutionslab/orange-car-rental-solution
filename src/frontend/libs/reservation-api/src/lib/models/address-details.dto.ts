/**
 * Address details
 */
import type { PostalCode, CountryCode } from '@orange-car-rental/shared';
import type { StreetAddress, CityName } from '@orange-car-rental/location-api';

export type AddressDetails = {
  readonly street: StreetAddress;
  readonly city: CityName;
  readonly postalCode: PostalCode;
  readonly country: CountryCode;
};
