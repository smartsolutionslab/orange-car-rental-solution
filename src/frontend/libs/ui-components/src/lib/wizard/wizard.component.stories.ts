import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { signal } from "@angular/core";
import { WizardComponent } from "./wizard.component";
import { WizardStepComponent } from "./wizard-step.component";
import type { WizardStepConfig } from "./wizard.types";

const meta: Meta<WizardComponent> = {
  title: "Components/Navigation/Wizard",
  component: WizardComponent,
  tags: ["autodocs"],
  decorators: [
    moduleMetadata({
      imports: [WizardComponent, WizardStepComponent],
    }),
  ],
  argTypes: {
    linear: {
      control: "boolean",
      description:
        "Whether navigation is linear (must complete steps in order)",
    },
    orientation: {
      control: "select",
      options: ["horizontal", "vertical"],
      description: "Layout orientation of the wizard",
    },
    showNavigation: {
      control: "boolean",
      description: "Whether to show navigation buttons",
    },
    previousLabel: {
      control: "text",
      description: "Label for the previous button",
    },
    nextLabel: {
      control: "text",
      description: "Label for the next button",
    },
    skipLabel: {
      control: "text",
      description: "Label for the skip button",
    },
    completeLabel: {
      control: "text",
      description: "Label for the complete button",
    },
  },
  parameters: {
    docs: {
      description: {
        component: `
A multi-step wizard component with configurable steps, validation support, and keyboard navigation.

## Features
- Linear or non-linear navigation
- Step validation with signals
- Skippable steps
- Keyboard navigation (arrow keys)
- Horizontal or vertical orientation
- Accessible ARIA attributes

## Usage
\`\`\`typescript
const steps: WizardStepConfig[] = [
  { id: 'account', title: 'Account', subtitle: 'Create your account' },
  { id: 'personal', title: 'Personal', subtitle: 'Your details' },
  { id: 'confirm', title: 'Confirm', subtitle: 'Review & submit' },
];
\`\`\`

\`\`\`html
<ocr-wizard [steps]="steps" (complete)="onComplete($event)">
  <ocr-wizard-step stepId="account">
    <ng-template>Account form content</ng-template>
  </ocr-wizard-step>
  <ocr-wizard-step stepId="personal">
    <ng-template>Personal form content</ng-template>
  </ocr-wizard-step>
  <ocr-wizard-step stepId="confirm">
    <ng-template>Confirmation content</ng-template>
  </ocr-wizard-step>
</ocr-wizard>
\`\`\`
        `,
      },
    },
  },
};

export default meta;
type Story = StoryObj<WizardComponent>;

const basicSteps: WizardStepConfig[] = [
  { id: "step1", title: "Fahrzeug", subtitle: "Wählen Sie Ihr Fahrzeug" },
  { id: "step2", title: "Zeitraum", subtitle: "Buchungszeitraum festlegen" },
  {
    id: "step3",
    title: "Extras",
    subtitle: "Zusatzoptionen auswählen",
    canSkip: true,
  },
  { id: "step4", title: "Bestätigung", subtitle: "Buchung abschließen" },
];

export const Default: Story = {
  render: (args) => ({
    props: {
      ...args,
      steps: basicSteps,
      onStepChange: (event: unknown) => console.log("Step changed:", event),
      onComplete: (event: unknown) => console.log("Wizard completed:", event),
    },
    template: `
      <ocr-wizard
        [steps]="steps"
        [linear]="linear"
        [orientation]="orientation"
        [showNavigation]="showNavigation"
        [previousLabel]="previousLabel"
        [nextLabel]="nextLabel"
        [skipLabel]="skipLabel"
        [completeLabel]="completeLabel"
        (stepChange)="onStepChange($event)"
        (complete)="onComplete($event)"
      >
        <ocr-wizard-step stepId="step1">
          <ng-template>
            <div style="padding: 1.5rem; border: 1px solid #e5e7eb; border-radius: 0.5rem;">
              <h3 style="margin: 0 0 1rem 0; font-size: 1.125rem; font-weight: 600;">Fahrzeugauswahl</h3>
              <p style="color: #6b7280; margin-bottom: 1rem;">Wählen Sie das gewünschte Fahrzeug aus unserem Fuhrpark.</p>
              <div style="display: grid; gap: 0.75rem;">
                <label style="display: flex; align-items: center; gap: 0.5rem; padding: 0.75rem; border: 1px solid #e5e7eb; border-radius: 0.375rem; cursor: pointer;">
                  <input type="radio" name="vehicle" value="compact" checked>
                  <span>Kompaktwagen (VW Golf)</span>
                </label>
                <label style="display: flex; align-items: center; gap: 0.5rem; padding: 0.75rem; border: 1px solid #e5e7eb; border-radius: 0.375rem; cursor: pointer;">
                  <input type="radio" name="vehicle" value="suv">
                  <span>SUV (BMW X5)</span>
                </label>
                <label style="display: flex; align-items: center; gap: 0.5rem; padding: 0.75rem; border: 1px solid #e5e7eb; border-radius: 0.375rem; cursor: pointer;">
                  <input type="radio" name="vehicle" value="luxury">
                  <span>Luxus (Mercedes S-Klasse)</span>
                </label>
              </div>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="step2">
          <ng-template>
            <div style="padding: 1.5rem; border: 1px solid #e5e7eb; border-radius: 0.5rem;">
              <h3 style="margin: 0 0 1rem 0; font-size: 1.125rem; font-weight: 600;">Buchungszeitraum</h3>
              <p style="color: #6b7280; margin-bottom: 1rem;">Legen Sie Start- und Enddatum Ihrer Buchung fest.</p>
              <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem;">
                <div>
                  <label style="display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 0.25rem;">Abholdatum</label>
                  <input type="date" style="width: 100%; padding: 0.5rem; border: 1px solid #d1d5db; border-radius: 0.375rem;">
                </div>
                <div>
                  <label style="display: block; font-size: 0.875rem; font-weight: 500; margin-bottom: 0.25rem;">Rückgabedatum</label>
                  <input type="date" style="width: 100%; padding: 0.5rem; border: 1px solid #d1d5db; border-radius: 0.375rem;">
                </div>
              </div>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="step3">
          <ng-template>
            <div style="padding: 1.5rem; border: 1px solid #e5e7eb; border-radius: 0.5rem;">
              <h3 style="margin: 0 0 1rem 0; font-size: 1.125rem; font-weight: 600;">Zusatzoptionen</h3>
              <p style="color: #6b7280; margin-bottom: 1rem;">Wählen Sie optionale Extras für Ihre Buchung.</p>
              <div style="display: grid; gap: 0.75rem;">
                <label style="display: flex; align-items: center; gap: 0.5rem; padding: 0.75rem; border: 1px solid #e5e7eb; border-radius: 0.375rem; cursor: pointer;">
                  <input type="checkbox">
                  <span>Navigationsgerät (+5€/Tag)</span>
                </label>
                <label style="display: flex; align-items: center; gap: 0.5rem; padding: 0.75rem; border: 1px solid #e5e7eb; border-radius: 0.375rem; cursor: pointer;">
                  <input type="checkbox">
                  <span>Kindersitz (+8€/Tag)</span>
                </label>
                <label style="display: flex; align-items: center; gap: 0.5rem; padding: 0.75rem; border: 1px solid #e5e7eb; border-radius: 0.375rem; cursor: pointer;">
                  <input type="checkbox">
                  <span>Zusatzfahrer (+10€/Tag)</span>
                </label>
              </div>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="step4">
          <ng-template>
            <div style="padding: 1.5rem; border: 1px solid #e5e7eb; border-radius: 0.5rem;">
              <h3 style="margin: 0 0 1rem 0; font-size: 1.125rem; font-weight: 600;">Buchungsübersicht</h3>
              <p style="color: #6b7280; margin-bottom: 1rem;">Überprüfen Sie Ihre Buchungsdetails.</p>
              <div style="background: #f9fafb; padding: 1rem; border-radius: 0.375rem;">
                <div style="display: flex; justify-content: space-between; margin-bottom: 0.5rem;">
                  <span style="color: #6b7280;">Fahrzeug:</span>
                  <span style="font-weight: 500;">VW Golf</span>
                </div>
                <div style="display: flex; justify-content: space-between; margin-bottom: 0.5rem;">
                  <span style="color: #6b7280;">Zeitraum:</span>
                  <span style="font-weight: 500;">3 Tage</span>
                </div>
                <div style="display: flex; justify-content: space-between; padding-top: 0.5rem; border-top: 1px solid #e5e7eb;">
                  <span style="font-weight: 600;">Gesamtpreis:</span>
                  <span style="font-weight: 600; color: #f97316;">€149,00</span>
                </div>
              </div>
            </div>
          </ng-template>
        </ocr-wizard-step>
      </ocr-wizard>
    `,
  }),
  args: {
    linear: true,
    orientation: "horizontal",
    showNavigation: true,
    previousLabel: "Zurück",
    nextLabel: "Weiter",
    skipLabel: "Überspringen",
    completeLabel: "Buchung abschließen",
  },
};

export const VerticalOrientation: Story = {
  render: (args) => ({
    props: {
      ...args,
      steps: basicSteps,
    },
    template: `
      <div style="height: 400px;">
        <ocr-wizard [steps]="steps" orientation="vertical">
          <ocr-wizard-step stepId="step1">
            <ng-template>
              <div style="padding: 1rem;">
                <h3 style="margin: 0 0 0.5rem 0;">Schritt 1: Fahrzeugauswahl</h3>
                <p style="color: #6b7280;">Wählen Sie Ihr gewünschtes Fahrzeug aus.</p>
              </div>
            </ng-template>
          </ocr-wizard-step>
          <ocr-wizard-step stepId="step2">
            <ng-template>
              <div style="padding: 1rem;">
                <h3 style="margin: 0 0 0.5rem 0;">Schritt 2: Zeitraum</h3>
                <p style="color: #6b7280;">Definieren Sie Ihren Buchungszeitraum.</p>
              </div>
            </ng-template>
          </ocr-wizard-step>
          <ocr-wizard-step stepId="step3">
            <ng-template>
              <div style="padding: 1rem;">
                <h3 style="margin: 0 0 0.5rem 0;">Schritt 3: Extras</h3>
                <p style="color: #6b7280;">Wählen Sie optionale Zusatzleistungen.</p>
              </div>
            </ng-template>
          </ocr-wizard-step>
          <ocr-wizard-step stepId="step4">
            <ng-template>
              <div style="padding: 1rem;">
                <h3 style="margin: 0 0 0.5rem 0;">Schritt 4: Bestätigung</h3>
                <p style="color: #6b7280;">Überprüfen und bestätigen Sie Ihre Buchung.</p>
              </div>
            </ng-template>
          </ocr-wizard-step>
        </ocr-wizard>
      </div>
    `,
  }),
  args: {
    linear: true,
    orientation: "vertical",
    showNavigation: true,
  },
};

export const NonLinear: Story = {
  render: (args) => ({
    props: {
      ...args,
      steps: basicSteps,
    },
    template: `
      <ocr-wizard [steps]="steps" [linear]="false">
        <ocr-wizard-step stepId="step1">
          <ng-template>
            <div style="padding: 1rem; text-align: center;">
              <h3>Fahrzeug</h3>
              <p style="color: #6b7280;">In non-linear mode, you can click any step.</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="step2">
          <ng-template>
            <div style="padding: 1rem; text-align: center;">
              <h3>Zeitraum</h3>
              <p style="color: #6b7280;">Navigate freely between steps.</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="step3">
          <ng-template>
            <div style="padding: 1rem; text-align: center;">
              <h3>Extras</h3>
              <p style="color: #6b7280;">This step is skippable.</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="step4">
          <ng-template>
            <div style="padding: 1rem; text-align: center;">
              <h3>Bestätigung</h3>
              <p style="color: #6b7280;">Complete the wizard from here.</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
      </ocr-wizard>
    `,
  }),
  args: {
    linear: false,
  },
  parameters: {
    docs: {
      description: {
        story:
          "In non-linear mode, users can navigate to any step by clicking on it directly.",
      },
    },
  },
};

export const WithoutNavigation: Story = {
  render: (args) => ({
    props: {
      ...args,
      steps: [
        { id: "s1", title: "Step 1" },
        { id: "s2", title: "Step 2" },
        { id: "s3", title: "Step 3" },
      ],
    },
    template: `
      <ocr-wizard [steps]="steps" [showNavigation]="false" [linear]="false">
        <ocr-wizard-step stepId="s1">
          <ng-template>
            <div style="padding: 1rem; text-align: center;">
              <p>Click the step indicators to navigate.</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="s2">
          <ng-template>
            <div style="padding: 1rem; text-align: center;">
              <p>No navigation buttons shown.</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="s3">
          <ng-template>
            <div style="padding: 1rem; text-align: center;">
              <p>Custom navigation can be added.</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
      </ocr-wizard>
    `,
  }),
  args: {
    showNavigation: false,
  },
  parameters: {
    docs: {
      description: {
        story:
          "Navigation buttons can be hidden for custom navigation implementations.",
      },
    },
  },
};

export const SimpleThreeSteps: Story = {
  render: () => ({
    props: {
      steps: [
        { id: "info", title: "Information" },
        { id: "review", title: "Review" },
        { id: "done", title: "Done" },
      ],
    },
    template: `
      <ocr-wizard [steps]="steps">
        <ocr-wizard-step stepId="info">
          <ng-template>
            <div style="padding: 2rem; text-align: center;">
              <h2 style="margin: 0 0 1rem 0;">Welcome</h2>
              <p style="color: #6b7280;">This is a simple 3-step wizard example.</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="review">
          <ng-template>
            <div style="padding: 2rem; text-align: center;">
              <h2 style="margin: 0 0 1rem 0;">Review</h2>
              <p style="color: #6b7280;">Review your information before proceeding.</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
        <ocr-wizard-step stepId="done">
          <ng-template>
            <div style="padding: 2rem; text-align: center;">
              <h2 style="margin: 0 0 1rem 0;">Complete!</h2>
              <p style="color: #6b7280;">Click "Abschließen" to finish the wizard.</p>
            </div>
          </ng-template>
        </ocr-wizard-step>
      </ocr-wizard>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: "A minimal wizard with just 3 steps.",
      },
    },
  },
};
