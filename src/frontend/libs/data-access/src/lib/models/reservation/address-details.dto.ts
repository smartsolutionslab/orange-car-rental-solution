/**
 * Address details
 */
import type { PostalCode } from '../common/postal-code.type';
import type { CountryCode } from '../common/country-code.type';
import type { StreetAddress, CityName } from '../location';

export type AddressDetails = {
  readonly street: StreetAddress;
  readonly city: CityName;
  readonly postalCode: PostalCode;
  readonly country: CountryCode;
};
