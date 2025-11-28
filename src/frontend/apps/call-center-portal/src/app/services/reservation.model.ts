import { VehicleId, CategoryCode, LocationCode, Currency } from './vehicle.model';

// Branded types for reservation domain
export type ReservationId = string;
export type CustomerId = string;
export type ISODateString = string;
export type Price = number;
export type RentalDays = number;
export type EmailAddress = string;
export type PhoneNumber = string;
export type LicenseNumber = string;
export type CountryCode = string;
export type PostalCode = string;

// Union types for constrained values
export type ReservationStatus = 'Pending' | 'Confirmed' | 'Active' | 'Completed' | 'Cancelled';
export type SortField = 'PickupDate' | 'Price' | 'Status' | 'CreatedDate';
export type SortOrder = 'asc' | 'desc';

/**
 * Reservation details
 */
export interface Reservation {
  readonly reservationId: ReservationId;
  readonly vehicleId: VehicleId;
  readonly customerId: CustomerId;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
  readonly rentalDays: RentalDays;
  readonly totalPriceNet: Price;
  readonly totalPriceVat: Price;
  readonly totalPriceGross: Price;
  readonly currency: Currency;
  readonly status: ReservationStatus;
  readonly cancellationReason?: string;
  readonly createdAt: ISODateString;
  readonly confirmedAt?: ISODateString;
  readonly cancelledAt?: ISODateString;
  readonly completedAt?: ISODateString;
}

/**
 * Create reservation request for registered customers
 */
export interface CreateReservationRequest {
  readonly vehicleId: VehicleId;
  readonly customerId: CustomerId;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
  readonly totalPriceNet?: Price;
}

/**
 * Create reservation response
 */
export interface CreateReservationResponse {
  readonly reservationId: ReservationId;
  readonly status: ReservationStatus;
  readonly totalPriceNet: Price;
  readonly totalPriceVat: Price;
  readonly totalPriceGross: Price;
  readonly currency: Currency;
}

/**
 * Guest reservation request (with customer details)
 */
export interface GuestReservationRequest {
  // Vehicle and booking details
  readonly vehicleId: VehicleId;
  readonly categoryCode: CategoryCode;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;

  // Customer details
  readonly firstName: string;
  readonly lastName: string;
  readonly email: EmailAddress;
  readonly phoneNumber: PhoneNumber;
  readonly dateOfBirth: ISODateString;

  // Address details
  readonly street: string;
  readonly city: string;
  readonly postalCode: PostalCode;
  readonly country: CountryCode;

  // Driver's license details
  readonly licenseNumber: LicenseNumber;
  readonly licenseIssueCountry: CountryCode;
  readonly licenseIssueDate: ISODateString;
  readonly licenseExpiryDate: ISODateString;
}

/**
 * Guest reservation response
 */
export interface GuestReservationResponse {
  readonly customerId: CustomerId;
  readonly reservationId: ReservationId;
  readonly totalPriceNet: Price;
  readonly totalPriceVat: Price;
  readonly totalPriceGross: Price;
  readonly currency: Currency;
}

/**
 * Reservation search query
 */
export interface ReservationSearchQuery {
  readonly customerId?: CustomerId;
  readonly vehicleId?: VehicleId;
  readonly status?: ReservationStatus;
  readonly pickupDateFrom?: ISODateString;
  readonly pickupDateTo?: ISODateString;
  readonly locationCode?: LocationCode;
  readonly categoryCode?: CategoryCode;
  readonly minPrice?: Price;
  readonly maxPrice?: Price;
  readonly sortBy?: SortField;
  readonly sortOrder?: SortOrder;
  readonly pageNumber?: number;
  readonly pageSize?: number;
}

/**
 * Reservation search result
 */
export interface ReservationSearchResult {
  readonly reservations: Reservation[];
  readonly totalCount: number;
  readonly pageNumber: number;
  readonly pageSize: number;
  readonly totalPages: number;
  readonly hasPreviousPage?: boolean;
  readonly hasNextPage?: boolean;
}

/**
 * Cancel reservation request
 */
export interface CancelReservationRequest {
  readonly reason: string;
}

/**
 * Confirm reservation request
 */
export interface ConfirmReservationRequest {
  readonly reservationId: ReservationId;
}

/**
 * Constants for reservation statuses with German labels
 */
export const RESERVATION_STATUSES = [
  { code: 'Pending', label: 'Ausstehend' },
  { code: 'Confirmed', label: 'Bestätigt' },
  { code: 'Active', label: 'Aktiv' },
  { code: 'Completed', label: 'Abgeschlossen' },
  { code: 'Cancelled', label: 'Storniert' }
] as const satisfies readonly { code: ReservationStatus; label: string }[];

/**
 * Constants for sort fields
 */
export const SORT_FIELDS = ['PickupDate', 'Price', 'Status', 'CreatedDate'] as const satisfies readonly SortField[];
