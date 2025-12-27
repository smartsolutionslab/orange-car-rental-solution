/**
 * Customer form values - uses plain strings for form binding
 * Convert to UpdateCustomerRequest with type assertions when submitting
 */
export interface CustomerFormValues {
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
}
