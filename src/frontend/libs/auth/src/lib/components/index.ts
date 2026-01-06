// Auth Form Components
export { LoginFormComponent } from './login-form';
export { RegisterFormComponent } from './register-form';
export { ForgotPasswordFormComponent } from './forgot-password-form';

// Types and Configurations
export {
  // Types
  type AuthFormConfig,
  type LoginFormSubmitEvent,
  type RegisterFormSubmitEvent,
  type ForgotPasswordFormSubmitEvent,
  type LoginFormLabels,
  type RegisterFormLabels,
  type ForgotPasswordFormLabels,
  // Default configurations
  DEFAULT_AUTH_CONFIG,
  DEFAULT_LOGIN_LABELS_DE,
  DEFAULT_REGISTER_LABELS_DE,
  DEFAULT_FORGOT_PASSWORD_LABELS_DE,
} from './auth-forms.types';
