import type { Meta, StoryObj } from "@storybook/angular";
import { SwitchComponent } from "./switch.component";

const meta: Meta<SwitchComponent> = {
  title: "Forms/Switch",
  component: SwitchComponent,
  parameters: {
    layout: "centered",
    docs: {
      description: {
        component: `
A styled toggle switch component with label support, multiple sizes,
and reactive forms integration.

## Features
- Smooth toggle animation
- Label on left or right
- Three sizes: sm, md, lg
- Optional description text
- Full reactive forms support (ControlValueAccessor)

## Usage

\`\`\`html
<!-- Basic switch -->
<ocr-switch
  [label]="'Enable notifications'"
  [(ngModel)]="notifications"
/>

<!-- With description -->
<ocr-switch
  [label]="'Dark mode'"
  [description]="'Enable dark theme'"
  [(ngModel)]="darkMode"
/>

<!-- With reactive forms -->
<ocr-switch
  [label]="'Subscribe to newsletter'"
  formControlName="newsletter"
/>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    size: {
      control: "select",
      options: ["sm", "md", "lg"],
    },
    labelPosition: {
      control: "select",
      options: ["left", "right"],
    },
  },
};

export default meta;
type Story = StoryObj<SwitchComponent>;

/**
 * Default switch (off)
 */
export const Default: Story = {
  args: {
    label: "Enable notifications",
    size: "md",
    labelPosition: "right",
  },
};

/**
 * Switch in on state
 */
export const Checked: Story = {
  render: () => ({
    template: `<ocr-switch label="Notifications enabled" />`,
  }),
};

/**
 * Label on left
 */
export const LabelLeft: Story = {
  args: {
    label: "Dark mode",
    labelPosition: "left",
  },
};

/**
 * With description
 */
export const WithDescription: Story = {
  args: {
    label: "Auto-save",
    description: "Automatically save your changes every 5 minutes",
  },
};

/**
 * Small size
 */
export const Small: Story = {
  args: {
    label: "Small switch",
    size: "sm",
  },
};

/**
 * Large size
 */
export const Large: Story = {
  args: {
    label: "Large switch",
    size: "lg",
  },
};

/**
 * Disabled off
 */
export const DisabledOff: Story = {
  args: {
    label: "Disabled switch",
    disabled: true,
  },
};

/**
 * Disabled on
 */
export const DisabledOn: Story = {
  render: () => ({
    template: `<ocr-switch label="Disabled (on)" [disabled]="true" />`,
  }),
};

/**
 * Without label
 */
export const WithoutLabel: Story = {
  args: {
    ariaLabel: "Toggle setting",
  },
};

/**
 * All sizes comparison
 */
export const AllSizes: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 1.5rem;">
        <ocr-switch label="Small switch" size="sm" />
        <ocr-switch label="Medium switch (default)" size="md" />
        <ocr-switch label="Large switch" size="lg" />
      </div>
    `,
  }),
};

/**
 * Settings panel example
 */
export const SettingsPanel: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 1rem; width: 300px; padding: 1rem; background: #f9fafb; border-radius: 0.5rem;">
        <h3 style="margin: 0 0 0.5rem; font-size: 1rem; font-weight: 600; color: #111827;">Notification Settings</h3>
        <ocr-switch label="Email notifications" labelPosition="left" />
        <ocr-switch label="Push notifications" labelPosition="left" />
        <ocr-switch label="SMS notifications" labelPosition="left" />
        <ocr-switch label="Marketing emails" labelPosition="left" />
      </div>
    `,
  }),
};

/**
 * Feature flags example
 */
export const FeatureFlags: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 1.25rem; width: 350px;">
        <ocr-switch
          label="Beta features"
          description="Enable experimental features that are still in development"
        />
        <ocr-switch
          label="Analytics"
          description="Help us improve by sending anonymous usage data"
        />
        <ocr-switch
          label="Auto-update"
          description="Automatically download and install updates"
        />
      </div>
    `,
  }),
};
