/**
 * Complete customer profile
 * Used for getting and updating customer information
 */
export interface CustomerProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  address: {
    street: string;
    city: string;
    postalCode: string;
    country: string;
  };
  driversLicense?: {
    licenseNumber: string;
    licenseIssueCountry: string;
    licenseIssueDate: string;
    licenseExpiryDate: string;
  };
  createdAt?: string;
  updatedAt?: string;
}

/**
 * Customer profile update request
 * Same structure as profile but without ID
 */
export interface UpdateCustomerProfileRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  address: {
    street: string;
    city: string;
    postalCode: string;
    country: string;
  };
  driversLicense?: {
    licenseNumber: string;
    licenseIssueCountry: string;
    licenseIssueDate: string;
    licenseExpiryDate: string;
  };
}
