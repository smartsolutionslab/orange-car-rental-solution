/**
 * Booking form value type
 */
export interface BookingFormValue {
  readonly vehicleId: string;
  readonly categoryCode: string;
  readonly pickupDate: string;
  readonly returnDate: string;
  readonly pickupLocationCode: string;
  readonly dropoffLocationCode: string;
  readonly firstName: string;
  readonly lastName: string;
  readonly email: string;
  readonly phoneNumber: string;
  readonly dateOfBirth: string;
  readonly street: string;
  readonly city: string;
  readonly postalCode: string;
  readonly country: string;
  readonly licenseNumber: string;
  readonly licenseIssueCountry: string;
  readonly licenseIssueDate: string;
  readonly licenseExpiryDate: string;
  readonly updateMyProfile: boolean;
};
