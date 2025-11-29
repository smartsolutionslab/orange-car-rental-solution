/**
 * Update customer request
 */
import type { Customer } from './customer.type';

export type UpdateCustomerRequest = Omit<Customer, 'id' | 'createdAt' | 'updatedAt'>;
