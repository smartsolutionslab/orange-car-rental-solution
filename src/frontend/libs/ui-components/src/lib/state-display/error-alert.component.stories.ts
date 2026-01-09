import type { Meta, StoryObj } from "@storybook/angular";
import { ErrorAlertComponent } from "./error-alert.component";

const meta: Meta<ErrorAlertComponent> = {
  title: "Feedback/Error Alert",
  component: ErrorAlertComponent,
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
An error alert component for displaying inline error messages.

## Features
- Red error styling
- Error icon
- Accessible role="alert"

## Usage

\`\`\`html
<ui-error-alert message="Ungültige E-Mail-Adresse" />
<ui-error-alert [message]="errorMessage" />
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    message: {
      control: "text",
      description: "Error message to display",
    },
  },
};

export default meta;
type Story = StoryObj<ErrorAlertComponent>;

/**
 * Default error alert
 */
export const Default: Story = {
  args: {
    message: "Ein Fehler ist aufgetreten. Bitte versuchen Sie es erneut.",
  },
};

/**
 * Validation error
 */
export const ValidationError: Story = {
  args: {
    message: "Bitte füllen Sie alle Pflichtfelder aus.",
  },
};

/**
 * Authentication error
 */
export const AuthenticationError: Story = {
  args: {
    message: "Die Anmeldedaten sind ungültig. Bitte überprüfen Sie E-Mail und Passwort.",
  },
};

/**
 * Network error
 */
export const NetworkError: Story = {
  args: {
    message: "Verbindungsfehler. Bitte überprüfen Sie Ihre Internetverbindung.",
  },
};

/**
 * Booking error
 */
export const BookingError: Story = {
  args: {
    message: "Das Fahrzeug ist im gewählten Zeitraum leider nicht verfügbar.",
  },
};

/**
 * In form context
 */
export const InFormContext: Story = {
  render: () => ({
    template: `
      <div style="max-width: 400px;">
        <ui-error-alert message="Die Eingabe ist ungültig"></ui-error-alert>
        <div style="padding: 1rem; background: #f9fafb; border-radius: 0.5rem;">
          <label style="display: block; margin-bottom: 0.25rem; font-size: 0.875rem; font-weight: 500; color: #374151;">
            E-Mail-Adresse
          </label>
          <input
            type="email"
            value="invalid-email"
            style="width: 100%; padding: 0.5rem 0.75rem; border: 1px solid #ef4444; border-radius: 0.375rem; font-size: 0.875rem;"
          />
        </div>
      </div>
    `,
  }),
};

/**
 * Multiple errors
 */
export const MultipleErrors: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 0;">
        <ui-error-alert message="E-Mail-Adresse ist ungültig"></ui-error-alert>
        <ui-error-alert message="Passwort ist zu kurz"></ui-error-alert>
        <ui-error-alert message="Telefonnummer fehlt"></ui-error-alert>
      </div>
    `,
  }),
};
