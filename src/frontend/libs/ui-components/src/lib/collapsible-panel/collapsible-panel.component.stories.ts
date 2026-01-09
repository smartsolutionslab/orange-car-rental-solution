import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { CollapsiblePanelComponent } from "./collapsible-panel.component";
import { CommonModule } from "@angular/common";

const meta: Meta<CollapsiblePanelComponent> = {
  title: "Layout/Collapsible Panel",
  component: CollapsiblePanelComponent,
  decorators: [
    moduleMetadata({
      imports: [CommonModule],
    }),
  ],
  parameters: {
    layout: "padded",
    docs: {
      description: {
        component: `
A single collapsible content panel with a customizable header.

## Features
- Smooth expand/collapse animation using CSS Grid
- Customizable header with title and subtitle
- Optional custom header template
- Show/hide toggle icon
- Bordered/borderless variants
- Lazy content loading
- Keyboard accessible
- ARIA support

## Usage

\`\`\`html
<!-- Basic usage -->
<ocr-collapsible-panel
  title="Advanced Settings"
  subtitle="Configure advanced options"
  [expanded]="false"
  (expandedChange)="onToggle($event)"
>
  <p>Panel content goes here...</p>
</ocr-collapsible-panel>

<!-- With custom header -->
<ocr-collapsible-panel [expanded]="true">
  <ng-template #headerTemplate>
    <div class="flex items-center gap-2">
      <span>Custom Header</span>
      <span class="badge">3 items</span>
    </div>
  </ng-template>
  <p>Content with custom header...</p>
</ocr-collapsible-panel>
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    expanded: {
      control: "boolean",
      description: "Initial expanded state",
    },
    disabled: {
      control: "boolean",
      description: "Disable the panel toggle",
    },
    bordered: {
      control: "boolean",
      description: "Show border around the panel",
    },
    showIcon: {
      control: "boolean",
      description: "Show the expand/collapse icon",
    },
    title: {
      control: "text",
      description: "Panel title text",
    },
    subtitle: {
      control: "text",
      description: "Optional subtitle text",
    },
  },
};

export default meta;
type Story = StoryObj<CollapsiblePanelComponent>;

/**
 * Default collapsible panel - collapsed
 */
export const Default: Story = {
  args: {
    title: "Advanced Settings",
    subtitle: "Configure advanced options",
    expanded: false,
    disabled: false,
    bordered: true,
    showIcon: true,
  },
  render: (args) => ({
    props: args,
    template: `
      <ocr-collapsible-panel
        [title]="title"
        [subtitle]="subtitle"
        [expanded]="expanded"
        [disabled]="disabled"
        [bordered]="bordered"
        [showIcon]="showIcon"
      >
        <div style="color: #374151;">
          <p style="margin: 0 0 1rem;">This is the panel content that can be expanded or collapsed.</p>
          <p style="margin: 0;">You can put any content here, including forms, lists, or other components.</p>
        </div>
      </ocr-collapsible-panel>
    `,
  }),
};

/**
 * Panel in expanded state
 */
export const Expanded: Story = {
  args: {
    title: "User Preferences",
    subtitle: "Customize your experience",
    expanded: true,
    bordered: true,
    showIcon: true,
  },
  render: (args) => ({
    props: args,
    template: `
      <ocr-collapsible-panel
        [title]="title"
        [subtitle]="subtitle"
        [expanded]="expanded"
        [bordered]="bordered"
        [showIcon]="showIcon"
      >
        <div style="display: flex; flex-direction: column; gap: 0.75rem;">
          <label style="display: flex; align-items: center; gap: 0.5rem;">
            <input type="checkbox" checked /> Enable notifications
          </label>
          <label style="display: flex; align-items: center; gap: 0.5rem;">
            <input type="checkbox" /> Dark mode
          </label>
          <label style="display: flex; align-items: center; gap: 0.5rem;">
            <input type="checkbox" checked /> Auto-save
          </label>
        </div>
      </ocr-collapsible-panel>
    `,
  }),
};

/**
 * Panel without border
 */
export const NoBorder: Story = {
  args: {
    title: "Additional Information",
    expanded: false,
    bordered: false,
    showIcon: true,
  },
  render: (args) => ({
    props: args,
    template: `
      <div style="background: #f9fafb; padding: 1rem; border-radius: 0.5rem;">
        <ocr-collapsible-panel
          [title]="title"
          [expanded]="expanded"
          [bordered]="bordered"
          [showIcon]="showIcon"
        >
          <p style="margin: 0; color: #374151;">
            This panel has no border and blends with the background.
            Useful for nested sections or subtle collapsible areas.
          </p>
        </ocr-collapsible-panel>
      </div>
    `,
  }),
};

/**
 * Panel without icon
 */
export const NoIcon: Story = {
  args: {
    title: "Click to Expand",
    expanded: false,
    bordered: true,
    showIcon: false,
  },
  render: (args) => ({
    props: args,
    template: `
      <ocr-collapsible-panel
        [title]="title"
        [expanded]="expanded"
        [bordered]="bordered"
        [showIcon]="showIcon"
      >
        <p style="margin: 0; color: #374151;">
          This panel has no toggle icon. The entire header is still clickable.
        </p>
      </ocr-collapsible-panel>
    `,
  }),
};

/**
 * Disabled panel
 */
export const Disabled: Story = {
  args: {
    title: "Locked Section",
    subtitle: "This section is currently unavailable",
    expanded: false,
    disabled: true,
    bordered: true,
    showIcon: true,
  },
  render: (args) => ({
    props: args,
    template: `
      <ocr-collapsible-panel
        [title]="title"
        [subtitle]="subtitle"
        [expanded]="expanded"
        [disabled]="disabled"
        [bordered]="bordered"
        [showIcon]="showIcon"
      >
        <p style="margin: 0; color: #374151;">
          You cannot see this content because the panel is disabled.
        </p>
      </ocr-collapsible-panel>
    `,
  }),
};

/**
 * Multiple panels stacked
 */
export const MultiplePanels: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 1rem;">
        <ocr-collapsible-panel
          title="General Settings"
          subtitle="Basic configuration options"
          [expanded]="true"
        >
          <p style="margin: 0; color: #374151;">General settings content...</p>
        </ocr-collapsible-panel>

        <ocr-collapsible-panel
          title="Security Settings"
          subtitle="Password and authentication"
          [expanded]="false"
        >
          <p style="margin: 0; color: #374151;">Security settings content...</p>
        </ocr-collapsible-panel>

        <ocr-collapsible-panel
          title="Notification Settings"
          subtitle="Email and push notifications"
          [expanded]="false"
        >
          <p style="margin: 0; color: #374151;">Notification settings content...</p>
        </ocr-collapsible-panel>
      </div>
    `,
  }),
};

/**
 * Panel with rich content
 */
export const RichContent: Story = {
  args: {
    title: "Order Summary",
    subtitle: "Review your order details",
    expanded: true,
    bordered: true,
  },
  render: (args) => ({
    props: args,
    template: `
      <ocr-collapsible-panel
        [title]="title"
        [subtitle]="subtitle"
        [expanded]="expanded"
        [bordered]="bordered"
      >
        <table style="width: 100%; border-collapse: collapse; font-size: 0.875rem;">
          <tr style="border-bottom: 1px solid #e5e7eb;">
            <td style="padding: 0.5rem 0; color: #6b7280;">Subtotal</td>
            <td style="padding: 0.5rem 0; text-align: right; color: #111827;">€89.99</td>
          </tr>
          <tr style="border-bottom: 1px solid #e5e7eb;">
            <td style="padding: 0.5rem 0; color: #6b7280;">Insurance</td>
            <td style="padding: 0.5rem 0; text-align: right; color: #111827;">€15.00</td>
          </tr>
          <tr style="border-bottom: 1px solid #e5e7eb;">
            <td style="padding: 0.5rem 0; color: #6b7280;">Tax (19%)</td>
            <td style="padding: 0.5rem 0; text-align: right; color: #111827;">€19.95</td>
          </tr>
          <tr>
            <td style="padding: 0.75rem 0; font-weight: 600; color: #111827;">Total</td>
            <td style="padding: 0.75rem 0; text-align: right; font-weight: 600; color: #f97316;">€124.94</td>
          </tr>
        </table>
      </ocr-collapsible-panel>
    `,
  }),
};
