import { Page, Locator, expect } from '@playwright/test';
import { BasePage } from './base.page';
import { testBooking, testVehicles } from '../fixtures/test-data';

/**
 * Customer information for booking
 */
export interface CustomerInfo {
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
}

/**
 * Address information for booking
 */
export interface AddressInfo {
  street: string;
  city: string;
  postalCode: string;
  country: string;
}

/**
 * Driver's license information for booking
 */
export interface DriversLicenseInfo {
  licenseNumber: string;
  licenseIssueCountry: string;
  licenseIssueDate: string;
  licenseExpiryDate: string;
}

/**
 * Page Object for Booking Flow (US-2)
 */
export class BookingPage extends BasePage {
  // Step indicators
  readonly progressSteps: Locator;
  readonly currentStep: Locator;

  // Navigation buttons
  readonly nextButton: Locator;
  readonly previousButton: Locator;
  readonly submitButton: Locator;

  // Form container
  readonly bookingForm: Locator;

  // Step 1: Vehicle Details
  readonly pickupDateInput: Locator;
  readonly returnDateInput: Locator;
  readonly pickupLocationSelect: Locator;
  readonly dropoffLocationSelect: Locator;
  readonly rentalDays: Locator;

  // Error display
  readonly errorMessage: Locator;
  readonly fieldErrors: Locator;

  constructor(page: Page) {
    super(page);

    // Navigation
    this.progressSteps = page.locator('.progress-step');
    this.currentStep = page.locator('.progress-step.current, .progress-step.active');
    this.nextButton = page.locator('button:has-text("Weiter")');
    this.previousButton = page.locator('button:has-text("Zurück")');
    this.submitButton = page.locator('button:has-text("Buchung abschließen")');
    this.bookingForm = page.locator('.booking-form');

    // Step 1
    this.pickupDateInput = page.locator('input[formControlName="pickupDate"]');
    this.returnDateInput = page.locator('input[formControlName="returnDate"]');
    this.pickupLocationSelect = page.locator('select[formControlName="pickupLocationCode"]');
    this.dropoffLocationSelect = page.locator('select[formControlName="dropoffLocationCode"]');
    this.rentalDays = page.locator('.rental-days, text=/\\d+.*Tage/i');

    // Errors
    this.errorMessage = page.locator('.error, .error-message, .alert-danger');
    this.fieldErrors = page.locator('.field-error, .invalid-feedback');
  }

  /**
   * Navigate to booking page with vehicle
   */
  async navigate(vehicleId?: string): Promise<void> {
    const vehicle = vehicleId || testVehicles.available.id;
    const pickupDate = testBooking.pickupDate();
    const returnDate = testBooking.returnDate();
    const locationCode = testBooking.locationCode;

    await this.goto(
      `/booking?vehicleId=${vehicle}&pickupDate=${pickupDate}&returnDate=${returnDate}&locationCode=${locationCode}`
    );
    await this.page.waitForSelector('.booking-form', { timeout: 10000 });
  }

  /**
   * Get current step number (1-5)
   */
  async getCurrentStepNumber(): Promise<number> {
    const steps = await this.progressSteps.all();
    for (let i = 0; i < steps.length; i++) {
      const classList = await steps[i].getAttribute('class');
      if (classList?.includes('current') || classList?.includes('active')) {
        return i + 1;
      }
    }
    return 1;
  }

  /**
   * Go to next step
   */
  async nextStep(): Promise<void> {
    await this.nextButton.click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Go to previous step
   */
  async previousStep(): Promise<void> {
    await this.previousButton.click();
    await this.page.waitForTimeout(500);
  }

  /**
   * Fill customer information (Step 2)
   */
  async fillCustomerInfo(info: CustomerInfo): Promise<void> {
    await this.fillInput('firstName', info.firstName);
    await this.fillInput('lastName', info.lastName);
    await this.fillInput('email', info.email);
    await this.fillInput('phoneNumber', info.phoneNumber);
    await this.fillInput('dateOfBirth', info.dateOfBirth);
  }

  /**
   * Fill address information (Step 3)
   */
  async fillAddress(info: AddressInfo): Promise<void> {
    await this.fillInput('street', info.street);
    await this.fillInput('city', info.city);
    await this.fillInput('postalCode', info.postalCode);
    await this.selectOption('country', info.country);
  }

  /**
   * Fill driver's license information (Step 4)
   */
  async fillDriversLicense(info: DriversLicenseInfo): Promise<void> {
    await this.fillInput('licenseNumber', info.licenseNumber);
    await this.selectOption('licenseIssueCountry', info.licenseIssueCountry);
    await this.fillInput('licenseIssueDate', info.licenseIssueDate);
    await this.fillInput('licenseExpiryDate', info.licenseExpiryDate);
  }

  /**
   * Submit the booking
   */
  async submitBooking(): Promise<void> {
    await this.submitButton.click();
    await this.waitForNavigation(/\/confirmation/, 15000);
  }

  /**
   * Complete entire booking flow
   */
  async completeBooking(data: {
    customer: CustomerInfo;
    address: AddressInfo;
    license: DriversLicenseInfo;
  }): Promise<void> {
    // Step 1: Vehicle details (pre-filled)
    await this.nextStep();

    // Step 2: Customer information
    await this.fillCustomerInfo(data.customer);
    await this.nextStep();

    // Step 3: Address
    await this.fillAddress(data.address);
    await this.nextStep();

    // Step 4: Driver's license
    await this.fillDriversLicense(data.license);
    await this.nextStep();

    // Step 5: Review and submit
    await this.submitBooking();
  }

  /**
   * Check if form has validation errors
   */
  async hasValidationErrors(): Promise<boolean> {
    return this.isVisible(this.fieldErrors);
  }

  /**
   * Get validation error messages
   */
  async getValidationErrors(): Promise<string[]> {
    const errors = await this.fieldErrors.allTextContents();
    return errors.filter(e => e.trim().length > 0);
  }

  /**
   * Check if customer info is pre-filled
   */
  async isCustomerInfoPrefilled(): Promise<boolean> {
    const firstName = await this.getInputValue('firstName');
    return firstName.length > 0;
  }

  /**
   * Get pre-filled customer data
   */
  async getPrefilledCustomerData(): Promise<Partial<CustomerInfo>> {
    return {
      firstName: await this.getInputValue('firstName'),
      lastName: await this.getInputValue('lastName'),
      email: await this.getInputValue('email')
    };
  }
}
