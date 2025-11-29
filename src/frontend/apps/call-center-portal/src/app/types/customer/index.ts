/**
 * Customer types barrel export
 */
export type { Customer } from './customer.type';
export type { CustomerSearchQuery } from './customer-search-query.type';
export type { CustomerSearchResult } from './customer-search-result.type';
export type { UpdateCustomerRequest } from './update-customer-request.type';
export type { PartialCustomerUpdate } from './partial-customer-update.type';
export { getCustomerDisplayName, isLicenseExpired, licenseExpiresSoon } from './customer.utils';
