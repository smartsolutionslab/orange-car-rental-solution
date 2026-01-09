import type { Meta, StoryObj } from "@storybook/angular";
import { IconComponent } from "./icon.component";
import { ICON_NAMES } from "./icons";

const meta: Meta<IconComponent> = {
  title: "General/Icon",
  component: IconComponent,
  parameters: {
    layout: "centered",
    docs: {
      description: {
        component: `
A reusable SVG icon component that renders icons from a centralized registry.
Icons are rendered inline as SVG elements for easy styling with CSS.

## Features
- Centralized icon registry
- Multiple size presets (xs, sm, md, lg, xl)
- Custom size support
- Outline and filled variants
- Configurable stroke width
- Accessibility support (aria-label, aria-hidden)

## Usage

\`\`\`html
<!-- Basic usage -->
<lib-icon name="eye" />
<lib-icon name="check-circle" class="text-green-600" />

<!-- With custom size -->
<lib-icon name="user" size="lg" />
<lib-icon name="car" [customSize]="32" />

<!-- Filled vs outline style -->
<lib-icon name="star" variant="filled" />
<lib-icon name="heart" variant="outline" />
\`\`\`
        `,
      },
    },
  },
  tags: ["autodocs"],
  argTypes: {
    name: {
      control: "select",
      options: ICON_NAMES,
      description: "Icon name from the registry",
    },
    size: {
      control: "select",
      options: ["xs", "sm", "md", "lg", "xl"],
      description: "Size preset",
    },
    variant: {
      control: "select",
      options: ["outline", "filled"],
      description: "Rendering variant",
    },
    strokeWidth: {
      control: { type: "number", min: 0.5, max: 3, step: 0.5 },
      description: "Stroke width for outline icons",
    },
  },
};

export default meta;
type Story = StoryObj<IconComponent>;

/**
 * Default icon with medium size
 */
export const Default: Story = {
  args: {
    name: "user",
    size: "md",
    variant: "outline",
  },
};

/**
 * Icon sizes comparison
 */
export const Sizes: Story = {
  render: () => ({
    template: `
      <div style="display: flex; align-items: center; gap: 1.5rem;">
        <div style="text-align: center;">
          <lib-icon name="star" size="xs" />
          <p style="margin: 0.5rem 0 0; font-size: 0.75rem; color: #6b7280;">xs (12px)</p>
        </div>
        <div style="text-align: center;">
          <lib-icon name="star" size="sm" />
          <p style="margin: 0.5rem 0 0; font-size: 0.75rem; color: #6b7280;">sm (16px)</p>
        </div>
        <div style="text-align: center;">
          <lib-icon name="star" size="md" />
          <p style="margin: 0.5rem 0 0; font-size: 0.75rem; color: #6b7280;">md (20px)</p>
        </div>
        <div style="text-align: center;">
          <lib-icon name="star" size="lg" />
          <p style="margin: 0.5rem 0 0; font-size: 0.75rem; color: #6b7280;">lg (24px)</p>
        </div>
        <div style="text-align: center;">
          <lib-icon name="star" size="xl" />
          <p style="margin: 0.5rem 0 0; font-size: 0.75rem; color: #6b7280;">xl (32px)</p>
        </div>
      </div>
    `,
  }),
};

/**
 * Outline vs Filled variants
 */
export const Variants: Story = {
  render: () => ({
    template: `
      <div style="display: flex; gap: 2rem;">
        <div style="text-align: center;">
          <lib-icon name="heart" variant="outline" size="lg" />
          <p style="margin: 0.5rem 0 0; font-size: 0.875rem; color: #6b7280;">Outline</p>
        </div>
        <div style="text-align: center;">
          <lib-icon name="heart" variant="filled" size="lg" />
          <p style="margin: 0.5rem 0 0; font-size: 0.875rem; color: #6b7280;">Filled</p>
        </div>
      </div>
    `,
  }),
};

/**
 * Icons with color styling
 */
export const WithColors: Story = {
  render: () => ({
    template: `
      <div style="display: flex; gap: 1.5rem; align-items: center;">
        <lib-icon name="check-circle" size="lg" style="color: #22c55e;" />
        <lib-icon name="x-circle" size="lg" style="color: #ef4444;" />
        <lib-icon name="alert-triangle" size="lg" style="color: #f97316;" />
        <lib-icon name="info" size="lg" style="color: #3b82f6;" />
        <lib-icon name="clock" size="lg" style="color: #6b7280;" />
      </div>
    `,
  }),
};

/**
 * Common action icons
 */
export const ActionIcons: Story = {
  render: () => ({
    template: `
      <div style="display: flex; gap: 1rem; flex-wrap: wrap;">
        <button style="display: flex; align-items: center; gap: 0.5rem; padding: 0.5rem 1rem; border: 1px solid #d1d5db; border-radius: 0.375rem; background: white; cursor: pointer;">
          <lib-icon name="plus" size="sm" />
          <span>Hinzufügen</span>
        </button>
        <button style="display: flex; align-items: center; gap: 0.5rem; padding: 0.5rem 1rem; border: 1px solid #d1d5db; border-radius: 0.375rem; background: white; cursor: pointer;">
          <lib-icon name="pencil" size="sm" />
          <span>Bearbeiten</span>
        </button>
        <button style="display: flex; align-items: center; gap: 0.5rem; padding: 0.5rem 1rem; border: 1px solid #ef4444; border-radius: 0.375rem; background: white; color: #ef4444; cursor: pointer;">
          <lib-icon name="trash" size="sm" />
          <span>Löschen</span>
        </button>
        <button style="display: flex; align-items: center; gap: 0.5rem; padding: 0.5rem 1rem; border: none; border-radius: 0.375rem; background: #f97316; color: white; cursor: pointer;">
          <lib-icon name="download" size="sm" />
          <span>Download</span>
        </button>
      </div>
    `,
  }),
};

/**
 * Navigation icons
 */
export const NavigationIcons: Story = {
  render: () => ({
    template: `
      <div style="display: flex; gap: 1rem;">
        <lib-icon name="chevron-left" size="md" />
        <lib-icon name="chevron-right" size="md" />
        <lib-icon name="chevron-up" size="md" />
        <lib-icon name="chevron-down" size="md" />
        <lib-icon name="arrow-left" size="md" />
        <lib-icon name="arrow-right" size="md" />
      </div>
    `,
  }),
};

/**
 * Status icons
 */
export const StatusIcons: Story = {
  render: () => ({
    template: `
      <div style="display: flex; gap: 1.5rem;">
        <div style="display: flex; align-items: center; gap: 0.5rem; color: #22c55e;">
          <lib-icon name="check-circle" size="md" />
          <span style="font-size: 0.875rem;">Erfolgreich</span>
        </div>
        <div style="display: flex; align-items: center; gap: 0.5rem; color: #ef4444;">
          <lib-icon name="x-circle" size="md" />
          <span style="font-size: 0.875rem;">Fehler</span>
        </div>
        <div style="display: flex; align-items: center; gap: 0.5rem; color: #f97316;">
          <lib-icon name="alert-triangle" size="md" />
          <span style="font-size: 0.875rem;">Warnung</span>
        </div>
        <div style="display: flex; align-items: center; gap: 0.5rem; color: #3b82f6;">
          <lib-icon name="info" size="md" />
          <span style="font-size: 0.875rem;">Info</span>
        </div>
      </div>
    `,
  }),
};

/**
 * All available icons
 */
export const AllIcons: Story = {
  render: () => ({
    props: {
      icons: ICON_NAMES,
    },
    template: `
      <div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(100px, 1fr)); gap: 1rem;">
        @for (icon of icons; track icon) {
          <div style="display: flex; flex-direction: column; align-items: center; padding: 1rem; border: 1px solid #e5e7eb; border-radius: 0.5rem;">
            <lib-icon [name]="icon" size="lg" />
            <span style="margin-top: 0.5rem; font-size: 0.75rem; color: #6b7280; text-align: center; word-break: break-all;">{{ icon }}</span>
          </div>
        }
      </div>
    `,
  }),
};
