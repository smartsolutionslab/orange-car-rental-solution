import type { Meta, StoryObj } from "@storybook/angular";
import { ProfileEditComponent } from "./profile-edit.component";
import type { UserProfile, ProfileEditLabels } from "../profile.types";
import { DEFAULT_PROFILE_EDIT_LABELS_DE } from "../profile.types";

const meta: Meta<ProfileEditComponent> = {
  title: "Auth/Profile Edit",
  component: ProfileEditComponent,
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A form component for editing user profile information.

## Features
- Edit personal information (name, phone, date of birth)
- Edit address information
- Form validation with error messages
- Loading state during save
- Full i18n support via labels

## Usage

\`\`\`html
<lib-profile-edit
  [profile]="userProfile()"
  [labels]="germanLabels"
  [loading]="isSaving()"
  [error]="saveError()"
  (formSubmit)="onSaveProfile($event)"
  (cancel)="onCancel()"
/>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    loading: {
      control: "boolean",
      description: "Shows loading spinner and disables form",
    },
    error: {
      control: "text",
      description: "Error message to display",
    },
  },
};

export default meta;
type Story = StoryObj<ProfileEditComponent>;

// German labels (default)
const germanLabels: ProfileEditLabels = DEFAULT_PROFILE_EDIT_LABELS_DE;

// English labels
const englishLabels: ProfileEditLabels = {
  title: "Edit Profile",
  personalInfoSection: "Personal Information",
  addressSection: "Address",
  firstNameLabel: "First Name",
  lastNameLabel: "Last Name",
  phoneLabel: "Phone Number",
  dateOfBirthLabel: "Date of Birth",
  streetLabel: "Street Address",
  cityLabel: "City",
  postalCodeLabel: "Postal Code",
  countryLabel: "Country",
  saveButton: "Save",
  savingButton: "Saving...",
  cancelButton: "Cancel",
  firstNameRequired: "First name is required",
  lastNameRequired: "Last name is required",
  phoneRequired: "Phone number is required",
  phoneInvalid: "Please enter a valid phone number",
  dateOfBirthRequired: "Date of birth is required",
  streetRequired: "Street is required",
  cityRequired: "City is required",
  postalCodeRequired: "Postal code is required",
  countryRequired: "Country is required",
};

// Sample profile data
const sampleProfile: UserProfile = {
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
};

/**
 * Default edit form with pre-filled data
 */
export const Default: Story = {
  args: {
    profile: sampleProfile,
    labels: germanLabels,
    loading: false,
    error: null,
  },
};

/**
 * Edit form with English labels
 */
export const English: Story = {
  args: {
    profile: sampleProfile,
    labels: englishLabels,
    loading: false,
    error: null,
  },
};

/**
 * Empty form for new profile
 */
export const Empty: Story = {
  args: {
    profile: null,
    labels: germanLabels,
    loading: false,
    error: null,
  },
};

/**
 * Form in loading state
 */
export const Loading: Story = {
  args: {
    profile: sampleProfile,
    labels: germanLabels,
    loading: true,
    error: null,
  },
};

/**
 * Form with error message
 */
export const WithError: Story = {
  args: {
    profile: sampleProfile,
    labels: germanLabels,
    loading: false,
    error: "Die Telefonnummer wird bereits von einem anderen Konto verwendet.",
  },
};

/**
 * Form with network error
 */
export const NetworkError: Story = {
  args: {
    profile: sampleProfile,
    labels: englishLabels,
    loading: false,
    error: "Unable to save profile. Please check your internet connection and try again.",
  },
};
