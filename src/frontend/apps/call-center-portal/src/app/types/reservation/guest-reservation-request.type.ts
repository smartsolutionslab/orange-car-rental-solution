/**
 * Guest reservation request (flat structure for call-center)
 */
import type {
  VehicleId,
  CategoryCode,
  LocationCode,
  ISODateString,
  EmailAddress,
  PhoneNumber,
  PostalCode,
  CountryCode,
  LicenseNumber,
} from '@orange-car-rental/data-access';

export type GuestReservationRequest = {
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
};
