/**
 * Profile component types and interfaces
 */

/**
 * User profile data for display
 */
export interface UserProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  dateOfBirth: string;
  address?: {
    street: string;
    city: string;
    postalCode: string;
    country: string;
  };
  driversLicense?: {
    licenseNumber: string;
    licenseIssueCountry: string;
    licenseIssueDate: string;
    licenseExpiryDate: string;
  };
  createdAt?: string;
  updatedAt?: string;
}

/**
 * Event emitted when profile is updated
 */
export interface ProfileUpdateEvent {
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  address: {
    street: string;
    city: string;
    postalCode: string;
    country: string;
  };
}

/**
 * Event emitted when password is changed
 */
export interface PasswordChangeEvent {
  currentPassword: string;
  newPassword: string;
}

/**
 * Event emitted when driver's license is updated
 */
export interface DriversLicenseUpdateEvent {
  licenseNumber: string;
  licenseIssueCountry: string;
  licenseIssueDate: string;
  licenseExpiryDate: string;
}

/**
 * Account settings data
 */
export interface AccountSettings {
  emailNotifications: boolean;
  smsNotifications: boolean;
  marketingEmails: boolean;
  language: string;
  currency: string;
}

/**
 * Event emitted when account settings are updated
 */
export interface AccountSettingsUpdateEvent {
  emailNotifications: boolean;
  smsNotifications: boolean;
  marketingEmails: boolean;
  language: string;
  currency: string;
}

/**
 * Labels for profile view component
 */
export interface ProfileViewLabels {
  title: string;
  personalInfoSection: string;
  contactInfoSection: string;
  addressSection: string;
  driversLicenseSection: string;
  firstNameLabel: string;
  lastNameLabel: string;
  emailLabel: string;
  phoneLabel: string;
  dateOfBirthLabel: string;
  streetLabel: string;
  cityLabel: string;
  postalCodeLabel: string;
  countryLabel: string;
  licenseNumberLabel: string;
  licenseIssueCountryLabel: string;
  licenseIssueDateLabel: string;
  licenseExpiryDateLabel: string;
  editButton: string;
  memberSinceLabel: string;
  noAddressText: string;
  noLicenseText: string;
  addAddressButton: string;
  addLicenseButton: string;
}

/**
 * Default German labels for profile view
 */
export const DEFAULT_PROFILE_VIEW_LABELS_DE: ProfileViewLabels = {
  title: "Mein Profil",
  personalInfoSection: "Persönliche Daten",
  contactInfoSection: "Kontaktdaten",
  addressSection: "Adresse",
  driversLicenseSection: "Führerschein",
  firstNameLabel: "Vorname",
  lastNameLabel: "Nachname",
  emailLabel: "E-Mail",
  phoneLabel: "Telefon",
  dateOfBirthLabel: "Geburtsdatum",
  streetLabel: "Straße",
  cityLabel: "Stadt",
  postalCodeLabel: "Postleitzahl",
  countryLabel: "Land",
  licenseNumberLabel: "Führerscheinnummer",
  licenseIssueCountryLabel: "Ausstellungsland",
  licenseIssueDateLabel: "Ausstellungsdatum",
  licenseExpiryDateLabel: "Gültig bis",
  editButton: "Bearbeiten",
  memberSinceLabel: "Mitglied seit",
  noAddressText: "Keine Adresse hinterlegt",
  noLicenseText: "Kein Führerschein hinterlegt",
  addAddressButton: "Adresse hinzufügen",
  addLicenseButton: "Führerschein hinzufügen",
};

/**
 * Labels for profile edit component
 */
export interface ProfileEditLabels {
  title: string;
  personalInfoSection: string;
  addressSection: string;
  firstNameLabel: string;
  lastNameLabel: string;
  phoneLabel: string;
  dateOfBirthLabel: string;
  streetLabel: string;
  cityLabel: string;
  postalCodeLabel: string;
  countryLabel: string;
  saveButton: string;
  savingButton: string;
  cancelButton: string;
  // Validation messages
  firstNameRequired: string;
  lastNameRequired: string;
  phoneRequired: string;
  phoneInvalid: string;
  dateOfBirthRequired: string;
  streetRequired: string;
  cityRequired: string;
  postalCodeRequired: string;
  countryRequired: string;
}

/**
 * Default German labels for profile edit
 */
export const DEFAULT_PROFILE_EDIT_LABELS_DE: ProfileEditLabels = {
  title: "Profil bearbeiten",
  personalInfoSection: "Persönliche Daten",
  addressSection: "Adresse",
  firstNameLabel: "Vorname",
  lastNameLabel: "Nachname",
  phoneLabel: "Telefonnummer",
  dateOfBirthLabel: "Geburtsdatum",
  streetLabel: "Straße und Hausnummer",
  cityLabel: "Stadt",
  postalCodeLabel: "Postleitzahl",
  countryLabel: "Land",
  saveButton: "Speichern",
  savingButton: "Wird gespeichert...",
  cancelButton: "Abbrechen",
  firstNameRequired: "Vorname ist erforderlich",
  lastNameRequired: "Nachname ist erforderlich",
  phoneRequired: "Telefonnummer ist erforderlich",
  phoneInvalid: "Bitte geben Sie eine gültige Telefonnummer ein",
  dateOfBirthRequired: "Geburtsdatum ist erforderlich",
  streetRequired: "Straße ist erforderlich",
  cityRequired: "Stadt ist erforderlich",
  postalCodeRequired: "Postleitzahl ist erforderlich",
  countryRequired: "Land ist erforderlich",
};

/**
 * Labels for password change component
 */
export interface PasswordChangeLabels {
  title: string;
  subtitle: string;
  currentPasswordLabel: string;
  newPasswordLabel: string;
  confirmPasswordLabel: string;
  saveButton: string;
  savingButton: string;
  cancelButton: string;
  passwordHint: string;
  // Validation messages
  currentPasswordRequired: string;
  newPasswordRequired: string;
  newPasswordMinLength: string;
  newPasswordWeak: string;
  confirmPasswordRequired: string;
  passwordMismatch: string;
  sameAsCurrentPassword: string;
}

/**
 * Default German labels for password change
 */
export const DEFAULT_PASSWORD_CHANGE_LABELS_DE: PasswordChangeLabels = {
  title: "Passwort ändern",
  subtitle: "Wählen Sie ein sicheres Passwort für Ihr Konto",
  currentPasswordLabel: "Aktuelles Passwort",
  newPasswordLabel: "Neues Passwort",
  confirmPasswordLabel: "Neues Passwort bestätigen",
  saveButton: "Passwort ändern",
  savingButton: "Wird geändert...",
  cancelButton: "Abbrechen",
  passwordHint: "Mindestens 8 Zeichen mit Groß-/Kleinbuchstaben, Zahlen und Sonderzeichen",
  currentPasswordRequired: "Aktuelles Passwort ist erforderlich",
  newPasswordRequired: "Neues Passwort ist erforderlich",
  newPasswordMinLength: "Passwort muss mindestens 8 Zeichen lang sein",
  newPasswordWeak: "Passwort muss Groß-/Kleinbuchstaben, Zahlen und Sonderzeichen enthalten",
  confirmPasswordRequired: "Passwortbestätigung ist erforderlich",
  passwordMismatch: "Passwörter stimmen nicht überein",
  sameAsCurrentPassword: "Das neue Passwort muss sich vom aktuellen unterscheiden",
};

/**
 * Labels for account settings component
 */
export interface AccountSettingsLabels {
  title: string;
  notificationsSection: string;
  preferencesSection: string;
  emailNotificationsLabel: string;
  emailNotificationsDescription: string;
  smsNotificationsLabel: string;
  smsNotificationsDescription: string;
  marketingEmailsLabel: string;
  marketingEmailsDescription: string;
  languageLabel: string;
  currencyLabel: string;
  saveButton: string;
  savingButton: string;
  dangerZoneSection: string;
  deleteAccountButton: string;
  deleteAccountWarning: string;
}

/**
 * Default German labels for account settings
 */
export const DEFAULT_ACCOUNT_SETTINGS_LABELS_DE: AccountSettingsLabels = {
  title: "Kontoeinstellungen",
  notificationsSection: "Benachrichtigungen",
  preferencesSection: "Präferenzen",
  emailNotificationsLabel: "E-Mail-Benachrichtigungen",
  emailNotificationsDescription: "Erhalten Sie wichtige Updates zu Ihren Buchungen per E-Mail",
  smsNotificationsLabel: "SMS-Benachrichtigungen",
  smsNotificationsDescription: "Erhalten Sie Buchungsbestätigungen und Erinnerungen per SMS",
  marketingEmailsLabel: "Marketing-E-Mails",
  marketingEmailsDescription: "Erhalten Sie Angebote, Rabatte und Neuigkeiten",
  languageLabel: "Sprache",
  currencyLabel: "Währung",
  saveButton: "Einstellungen speichern",
  savingButton: "Wird gespeichert...",
  dangerZoneSection: "Gefahrenzone",
  deleteAccountButton: "Konto löschen",
  deleteAccountWarning: "Diese Aktion kann nicht rückgängig gemacht werden. Alle Ihre Daten werden unwiderruflich gelöscht.",
};

/**
 * Available languages
 */
export const AVAILABLE_LANGUAGES = [
  { code: "de", name: "Deutsch" },
  { code: "en", name: "English" },
];

/**
 * Available currencies
 */
export const AVAILABLE_CURRENCIES = [
  { code: "EUR", name: "Euro (€)", symbol: "€" },
  { code: "USD", name: "US Dollar ($)", symbol: "$" },
  { code: "GBP", name: "British Pound (£)", symbol: "£" },
];
