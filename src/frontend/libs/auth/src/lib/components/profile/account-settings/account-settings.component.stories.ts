import type { Meta, StoryObj } from "@storybook/angular";
import { AccountSettingsComponent } from "./account-settings.component";
import type { AccountSettings, AccountSettingsLabels } from "../profile.types";
import { DEFAULT_ACCOUNT_SETTINGS_LABELS_DE } from "../profile.types";

const meta: Meta<AccountSettingsComponent> = {
  title: "Auth/Account Settings",
  component: AccountSettingsComponent,
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A form component for managing account settings and preferences.

## Features
- Notification preferences (email, SMS, marketing)
- Language selection
- Currency selection
- Account deletion (danger zone)
- Loading and success/error states
- Full i18n support via labels

## Usage

\`\`\`html
<lib-account-settings
  [settings]="accountSettings()"
  [labels]="germanLabels"
  [loading]="isSaving()"
  [error]="saveError()"
  [success]="saveSuccess()"
  (formSubmit)="onSaveSettings($event)"
  (deleteAccount)="onDeleteAccount()"
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
  },
};

export default meta;
type Story = StoryObj<AccountSettingsComponent>;

// German labels (default)
const germanLabels: AccountSettingsLabels = DEFAULT_ACCOUNT_SETTINGS_LABELS_DE;

// English labels
const englishLabels: AccountSettingsLabels = {
  title: "Account Settings",
  notificationsSection: "Notifications",
  preferencesSection: "Preferences",
  emailNotificationsLabel: "Email Notifications",
  emailNotificationsDescription: "Receive important updates about your bookings via email",
  smsNotificationsLabel: "SMS Notifications",
  smsNotificationsDescription: "Receive booking confirmations and reminders via SMS",
  marketingEmailsLabel: "Marketing Emails",
  marketingEmailsDescription: "Receive offers, discounts, and news",
  languageLabel: "Language",
  currencyLabel: "Currency",
  saveButton: "Save Settings",
  savingButton: "Saving...",
  dangerZoneSection: "Danger Zone",
  deleteAccountButton: "Delete Account",
  deleteAccountWarning: "This action cannot be undone. All your data will be permanently deleted.",
};

// Default settings
const defaultSettings: AccountSettings = {
  emailNotifications: true,
  smsNotifications: false,
  marketingEmails: false,
  language: "de",
  currency: "EUR",
};

// All notifications enabled
const allNotificationsEnabled: AccountSettings = {
  emailNotifications: true,
  smsNotifications: true,
  marketingEmails: true,
  language: "de",
  currency: "EUR",
};

// English user settings
const englishUserSettings: AccountSettings = {
  emailNotifications: true,
  smsNotifications: false,
  marketingEmails: true,
  language: "en",
  currency: "USD",
};

/**
 * Default account settings
 */
export const Default: Story = {
  args: {
    settings: defaultSettings,
    labels: germanLabels,
    loading: false,
    error: null,
    success: null,
  },
};

/**
 * Account settings with English labels
 */
export const English: Story = {
  args: {
    settings: englishUserSettings,
    labels: englishLabels,
    loading: false,
    error: null,
    success: null,
  },
};

/**
 * All notifications enabled
 */
export const AllNotificationsEnabled: Story = {
  args: {
    settings: allNotificationsEnabled,
    labels: germanLabels,
    loading: false,
    error: null,
    success: null,
  },
};

/**
 * Form in loading state
 */
export const Loading: Story = {
  args: {
    settings: defaultSettings,
    labels: germanLabels,
    loading: true,
    error: null,
    success: null,
  },
};

/**
 * Form with success message
 */
export const Success: Story = {
  args: {
    settings: defaultSettings,
    labels: germanLabels,
    loading: false,
    error: null,
    success: "Ihre Einstellungen wurden erfolgreich gespeichert.",
  },
};

/**
 * Form with error message
 */
export const WithError: Story = {
  args: {
    settings: defaultSettings,
    labels: germanLabels,
    loading: false,
    error: "Die Einstellungen konnten nicht gespeichert werden. Bitte versuchen Sie es erneut.",
    success: null,
  },
};

/**
 * Empty settings (new user)
 */
export const Empty: Story = {
  args: {
    settings: null,
    labels: germanLabels,
    loading: false,
    error: null,
    success: null,
  },
};
