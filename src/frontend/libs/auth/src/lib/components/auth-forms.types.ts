/**
 * Configuration for auth form components
 */
export interface AuthFormConfig {
  /** Brand/logo URL to display */
  logoUrl?: string;

  /** Application/brand name */
  brandName?: string;

  /** Show "Remember Me" checkbox in login */
  showRememberMe?: boolean;

  /** Show social login options */
  showSocialLogin?: boolean;

  /** Terms of service URL */
  termsUrl?: string;

  /** Privacy policy URL */
  privacyUrl?: string;

  /** Route for login page */
  loginRoute?: string;

  /** Route for register page */
  registerRoute?: string;

  /** Route for forgot password page */
  forgotPasswordRoute?: string;

  /** Minimum password length */
  minPasswordLength?: number;

  /** Minimum age for registration */
  minAge?: number;
}

/**
 * Default auth form configuration
 */
export const DEFAULT_AUTH_CONFIG: AuthFormConfig = {
  showRememberMe: true,
  showSocialLogin: false,
  loginRoute: "/login",
  registerRoute: "/register",
  forgotPasswordRoute: "/forgot-password",
  minPasswordLength: 8,
  minAge: 21,
};

/**
 * Event emitted when login form is submitted
 */
export interface LoginFormSubmitEvent {
  email: string;
  password: string;
  rememberMe: boolean;
}

/**
 * Event emitted when register form is submitted
 */
export interface RegisterFormSubmitEvent {
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
  dateOfBirth: string;
  acceptTerms: boolean;
  acceptPrivacy: boolean;
  acceptMarketing: boolean;
}

/**
 * Event emitted when forgot password form is submitted
 */
export interface ForgotPasswordFormSubmitEvent {
  email: string;
}

/**
 * Labels for login form (i18n support)
 */
export interface LoginFormLabels {
  title: string;
  subtitle: string;
  emailLabel: string;
  emailPlaceholder: string;
  passwordLabel: string;
  forgotPasswordLink: string;
  rememberMeLabel: string;
  submitButton: string;
  submittingButton: string;
  noAccountText: string;
  registerLink: string;
  orDivider: string;
  googleLoginButton: string;
  showPassword: string;
  hidePassword: string;
  emailRequired: string;
  emailInvalid: string;
  passwordRequired: string;
  passwordMinLength: string;
}

/**
 * Default German labels for login form
 */
export const DEFAULT_LOGIN_LABELS_DE: LoginFormLabels = {
  title: "Willkommen zurück",
  subtitle: "Melden Sie sich an, um fortzufahren",
  emailLabel: "E-Mail-Adresse",
  emailPlaceholder: "ihre@email.de",
  passwordLabel: "Passwort",
  forgotPasswordLink: "Passwort vergessen?",
  rememberMeLabel: "Angemeldet bleiben",
  submitButton: "Anmelden",
  submittingButton: "Wird angemeldet...",
  noAccountText: "Noch kein Konto?",
  registerLink: "Jetzt registrieren",
  orDivider: "oder",
  googleLoginButton: "Mit Google anmelden",
  showPassword: "Passwort anzeigen",
  hidePassword: "Passwort verbergen",
  emailRequired: "E-Mail-Adresse ist erforderlich",
  emailInvalid: "Bitte geben Sie eine gültige E-Mail-Adresse ein",
  passwordRequired: "Passwort ist erforderlich",
  passwordMinLength: "Passwort muss mindestens 8 Zeichen lang sein",
};

/**
 * Labels for register form (i18n support)
 */
export interface RegisterFormLabels {
  title: string;
  subtitle: string;
  step1Title: string;
  step2Title: string;
  step3Title: string;
  emailLabel: string;
  passwordLabel: string;
  confirmPasswordLabel: string;
  firstNameLabel: string;
  lastNameLabel: string;
  phoneLabel: string;
  dateOfBirthLabel: string;
  acceptTermsLabel: string;
  acceptPrivacyLabel: string;
  acceptMarketingLabel: string;
  nextButton: string;
  previousButton: string;
  submitButton: string;
  submittingButton: string;
  hasAccountText: string;
  loginLink: string;
  // Validation messages
  emailRequired: string;
  emailInvalid: string;
  passwordRequired: string;
  passwordMinLength: string;
  passwordWeak: string;
  confirmPasswordRequired: string;
  passwordMismatch: string;
  firstNameRequired: string;
  lastNameRequired: string;
  phoneRequired: string;
  phoneInvalid: string;
  dateOfBirthRequired: string;
  minAgeError: string;
  termsRequired: string;
  privacyRequired: string;
}

/**
 * Default German labels for register form
 */
export const DEFAULT_REGISTER_LABELS_DE: RegisterFormLabels = {
  title: "Konto erstellen",
  subtitle: "Registrieren Sie sich für Orange Car Rental",
  step1Title: "Zugangsdaten",
  step2Title: "Persönliche Daten",
  step3Title: "Bestätigung",
  emailLabel: "E-Mail-Adresse",
  passwordLabel: "Passwort",
  confirmPasswordLabel: "Passwort bestätigen",
  firstNameLabel: "Vorname",
  lastNameLabel: "Nachname",
  phoneLabel: "Telefonnummer",
  dateOfBirthLabel: "Geburtsdatum",
  acceptTermsLabel: "Ich akzeptiere die AGB",
  acceptPrivacyLabel: "Ich akzeptiere die Datenschutzerklärung",
  acceptMarketingLabel: "Ich möchte Newsletter erhalten (optional)",
  nextButton: "Weiter",
  previousButton: "Zurück",
  submitButton: "Registrieren",
  submittingButton: "Wird registriert...",
  hasAccountText: "Bereits ein Konto?",
  loginLink: "Jetzt anmelden",
  emailRequired: "E-Mail-Adresse ist erforderlich",
  emailInvalid: "Bitte geben Sie eine gültige E-Mail-Adresse ein",
  passwordRequired: "Passwort ist erforderlich",
  passwordMinLength: "Passwort muss mindestens 8 Zeichen lang sein",
  passwordWeak:
    "Passwort muss Groß-/Kleinbuchstaben, Zahlen und Sonderzeichen enthalten",
  confirmPasswordRequired: "Passwortbestätigung ist erforderlich",
  passwordMismatch: "Passwörter stimmen nicht überein",
  firstNameRequired: "Vorname ist erforderlich",
  lastNameRequired: "Nachname ist erforderlich",
  phoneRequired: "Telefonnummer ist erforderlich",
  phoneInvalid: "Bitte geben Sie eine gültige deutsche Telefonnummer ein",
  dateOfBirthRequired: "Geburtsdatum ist erforderlich",
  minAgeError: "Sie müssen mindestens {minAge} Jahre alt sein",
  termsRequired: "Sie müssen die AGB akzeptieren",
  privacyRequired: "Sie müssen die Datenschutzerklärung akzeptieren",
};

/**
 * Labels for forgot password form
 */
export interface ForgotPasswordFormLabels {
  title: string;
  subtitle: string;
  emailLabel: string;
  emailPlaceholder: string;
  submitButton: string;
  submittingButton: string;
  backToLoginLink: string;
  successTitle: string;
  successMessage: string;
  emailRequired: string;
  emailInvalid: string;
}

/**
 * Default German labels for forgot password form
 */
export const DEFAULT_FORGOT_PASSWORD_LABELS_DE: ForgotPasswordFormLabels = {
  title: "Passwort vergessen",
  subtitle:
    "Geben Sie Ihre E-Mail-Adresse ein, um einen Link zum Zurücksetzen Ihres Passworts zu erhalten.",
  emailLabel: "E-Mail-Adresse",
  emailPlaceholder: "ihre@email.de",
  submitButton: "Link senden",
  submittingButton: "Wird gesendet...",
  backToLoginLink: "Zurück zur Anmeldung",
  successTitle: "E-Mail gesendet",
  successMessage:
    "Falls ein Konto mit dieser E-Mail-Adresse existiert, haben wir Ihnen einen Link zum Zurücksetzen des Passworts gesendet.",
  emailRequired: "E-Mail-Adresse ist erforderlich",
  emailInvalid: "Bitte geben Sie eine gültige E-Mail-Adresse ein",
};
