import { Component, input, output, computed } from "@angular/core";
import { CommonModule, DatePipe } from "@angular/common";
import { IconComponent } from "@orange-car-rental/ui-components";
import type { UserProfile, ProfileViewLabels } from "../profile.types";
import { DEFAULT_PROFILE_VIEW_LABELS_DE } from "../profile.types";

/**
 * Profile View Component
 *
 * Displays user profile information in a read-only format.
 * Supports editing via emitted events.
 *
 * @example
 * <lib-profile-view
 *   [profile]="userProfile()"
 *   [labels]="germanLabels"
 *   (editProfile)="onEditProfile()"
 *   (editAddress)="onEditAddress()"
 *   (editLicense)="onEditLicense()"
 * />
 */
@Component({
  selector: "lib-profile-view",
  standalone: true,
  imports: [CommonModule, DatePipe, IconComponent],
  template: `
    <div class="profile-container">
      <div class="profile-card">
        <!-- Header -->
        <div class="profile-header">
          <div class="profile-avatar">
            <span class="avatar-initials">{{ initials() }}</span>
          </div>
          <div class="profile-header-info">
            <h1 class="profile-name">{{ fullName() }}</h1>
            @if (profile()?.createdAt) {
              <p class="member-since">
                {{ labels().memberSinceLabel }}
                {{ profile()!.createdAt | date : "MMMM yyyy" }}
              </p>
            }
          </div>
          <button
            type="button"
            class="edit-button edit-button-header"
            (click)="editProfile.emit()"
            [attr.aria-label]="labels().editButton"
          >
            <lib-icon name="pencil" variant="outline" size="sm" />
            <span>{{ labels().editButton }}</span>
          </button>
        </div>

        <!-- Personal Information Section -->
        <section class="profile-section">
          <h2 class="section-title">{{ labels().personalInfoSection }}</h2>
          <div class="info-grid">
            <div class="info-item">
              <span class="info-label">{{ labels().firstNameLabel }}</span>
              <span class="info-value">{{ profile()?.firstName || "-" }}</span>
            </div>
            <div class="info-item">
              <span class="info-label">{{ labels().lastNameLabel }}</span>
              <span class="info-value">{{ profile()?.lastName || "-" }}</span>
            </div>
            <div class="info-item">
              <span class="info-label">{{ labels().dateOfBirthLabel }}</span>
              <span class="info-value">{{
                profile()?.dateOfBirth
                  ? (profile()!.dateOfBirth | date : "dd.MM.yyyy")
                  : "-"
              }}</span>
            </div>
          </div>
        </section>

        <!-- Contact Information Section -->
        <section class="profile-section">
          <h2 class="section-title">{{ labels().contactInfoSection }}</h2>
          <div class="info-grid">
            <div class="info-item">
              <span class="info-label">{{ labels().emailLabel }}</span>
              <span class="info-value">{{ profile()?.email || "-" }}</span>
            </div>
            <div class="info-item">
              <span class="info-label">{{ labels().phoneLabel }}</span>
              <span class="info-value">{{
                profile()?.phoneNumber || "-"
              }}</span>
            </div>
          </div>
        </section>

        <!-- Address Section -->
        <section class="profile-section">
          <div class="section-header">
            <h2 class="section-title">{{ labels().addressSection }}</h2>
            @if (hasAddress()) {
              <button
                type="button"
                class="edit-button edit-button-section"
                (click)="editAddress.emit()"
              >
                <lib-icon name="pencil" variant="outline" size="xs" />
              </button>
            }
          </div>
          @if (hasAddress()) {
            <div class="info-grid">
              <div class="info-item info-item-full">
                <span class="info-label">{{ labels().streetLabel }}</span>
                <span class="info-value">{{
                  profile()?.address?.street || "-"
                }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">{{ labels().postalCodeLabel }}</span>
                <span class="info-value">{{
                  profile()?.address?.postalCode || "-"
                }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">{{ labels().cityLabel }}</span>
                <span class="info-value">{{
                  profile()?.address?.city || "-"
                }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">{{ labels().countryLabel }}</span>
                <span class="info-value">{{
                  profile()?.address?.country || "-"
                }}</span>
              </div>
            </div>
          } @else {
            <div class="empty-section">
              <p class="empty-text">{{ labels().noAddressText }}</p>
              <button
                type="button"
                class="add-button"
                (click)="editAddress.emit()"
              >
                <lib-icon name="plus" variant="outline" size="sm" />
                <span>{{ labels().addAddressButton }}</span>
              </button>
            </div>
          }
        </section>

        <!-- Driver's License Section -->
        <section class="profile-section">
          <div class="section-header">
            <h2 class="section-title">{{ labels().driversLicenseSection }}</h2>
            @if (hasLicense()) {
              <button
                type="button"
                class="edit-button edit-button-section"
                (click)="editLicense.emit()"
              >
                <lib-icon name="pencil" variant="outline" size="xs" />
              </button>
            }
          </div>
          @if (hasLicense()) {
            <div class="info-grid">
              <div class="info-item">
                <span class="info-label">{{
                  labels().licenseNumberLabel
                }}</span>
                <span class="info-value">{{
                  profile()?.driversLicense?.licenseNumber || "-"
                }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">{{
                  labels().licenseIssueCountryLabel
                }}</span>
                <span class="info-value">{{
                  profile()?.driversLicense?.licenseIssueCountry || "-"
                }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">{{
                  labels().licenseIssueDateLabel
                }}</span>
                <span class="info-value">{{
                  profile()?.driversLicense?.licenseIssueDate
                    ? (profile()!.driversLicense!.licenseIssueDate
                      | date : "dd.MM.yyyy")
                    : "-"
                }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">{{
                  labels().licenseExpiryDateLabel
                }}</span>
                <span class="info-value">{{
                  profile()?.driversLicense?.licenseExpiryDate
                    ? (profile()!.driversLicense!.licenseExpiryDate
                      | date : "dd.MM.yyyy")
                    : "-"
                }}</span>
              </div>
            </div>
          } @else {
            <div class="empty-section">
              <p class="empty-text">{{ labels().noLicenseText }}</p>
              <button
                type="button"
                class="add-button"
                (click)="editLicense.emit()"
              >
                <lib-icon name="plus" variant="outline" size="sm" />
                <span>{{ labels().addLicenseButton }}</span>
              </button>
            </div>
          }
        </section>
      </div>
    </div>
  `,
  styles: [
    `
      .profile-container {
        padding: 1.5rem;
        max-width: 48rem;
        margin: 0 auto;
      }

      .profile-card {
        background: white;
        border-radius: 0.75rem;
        box-shadow:
          0 4px 6px -1px rgba(0, 0, 0, 0.1),
          0 2px 4px -1px rgba(0, 0, 0, 0.06);
        overflow: hidden;
      }

      .profile-header {
        display: flex;
        align-items: center;
        gap: 1rem;
        padding: 1.5rem;
        background: linear-gradient(135deg, #f97316 0%, #ea580c 100%);
        color: white;
      }

      .profile-avatar {
        width: 4rem;
        height: 4rem;
        border-radius: 50%;
        background: rgba(255, 255, 255, 0.2);
        display: flex;
        align-items: center;
        justify-content: center;
        flex-shrink: 0;
      }

      .avatar-initials {
        font-size: 1.25rem;
        font-weight: 600;
        text-transform: uppercase;
      }

      .profile-header-info {
        flex: 1;
        min-width: 0;
      }

      .profile-name {
        margin: 0;
        font-size: 1.25rem;
        font-weight: 600;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }

      .member-since {
        margin: 0.25rem 0 0;
        font-size: 0.875rem;
        opacity: 0.9;
      }

      .edit-button {
        display: flex;
        align-items: center;
        gap: 0.375rem;
        padding: 0.5rem 0.75rem;
        background: transparent;
        border: none;
        border-radius: 0.375rem;
        cursor: pointer;
        transition: background-color 0.15s ease;
      }

      .edit-button-header {
        color: white;
        background: rgba(255, 255, 255, 0.1);
      }

      .edit-button-header:hover {
        background: rgba(255, 255, 255, 0.2);
      }

      .edit-button-section {
        color: #6b7280;
        padding: 0.375rem;
      }

      .edit-button-section:hover {
        background: #f3f4f6;
        color: #374151;
      }

      .profile-section {
        padding: 1.5rem;
        border-bottom: 1px solid #e5e7eb;
      }

      .profile-section:last-child {
        border-bottom: none;
      }

      .section-header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 1rem;
      }

      .section-title {
        margin: 0 0 1rem;
        font-size: 1rem;
        font-weight: 600;
        color: #374151;
      }

      .section-header .section-title {
        margin-bottom: 0;
      }

      .info-grid {
        display: grid;
        grid-template-columns: repeat(2, 1fr);
        gap: 1rem;
      }

      @media (max-width: 640px) {
        .info-grid {
          grid-template-columns: 1fr;
        }
      }

      .info-item {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
      }

      .info-item-full {
        grid-column: 1 / -1;
      }

      .info-label {
        font-size: 0.75rem;
        font-weight: 500;
        color: #6b7280;
        text-transform: uppercase;
        letter-spacing: 0.025em;
      }

      .info-value {
        font-size: 0.9375rem;
        color: #111827;
      }

      .empty-section {
        display: flex;
        flex-direction: column;
        align-items: center;
        padding: 1.5rem;
        background: #f9fafb;
        border-radius: 0.5rem;
        text-align: center;
      }

      .empty-text {
        margin: 0 0 1rem;
        font-size: 0.875rem;
        color: #6b7280;
      }

      .add-button {
        display: flex;
        align-items: center;
        gap: 0.375rem;
        padding: 0.5rem 1rem;
        background: white;
        color: #f97316;
        font-size: 0.875rem;
        font-weight: 500;
        border: 1px solid #f97316;
        border-radius: 0.375rem;
        cursor: pointer;
        transition: all 0.15s ease;
      }

      .add-button:hover {
        background: #fff7ed;
      }
    `,
  ],
})
export class ProfileViewComponent {
  /**
   * User profile data
   */
  readonly profile = input<UserProfile | null>(null);

  /**
   * Component labels (for i18n)
   */
  readonly labels = input<ProfileViewLabels>(DEFAULT_PROFILE_VIEW_LABELS_DE);

  /**
   * Emitted when edit profile button is clicked
   */
  readonly editProfile = output<void>();

  /**
   * Emitted when edit address button is clicked
   */
  readonly editAddress = output<void>();

  /**
   * Emitted when edit license button is clicked
   */
  readonly editLicense = output<void>();

  /**
   * Computed user initials
   */
  readonly initials = computed(() => {
    const p = this.profile();
    if (!p) return "?";
    const first = p.firstName?.charAt(0) || "";
    const last = p.lastName?.charAt(0) || "";
    return first + last || "?";
  });

  /**
   * Computed full name
   */
  readonly fullName = computed(() => {
    const p = this.profile();
    if (!p) return "";
    return `${p.firstName || ""} ${p.lastName || ""}`.trim();
  });

  /**
   * Check if address is available
   */
  readonly hasAddress = computed(() => {
    const address = this.profile()?.address;
    return !!(
      address?.street ||
      address?.city ||
      address?.postalCode ||
      address?.country
    );
  });

  /**
   * Check if driver's license is available
   */
  readonly hasLicense = computed(() => {
    const license = this.profile()?.driversLicense;
    return !!(
      license?.licenseNumber ||
      license?.licenseIssueCountry ||
      license?.licenseIssueDate ||
      license?.licenseExpiryDate
    );
  });
}
