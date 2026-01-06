import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { ForgotPasswordFormComponent } from "./forgot-password-form.component";
import { RouterModule } from "@angular/router";
import type {
  ForgotPasswordFormLabels,
  AuthFormConfig,
} from "../auth-forms.types";
import {
  DEFAULT_FORGOT_PASSWORD_LABELS_DE,
  DEFAULT_AUTH_CONFIG,
} from "../auth-forms.types";

const meta: Meta<ForgotPasswordFormComponent> = {
  title: "Auth/Forgot Password Form",
  component: ForgotPasswordFormComponent,
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
A configurable, presentational forgot password form component that handles validation
and emits submit events. The parent component handles actual password reset logic.

## Features
- Email validation
- Success state with confirmation message
- Back to login link
- Full i18n support via labels
- Loading and error states

## Usage

\`\`\`html
<lib-forgot-password-form
  [config]="authConfig"
  [labels]="germanLabels"
  [loading]="isSending()"
  [error]="sendError()"
  [success]="emailSent()"
  (formSubmit)="onResetPassword($event)"
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
      control: "boolean",
      description: "Shows success state with confirmation message",
    },
  },
};

export default meta;
type Story = StoryObj<ForgotPasswordFormComponent>;

// German labels (default)
const germanLabels: ForgotPasswordFormLabels =
  DEFAULT_FORGOT_PASSWORD_LABELS_DE;

// English labels example
const englishLabels: ForgotPasswordFormLabels = {
  title: "Forgot Password?",
  subtitle: "No worries, we'll send you reset instructions.",
  emailLabel: "Email Address",
  emailPlaceholder: "you@example.com",
  emailRequired: "Email is required",
  emailInvalid: "Please enter a valid email address",
  submitButton: "Send Reset Link",
  submittingButton: "Sending...",
  backToLoginLink: "Back to Login",
  successTitle: "Email Sent",
  successMessage:
    "Check your email for a link to reset your password. If it doesn't appear within a few minutes, check your spam folder.",
};

// Config
const config: AuthFormConfig = {
  logoUrl: "https://placehold.co/120x40/f97316/white?text=OCR",
  brandName: "Orange Car Rental",
  loginRoute: "/auth/login",
};

// Minimal config
const minimalConfig: AuthFormConfig = {
  loginRoute: "/auth/login",
};

/**
 * Default forgot password form with German labels
 */
export const Default: Story = {
  args: {
    config: config,
    labels: germanLabels,
    loading: false,
    error: null,
    success: false,
  },
};

/**
 * Forgot password form with English labels
 */
export const English: Story = {
  args: {
    config: config,
    labels: englishLabels,
    loading: false,
    error: null,
    success: false,
  },
};

/**
 * Form in loading state (sending email)
 */
export const Loading: Story = {
  args: {
    config: config,
    labels: germanLabels,
    loading: true,
    error: null,
    success: false,
  },
};

/**
 * Success state after email was sent
 */
export const Success: Story = {
  args: {
    config: config,
    labels: germanLabels,
    loading: false,
    error: null,
    success: true,
  },
};

/**
 * Success state with English labels
 */
export const SuccessEnglish: Story = {
  args: {
    config: config,
    labels: englishLabels,
    loading: false,
    error: null,
    success: true,
  },
};

/**
 * Form with error message
 */
export const WithError: Story = {
  args: {
    config: config,
    labels: germanLabels,
    loading: false,
    error: "Kein Konto mit dieser E-Mail-Adresse gefunden.",
    success: false,
  },
};

/**
 * Form with network error
 */
export const NetworkError: Story = {
  args: {
    config: config,
    labels: englishLabels,
    loading: false,
    error: "Unable to connect to the server. Please try again later.",
    success: false,
  },
};

/**
 * Minimal form without logo
 */
export const NoLogo: Story = {
  args: {
    config: minimalConfig,
    labels: germanLabels,
    loading: false,
    error: null,
    success: false,
  },
};
