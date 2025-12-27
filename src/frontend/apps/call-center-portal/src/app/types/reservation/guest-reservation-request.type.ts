/**
 * Guest reservation request (flat structure for call-center)
 */
import type { VehicleId, CategoryCode } from '@orange-car-rental/vehicle-api';
import type { LocationCode } from '@orange-car-rental/location-api';
import type {
  ISODateString,
  EmailAddress,
  PhoneNumber,
  PostalCode,
  CountryCode,
} from '@orange-car-rental/shared';
import type { LicenseNumber } from '@orange-car-rental/reservation-api';

export interface GuestReservationRequest {
  readonly vehicleId: VehicleId;
  readonly categoryCode: CategoryCode;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
  readonly firstName: string;
  readonly lastName: string;
  readonly email: EmailAddress;
  readonly phoneNumber: PhoneNumber;
  readonly dateOfBirth: ISODateString;
  readonly street: string;
  readonly city: string;
  readonly postalCode: PostalCode;
  readonly country: CountryCode;
  readonly licenseNumber: LicenseNumber;
  readonly licenseIssueCountry: CountryCode;
  readonly licenseIssueDate: ISODateString;
  readonly licenseExpiryDate: ISODateString;
}
