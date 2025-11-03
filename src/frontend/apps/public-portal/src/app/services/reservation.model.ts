/**
 * Guest reservation request matching the backend CreateGuestReservationCommand
 * Used for creating reservations for users who haven't registered yet
 */
export interface GuestReservationRequest {
  // Vehicle and booking details
  vehicleId: string;
  categoryCode: string;
  pickupDate: string;  // ISO date string
  returnDate: string;  // ISO date string
  pickupLocationCode: string;
  dropoffLocationCode: string;

  // Customer details
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;  // ISO date string

  // Address details
  street: string;
  city: string;
  postalCode: string;
  country: string;

  // Driver's license details
  licenseNumber: string;
  licenseIssueCountry: string;
  licenseIssueDate: string;  // ISO date string
  licenseExpiryDate: string;  // ISO date string
}

/**
 * Response from creating a guest reservation
 * Includes both customer and reservation IDs along with pricing breakdown
 */
export interface GuestReservationResponse {
  customerId: string;
  reservationId: string;
  totalPriceNet: number;
  totalPriceVat: number;
  totalPriceGross: number;
  currency: string;
}

/**
 * Reservation details
 */
export interface Reservation {
  id: string;
  vehicleId: string;
  customerId: string;
  pickupDate: string;
  returnDate: string;
  pickupLocationCode: string;
  dropoffLocationCode: string;
  totalPriceNet: number;
  totalPriceVat: number;
  totalPriceGross: number;
  currency: string;
  status: string;
}
