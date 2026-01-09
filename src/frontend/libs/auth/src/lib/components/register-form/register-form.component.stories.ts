import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { RegisterFormComponent } from "./register-form.component";
import { RouterModule } from "@angular/router";
import type { RegisterFormLabels, AuthFormConfig } from "../auth-forms.types";
import {
  DEFAULT_REGISTER_LABELS_DE,
  DEFAULT_AUTH_CONFIG,
} from "../auth-forms.types";

const meta: Meta<RegisterFormComponent> = {
  title: "Auth/Register Form",
  component: RegisterFormComponent,
  decorators: [
    moduleMetadata({
      imports: [RouterModule.forRoot([])],
    }),
  ],
  parameters: {
    layout: "centered",
    docs: {
      description: {
        component: `
A configurable, multi-step registration form component that handles validation
and emits submit events. The parent component handles actual registration.

## Features
- Multi-step registration flow (3 steps)
- Step 1: Account information (email, password)
- Step 2: Personal information (name, phone, date of birth)
- Step 3: Terms and conditions
- Password strength validation
- Age verification (minimum 21 years)
- German phone number validation
- Full i18n support via labels
- Loading and error states
- GDPR-compliant privacy notice

## Usage

\`\`\`html
<lib-register-form
  [config]="registerConfig"
  [labels]="germanLabels"
  [loading]="isRegistering()"
  [error]="registrationError()"
  [success]="registrationSuccess()"
  (formSubmit)="onRegister($event)"
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
      description: "Shows loading spinner and disables submit button",
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
type Story = StoryObj<RegisterFormComponent>;

// German labels (default)
const germanLabels: RegisterFormLabels = DEFAULT_REGISTER_LABELS_DE;

// English labels example
const englishLabels: RegisterFormLabels = {
  title: "Create Account",
  subtitle: "Join us and start renting vehicles today",
  step1Title: "Account",
  step2Title: "Personal",
  step3Title: "Terms",
  emailLabel: "Email Address",
  emailRequired: "Email is required",
  emailInvalid: "Please enter a valid email address",
  passwordLabel: "Password",
  passwordRequired: "Password is required",
  passwordMinLength: "Password must be at least 8 characters",
  passwordWeak: "Password must contain uppercase, lowercase, numbers, and special characters",
  confirmPasswordLabel: "Confirm Password",
  confirmPasswordRequired: "Please confirm your password",
  passwordMismatch: "Passwords do not match",
  firstNameLabel: "First Name",
  firstNameRequired: "First name is required",
  lastNameLabel: "Last Name",
  lastNameRequired: "Last name is required",
  phoneLabel: "Phone Number",
  phoneRequired: "Phone number is required",
  phoneInvalid: "Please enter a valid phone number",
  dateOfBirthLabel: "Date of Birth",
  dateOfBirthRequired: "Date of birth is required",
  minAgeError: "You must be at least {minAge} years old",
  acceptTermsLabel: "I accept the Terms and Conditions",
  acceptPrivacyLabel: "I accept the Privacy Policy",
  acceptMarketingLabel: "I want to receive news and offers (optional)",
  previousButton: "Back",
  nextButton: "Continue",
  submitButton: "Create Account",
  submittingButton: "Creating account...",
  hasAccountText: "Already have an account?",
  loginLink: "Sign in",
  termsRequired: "You must accept the Terms and Conditions",
  privacyRequired: "You must accept the Privacy Policy",
};

// Config with all features enabled
const fullConfig: AuthFormConfig = {
  logoUrl: "https://placehold.co/120x40/f97316/white?text=OCR",
  brandName: "Orange Car Rental",
  loginRoute: "/auth/login",
  registerRoute: "/auth/register",
  termsUrl: "/terms",
  privacyUrl: "/privacy",
  minAge: 21,
  minPasswordLength: 8,
};

// Minimal config
const minimalConfig: AuthFormConfig = {
  loginRoute: "/auth/login",
  minAge: 18,
  minPasswordLength: 8,
};

/**
 * Default registration form with German labels - Step 1
 */
export const Step1Account: Story = {
  args: {
    config: fullConfig,
    labels: germanLabels,
    loading: false,
    error: null,
    success: null,
  },
};

/**
 * Registration form with English labels
 */
export const English: Story = {
  args: {
    config: fullConfig,
    labels: englishLabels,
    loading: false,
    error: null,
    success: null,
  },
};

/**
 * Minimal registration form without logo or terms links
 */
export const Minimal: Story = {
  args: {
    config: minimalConfig,
    labels: germanLabels,
    loading: false,
    error: null,
    success: null,
  },
};

/**
 * Registration form in loading state
 */
export const Loading: Story = {
  args: {
    config: fullConfig,
    labels: germanLabels,
    loading: true,
    error: null,
    success: null,
  },
};

/**
 * Registration form with validation error
 */
export const WithError: Story = {
  args: {
    config: fullConfig,
    labels: germanLabels,
    loading: false,
    error: "Diese E-Mail-Adresse ist bereits registriert. Bitte verwenden Sie eine andere E-Mail-Adresse oder melden Sie sich an.",
    success: null,
  },
};

/**
 * Registration form with network error
 */
export const NetworkError: Story = {
  args: {
    config: fullConfig,
    labels: englishLabels,
    loading: false,
    error: "Unable to connect to the server. Please check your internet connection and try again.",
    success: null,
  },
};

/**
 * Registration form with success message
 */
export const Success: Story = {
  args: {
    config: fullConfig,
    labels: germanLabels,
    loading: false,
    error: null,
    success: "Ihr Konto wurde erfolgreich erstellt! Sie werden in Kürze eine Bestätigungs-E-Mail erhalten.",
  },
};

/**
 * Registration form without logo
 */
export const NoLogo: Story = {
  args: {
    config: {
      ...fullConfig,
      logoUrl: undefined,
    },
    labels: germanLabels,
    loading: false,
    error: null,
    success: null,
  },
};
