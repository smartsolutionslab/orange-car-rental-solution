import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { FormSectionComponent } from "./form-section.component";
import { InputComponent } from "../input";
import { TextareaComponent } from "../textarea";
import { CheckboxComponent } from "../checkbox";

const meta: Meta<FormSectionComponent> = {
  title: "Forms/FormSection",
  component: FormSectionComponent,
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A container component for grouping related form fields with a header,
optional description, and optional icon.

## Features
- Title and description text
- Optional section icon
- Multiple visual variants (default, card, bordered, compact)
- Collapsible sections
- Consistent spacing between form fields

## Usage

\`\`\`html
<!-- Basic section -->
<ocr-form-section
  title="Kontaktdaten"
  description="Ihre Kontaktinformationen"
  icon="user"
>
  <ocr-input label="E-Mail" ... />
  <ocr-input label="Telefon" ... />
</ocr-form-section>

<!-- Card variant -->
<ocr-form-section
  title="Zahlungsinformationen"
  variant="card"
  icon="credit-card"
>
  <ocr-input label="Kartennummer" ... />
</ocr-form-section>

<!-- Collapsible section -->
<ocr-form-section
  title="Optionale Angaben"
  [collapsible]="true"
  [initiallyCollapsed]="true"
>
  ...
</ocr-form-section>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    variant: {
      control: "select",
      options: ["default", "card", "bordered", "compact"],
    },
  },
  decorators: [
    moduleMetadata({
      imports: [InputComponent, TextareaComponent, CheckboxComponent],
    }),
    (story) => ({
      ...story,
      styles: ["div { max-width: 500px; }"],
    }),
  ],
};

export default meta;
type Story = StoryObj<FormSectionComponent>;

/**
 * Default form section with title and description
 */
export const Default: Story = {
  render: () => ({
    template: `
      <ocr-form-section
        title="Kontaktdaten"
        description="Geben Sie Ihre Kontaktinformationen ein"
      >
        <ocr-input label="E-Mail" placeholder="ihre@email.de" leadingIcon="mail" />
        <ocr-input label="Telefon" placeholder="+49 123 456789" leadingIcon="phone" />
      </ocr-form-section>
    `,
  }),
};

/**
 * Section with icon
 */
export const WithIcon: Story = {
  render: () => ({
    template: `
      <ocr-form-section
        title="Persönliche Daten"
        description="Ihre persönlichen Informationen"
        icon="user"
      >
        <ocr-input label="Vorname" placeholder="Max" />
        <ocr-input label="Nachname" placeholder="Mustermann" />
        <ocr-input label="Geburtsdatum" placeholder="TT.MM.JJJJ" type="text" leadingIcon="calendar" />
      </ocr-form-section>
    `,
  }),
};

/**
 * Card variant - elevated with shadow
 */
export const CardVariant: Story = {
  render: () => ({
    template: `
      <ocr-form-section
        title="Zahlungsinformationen"
        description="Ihre Zahlungsmethode"
        icon="credit-card"
        variant="card"
      >
        <ocr-input label="Karteninhaber" placeholder="Max Mustermann" />
        <ocr-input label="Kartennummer" placeholder="1234 5678 9012 3456" />
        <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
          <ocr-input label="Gültig bis" placeholder="MM/JJ" />
          <ocr-input label="CVV" placeholder="123" />
        </div>
      </ocr-form-section>
    `,
  }),
};

/**
 * Bordered variant - with border, no shadow
 */
export const BorderedVariant: Story = {
  render: () => ({
    template: `
      <ocr-form-section
        title="Adresse"
        icon="map-pin"
        variant="bordered"
      >
        <ocr-input label="Straße und Hausnummer" placeholder="Musterstraße 123" />
        <div style="display: grid; grid-template-columns: 1fr 2fr; gap: 1rem;">
          <ocr-input label="PLZ" placeholder="12345" />
          <ocr-input label="Stadt" placeholder="Musterstadt" />
        </div>
      </ocr-form-section>
    `,
  }),
};

/**
 * Compact variant - less spacing
 */
export const CompactVariant: Story = {
  render: () => ({
    template: `
      <ocr-form-section
        title="Schnellkontakt"
        variant="compact"
      >
        <ocr-input label="E-Mail" placeholder="ihre@email.de" size="sm" />
        <ocr-input label="Telefon" placeholder="+49 123 456789" size="sm" />
      </ocr-form-section>
    `,
  }),
};

/**
 * Collapsible section
 */
export const Collapsible: Story = {
  render: () => ({
    template: `
      <ocr-form-section
        title="Optionale Angaben"
        description="Diese Felder sind optional"
        icon="settings"
        [collapsible]="true"
      >
        <ocr-textarea label="Anmerkungen" placeholder="Zusätzliche Hinweise..." />
        <ocr-checkbox label="Newsletter abonnieren" />
      </ocr-form-section>
    `,
  }),
};

/**
 * Initially collapsed section
 */
export const InitiallyCollapsed: Story = {
  render: () => ({
    template: `
      <ocr-form-section
        title="Erweiterte Optionen"
        description="Klicken Sie zum Erweitern"
        icon="sliders"
        [collapsible]="true"
        [initiallyCollapsed]="true"
      >
        <ocr-input label="Sonderanforderungen" placeholder="z.B. Kindersitz" />
        <ocr-input label="Firmenrabatt-Code" placeholder="ABC123" />
      </ocr-form-section>
    `,
  }),
};

/**
 * Multiple sections example
 */
export const MultipleSections: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column;">
        <ocr-form-section
          title="Fahrerinformationen"
          description="Hauptfahrer des Fahrzeugs"
          icon="user"
        >
          <ocr-input label="Vollständiger Name" placeholder="Max Mustermann" />
          <ocr-input label="Führerscheinnummer" placeholder="B123456789" leadingIcon="file-text" />
        </ocr-form-section>

        <ocr-form-section
          title="Kontaktdaten"
          description="Wie können wir Sie erreichen?"
          icon="mail"
        >
          <ocr-input label="E-Mail" placeholder="ihre@email.de" />
          <ocr-input label="Telefon" placeholder="+49 123 456789" />
        </ocr-form-section>

        <ocr-form-section
          title="Zusätzliche Optionen"
          icon="settings"
          [collapsible]="true"
          [initiallyCollapsed]="true"
        >
          <ocr-checkbox label="Zusatzfahrer hinzufügen" />
          <ocr-checkbox label="Vollkaskoversicherung" />
        </ocr-form-section>
      </div>
    `,
  }),
};

/**
 * Card variant multiple sections
 */
export const CardMultipleSections: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 1rem;">
        <ocr-form-section
          title="Schritt 1: Persönliche Daten"
          icon="user"
          variant="card"
        >
          <ocr-input label="Vorname" placeholder="Max" />
          <ocr-input label="Nachname" placeholder="Mustermann" />
        </ocr-form-section>

        <ocr-form-section
          title="Schritt 2: Kontakt"
          icon="phone"
          variant="card"
        >
          <ocr-input label="E-Mail" placeholder="ihre@email.de" />
          <ocr-input label="Telefon" placeholder="+49 123 456789" />
        </ocr-form-section>
      </div>
    `,
  }),
};

/**
 * Section without header (content only)
 */
export const ContentOnly: Story = {
  render: () => ({
    template: `
      <ocr-form-section>
        <ocr-input label="Benutzername" placeholder="Wählen Sie einen Benutzernamen" />
        <ocr-input label="Passwort" placeholder="Sicheres Passwort" type="password" />
      </ocr-form-section>
    `,
  }),
};

/**
 * Title only (no description)
 */
export const TitleOnly: Story = {
  render: () => ({
    template: `
      <ocr-form-section title="Anmeldedaten">
        <ocr-input label="E-Mail" placeholder="ihre@email.de" leadingIcon="mail" />
        <ocr-input label="Passwort" placeholder="Ihr Passwort" type="password" leadingIcon="lock" />
      </ocr-form-section>
    `,
  }),
};
