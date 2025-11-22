/**
 * Reservation details
 */
export interface Reservation {
  reservationId: string;
  vehicleId: string;
  customerId: string;
  pickupDate: string;
  returnDate: string;
  pickupLocationCode: string;
  dropoffLocationCode: string;
  rentalDays: number;
  totalPriceNet: number;
  totalPriceVat: number;
  totalPriceGross: number;
  currency: string;
  status: string;
  cancellationReason?: string;
  createdAt: string;
  confirmedAt?: string;
  cancelledAt?: string;
  completedAt?: string;
}

/**
 * Create reservation request for registered customers
 */
export interface CreateReservationRequest {
  vehicleId: string;
  customerId: string;
  pickupDate: string;
  returnDate: string;
  pickupLocationCode: string;
  dropoffLocationCode: string;
  totalPriceNet?: number; // Optional, will be calculated if not provided
}

/**
 * Create reservation response
 */
export interface CreateReservationResponse {
  reservationId: string;
  status: string;
  totalPriceNet: number;
  totalPriceVat: number;
  totalPriceGross: number;
  currency: string;
}

/**
 * Guest reservation request (with customer details)
 */
export interface GuestReservationRequest {
  // Vehicle and booking details
  vehicleId: string;
  categoryCode: string;
  pickupDate: string;
  returnDate: string;
  pickupLocationCode: string;
  dropoffLocationCode: string;

  // Customer details
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;

  // Address details
  street: string;
  city: string;
  postalCode: string;
  country: string;

  // Driver's license details
  licenseNumber: string;
  licenseIssueCountry: string;
  licenseIssueDate: string;
  licenseExpiryDate: string;
}

/**
 * Guest reservation response
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
 * Reservation search query
 */
export interface ReservationSearchQuery {
  customerId?: string;
  vehicleId?: string;
  status?: string;
  pickupDateFrom?: string;
  pickupDateTo?: string;
  locationCode?: string;
  categoryCode?: string;
  minPrice?: number;
  maxPrice?: number;
  sortBy?: 'PickupDate' | 'Price' | 'Status' | 'CreatedDate';
  sortOrder?: 'asc' | 'desc';
  pageNumber?: number;
  pageSize?: number;
}

/**
 * Reservation search result
 */
export interface ReservationSearchResult {
  reservations: Reservation[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}
