/**
 * Vehicle and reservation details
 */
export interface ReservationDetails {
  vehicleId: string;
  categoryCode: string;
  pickupDate: string;  // ISO date string
  returnDate: string;  // ISO date string
  pickupLocationCode: string;
  dropoffLocationCode: string;
}

/**
 * Customer details
 */
export interface CustomerDetails {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;  // ISO date string
}

/**
 * Address details
 */
export interface AddressDetails {
  street: string;
  city: string;
  postalCode: string;
  country: string;
}

/**
 * Driver's license details
 */
export interface DriversLicenseDetails {
  licenseNumber: string;
  licenseIssueCountry: string;
  licenseIssueDate: string;  // ISO date string
  licenseExpiryDate: string;  // ISO date string
}

/**
 * Guest reservation request matching the backend CreateGuestReservationRequest
 * Used for creating reservations for users who haven't registered yet
 */
export interface GuestReservationRequest {
  reservation: ReservationDetails;
  customer: CustomerDetails;
  address: AddressDetails;
  driversLicense: DriversLicenseDetails;
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
  createdAt?: string;
}

/**
 * Reservation search filters
 */
export interface ReservationSearchFilters {
  customerId?: string;
  status?: string;
  pickupDateFrom?: string;
  pickupDateTo?: string;
  sortBy?: 'PickupDate' | 'Price' | 'Status' | 'CreatedDate';
  sortOrder?: 'asc' | 'desc';
  pageNumber?: number;
  pageSize?: number;
}

/**
 * Paginated search results
 */
export interface ReservationSearchResponse {
  items: Reservation[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

/**
 * Cancel reservation request
 */
export interface CancelReservationRequest {
  reason: string;
}

/**
 * Guest lookup request
 */
export interface GuestLookupRequest {
  reservationId: string;
  email: string;
}
