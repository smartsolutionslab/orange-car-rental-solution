/**
 * Customer details
 */
export interface Customer {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  street: string;
  city: string;
  postalCode: string;
  country: string;
  licenseNumber: string;
  licenseIssueCountry: string;
  licenseIssueDate: string;
  licenseExpiryDate: string;
  createdAt: string;
  updatedAt?: string;
}

/**
 * Customer search query
 */
export interface CustomerSearchQuery {
  email?: string;
  phoneNumber?: string;
  lastName?: string;
  pageNumber?: number;
  pageSize?: number;
}

/**
 * Customer search result
 */
export interface CustomerSearchResult {
  customers: Customer[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPreviousPage?: boolean;
  hasNextPage?: boolean;
}

/**
 * Update customer request
 */
export interface UpdateCustomerRequest {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  street: string;
  city: string;
  postalCode: string;
  country: string;
  licenseNumber: string;
  licenseIssueCountry: string;
  licenseIssueDate: string;
  licenseExpiryDate: string;
}
