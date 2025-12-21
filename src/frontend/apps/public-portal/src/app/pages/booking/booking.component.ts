import { Component, signal, inject, DestroyRef } from '@angular/core';
import type { OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CustomValidators } from '@orange-car-rental/shared';
import type {
  EmailAddress,
  PhoneNumber,
  ISODateString,
  PostalCode,
  CountryCode,
  FirstName,
  LastName,
} from '@orange-car-rental/shared';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import type { Vehicle, VehicleId } from '@orange-car-rental/vehicle-api';
import type { GuestReservationRequest, LicenseNumber } from '@orange-car-rental/reservation-api';
import type { CityName, StreetAddress } from '@orange-car-rental/location-api';
import type {
  CustomerProfile,
  UpdateCustomerProfileRequest,
} from '@orange-car-rental/customer-api';
import { logError } from '@orange-car-rental/util';
import {
  SelectLocationComponent,
  ErrorAlertComponent,
  calculateRentalDays,
  getTodayDateString,
  addDays,
  IconComponent,
} from '@orange-car-rental/ui-components';
import { VehicleService } from '../../services/vehicle.service';
import { ReservationService } from '../../services/reservation.service';
import { AuthService } from '../../services/auth.service';
import { CustomerService } from '../../services/customer.service';
import { SimilarVehiclesComponent } from '../../components/similar-vehicles/similar-vehicles.component';
import type { BookingFormValue } from '../../types';

/**
 * Booking form component for creating guest and authenticated user reservations
 *
 * Features:
 * - Multi-step form (Vehicle Details → Customer Info → Address → Driver's License → Review)
 * - Form validation at each step
 * - Auto-fill customer data for authenticated users
 * - Optional profile update after booking
 * - Integration with Reservations API
 * - Navigation to confirmation page on success
 */
@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [
    ReactiveFormsModule,
    CommonModule,
    TranslateModule,
    SimilarVehiclesComponent,
    SelectLocationComponent,
    ErrorAlertComponent,
    IconComponent,
  ],
  templateUrl: './booking.component.html',
  styleUrl: './booking.component.css',
})
export class BookingComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly vehicleService = inject(VehicleService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly reservationService = inject(ReservationService);
  private readonly authService = inject(AuthService);
  private readonly customerService = inject(CustomerService);
  private readonly translate = inject(TranslateService);

  // Form state
  protected readonly bookingForm: FormGroup;
  protected readonly currentStep = signal(1);
  protected readonly totalSteps = 5;

  // Loading and error states
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly profileLoading = signal(false);

  // Data
  protected readonly selectedVehicle = signal<Vehicle | null>(null);
  protected readonly similarVehicles = signal<Vehicle[]>([]);
  protected readonly customerProfile = signal<CustomerProfile | null>(null);
  protected readonly isAuthenticated = signal(false);
  protected readonly showVehicleUnavailableWarning = signal(false);

  // Date constraints
  protected readonly today = getTodayDateString();
  protected readonly minReturnDate = signal(this.today);
  protected readonly maxLicenseIssueDate = signal(this.today);
  protected readonly minLicenseExpiryDate = signal(this.today);

  constructor() {
    // Initialize form with all fields
    this.bookingForm = this.fb.group({
      // Vehicle and booking details
      vehicleId: ['', Validators.required],
      categoryCode: ['', Validators.required],
      pickupDate: ['', Validators.required],
      returnDate: ['', Validators.required],
      pickupLocationCode: ['', Validators.required],
      dropoffLocationCode: ['', Validators.required],

      // Customer details
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.required, CustomValidators.germanPhone()]],
      dateOfBirth: ['', Validators.required],

      // Address details
      street: ['', [Validators.required, Validators.minLength(5)]],
      city: ['', [Validators.required, Validators.minLength(2)]],
      postalCode: ['', [Validators.required, CustomValidators.germanPostalCode()]],
      country: ['Deutschland', Validators.required],

      // Driver's license details
      licenseNumber: ['', [Validators.required, Validators.minLength(5)]],
      licenseIssueCountry: ['Deutschland', Validators.required],
      licenseIssueDate: ['', Validators.required],
      licenseExpiryDate: ['', Validators.required],

      // Profile update option (only for authenticated users)
      updateMyProfile: [false],
    });
  }

  ngOnInit(): void {
    // Check if user is authenticated and load their profile
    this.isAuthenticated.set(this.authService.isAuthenticated());
    if (this.isAuthenticated()) {
      this.loadCustomerProfile();
    }

    // Get vehicle ID from route query params
    this.route.queryParams.pipe(takeUntilDestroyed(this.destroyRef)).subscribe((params) => {
      const vehicleId = params['vehicleId'];
      const pickupDate = params['pickupDate'];
      const returnDate = params['returnDate'];
      const locationCode = params['locationCode'];

      if (vehicleId) {
        this.loadVehicle(vehicleId);
        this.bookingForm.patchValue({
          vehicleId,
          pickupDate: pickupDate || '',
          returnDate: returnDate || '',
          pickupLocationCode: locationCode || '',
          dropoffLocationCode: locationCode || '',
        });
      }
    });

    // Set up date validation
    this.setupDateValidation();
  }

  /**
   * Load vehicle details and similar vehicles
   */
  private loadVehicle(vehicleId: string): void {
    this.vehicleService
      .getVehicleById(vehicleId as VehicleId)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (vehicle) => {
          this.selectedVehicle.set(vehicle);
          // Auto-populate categoryCode from loaded vehicle
          this.bookingForm.patchValue({
            categoryCode: vehicle.categoryCode,
          });

          // Load similar vehicles
          this.loadSimilarVehicles(vehicle);
        },
        error: (err) => {
          logError('BookingComponent', 'Error loading vehicle', err);
          this.showVehicleUnavailableWarning.set(true);
          // Don't show error to user, just log it
          // The form will require manual categoryCode entry if vehicle load fails
        },
      });
  }

  /**
   * Load similar vehicles for the selected vehicle
   */
  private loadSimilarVehicles(vehicle: Vehicle): void {
    const pickupDate = this.bookingForm.get('pickupDate')?.value;
    const returnDate = this.bookingForm.get('returnDate')?.value;

    this.vehicleService
      .getSimilarVehicles(vehicle, pickupDate, returnDate)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (similar) => {
          this.similarVehicles.set(similar);
        },
        error: (err) => {
          logError('BookingComponent', 'Error loading similar vehicles', err);
          // Non-critical error, just log it
        },
      });
  }

  /**
   * Load customer profile for authenticated users
   * Pre-fills the form with saved profile data
   */
  private loadCustomerProfile(): void {
    this.profileLoading.set(true);

    this.customerService
      .getMyProfile()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (profile) => {
          this.customerProfile.set(profile);
          this.profileLoading.set(false);

          // Pre-fill customer information
          this.bookingForm.patchValue({
            firstName: profile.firstName,
            lastName: profile.lastName,
            email: profile.email,
            phoneNumber: profile.phoneNumber,
            dateOfBirth: profile.dateOfBirth,

            // Pre-fill address
            street: profile.address.street,
            city: profile.address.city,
            postalCode: profile.address.postalCode,
            country: profile.address.country,

            // Pre-fill driver's license if available
            ...(profile.driversLicense && {
              licenseNumber: profile.driversLicense.licenseNumber,
              licenseIssueCountry: profile.driversLicense.licenseIssueCountry,
              licenseIssueDate: profile.driversLicense.licenseIssueDate,
              licenseExpiryDate: profile.driversLicense.licenseExpiryDate,
            }),
          });
        },
        error: (err) => {
          logError('BookingComponent', 'Error loading customer profile', err);
          this.profileLoading.set(false);
          // Don't show error to user, just log it
          // User can still fill the form manually
        },
      });
  }

  /**
   * Set up date validation logic
   */
  private setupDateValidation(): void {
    // Update min return date when pickup date changes
    this.bookingForm
      .get('pickupDate')
      ?.valueChanges.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((pickupDate: string) => {
        if (pickupDate) {
          const minReturn = addDays(pickupDate, 1);
          this.minReturnDate.set(minReturn);

          // Ensure return date is after pickup date
          const currentReturnDate = this.bookingForm.get('returnDate')?.value;
          if (currentReturnDate && currentReturnDate <= pickupDate) {
            this.bookingForm.patchValue({
              returnDate: minReturn,
            });
          }
        }
      });

    // Update min expiry date when issue date changes
    this.bookingForm
      .get('licenseIssueDate')
      ?.valueChanges.pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe((issueDate: string) => {
        if (issueDate) {
          const minExpiry = addDays(issueDate, 1);
          this.minLicenseExpiryDate.set(minExpiry);

          // Ensure expiry date is after issue date
          const currentExpiryDate = this.bookingForm.get('licenseExpiryDate')?.value;
          if (currentExpiryDate && currentExpiryDate <= issueDate) {
            this.bookingForm.patchValue({
              licenseExpiryDate: minExpiry,
            });
          }
        }
      });
  }

  /**
   * Navigate to next step
   */
  protected nextStep(): void {
    if (this.currentStep() < this.totalSteps) {
      // Validate current step before moving forward
      if (this.isCurrentStepValid()) {
        this.currentStep.set(this.currentStep() + 1);
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    }
  }

  /**
   * Navigate to previous step
   */
  protected previousStep(): void {
    if (this.currentStep() > 1) {
      this.currentStep.set(this.currentStep() - 1);
      window.scrollTo({ top: 0, behavior: 'smooth' });
    }
  }

  /**
   * Check if current step is valid
   */
  protected isCurrentStepValid(): boolean {
    const step = this.currentStep();

    switch (step) {
      case 1: // Vehicle details
        return !!(
          this.bookingForm.get('vehicleId')?.valid &&
          this.bookingForm.get('categoryCode')?.valid &&
          this.bookingForm.get('pickupDate')?.valid &&
          this.bookingForm.get('returnDate')?.valid &&
          this.bookingForm.get('pickupLocationCode')?.valid &&
          this.bookingForm.get('dropoffLocationCode')?.valid
        );
      case 2: // Customer info
        return !!(
          this.bookingForm.get('firstName')?.valid &&
          this.bookingForm.get('lastName')?.valid &&
          this.bookingForm.get('email')?.valid &&
          this.bookingForm.get('phoneNumber')?.valid &&
          this.bookingForm.get('dateOfBirth')?.valid
        );
      case 3: // Address
        return !!(
          this.bookingForm.get('street')?.valid &&
          this.bookingForm.get('city')?.valid &&
          this.bookingForm.get('postalCode')?.valid &&
          this.bookingForm.get('country')?.valid
        );
      case 4: // Driver's license
        return !!(
          this.bookingForm.get('licenseNumber')?.valid &&
          this.bookingForm.get('licenseIssueCountry')?.valid &&
          this.bookingForm.get('licenseIssueDate')?.valid &&
          this.bookingForm.get('licenseExpiryDate')?.valid
        );
      case 5: // Review
        return this.bookingForm.valid;
      default:
        return false;
    }
  }

  /**
   * Submit the booking form
   */
  protected onSubmit(): void {
    if (this.bookingForm.invalid) {
      // Mark all fields as touched to show validation errors
      Object.keys(this.bookingForm.controls).forEach((key) => {
        this.bookingForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.submitting.set(true);
    this.error.set(null);

    const formValue = this.bookingForm.value;

    // Build the guest reservation request with nested structure
    const request: GuestReservationRequest = {
      reservation: {
        vehicleId: formValue.vehicleId,
        categoryCode: formValue.categoryCode,
        pickupDate: formValue.pickupDate,
        returnDate: formValue.returnDate,
        pickupLocationCode: formValue.pickupLocationCode,
        dropoffLocationCode: formValue.dropoffLocationCode,
      },
      customer: {
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        email: formValue.email,
        phoneNumber: formValue.phoneNumber,
        dateOfBirth: formValue.dateOfBirth,
      },
      address: {
        street: formValue.street,
        city: formValue.city,
        postalCode: formValue.postalCode,
        country: formValue.country,
      },
      driversLicense: {
        licenseNumber: formValue.licenseNumber,
        licenseIssueCountry: formValue.licenseIssueCountry,
        licenseIssueDate: formValue.licenseIssueDate,
        licenseExpiryDate: formValue.licenseExpiryDate,
      },
    };

    this.reservationService
      .createGuestReservation(request)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (response) => {
          // Update profile if checkbox is checked and user is authenticated
          if (this.isAuthenticated() && this.bookingForm.get('updateMyProfile')?.value) {
            this.updateProfileAfterBooking(formValue);
          }

          this.submitting.set(false);
          // Navigate to confirmation page with reservation details
          this.router.navigate(['/confirmation'], {
            queryParams: {
              reservationId: response.reservationId,
              customerId: response.customerId,
            },
          });
        },
        error: (err) => {
          logError('BookingComponent', 'Error creating reservation', err);
          this.submitting.set(false);
          this.error.set(this.translate.instant('booking.errors.create'));
          window.scrollTo({ top: 0, behavior: 'smooth' });
        },
      });
  }

  /**
   * Update customer profile after successful booking
   * Called when updateMyProfile checkbox is checked
   */
  private updateProfileAfterBooking(formValue: BookingFormValue): void {
    const profile = this.customerProfile();
    if (!profile) {
      return;
    }

    const updateRequest: UpdateCustomerProfileRequest = {
      firstName: formValue.firstName as FirstName,
      lastName: formValue.lastName as LastName,
      email: formValue.email as EmailAddress,
      phoneNumber: formValue.phoneNumber as PhoneNumber,
      dateOfBirth: formValue.dateOfBirth as ISODateString,
      address: {
        street: formValue.street as StreetAddress,
        city: formValue.city as CityName,
        postalCode: formValue.postalCode as PostalCode,
        country: formValue.country as CountryCode,
      },
      driversLicense: {
        licenseNumber: formValue.licenseNumber as LicenseNumber,
        licenseIssueCountry: formValue.licenseIssueCountry as CountryCode,
        licenseIssueDate: formValue.licenseIssueDate as ISODateString,
        licenseExpiryDate: formValue.licenseExpiryDate as ISODateString,
      },
    };

    this.customerService
      .updateMyProfile(updateRequest)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (updatedProfile) => {
          this.customerProfile.set(updatedProfile);
        },
        error: (err) => {
          logError('BookingComponent', 'Error updating profile', err);
          // Don't show error to user since booking was successful
          // Profile update failure should not block the booking flow
        },
      });
  }

  /**
   * Calculate number of rental days
   */
  protected getRentalDays(): number {
    return calculateRentalDays(
      this.bookingForm.get('pickupDate')?.value,
      this.bookingForm.get('returnDate')?.value,
    );
  }

  /**
   * Handle selection of an alternative vehicle
   * Replaces the current vehicle while preserving all other booking details
   */
  protected onSimilarVehicleSelected(vehicle: Vehicle): void {
    // Update the selected vehicle
    this.selectedVehicle.set(vehicle);

    // Update form with new vehicle details
    this.bookingForm.patchValue({
      vehicleId: vehicle.id,
      categoryCode: vehicle.categoryCode,
    });

    // Clear unavailable warning if it was shown
    this.showVehicleUnavailableWarning.set(false);

    // Reload similar vehicles for the new selection
    this.loadSimilarVehicles(vehicle);

    // Scroll to top to show the updated vehicle
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }
}
