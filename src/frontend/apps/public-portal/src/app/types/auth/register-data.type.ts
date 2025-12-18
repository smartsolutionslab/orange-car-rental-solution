/**
 * User registration data
 */
export interface RegisterData {
  readonly email: string;
  readonly password: string;
  readonly firstName: string;
  readonly lastName: string;
  readonly phoneNumber: string;
  readonly dateOfBirth: string;
  readonly acceptMarketing: boolean;
}
