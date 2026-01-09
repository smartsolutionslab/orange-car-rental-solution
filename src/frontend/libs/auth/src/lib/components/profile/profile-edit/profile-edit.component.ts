import {
  Component,
  input,
  output,
  signal,
  computed,
  inject,
  effect,
} from "@angular/core";
import { CommonModule } from "@angular/common";
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from "@angular/forms";
import { ErrorAlertComponent } from "@orange-car-rental/ui-components";
import type {
  UserProfile,
  ProfileEditLabels,
  ProfileUpdateEvent,
} from "../profile.types";
import { DEFAULT_PROFILE_EDIT_LABELS_DE } from "../profile.types";

/**
 * Profile Edit Component
 *
 * Form for editing user profile information.
 *
 * @example
 * <lib-profile-edit
 *   [profile]="userProfile()"
 *   [labels]="germanLabels"
 *   [loading]="isSaving()"
 *   [error]="saveError()"
 *   (formSubmit)="onSaveProfile($event)"
 *   (cancel)="onCancel()"
 * />
 */
@Component({
  selector: "lib-profile-edit",
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ErrorAlertComponent,
  ],
  template: `
    <div class="edit-container">
      <div class="edit-card">
        <!-- Header -->
        <div class="edit-header">
          <h1 class="edit-title">{{ labels().title }}</h1>
        </div>

        <!-- Error Message -->
        @if (error()) {
          <div class="error-container">
            <ui-error-alert [message]="error()!"></ui-error-alert>
          </div>
        }

        <!-- Edit Form -->
        <form
          [formGroup]="editForm"
          (ngSubmit)="onSubmit()"
          class="edit-form"
        >
          <!-- Personal Information Section -->
          <section class="form-section">
            <h2 class="section-title">{{ labels().personalInfoSection }}</h2>
            <div class="form-grid">
              <!-- First Name -->
              <div class="form-group">
                <label for="firstName" class="form-label">{{
                  labels().firstNameLabel
                }}</label>
                <input
                  id="firstName"
                  type="text"
                  formControlName="firstName"
                  class="form-input"
                  [class.input-error]="firstNameError()"
                />
                @if (firstNameError()) {
                  <span class="field-error">{{ firstNameError() }}</span>
                }
              </div>

              <!-- Last Name -->
              <div class="form-group">
                <label for="lastName" class="form-label">{{
                  labels().lastNameLabel
                }}</label>
                <input
                  id="lastName"
                  type="text"
                  formControlName="lastName"
                  class="form-input"
                  [class.input-error]="lastNameError()"
                />
                @if (lastNameError()) {
                  <span class="field-error">{{ lastNameError() }}</span>
                }
              </div>

              <!-- Phone Number -->
              <div class="form-group">
                <label for="phoneNumber" class="form-label">{{
                  labels().phoneLabel
                }}</label>
                <input
                  id="phoneNumber"
                  type="tel"
                  formControlName="phoneNumber"
                  class="form-input"
                  [class.input-error]="phoneError()"
                />
                @if (phoneError()) {
                  <span class="field-error">{{ phoneError() }}</span>
                }
              </div>

              <!-- Date of Birth -->
              <div class="form-group">
                <label for="dateOfBirth" class="form-label">{{
                  labels().dateOfBirthLabel
                }}</label>
                <input
                  id="dateOfBirth"
                  type="date"
                  formControlName="dateOfBirth"
                  class="form-input"
                  [class.input-error]="dateOfBirthError()"
                />
                @if (dateOfBirthError()) {
                  <span class="field-error">{{ dateOfBirthError() }}</span>
                }
              </div>
            </div>
          </section>

          <!-- Address Section -->
          <section class="form-section" formGroupName="address">
            <h2 class="section-title">{{ labels().addressSection }}</h2>
            <div class="form-grid">
              <!-- Street -->
              <div class="form-group form-group-full">
                <label for="street" class="form-label">{{
                  labels().streetLabel
                }}</label>
                <input
                  id="street"
                  type="text"
                  formControlName="street"
                  class="form-input"
                  [class.input-error]="streetError()"
                />
                @if (streetError()) {
                  <span class="field-error">{{ streetError() }}</span>
                }
              </div>

              <!-- Postal Code -->
              <div class="form-group">
                <label for="postalCode" class="form-label">{{
                  labels().postalCodeLabel
                }}</label>
                <input
                  id="postalCode"
                  type="text"
                  formControlName="postalCode"
                  class="form-input"
                  [class.input-error]="postalCodeError()"
                />
                @if (postalCodeError()) {
                  <span class="field-error">{{ postalCodeError() }}</span>
                }
              </div>

              <!-- City -->
              <div class="form-group">
                <label for="city" class="form-label">{{
                  labels().cityLabel
                }}</label>
                <input
                  id="city"
                  type="text"
                  formControlName="city"
                  class="form-input"
                  [class.input-error]="cityError()"
                />
                @if (cityError()) {
                  <span class="field-error">{{ cityError() }}</span>
                }
              </div>

              <!-- Country -->
              <div class="form-group form-group-full">
                <label for="country" class="form-label">{{
                  labels().countryLabel
                }}</label>
                <input
                  id="country"
                  type="text"
                  formControlName="country"
                  class="form-input"
                  [class.input-error]="countryError()"
                />
                @if (countryError()) {
                  <span class="field-error">{{ countryError() }}</span>
                }
              </div>
            </div>
          </section>

          <!-- Form Actions -->
          <div class="form-actions">
            <button
              type="button"
              class="cancel-button"
              (click)="cancel.emit()"
              [disabled]="loading()"
            >
              {{ labels().cancelButton }}
            </button>
            <button
              type="submit"
              class="submit-button"
              [disabled]="loading()"
            >
              @if (loading()) {
                <span class="spinner"></span>
                <span>{{ labels().savingButton }}</span>
              } @else {
                <span>{{ labels().saveButton }}</span>
              }
            </button>
          </div>
        </form>
      </div>
    </div>
  `,
  styles: [
    `
      .edit-container {
        padding: 1.5rem;
        max-width: 48rem;
        margin: 0 auto;
      }

      .edit-card {
        background: white;
        border-radius: 0.75rem;
        box-shadow:
          0 4px 6px -1px rgba(0, 0, 0, 0.1),
          0 2px 4px -1px rgba(0, 0, 0, 0.06);
        overflow: hidden;
      }

      .edit-header {
        padding: 1.5rem;
        border-bottom: 1px solid #e5e7eb;
      }

      .edit-title {
        margin: 0;
        font-size: 1.25rem;
        font-weight: 600;
        color: #111827;
      }

      .error-container {
        padding: 1rem 1.5rem 0;
      }

      .edit-form {
        padding: 1.5rem;
      }

      .form-section {
        margin-bottom: 2rem;
      }

      .form-section:last-of-type {
        margin-bottom: 0;
      }

      .section-title {
        margin: 0 0 1rem;
        font-size: 1rem;
        font-weight: 600;
        color: #374151;
      }

      .form-grid {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        gap: 1rem;
      }

      @media (max-width: 640px) {
        .form-grid {
          grid-template-columns: 1fr;
        }
      }

      .form-group {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
      }

      .form-group-full {
        grid-column: 1 / -1;
      }

      .form-label {
        font-size: 0.875rem;
        font-weight: 500;
        color: #374151;
      }

      .form-input {
        width: 100%;
        padding: 0.625rem 0.75rem;
        border: 1px solid #d1d5db;
        border-radius: 0.375rem;
        font-size: 0.875rem;
        transition:
          border-color 0.15s ease,
          box-shadow 0.15s ease;
      }

      .form-input:focus {
        outline: none;
        border-color: #f97316;
        box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.1);
      }

      .form-input.input-error {
        border-color: #ef4444;
      }

      .field-error {
        font-size: 0.75rem;
        color: #ef4444;
        margin-top: 0.25rem;
      }

      .form-actions {
        display: flex;
        justify-content: flex-end;
        gap: 0.75rem;
        padding-top: 1.5rem;
        border-top: 1px solid #e5e7eb;
        margin-top: 1.5rem;
      }

      .cancel-button {
        padding: 0.625rem 1.25rem;
        background: white;
        color: #374151;
        font-size: 0.875rem;
        font-weight: 500;
        border: 1px solid #d1d5db;
        border-radius: 0.375rem;
        cursor: pointer;
        transition: all 0.15s ease;
      }

      .cancel-button:hover:not(:disabled) {
        background: #f9fafb;
      }

      .cancel-button:disabled {
        opacity: 0.7;
        cursor: not-allowed;
      }

      .submit-button {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 0.5rem;
        padding: 0.625rem 1.25rem;
        background-color: #f97316;
        color: white;
        font-size: 0.875rem;
        font-weight: 500;
        border: none;
        border-radius: 0.375rem;
        cursor: pointer;
        transition: background-color 0.15s ease;
      }

      .submit-button:hover:not(:disabled) {
        background-color: #ea580c;
      }

      .submit-button:disabled {
        opacity: 0.7;
        cursor: not-allowed;
      }

      .spinner {
        width: 1rem;
        height: 1rem;
        border: 2px solid rgba(255, 255, 255, 0.3);
        border-top-color: white;
        border-radius: 50%;
        animation: spin 0.8s linear infinite;
      }

      @keyframes spin {
        to {
          transform: rotate(360deg);
        }
      }
    `,
  ],
})
export class ProfileEditComponent {
  private readonly fb = inject(FormBuilder);

  /**
   * Current profile data to edit
   */
  readonly profile = input<UserProfile | null>(null);

  /**
   * Component labels (for i18n)
   */
  readonly labels = input<ProfileEditLabels>(DEFAULT_PROFILE_EDIT_LABELS_DE);

  /**
   * External loading state
   */
  readonly loading = input(false);

  /**
   * External error message
   */
  readonly error = input<string | null>(null);

  /**
   * Emitted when form is submitted with valid data
   */
  readonly formSubmit = output<ProfileUpdateEvent>();

  /**
   * Emitted when cancel button is clicked
   */
  readonly cancel = output<void>();

  /**
   * Form group
   */
  readonly editForm: FormGroup;

  /**
   * Form touched state for validation
   */
  private readonly formTouched = signal(false);

  constructor() {
    this.editForm = this.fb.group({
      firstName: ["", [Validators.required]],
      lastName: ["", [Validators.required]],
      phoneNumber: [
        "",
        [Validators.required, Validators.pattern(/^\+?[\d\s-]{10,}$/)],
      ],
      dateOfBirth: ["", [Validators.required]],
      address: this.fb.group({
        street: ["", [Validators.required]],
        city: ["", [Validators.required]],
        postalCode: ["", [Validators.required]],
        country: ["", [Validators.required]],
      }),
    });

    // Populate form when profile changes
    effect(() => {
      const p = this.profile();
      if (p) {
        this.editForm.patchValue({
          firstName: p.firstName || "",
          lastName: p.lastName || "",
          phoneNumber: p.phoneNumber || "",
          dateOfBirth: p.dateOfBirth || "",
          address: {
            street: p.address?.street || "",
            city: p.address?.city || "",
            postalCode: p.address?.postalCode || "",
            country: p.address?.country || "",
          },
        });
      }
    });
  }

  // Validation error computed signals
  readonly firstNameError = computed(() => {
    this.formTouched();
    const ctrl = this.editForm.get("firstName");
    if (ctrl?.hasError("required") && ctrl?.touched) {
      return this.labels().firstNameRequired;
    }
    return null;
  });

  readonly lastNameError = computed(() => {
    this.formTouched();
    const ctrl = this.editForm.get("lastName");
    if (ctrl?.hasError("required") && ctrl?.touched) {
      return this.labels().lastNameRequired;
    }
    return null;
  });

  readonly phoneError = computed(() => {
    this.formTouched();
    const ctrl = this.editForm.get("phoneNumber");
    if (ctrl?.hasError("required") && ctrl?.touched) {
      return this.labels().phoneRequired;
    }
    if (ctrl?.hasError("pattern") && ctrl?.touched) {
      return this.labels().phoneInvalid;
    }
    return null;
  });

  readonly dateOfBirthError = computed(() => {
    this.formTouched();
    const ctrl = this.editForm.get("dateOfBirth");
    if (ctrl?.hasError("required") && ctrl?.touched) {
      return this.labels().dateOfBirthRequired;
    }
    return null;
  });

  readonly streetError = computed(() => {
    this.formTouched();
    const ctrl = this.editForm.get("address.street");
    if (ctrl?.hasError("required") && ctrl?.touched) {
      return this.labels().streetRequired;
    }
    return null;
  });

  readonly cityError = computed(() => {
    this.formTouched();
    const ctrl = this.editForm.get("address.city");
    if (ctrl?.hasError("required") && ctrl?.touched) {
      return this.labels().cityRequired;
    }
    return null;
  });

  readonly postalCodeError = computed(() => {
    this.formTouched();
    const ctrl = this.editForm.get("address.postalCode");
    if (ctrl?.hasError("required") && ctrl?.touched) {
      return this.labels().postalCodeRequired;
    }
    return null;
  });

  readonly countryError = computed(() => {
    this.formTouched();
    const ctrl = this.editForm.get("address.country");
    if (ctrl?.hasError("required") && ctrl?.touched) {
      return this.labels().countryRequired;
    }
    return null;
  });

  /**
   * Handle form submission
   */
  onSubmit(): void {
    if (this.editForm.invalid) {
      this.editForm.markAllAsTouched();
      this.formTouched.set(true);
      return;
    }

    const formValue = this.editForm.value;
    this.formSubmit.emit({
      firstName: formValue.firstName,
      lastName: formValue.lastName,
      phoneNumber: formValue.phoneNumber,
      dateOfBirth: formValue.dateOfBirth,
      address: {
        street: formValue.address.street,
        city: formValue.address.city,
        postalCode: formValue.address.postalCode,
        country: formValue.address.country,
      },
    });
  }

  /**
   * Reset the form to original values
   */
  reset(): void {
    const p = this.profile();
    if (p) {
      this.editForm.patchValue({
        firstName: p.firstName || "",
        lastName: p.lastName || "",
        phoneNumber: p.phoneNumber || "",
        dateOfBirth: p.dateOfBirth || "",
        address: {
          street: p.address?.street || "",
          city: p.address?.city || "",
          postalCode: p.address?.postalCode || "",
          country: p.address?.country || "",
        },
      });
    }
    this.editForm.markAsUntouched();
    this.formTouched.set(false);
  }
}
