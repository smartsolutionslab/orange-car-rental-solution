import type { Meta, StoryObj } from "@storybook/angular";
import { PasswordChangeComponent } from "./password-change.component";
import type { PasswordChangeLabels } from "../profile.types";
import { DEFAULT_PASSWORD_CHANGE_LABELS_DE } from "../profile.types";

const meta: Meta<PasswordChangeComponent> = {
  title: "Auth/Password Change",
  component: PasswordChangeComponent,
  parameters: {
    layout: "centered",
    docs: {
      description: {
        component: `
A form component for changing user password.

## Features
- Current password verification
- New password with strength validation
- Password confirmation matching
- Password visibility toggles
- Loading and success/error states
- Full i18n support via labels

## Usage

\`\`\`html
<lib-password-change
  [labels]="germanLabels"
  [loading]="isChanging()"
  [error]="changeError()"
  [success]="changeSuccess()"
  (formSubmit)="onChangePassword($event)"
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
    success: {
      control: "text",
      description: "Success message to display",
    },
    minPasswordLength: {
      control: "number",
      description: "Minimum password length requirement",
    },
  },
};

export default meta;
type Story = StoryObj<PasswordChangeComponent>;

// German labels (default)
const germanLabels: PasswordChangeLabels = DEFAULT_PASSWORD_CHANGE_LABELS_DE;

// English labels
const englishLabels: PasswordChangeLabels = {
  title: "Change Password",
  subtitle: "Choose a secure password for your account",
  currentPasswordLabel: "Current Password",
  newPasswordLabel: "New Password",
  confirmPasswordLabel: "Confirm New Password",
  saveButton: "Change Password",
  savingButton: "Changing...",
  cancelButton: "Cancel",
  passwordHint: "At least 8 characters with uppercase, lowercase, numbers, and special characters",
  currentPasswordRequired: "Current password is required",
  newPasswordRequired: "New password is required",
  newPasswordMinLength: "Password must be at least 8 characters",
  newPasswordWeak: "Password must contain uppercase, lowercase, numbers, and special characters",
  confirmPasswordRequired: "Password confirmation is required",
  passwordMismatch: "Passwords do not match",
  sameAsCurrentPassword: "New password must be different from current password",
};

/**
 * Default password change form
 */
export const Default: Story = {
  args: {
    labels: germanLabels,
    loading: false,
    error: null,
    success: null,
    minPasswordLength: 8,
  },
};

/**
 * Password change form with English labels
 */
export const English: Story = {
  args: {
    labels: englishLabels,
    loading: false,
    error: null,
    success: null,
    minPasswordLength: 8,
  },
};

/**
 * Form in loading state
 */
export const Loading: Story = {
  args: {
    labels: germanLabels,
    loading: true,
    error: null,
    success: null,
  },
};

/**
 * Form with wrong password error
 */
export const WrongPasswordError: Story = {
  args: {
    labels: germanLabels,
    loading: false,
    error: "Das aktuelle Passwort ist nicht korrekt.",
    success: null,
  },
};

/**
 * Form with success message
 */
export const Success: Story = {
  args: {
    labels: germanLabels,
    loading: false,
    error: null,
    success: "Ihr Passwort wurde erfolgreich geändert.",
  },
};

/**
 * Form with network error
 */
export const NetworkError: Story = {
  args: {
    labels: englishLabels,
    loading: false,
    error: "Unable to change password. Please check your internet connection and try again.",
    success: null,
  },
};
