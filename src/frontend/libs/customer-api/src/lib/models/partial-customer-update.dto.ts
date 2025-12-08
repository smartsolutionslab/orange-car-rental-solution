/**
 * Partial customer update - allows updating individual fields
 */
import type { UpdateCustomerProfileRequest } from './update-customer-profile-request.dto';

export type PartialCustomerUpdate = Partial<UpdateCustomerProfileRequest>;
