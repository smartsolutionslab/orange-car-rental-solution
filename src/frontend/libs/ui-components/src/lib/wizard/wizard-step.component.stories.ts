import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata } from '@storybook/angular';
import { WizardComponent } from './wizard.component';
import { WizardStepComponent } from './wizard-step.component';
import type { WizardStepConfig } from './wizard.types';

const simpleSteps: WizardStepConfig[] = [
  { id: 'step1', title: 'Schritt 1', subtitle: 'Erster Schritt' },
  { id: 'step2', title: 'Schritt 2', subtitle: 'Zweiter Schritt' },
  { id: 'step3', title: 'Schritt 3', subtitle: 'Dritter Schritt' },
];

const meta: Meta<WizardStepComponent> = {
  title: 'Components/Navigation/WizardStep',
  component: WizardStepComponent,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      imports: [WizardComponent, WizardStepComponent],
    }),
  ],
  argTypes: {
    stepId: {
      control: 'text',
      description: 'Unique identifier for this step (must match step config id)',
    },
    title: {
      control: 'text',
      description: 'Optional: Override the step title from config',
    },
    subtitle: {
      control: 'text',
      description: 'Optional: Override the step subtitle from config',
    },
  },
  parameters: {
    docs: {
      description: {
        component: `
A single step definition within a Wizard container.

## Features
- Required unique stepId matching the wizard configuration
- Optional title/subtitle overrides
- Lazy content loading via ng-template
- Internal state tracking (active, visited)

## Usage
WizardStepComponent must be used inside a WizardComponent:

\`\`\`html
<ocr-wizard [steps]="steps">
  <ocr-wizard-step stepId="account">
    <ng-template>
      <form>Account form content</form>
    </ng-template>
  </ocr-wizard-step>
</ocr-wizard>
\`\`\`

## Inputs
| Input | Type | Default | Description |
|-------|------|---------|-------------|
| stepId | string | required | Must match id in step config |
| title | string | undefined | Override config title |
| subtitle | string | undefined | Override config subtitle |

## Important
The stepId MUST match an id in the wizard's steps configuration array.
        `,
      },
    },
  },
};

export default meta;
type Story = StoryObj<WizardStepComponent>;

export const Default: Story = {
  render: (args) => ({
    props: {
      ...args,
      steps: simpleSteps,
    },
    template: `
      <ocr-wizard [steps]="steps" [linear]="false">
        <ocr-wizard-step
          [stepId]="stepId"
          [title]="title"
          [subtitle]="subtitle"
        >
          <ng-template>
            <div style="padding: 1.5rem; border: 1px solid #e5e7eb; border-radius: 0.5rem;">
              <h3 style="margin: 0 0 0.5rem 0; font-size: 1.125rem; font-weight: 600;">
                Step Content
              </h3>
              <p style="margin: 0; color: #6b7280;">
                This is the content for step "{{ stepId }}".
                The content is lazily rendered when the step becomes active.
              </p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="step2">
          <ng-template>
            <div style="padding: 1.5rem; border: 1px solid #e5e7eb; border-radius: 0.5rem;">
              <p style="margin: 0; color: #6b7280;">Content for step 2</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="step3">
          <ng-template>
            <div style="padding: 1.5rem; border: 1px solid #e5e7eb; border-radius: 0.5rem;">
              <p style="margin: 0; color: #6b7280;">Content for step 3</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
      </ocr-wizard>
    `,
  }),
  args: {
    stepId: 'step1',
    title: undefined,
    subtitle: undefined,
  },
};

export const WithTitleOverride: Story = {
  render: () => ({
    props: {
      steps: [
        { id: 'account', title: 'Account', subtitle: 'Original subtitle' },
        { id: 'profile', title: 'Profile' },
      ],
    },
    template: `
      <ocr-wizard [steps]="steps">
        <ocr-wizard-step stepId="account" title="Überschriebener Titel" subtitle="Neuer Untertitel">
          <ng-template>
            <div style="padding: 1rem; background: #fef3c7; border-radius: 0.375rem;">
              <p style="margin: 0; color: #92400e;">
                <strong>Hinweis:</strong> Dieser Schritt hat title und subtitle überschrieben.
                Die Wizard-Header zeigt die überschriebenen Werte.
              </p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="profile">
          <ng-template>
            <div style="padding: 1rem;">
              <p style="margin: 0; color: #6b7280;">Profil-Inhalt</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
      </ocr-wizard>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'WizardStep can override the title and subtitle from the step configuration.',
      },
    },
  },
};

export const LazyContentLoading: Story = {
  render: () => ({
    props: {
      steps: [
        { id: 'lazy1', title: 'Lazy Step 1' },
        { id: 'lazy2', title: 'Lazy Step 2' },
        { id: 'lazy3', title: 'Lazy Step 3' },
      ],
    },
    template: `
      <ocr-wizard [steps]="steps" [linear]="false">
        <ocr-wizard-step stepId="lazy1">
          <ng-template>
            <div style="padding: 1rem; background: #f0fdf4; border-radius: 0.375rem;">
              <p style="margin: 0; color: #166534;">
                <strong>Step 1 loaded!</strong><br>
                This content was lazily rendered when step 1 became active.
              </p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="lazy2">
          <ng-template>
            <div style="padding: 1rem; background: #fef3c7; border-radius: 0.375rem;">
              <p style="margin: 0; color: #92400e;">
                <strong>Step 2 loaded!</strong><br>
                Click on step 2 to see this content render.
              </p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="lazy3">
          <ng-template>
            <div style="padding: 1rem; background: #fee2e2; border-radius: 0.375rem;">
              <p style="margin: 0; color: #991b1b;">
                <strong>Step 3 loaded!</strong><br>
                Content is only rendered once the step is visited.
              </p>
            </div>
          </ng-template>
        </ocr-wizard-step>
      </ocr-wizard>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Step content inside ng-template is lazily loaded when the step becomes active for the first time.',
      },
    },
  },
};

export const FormContent: Story = {
  render: () => ({
    props: {
      steps: [
        { id: 'personal', title: 'Persönliche Daten', subtitle: 'Name und Kontakt' },
        { id: 'address', title: 'Adresse', subtitle: 'Wohnanschrift' },
      ],
    },
    template: `
      <ocr-wizard [steps]="steps">
        <ocr-wizard-step stepId="personal">
          <ng-template>
            <div style="display: grid; gap: 1rem; padding: 1rem;">
              <div>
                <label style="display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 0.25rem;">
                  Vorname
                </label>
                <input
                  type="text"
                  placeholder="Max"
                  style="width: 100%; padding: 0.5rem 0.75rem; border: 1px solid #d1d5db; border-radius: 0.375rem;"
                >
              </div>
              <div>
                <label style="display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 0.25rem;">
                  Nachname
                </label>
                <input
                  type="text"
                  placeholder="Mustermann"
                  style="width: 100%; padding: 0.5rem 0.75rem; border: 1px solid #d1d5db; border-radius: 0.375rem;"
                >
              </div>
              <div>
                <label style="display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 0.25rem;">
                  E-Mail
                </label>
                <input
                  type="email"
                  placeholder="max@example.com"
                  style="width: 100%; padding: 0.5rem 0.75rem; border: 1px solid #d1d5db; border-radius: 0.375rem;"
                >
              </div>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="address">
          <ng-template>
            <div style="display: grid; gap: 1rem; padding: 1rem;">
              <div>
                <label style="display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 0.25rem;">
                  Straße
                </label>
                <input
                  type="text"
                  placeholder="Musterstraße 123"
                  style="width: 100%; padding: 0.5rem 0.75rem; border: 1px solid #d1d5db; border-radius: 0.375rem;"
                >
              </div>
              <div style="display: grid; grid-template-columns: 1fr 2fr; gap: 1rem;">
                <div>
                  <label style="display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 0.25rem;">
                    PLZ
                  </label>
                  <input
                    type="text"
                    placeholder="12345"
                    style="width: 100%; padding: 0.5rem 0.75rem; border: 1px solid #d1d5db; border-radius: 0.375rem;"
                  >
                </div>
                <div>
                  <label style="display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 0.25rem;">
                    Stadt
                  </label>
                  <input
                    type="text"
                    placeholder="München"
                    style="width: 100%; padding: 0.5rem 0.75rem; border: 1px solid #d1d5db; border-radius: 0.375rem;"
                  >
                </div>
              </div>
            </div>
          </ng-template>
        </ocr-wizard-step>
      </ocr-wizard>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Example showing WizardStep containing form fields for a multi-step registration flow.',
      },
    },
  },
};

export const RichContent: Story = {
  render: () => ({
    props: {
      steps: [
        { id: 'select', title: 'Auswahl', subtitle: 'Fahrzeug wählen' },
        { id: 'confirm', title: 'Bestätigung' },
      ],
    },
    template: `
      <ocr-wizard [steps]="steps">
        <ocr-wizard-step stepId="select">
          <ng-template>
            <div style="padding: 1rem;">
              <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 1rem;">
                <div style="padding: 1rem; border: 2px solid #f97316; border-radius: 0.5rem; background: #fff7ed;">
                  <h4 style="margin: 0 0 0.5rem 0;">VW Golf</h4>
                  <p style="margin: 0 0 0.5rem 0; font-size: 0.875rem; color: #6b7280;">Kompaktwagen</p>
                  <p style="margin: 0; font-weight: 600; color: #f97316;">€45/Tag</p>
                </div>
                <div style="padding: 1rem; border: 1px solid #e5e7eb; border-radius: 0.5rem;">
                  <h4 style="margin: 0 0 0.5rem 0;">BMW X5</h4>
                  <p style="margin: 0 0 0.5rem 0; font-size: 0.875rem; color: #6b7280;">SUV</p>
                  <p style="margin: 0; font-weight: 600; color: #f97316;">€89/Tag</p>
                </div>
              </div>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="confirm">
          <ng-template>
            <div style="padding: 1rem; text-align: center;">
              <div style="width: 64px; height: 64px; margin: 0 auto 1rem; background: #f0fdf4; border-radius: 50%; display: flex; align-items: center; justify-content: center;">
                <span style="font-size: 2rem;">✓</span>
              </div>
              <h3 style="margin: 0 0 0.5rem 0;">Bereit zur Buchung</h3>
              <p style="margin: 0; color: #6b7280;">Klicken Sie auf "Abschließen" um die Buchung zu bestätigen.</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
      </ocr-wizard>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'WizardStep can contain any rich content including cards, grids, and icons.',
      },
    },
  },
};
