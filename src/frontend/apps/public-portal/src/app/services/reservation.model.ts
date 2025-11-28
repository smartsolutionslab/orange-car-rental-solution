import { VehicleId, CategoryCode, LocationCode, Currency } from './vehicle.model';

// Branded types for reservation domain
export type ReservationId = string;
export type CustomerId = string;
export type ISODateString = string;
export type Price = number;
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
 * Vehicle and reservation details
 */
export interface ReservationDetails {
  readonly vehicleId: VehicleId;
  readonly categoryCode: CategoryCode;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
}

/**
 * Customer details
 */
export interface CustomerDetails {
  readonly firstName: string;
  readonly lastName: string;
  readonly email: EmailAddress;
  readonly phoneNumber: PhoneNumber;
  readonly dateOfBirth: ISODateString;
}

/**
 * Address details
 */
export interface AddressDetails {
  readonly street: string;
  readonly city: string;
  readonly postalCode: PostalCode;
  readonly country: CountryCode;
}

/**
 * Driver's license details
 */
export interface DriversLicenseDetails {
  readonly licenseNumber: LicenseNumber;
  readonly licenseIssueCountry: CountryCode;
  readonly licenseIssueDate: ISODateString;
  readonly licenseExpiryDate: ISODateString;
}

/**
 * Guest reservation request matching the backend CreateGuestReservationRequest
 * Used for creating reservations for users who haven't registered yet
 */
export interface GuestReservationRequest {
  readonly reservation: ReservationDetails;
  readonly customer: CustomerDetails;
  readonly address: AddressDetails;
  readonly driversLicense: DriversLicenseDetails;
}

/**
 * Response from creating a guest reservation
 * Includes both customer and reservation IDs along with pricing breakdown
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
 * Reservation details
 */
export interface Reservation {
  readonly id: ReservationId;
  readonly vehicleId: VehicleId;
  readonly customerId: CustomerId;
  readonly pickupDate: ISODateString;
  readonly returnDate: ISODateString;
  readonly pickupLocationCode: LocationCode;
  readonly dropoffLocationCode: LocationCode;
  readonly totalPriceNet: Price;
  readonly totalPriceVat: Price;
  readonly totalPriceGross: Price;
  readonly currency: Currency;
  readonly status: ReservationStatus;
  readonly createdAt?: ISODateString;
}

/**
 * Reservation search filters
 */
export interface ReservationSearchFilters {
  readonly customerId?: CustomerId;
  readonly status?: ReservationStatus;
  readonly pickupDateFrom?: ISODateString;
  readonly pickupDateTo?: ISODateString;
  readonly sortBy?: SortField;
  readonly sortOrder?: SortOrder;
  readonly pageNumber?: number;
  readonly pageSize?: number;
}

/**
 * Paginated search results
 */
export interface ReservationSearchResponse {
  readonly items: Reservation[];
  readonly totalCount: number;
  readonly pageNumber: number;
  readonly pageSize: number;
  readonly totalPages: number;
}

/**
 * Cancel reservation request
 */
export interface CancelReservationRequest {
  readonly reason: string;
}

/**
 * Guest lookup request
 */
export interface GuestLookupRequest {
  readonly reservationId: ReservationId;
  readonly email: EmailAddress;
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
