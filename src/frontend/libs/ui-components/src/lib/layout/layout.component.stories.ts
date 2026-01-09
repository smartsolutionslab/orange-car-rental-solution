import type { Meta, StoryObj } from '@storybook/angular';
import { LayoutComponent } from './layout.component';

const meta: Meta<LayoutComponent> = {
  title: 'Components/Layout/Layout',
  component: LayoutComponent,
  tags: ['autodocs'],
  argTypes: {
    navPosition: {
      control: 'select',
      options: ['top', 'left'],
      description: 'Navigation position',
    },
    showSidebar: {
      control: 'boolean',
      description: 'Show sidebar area',
    },
    fullWidth: {
      control: 'boolean',
      description: 'Full-width layout',
    },
    maxWidth: {
      control: 'text',
      description: 'Maximum width (CSS value)',
    },
  },
  parameters: {
    layout: 'fullscreen',
    docs: {
      description: {
        component: `
A shared layout component providing consistent page structure across all Orange Car Rental portals.

## Features
- Top or left navigation position
- Optional sidebar area
- Responsive design with mobile breakpoints
- Configurable max-width
- Sticky header and sidebar

## Content Projection Slots
- \`[navigation]\` - Navigation bar content
- \`[content]\` - Main page content
- \`[sidebar]\` - Optional sidebar content (requires showSidebar=true)

## Usage
\`\`\`html
<ocr-layout [navPosition]="'top'" [showSidebar]="false">
  <nav navigation>
    <ocr-navigation [title]="'My App'" ... />
  </nav>

  <main content>
    <router-outlet />
  </main>

  <aside sidebar>
    <!-- Sidebar content here -->
  </aside>
</ocr-layout>
\`\`\`
        `,
      },
    },
  },
};

export default meta;
type Story = StoryObj<LayoutComponent>;

export const TopNavigation: Story = {
  render: (args) => ({
    props: args,
    template: `
      <ocr-layout
        [navPosition]="navPosition"
        [showSidebar]="showSidebar"
        [fullWidth]="fullWidth"
        [maxWidth]="maxWidth"
      >
        <nav navigation>
          <div style="display: flex; justify-content: space-between; align-items: center; padding: 1rem 0;">
            <div style="display: flex; align-items: center; gap: 0.75rem;">
              <span style="font-weight: 600; font-size: 1.25rem; color: #ff6b35;">🚗</span>
              <span style="font-weight: 600;">Orange Car Rental</span>
            </div>
            <div style="display: flex; gap: 1rem;">
              <a href="#" style="color: #666; text-decoration: none;">Fahrzeuge</a>
              <a href="#" style="color: #666; text-decoration: none;">Buchungen</a>
              <a href="#" style="color: #666; text-decoration: none;">Kontakt</a>
            </div>
          </div>
        </nav>

        <main content>
          <h1 style="margin: 0 0 1rem 0;">Willkommen</h1>
          <p style="color: #6b7280;">Dies ist der Hauptinhalt der Seite.</p>
          <div style="margin-top: 2rem; padding: 2rem; background: #f9fafb; border-radius: 0.5rem;">
            <p style="margin: 0; color: #9ca3af;">Beispielinhalt...</p>
          </div>
        </main>
      </ocr-layout>
    `,
  }),
  args: {
    navPosition: 'top',
    showSidebar: false,
    fullWidth: false,
    maxWidth: '1400px',
  },
};

export const WithSidebar: Story = {
  render: () => ({
    template: `
      <ocr-layout [showSidebar]="true">
        <nav navigation>
          <div style="display: flex; justify-content: space-between; align-items: center; padding: 1rem 0;">
            <span style="font-weight: 600;">Orange Car Rental</span>
            <div style="display: flex; gap: 1rem;">
              <a href="#" style="color: #666; text-decoration: none;">Home</a>
              <a href="#" style="color: #ff6b35; text-decoration: none;">Fahrzeuge</a>
            </div>
          </div>
        </nav>

        <main content>
          <h1 style="margin: 0 0 1rem 0;">Fahrzeugauswahl</h1>
          <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 1rem;">
            <div style="padding: 1rem; background: #f9fafb; border-radius: 0.5rem;">
              <h3 style="margin: 0 0 0.5rem 0;">BMW X5</h3>
              <p style="margin: 0; color: #6b7280;">SUV • Automatik</p>
            </div>
            <div style="padding: 1rem; background: #f9fafb; border-radius: 0.5rem;">
              <h3 style="margin: 0 0 0.5rem 0;">VW Golf</h3>
              <p style="margin: 0; color: #6b7280;">Kompakt • Schaltung</p>
            </div>
          </div>
        </main>

        <aside sidebar>
          <h3 style="margin: 0 0 1rem 0; font-size: 1rem;">Filter</h3>
          <div style="display: flex; flex-direction: column; gap: 0.75rem;">
            <label style="display: flex; align-items: center; gap: 0.5rem; color: #6b7280;">
              <input type="checkbox" /> SUV
            </label>
            <label style="display: flex; align-items: center; gap: 0.5rem; color: #6b7280;">
              <input type="checkbox" /> Kompakt
            </label>
            <label style="display: flex; align-items: center; gap: 0.5rem; color: #6b7280;">
              <input type="checkbox" /> Luxus
            </label>
          </div>
        </aside>
      </ocr-layout>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Layout with a sidebar for filters or additional content.',
      },
    },
  },
};

export const LeftNavigation: Story = {
  render: () => ({
    template: `
      <ocr-layout navPosition="left">
        <nav navigation>
          <div style="padding: 1.5rem;">
            <h2 style="margin: 0 0 2rem 0; font-size: 1.125rem; color: #ff6b35;">Call Center</h2>
            <div style="display: flex; flex-direction: column; gap: 0.5rem;">
              <a href="#" style="padding: 0.75rem; background: #fff3ed; color: #ff6b35; border-radius: 0.375rem; text-decoration: none;">Dashboard</a>
              <a href="#" style="padding: 0.75rem; color: #666; border-radius: 0.375rem; text-decoration: none;">Reservierungen</a>
              <a href="#" style="padding: 0.75rem; color: #666; border-radius: 0.375rem; text-decoration: none;">Kunden</a>
              <a href="#" style="padding: 0.75rem; color: #666; border-radius: 0.375rem; text-decoration: none;">Fahrzeuge</a>
            </div>
          </div>
        </nav>

        <main content>
          <h1 style="margin: 0 0 1rem 0;">Dashboard</h1>
          <div style="display: grid; grid-template-columns: repeat(3, 1fr); gap: 1rem;">
            <div style="padding: 1.5rem; background: #f0fdf4; border-radius: 0.5rem;">
              <p style="margin: 0; color: #166534; font-size: 2rem; font-weight: 600;">42</p>
              <p style="margin: 0.5rem 0 0 0; color: #6b7280;">Aktive Buchungen</p>
            </div>
            <div style="padding: 1.5rem; background: #fef3c7; border-radius: 0.5rem;">
              <p style="margin: 0; color: #92400e; font-size: 2rem; font-weight: 600;">8</p>
              <p style="margin: 0.5rem 0 0 0; color: #6b7280;">Heute abholen</p>
            </div>
            <div style="padding: 1.5rem; background: #fee2e2; border-radius: 0.5rem;">
              <p style="margin: 0; color: #991b1b; font-size: 2rem; font-weight: 600;">3</p>
              <p style="margin: 0.5rem 0 0 0; color: #6b7280;">Überfällig</p>
            </div>
          </div>
        </main>
      </ocr-layout>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Left-side navigation layout, suitable for admin/dashboard applications.',
      },
    },
  },
};

export const FullWidth: Story = {
  render: () => ({
    template: `
      <ocr-layout [fullWidth]="true">
        <nav navigation>
          <div style="display: flex; justify-content: space-between; align-items: center; padding: 1rem 0;">
            <span style="font-weight: 600;">Orange Car Rental</span>
            <span style="color: #6b7280;">Full Width Layout</span>
          </div>
        </nav>

        <main content>
          <h1 style="margin: 0 0 1rem 0;">Vollbreiten-Layout</h1>
          <p style="color: #6b7280;">
            Dieses Layout nutzt die volle Breite des Bildschirms ohne maximale Breitenbeschränkung.
          </p>
        </main>
      </ocr-layout>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Full-width layout without max-width constraint.',
      },
    },
  },
};
