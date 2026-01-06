/**
 * Guest reservation request matching the backend CreateGuestReservationRequest
 * Used for creating reservations for users who haven't registered yet
 */
import type { ReservationDetails } from "./reservation-details.dto";
import type { CustomerDetails } from "./customer-details.dto";
import type { AddressDetails } from "./address-details.dto";
import type { DriversLicenseDetails } from "./drivers-license-details.dto";

export type GuestReservationRequest = {
  readonly reservation: ReservationDetails;
  readonly customer: CustomerDetails;
  readonly address: AddressDetails;
  readonly driversLicense: DriversLicenseDetails;
};
