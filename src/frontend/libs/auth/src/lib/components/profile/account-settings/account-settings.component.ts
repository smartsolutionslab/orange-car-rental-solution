import {
  Component,
  input,
  output,
  inject,
  effect,
} from "@angular/core";
import { CommonModule } from "@angular/common";
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
} from "@angular/forms";
import {
  ErrorAlertComponent,
  SuccessAlertComponent,
  IconComponent,
} from "@orange-car-rental/ui-components";
import type {
  AccountSettings,
  AccountSettingsLabels,
  AccountSettingsUpdateEvent,
} from "../profile.types";
import {
  DEFAULT_ACCOUNT_SETTINGS_LABELS_DE,
  AVAILABLE_LANGUAGES,
  AVAILABLE_CURRENCIES,
} from "../profile.types";

/**
 * Account Settings Component
 *
 * Form for managing account preferences and notifications.
 *
 * @example
 * <lib-account-settings
 *   [settings]="accountSettings()"
 *   [labels]="germanLabels"
 *   [loading]="isSaving()"
 *   [error]="saveError()"
 *   [success]="saveSuccess()"
 *   (formSubmit)="onSaveSettings($event)"
 *   (deleteAccount)="onDeleteAccount()"
 * />
 */
@Component({
  selector: "lib-account-settings",
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    ErrorAlertComponent,
    SuccessAlertComponent,
    IconComponent,
  ],
  template: `
    <div class="settings-container">
      <div class="settings-card">
        <!-- Header -->
        <div class="settings-header">
          <h1 class="settings-title">{{ labels().title }}</h1>
        </div>

        <!-- Success Message -->
        @if (success()) {
          <div class="alert-container">
            <ui-success-alert [message]="success()!"></ui-success-alert>
          </div>
        }

        <!-- Error Message -->
        @if (error()) {
          <div class="alert-container">
            <ui-error-alert [message]="error()!"></ui-error-alert>
          </div>
        }

        <!-- Settings Form -->
        <form
          [formGroup]="settingsForm"
          (ngSubmit)="onSubmit()"
          class="settings-form"
        >
          <!-- Notifications Section -->
          <section class="form-section">
            <h2 class="section-title">{{ labels().notificationsSection }}</h2>

            <!-- Email Notifications -->
            <label class="toggle-item">
              <div class="toggle-info">
                <span class="toggle-label">{{
                  labels().emailNotificationsLabel
                }}</span>
                <span class="toggle-description">{{
                  labels().emailNotificationsDescription
                }}</span>
              </div>
              <div class="toggle-wrapper">
                <input
                  type="checkbox"
                  formControlName="emailNotifications"
                  class="toggle-input"
                />
                <span class="toggle-slider"></span>
              </div>
            </label>

            <!-- SMS Notifications -->
            <label class="toggle-item">
              <div class="toggle-info">
                <span class="toggle-label">{{
                  labels().smsNotificationsLabel
                }}</span>
                <span class="toggle-description">{{
                  labels().smsNotificationsDescription
                }}</span>
              </div>
              <div class="toggle-wrapper">
                <input
                  type="checkbox"
                  formControlName="smsNotifications"
                  class="toggle-input"
                />
                <span class="toggle-slider"></span>
              </div>
            </label>

            <!-- Marketing Emails -->
            <label class="toggle-item">
              <div class="toggle-info">
                <span class="toggle-label">{{
                  labels().marketingEmailsLabel
                }}</span>
                <span class="toggle-description">{{
                  labels().marketingEmailsDescription
                }}</span>
              </div>
              <div class="toggle-wrapper">
                <input
                  type="checkbox"
                  formControlName="marketingEmails"
                  class="toggle-input"
                />
                <span class="toggle-slider"></span>
              </div>
            </label>
          </section>

          <!-- Preferences Section -->
          <section class="form-section">
            <h2 class="section-title">{{ labels().preferencesSection }}</h2>

            <div class="preferences-grid">
              <!-- Language -->
              <div class="form-group">
                <label for="language" class="form-label">{{
                  labels().languageLabel
                }}</label>
                <select
                  id="language"
                  formControlName="language"
                  class="form-select"
                >
                  @for (lang of languages; track lang.code) {
                    <option [value]="lang.code">{{ lang.name }}</option>
                  }
                </select>
              </div>

              <!-- Currency -->
              <div class="form-group">
                <label for="currency" class="form-label">{{
                  labels().currencyLabel
                }}</label>
                <select
                  id="currency"
                  formControlName="currency"
                  class="form-select"
                >
                  @for (curr of currencies; track curr.code) {
                    <option [value]="curr.code">{{ curr.name }}</option>
                  }
                </select>
              </div>
            </div>
          </section>

          <!-- Save Button -->
          <div class="form-actions">
            <button
              type="submit"
              class="submit-button"
              [disabled]="loading() || !settingsForm.dirty"
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

        <!-- Danger Zone -->
        <section class="danger-zone">
          <h2 class="section-title danger-title">
            {{ labels().dangerZoneSection }}
          </h2>
          <p class="danger-warning">{{ labels().deleteAccountWarning }}</p>
          <button
            type="button"
            class="delete-button"
            (click)="onDeleteAccountClick()"
          >
            <lib-icon name="trash" variant="outline" size="sm" />
            <span>{{ labels().deleteAccountButton }}</span>
          </button>
        </section>
      </div>
    </div>
  `,
  styles: [
    `
      .settings-container {
        padding: 1.5rem;
        max-width: 40rem;
        margin: 0 auto;
      }

      .settings-card {
        background: white;
        border-radius: 0.75rem;
        box-shadow:
          0 4px 6px -1px rgba(0, 0, 0, 0.1),
          0 2px 4px -1px rgba(0, 0, 0, 0.06);
        overflow: hidden;
      }

      .settings-header {
        padding: 1.5rem;
        border-bottom: 1px solid #e5e7eb;
      }

      .settings-title {
        margin: 0;
        font-size: 1.25rem;
        font-weight: 600;
        color: #111827;
      }

      .alert-container {
        padding: 1rem 1.5rem 0;
      }

      .settings-form {
        padding: 1.5rem;
      }

      .form-section {
        margin-bottom: 2rem;
      }

      .section-title {
        margin: 0 0 1rem;
        font-size: 1rem;
        font-weight: 600;
        color: #374151;
      }

      .toggle-item {
        display: flex;
        justify-content: space-between;
        align-items: center;
        padding: 1rem;
        margin: 0 -1rem;
        cursor: pointer;
        border-radius: 0.5rem;
        transition: background-color 0.15s ease;
      }

      .toggle-item:hover {
        background: #f9fafb;
      }

      .toggle-info {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
        padding-right: 1rem;
      }

      .toggle-label {
        font-size: 0.9375rem;
        font-weight: 500;
        color: #111827;
      }

      .toggle-description {
        font-size: 0.8125rem;
        color: #6b7280;
      }

      .toggle-wrapper {
        position: relative;
        width: 2.75rem;
        height: 1.5rem;
        flex-shrink: 0;
      }

      .toggle-input {
        position: absolute;
        width: 100%;
        height: 100%;
        opacity: 0;
        cursor: pointer;
        z-index: 1;
      }

      .toggle-slider {
        position: absolute;
        inset: 0;
        background-color: #d1d5db;
        border-radius: 1rem;
        transition: background-color 0.2s ease;
      }

      .toggle-slider::before {
        content: "";
        position: absolute;
        left: 0.125rem;
        top: 0.125rem;
        width: 1.25rem;
        height: 1.25rem;
        background: white;
        border-radius: 50%;
        box-shadow: 0 1px 3px rgba(0, 0, 0, 0.2);
        transition: transform 0.2s ease;
      }

      .toggle-input:checked + .toggle-slider {
        background-color: #f97316;
      }

      .toggle-input:checked + .toggle-slider::before {
        transform: translateX(1.25rem);
      }

      .toggle-input:focus + .toggle-slider {
        box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.2);
      }

      .preferences-grid {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        gap: 1rem;
      }

      @media (max-width: 640px) {
        .preferences-grid {
          grid-template-columns: 1fr;
        }
      }

      .form-group {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
      }

      .form-label {
        font-size: 0.875rem;
        font-weight: 500;
        color: #374151;
      }

      .form-select {
        width: 100%;
        padding: 0.625rem 2rem 0.625rem 0.75rem;
        border: 1px solid #d1d5db;
        border-radius: 0.375rem;
        font-size: 0.875rem;
        background-color: white;
        background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' fill='none' viewBox='0 0 20 20'%3e%3cpath stroke='%236b7280' stroke-linecap='round' stroke-linejoin='round' stroke-width='1.5' d='M6 8l4 4 4-4'/%3e%3c/svg%3e");
        background-position: right 0.5rem center;
        background-repeat: no-repeat;
        background-size: 1.5em 1.5em;
        appearance: none;
        cursor: pointer;
        transition:
          border-color 0.15s ease,
          box-shadow 0.15s ease;
      }

      .form-select:focus {
        outline: none;
        border-color: #f97316;
        box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.1);
      }

      .form-actions {
        display: flex;
        justify-content: flex-end;
        padding-top: 1rem;
        border-top: 1px solid #e5e7eb;
      }

      .submit-button {
        display: flex;
        align-items: center;
        justify-content: center;
        gap: 0.5rem;
        padding: 0.625rem 1.5rem;
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

      .danger-zone {
        padding: 1.5rem;
        background: #fef2f2;
        border-top: 1px solid #fecaca;
      }

      .danger-title {
        color: #b91c1c;
      }

      .danger-warning {
        margin: 0 0 1rem;
        font-size: 0.875rem;
        color: #991b1b;
      }

      .delete-button {
        display: flex;
        align-items: center;
        gap: 0.5rem;
        padding: 0.625rem 1.25rem;
        background: white;
        color: #dc2626;
        font-size: 0.875rem;
        font-weight: 500;
        border: 1px solid #dc2626;
        border-radius: 0.375rem;
        cursor: pointer;
        transition: all 0.15s ease;
      }

      .delete-button:hover {
        background: #dc2626;
        color: white;
      }
    `,
  ],
})
export class AccountSettingsComponent {
  private readonly fb = inject(FormBuilder);

  /**
   * Current account settings
   */
  readonly settings = input<AccountSettings | null>(null);

  /**
   * Component labels (for i18n)
   */
  readonly labels = input<AccountSettingsLabels>(
    DEFAULT_ACCOUNT_SETTINGS_LABELS_DE
  );

  /**
   * External loading state
   */
  readonly loading = input(false);

  /**
   * External error message
   */
  readonly error = input<string | null>(null);

  /**
   * External success message
   */
  readonly success = input<string | null>(null);

  /**
   * Emitted when form is submitted with valid data
   */
  readonly formSubmit = output<AccountSettingsUpdateEvent>();

  /**
   * Emitted when delete account button is clicked
   */
  readonly deleteAccount = output<void>();

  /**
   * Available languages
   */
  readonly languages = AVAILABLE_LANGUAGES;

  /**
   * Available currencies
   */
  readonly currencies = AVAILABLE_CURRENCIES;

  /**
   * Form group
   */
  readonly settingsForm: FormGroup;

  constructor() {
    this.settingsForm = this.fb.group({
      emailNotifications: [true],
      smsNotifications: [false],
      marketingEmails: [false],
      language: ["de"],
      currency: ["EUR"],
    });

    // Populate form when settings change
    effect(() => {
      const s = this.settings();
      if (s) {
        this.settingsForm.patchValue({
          emailNotifications: s.emailNotifications ?? true,
          smsNotifications: s.smsNotifications ?? false,
          marketingEmails: s.marketingEmails ?? false,
          language: s.language || "de",
          currency: s.currency || "EUR",
        });
        this.settingsForm.markAsPristine();
      }
    });
  }

  /**
   * Handle form submission
   */
  onSubmit(): void {
    const formValue = this.settingsForm.value;
    this.formSubmit.emit({
      emailNotifications: formValue.emailNotifications,
      smsNotifications: formValue.smsNotifications,
      marketingEmails: formValue.marketingEmails,
      language: formValue.language,
      currency: formValue.currency,
    });
  }

  /**
   * Handle delete account click
   */
  onDeleteAccountClick(): void {
    this.deleteAccount.emit();
  }

  /**
   * Reset the form to original values
   */
  reset(): void {
    const s = this.settings();
    if (s) {
      this.settingsForm.patchValue({
        emailNotifications: s.emailNotifications ?? true,
        smsNotifications: s.smsNotifications ?? false,
        marketingEmails: s.marketingEmails ?? false,
        language: s.language || "de",
        currency: s.currency || "EUR",
      });
    }
    this.settingsForm.markAsPristine();
  }
}
