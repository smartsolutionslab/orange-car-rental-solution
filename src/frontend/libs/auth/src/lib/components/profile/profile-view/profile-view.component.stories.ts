import type { Meta, StoryObj } from "@storybook/angular";
import { ProfileViewComponent } from "./profile-view.component";
import type { UserProfile, ProfileViewLabels } from "../profile.types";
import { DEFAULT_PROFILE_VIEW_LABELS_DE } from "../profile.types";

const meta: Meta<ProfileViewComponent> = {
  title: "Auth/Profile View",
  component: ProfileViewComponent,
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A read-only profile view component that displays user information.

## Features
- Displays personal information (name, date of birth)
- Displays contact information (email, phone)
- Displays address (if available)
- Displays driver's license (if available)
- Emits events for editing different sections
- Full i18n support via labels

## Usage

\`\`\`html
<lib-profile-view
  [profile]="userProfile()"
  [labels]="germanLabels"
  (editProfile)="onEditProfile()"
  (editAddress)="onEditAddress()"
  (editLicense)="onEditLicense()"
/>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
};

export default meta;
type Story = StoryObj<ProfileViewComponent>;

// German labels (default)
const germanLabels: ProfileViewLabels = DEFAULT_PROFILE_VIEW_LABELS_DE;

// English labels
const englishLabels: ProfileViewLabels = {
  title: "My Profile",
  personalInfoSection: "Personal Information",
  contactInfoSection: "Contact Information",
  addressSection: "Address",
  driversLicenseSection: "Driver's License",
  firstNameLabel: "First Name",
  lastNameLabel: "Last Name",
  emailLabel: "Email",
  phoneLabel: "Phone",
  dateOfBirthLabel: "Date of Birth",
  streetLabel: "Street",
  cityLabel: "City",
  postalCodeLabel: "Postal Code",
  countryLabel: "Country",
  licenseNumberLabel: "License Number",
  licenseIssueCountryLabel: "Issue Country",
  licenseIssueDateLabel: "Issue Date",
  licenseExpiryDateLabel: "Expiry Date",
  editButton: "Edit",
  memberSinceLabel: "Member since",
  noAddressText: "No address on file",
  noLicenseText: "No driver's license on file",
  addAddressButton: "Add Address",
  addLicenseButton: "Add License",
};

// Complete profile data
const completeProfile: UserProfile = {
  id: "user-123",
  firstName: "Max",
  lastName: "Mustermann",
  email: "max.mustermann@example.com",
  phoneNumber: "+49 151 12345678",
  dateOfBirth: "1990-05-15",
  address: {
    street: "Musterstraße 123",
    city: "Berlin",
    postalCode: "10115",
    country: "Deutschland",
  },
  driversLicense: {
    licenseNumber: "B072RRE2I55",
    licenseIssueCountry: "Deutschland",
    licenseIssueDate: "2010-06-20",
    licenseExpiryDate: "2030-06-20",
  },
  createdAt: "2023-01-15T10:30:00Z",
};

// Minimal profile (no address or license)
const minimalProfile: UserProfile = {
  id: "user-456",
  firstName: "Anna",
  lastName: "Schmidt",
  email: "anna.schmidt@example.com",
  phoneNumber: "+49 170 98765432",
  dateOfBirth: "1985-12-03",
  createdAt: "2024-06-01T14:00:00Z",
};

/**
 * Complete profile with all information
 */
export const Complete: Story = {
  args: {
    profile: completeProfile,
    labels: germanLabels,
  },
};

/**
 * Profile with English labels
 */
export const English: Story = {
  args: {
    profile: completeProfile,
    labels: englishLabels,
  },
};

/**
 * Minimal profile without address and license
 */
export const Minimal: Story = {
  args: {
    profile: minimalProfile,
    labels: germanLabels,
  },
};

/**
 * Profile with only address (no license)
 */
export const WithAddressOnly: Story = {
  args: {
    profile: {
      ...minimalProfile,
      address: {
        street: "Hauptstraße 42",
        city: "München",
        postalCode: "80331",
        country: "Deutschland",
      },
    },
    labels: germanLabels,
  },
};

/**
 * Profile with only license (no address)
 */
export const WithLicenseOnly: Story = {
  args: {
    profile: {
      ...minimalProfile,
      driversLicense: {
        licenseNumber: "C123456789",
        licenseIssueCountry: "Deutschland",
        licenseIssueDate: "2015-03-10",
        licenseExpiryDate: "2025-03-10",
      },
    },
    labels: germanLabels,
  },
};

/**
 * Empty profile state
 */
export const Empty: Story = {
  args: {
    profile: null,
    labels: germanLabels,
  },
};
