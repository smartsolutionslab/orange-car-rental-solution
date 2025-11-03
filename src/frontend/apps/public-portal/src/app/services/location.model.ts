/**
 * Location model representing a rental location
 */
export interface Location {
  code: string;
  name: string;
  street: string;
  city: string;
  postalCode: string;
  fullAddress: string;
}
