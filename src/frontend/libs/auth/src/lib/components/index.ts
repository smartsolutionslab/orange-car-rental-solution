// Auth Form Components
export { LoginFormComponent } from "./login-form";
export { RegisterFormComponent } from "./register-form";
export { ForgotPasswordFormComponent } from "./forgot-password-form";

// Profile Components
export { ProfileViewComponent } from "./profile/profile-view/profile-view.component";
export { ProfileEditComponent } from "./profile/profile-edit/profile-edit.component";
export { PasswordChangeComponent } from "./profile/password-change/password-change.component";
export { AccountSettingsComponent } from "./profile/account-settings/account-settings.component";

// Auth Form Types and Configurations
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
} from "./auth-forms.types";

// Profile Types and Configurations
export {
  // Types
  type UserProfile,
  type ProfileUpdateEvent,
  type PasswordChangeEvent,
  type DriversLicenseUpdateEvent,
  type AccountSettings,
  type AccountSettingsUpdateEvent,
  type ProfileViewLabels,
  type ProfileEditLabels,
  type PasswordChangeLabels,
  type AccountSettingsLabels,
  // Default configurations
  DEFAULT_PROFILE_VIEW_LABELS_DE,
  DEFAULT_PROFILE_EDIT_LABELS_DE,
  DEFAULT_PASSWORD_CHANGE_LABELS_DE,
  DEFAULT_ACCOUNT_SETTINGS_LABELS_DE,
  AVAILABLE_LANGUAGES,
  AVAILABLE_CURRENCIES,
} from "./profile/profile.types";
