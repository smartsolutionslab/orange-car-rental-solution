import type { Meta, StoryObj } from "@storybook/angular";
import { ErrorStateComponent } from "./error-state.component";

const meta: Meta<ErrorStateComponent> = {
  title: "Feedback/Error State",
  component: ErrorStateComponent,
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
An error state component for displaying errors with optional retry action.

## Features
- Multiple variants: default (centered), inline, banner
- Optional title
- Optional retry button
- Custom retry label

## Variants
- **default**: Centered error display for empty states
- **inline**: Compact inline error for forms
- **banner**: Full-width banner with left border

## Usage

\`\`\`html
<!-- Default centered -->
<ui-error-state message="Fehler beim Laden der Daten" />

<!-- Inline variant -->
<ui-error-state message="Ungültige Eingabe" variant="inline" />

<!-- With retry -->
<ui-error-state
  message="Verbindungsfehler"
  [showRetry]="true"
  (retry)="onRetry()"
/>
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
    title: {
      control: "text",
      description: "Error title (hidden in inline variant)",
    },
    variant: {
      control: "select",
      options: ["default", "inline", "banner"],
      description: "Visual variant",
    },
    showRetry: {
      control: "boolean",
      description: "Show retry button",
    },
    retryLabel: {
      control: "text",
      description: "Retry button label",
    },
  },
};

export default meta;
type Story = StoryObj<ErrorStateComponent>;

/**
 * Default centered error state
 */
export const Default: Story = {
  args: {
    message: "Die Daten konnten nicht geladen werden.",
    title: "Fehler",
    variant: "default",
    showRetry: false,
  },
};

/**
 * With retry button
 */
export const WithRetry: Story = {
  args: {
    message: "Verbindungsfehler. Bitte versuchen Sie es erneut.",
    title: "Verbindungsfehler",
    variant: "default",
    showRetry: true,
    retryLabel: "Erneut versuchen",
  },
};

/**
 * Inline variant for forms
 */
export const Inline: Story = {
  args: {
    message: "Die Eingabe ist ungültig. Bitte überprüfen Sie Ihre Angaben.",
    variant: "inline",
  },
};

/**
 * Banner variant
 */
export const Banner: Story = {
  args: {
    message: "Es ist ein Systemfehler aufgetreten. Unser Team wurde benachrichtigt.",
    title: "Systemfehler",
    variant: "banner",
    showRetry: true,
  },
};

/**
 * Loading error with retry
 */
export const LoadingError: Story = {
  args: {
    message: "Die Buchungen konnten nicht geladen werden.",
    title: "Ladefehler",
    variant: "default",
    showRetry: true,
    retryLabel: "Neu laden",
  },
};

/**
 * Network error
 */
export const NetworkError: Story = {
  args: {
    message: "Keine Internetverbindung. Bitte überprüfen Sie Ihre Verbindung und versuchen Sie es erneut.",
    title: "Netzwerkfehler",
    variant: "banner",
    showRetry: true,
    retryLabel: "Verbindung prüfen",
  },
};

/**
 * In page context
 */
export const InPageContext: Story = {
  render: () => ({
    template: `
      <div style="max-width: 600px; border: 1px solid #e5e7eb; border-radius: 0.5rem; overflow: hidden;">
        <div style="padding: 1rem; background: #f9fafb; border-bottom: 1px solid #e5e7eb;">
          <h2 style="margin: 0; font-size: 1.125rem; font-weight: 600;">Buchungsübersicht</h2>
        </div>
        <div style="padding: 2rem;">
          <ui-error-state
            message="Die Buchungen konnten nicht geladen werden."
            title="Ladefehler"
            [showRetry]="true"
          ></ui-error-state>
        </div>
      </div>
    `,
  }),
};

/**
 * Inline in form
 */
export const InlineInForm: Story = {
  render: () => ({
    template: `
      <div style="max-width: 400px; padding: 1rem; background: white; border: 1px solid #e5e7eb; border-radius: 0.5rem;">
        <div style="margin-bottom: 1rem;">
          <label style="display: block; margin-bottom: 0.25rem; font-size: 0.875rem; font-weight: 500;">
            E-Mail-Adresse
          </label>
          <input
            type="email"
            value="test@example"
            style="width: 100%; padding: 0.5rem 0.75rem; border: 1px solid #d1d5db; border-radius: 0.375rem; font-size: 0.875rem;"
          />
        </div>
        <ui-error-state
          message="Die E-Mail-Adresse konnte nicht verifiziert werden."
          variant="inline"
        ></ui-error-state>
      </div>
    `,
  }),
};

/**
 * All variants comparison
 */
export const AllVariants: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 2rem;">
        <div>
          <h3 style="margin: 0 0 0.5rem; font-size: 0.875rem; font-weight: 600; color: #6b7280;">Default (Centered)</h3>
          <div style="padding: 1rem; background: #f9fafb; border-radius: 0.5rem;">
            <ui-error-state
              message="Zentrierte Fehleranzeige"
              title="Fehler"
            ></ui-error-state>
          </div>
        </div>

        <div>
          <h3 style="margin: 0 0 0.5rem; font-size: 0.875rem; font-weight: 600; color: #6b7280;">Inline</h3>
          <ui-error-state
            message="Kompakte Inline-Fehleranzeige"
            variant="inline"
          ></ui-error-state>
        </div>

        <div>
          <h3 style="margin: 0 0 0.5rem; font-size: 0.875rem; font-weight: 600; color: #6b7280;">Banner</h3>
          <ui-error-state
            message="Banner-Fehleranzeige mit linkem Rand"
            title="Banner"
            variant="banner"
            [showRetry]="true"
          ></ui-error-state>
        </div>
      </div>
    `,
  }),
};
