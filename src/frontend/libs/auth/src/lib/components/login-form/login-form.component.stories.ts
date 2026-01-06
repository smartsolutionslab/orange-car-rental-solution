import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { LoginFormComponent } from "./login-form.component";
import { RouterModule } from "@angular/router";
import type { LoginFormLabels, AuthFormConfig } from "../auth-forms.types";
import {
  DEFAULT_LOGIN_LABELS_DE,
  DEFAULT_AUTH_CONFIG,
} from "../auth-forms.types";

const meta: Meta<LoginFormComponent> = {
  title: "Auth/Login Form",
  component: LoginFormComponent,
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
A configurable, presentational login form component that handles validation
and emits submit events. The parent component handles actual authentication.

## Features
- Email and password validation
- Password visibility toggle
- Remember me checkbox (optional)
- Forgot password link (optional)
- Social login button (optional)
- Registration link (optional)
- Full i18n support via labels
- Loading and error states

## Usage

\`\`\`html
<lib-login-form
  [config]="loginConfig"
  [labels]="germanLabels"
  [loading]="isAuthenticating()"
  [error]="authError()"
  (formSubmit)="onLogin($event)"
  (googleLogin)="onGoogleLogin()"
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
  },
};

export default meta;
type Story = StoryObj<LoginFormComponent>;

// German labels (default)
const germanLabels: LoginFormLabels = DEFAULT_LOGIN_LABELS_DE;

// English labels example
const englishLabels: LoginFormLabels = {
  title: "Welcome Back",
  subtitle: "Sign in to your account",
  emailLabel: "Email Address",
  emailPlaceholder: "you@example.com",
  emailRequired: "Email is required",
  emailInvalid: "Please enter a valid email address",
  passwordLabel: "Password",
  passwordRequired: "Password is required",
  passwordMinLength: "Password must be at least 8 characters",
  showPassword: "Show password",
  hidePassword: "Hide password",
  forgotPasswordLink: "Forgot password?",
  rememberMeLabel: "Remember me",
  submitButton: "Sign In",
  submittingButton: "Signing in...",
  noAccountText: "Don't have an account?",
  registerLink: "Sign up",
  orDivider: "or",
  googleLoginButton: "Continue with Google",
};

// Config with all features enabled
const fullConfig: AuthFormConfig = {
  logoUrl: "https://placehold.co/120x40/f97316/white?text=OCR",
  brandName: "Orange Car Rental",
  loginRoute: "/auth/login",
  registerRoute: "/auth/register",
  forgotPasswordRoute: "/auth/forgot-password",
  showRememberMe: true,
  showSocialLogin: true,
};

// Minimal config
const minimalConfig: AuthFormConfig = {
  loginRoute: "/auth/login",
  showRememberMe: false,
  showSocialLogin: false,
};

/**
 * Default login form with German labels
 */
export const Default: Story = {
  args: {
    config: fullConfig,
    labels: germanLabels,
    loading: false,
    error: null,
  },
};

/**
 * Login form with English labels
 */
export const English: Story = {
  args: {
    config: fullConfig,
    labels: englishLabels,
    loading: false,
    error: null,
  },
};

/**
 * Minimal login form without social login or remember me
 */
export const Minimal: Story = {
  args: {
    config: minimalConfig,
    labels: germanLabels,
    loading: false,
    error: null,
  },
};

/**
 * Login form in loading state
 */
export const Loading: Story = {
  args: {
    config: fullConfig,
    labels: germanLabels,
    loading: true,
    error: null,
  },
};

/**
 * Login form with authentication error
 */
export const WithError: Story = {
  args: {
    config: fullConfig,
    labels: germanLabels,
    loading: false,
    error:
      "Ungültige E-Mail-Adresse oder Passwort. Bitte versuchen Sie es erneut.",
  },
};

/**
 * Login form with network error
 */
export const NetworkError: Story = {
  args: {
    config: fullConfig,
    labels: englishLabels,
    loading: false,
    error:
      "Unable to connect to the server. Please check your internet connection.",
  },
};

/**
 * Login form without logo
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
  },
};
