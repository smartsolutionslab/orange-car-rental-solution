import type { Meta, StoryObj } from "@storybook/angular";
import { SuccessAlertComponent } from "./success-alert.component";

const meta: Meta<SuccessAlertComponent> = {
  title: "Feedback/Success Alert",
  component: SuccessAlertComponent,
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A success alert component for displaying positive feedback messages.

## Features
- Green success styling
- Check icon
- Accessible role="alert"

## Usage

\`\`\`html
<ui-success-alert message="Daten erfolgreich gespeichert" />
<ui-success-alert [message]="successMessage" />
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    message: {
      control: "text",
      description: "Success message to display",
    },
  },
};

export default meta;
type Story = StoryObj<SuccessAlertComponent>;

/**
 * Default success alert
 */
export const Default: Story = {
  args: {
    message: "Die Daten wurden erfolgreich gespeichert.",
  },
};

/**
 * Booking confirmation
 */
export const BookingConfirmation: Story = {
  args: {
    message: "Ihre Buchung wurde erfolgreich bestätigt. Eine Bestätigungs-E-Mail wurde an Sie gesendet.",
  },
};

/**
 * Profile update
 */
export const ProfileUpdate: Story = {
  args: {
    message: "Ihr Profil wurde erfolgreich aktualisiert.",
  },
};

/**
 * Password change
 */
export const PasswordChange: Story = {
  args: {
    message: "Ihr Passwort wurde erfolgreich geändert.",
  },
};

/**
 * Multiple alerts in sequence
 */
export const MultipleAlerts: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 0;">
        <ui-success-alert message="Buchung erstellt"></ui-success-alert>
        <ui-success-alert message="E-Mail wurde versendet"></ui-success-alert>
        <ui-success-alert message="Kalender aktualisiert"></ui-success-alert>
      </div>
    `,
  }),
};

/**
 * In form context
 */
export const InFormContext: Story = {
  render: () => ({
    template: `
      <div style="max-width: 400px;">
        <ui-success-alert message="Änderungen gespeichert"></ui-success-alert>
        <div style="padding: 1rem; background: #f9fafb; border-radius: 0.5rem;">
          <p style="margin: 0; font-size: 0.875rem; color: #6b7280;">
            Formularinhalt hier...
          </p>
        </div>
      </div>
    `,
  }),
};
