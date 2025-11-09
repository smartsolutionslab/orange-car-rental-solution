import { Component, signal, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Router, ActivatedRoute } from '@angular/router';
import { LocationService } from '../../services/location.service';
import { Location } from '../../services/location.model';
import { VehicleService } from '../../services/vehicle.service';
import { ReservationService } from '../../services/reservation.service';
import { Vehicle } from '../../services/vehicle.model';
import { GuestReservationRequest } from '../../services/reservation.model';

/**
 * Booking form component for creating guest reservations
 *
 * Features:
 * - Multi-step form (Vehicle Details → Customer Info → Address → Driver's License → Review)
 * - Form validation at each step
 * - Integration with Reservations API
 * - Navigation to confirmation page on success
 */
@Component({
  selector: 'app-booking',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  templateUrl: './booking.component.html',
  styleUrl: './booking.component.css'
})
export class BookingComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  private readonly locationService = inject(LocationService);
  private readonly vehicleService = inject(VehicleService);
  private readonly reservationService = inject(ReservationService);

  // Form state
  protected readonly bookingForm: FormGroup;
  protected readonly currentStep = signal(1);
  protected readonly totalSteps = 5;

  // Loading and error states
  protected readonly submitting = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly locationsLoading = signal(false);

  // Data
  protected readonly locations = signal<Location[]>([]);
  protected readonly selectedVehicle = signal<Vehicle | null>(null);

  // Date constraints
  protected readonly today = new Date().toISOString().split('T')[0];
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
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[0-9\s\-()]+$/)]],
      dateOfBirth: ['', Validators.required],

      // Address details
      street: ['', [Validators.required, Validators.minLength(5)]],
      city: ['', [Validators.required, Validators.minLength(2)]],
      postalCode: ['', [Validators.required, Validators.pattern(/^[0-9]{5}$/)]],
      country: ['Deutschland', Validators.required],

      // Driver's license details
      licenseNumber: ['', [Validators.required, Validators.minLength(5)]],
      licenseIssueCountry: ['Deutschland', Validators.required],
      licenseIssueDate: ['', Validators.required],
      licenseExpiryDate: ['', Validators.required]
    });
  }

  ngOnInit(): void {
    // Load locations
    this.loadLocations();

    // Get vehicle ID from route query params
    this.route.queryParams.subscribe(params => {
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
          dropoffLocationCode: locationCode || ''
        });
      }
    });

    // Set up date validation
    this.setupDateValidation();
  }

  /**
   * Load locations from API
   */
  private loadLocations(): void {
    this.locationsLoading.set(true);

    this.locationService.getAllLocations().subscribe({
      next: (locations) => {
        this.locations.set(locations);
        this.locationsLoading.set(false);
      },
      error: (error) => {
        console.error('Error loading locations:', error);
        this.locationsLoading.set(false);
      }
    });
  }

  /**
   * Load vehicle details
   */
  private loadVehicle(vehicleId: string): void {
    this.vehicleService.getVehicleById(vehicleId).subscribe({
      next: (vehicle) => {
        this.selectedVehicle.set(vehicle);
        // Auto-populate categoryCode from loaded vehicle
        this.bookingForm.patchValue({
          categoryCode: vehicle.categoryCode
        });
      },
      error: (error) => {
        console.error('Error loading vehicle:', error);
        // Don't show error to user, just log it
        // The form will require manual categoryCode entry if vehicle load fails
      }
    });
  }

  /**
   * Set up date validation logic
   */
  private setupDateValidation(): void {
    // Update min return date when pickup date changes
    this.bookingForm.get('pickupDate')?.valueChanges.subscribe((pickupDate: string) => {
      if (pickupDate) {
        const pickup = new Date(pickupDate);
        pickup.setDate(pickup.getDate() + 1);
        this.minReturnDate.set(pickup.toISOString().split('T')[0]);

        // Ensure return date is after pickup date
        const currentReturnDate = this.bookingForm.get('returnDate')?.value;
        if (currentReturnDate && currentReturnDate <= pickupDate) {
          this.bookingForm.patchValue({
            returnDate: pickup.toISOString().split('T')[0]
          });
        }
      }
    });

    // Update min expiry date when issue date changes
    this.bookingForm.get('licenseIssueDate')?.valueChanges.subscribe((issueDate: string) => {
      if (issueDate) {
        const issue = new Date(issueDate);
        issue.setDate(issue.getDate() + 1);
        this.minLicenseExpiryDate.set(issue.toISOString().split('T')[0]);

        // Ensure expiry date is after issue date
        const currentExpiryDate = this.bookingForm.get('licenseExpiryDate')?.value;
        if (currentExpiryDate && currentExpiryDate <= issueDate) {
          this.bookingForm.patchValue({
            licenseExpiryDate: issue.toISOString().split('T')[0]
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
      Object.keys(this.bookingForm.controls).forEach(key => {
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
        dropoffLocationCode: formValue.dropoffLocationCode
      },
      customer: {
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        email: formValue.email,
        phoneNumber: formValue.phoneNumber,
        dateOfBirth: formValue.dateOfBirth
      },
      address: {
        street: formValue.street,
        city: formValue.city,
        postalCode: formValue.postalCode,
        country: formValue.country
      },
      driversLicense: {
        licenseNumber: formValue.licenseNumber,
        licenseIssueCountry: formValue.licenseIssueCountry,
        licenseIssueDate: formValue.licenseIssueDate,
        licenseExpiryDate: formValue.licenseExpiryDate
      }
    };

    this.reservationService.createGuestReservation(request).subscribe({
      next: (response) => {
        this.submitting.set(false);
        // Navigate to confirmation page with reservation details
        this.router.navigate(['/confirmation'], {
          queryParams: {
            reservationId: response.reservationId,
            customerId: response.customerId
          }
        });
      },
      error: (err) => {
        console.error('Error creating reservation:', err);
        this.submitting.set(false);
        this.error.set(
          'Fehler beim Erstellen der Reservierung. Bitte überprüfen Sie Ihre Eingaben und versuchen Sie es erneut.'
        );
        window.scrollTo({ top: 0, behavior: 'smooth' });
      }
    });
  }

  /**
   * Calculate number of rental days
   */
  protected getRentalDays(): number {
    const pickupDate = this.bookingForm.get('pickupDate')?.value;
    const returnDate = this.bookingForm.get('returnDate')?.value;

    if (!pickupDate || !returnDate) {
      return 0;
    }

    const pickup = new Date(pickupDate);
    const returnD = new Date(returnDate);
    const diffTime = Math.abs(returnD.getTime() - pickup.getTime());
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));

    return diffDays;
  }
}
