import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata } from '@storybook/angular';
import { TabsComponent } from './tabs.component';
import { TabComponent } from './tab.component';

const meta: Meta<TabComponent> = {
  title: 'Components/Navigation/Tab',
  component: TabComponent,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      imports: [TabsComponent, TabComponent],
    }),
  ],
  argTypes: {
    tabId: {
      control: 'text',
      description: 'Unique identifier for this tab',
    },
    label: {
      control: 'text',
      description: 'Display label for the tab button',
    },
    icon: {
      control: 'text',
      description: 'Optional icon name',
    },
    disabled: {
      control: 'boolean',
      description: 'Whether this tab is disabled',
    },
    badge: {
      control: 'text',
      description: 'Optional badge/count to display',
    },
  },
  parameters: {
    docs: {
      description: {
        component: `
A single tab definition within a Tabs container.

## Features
- Required unique ID and label
- Optional icon support
- Badge/count display
- Disabled state
- Lazy content loading via ng-template

## Usage
TabComponent must be used inside a TabsComponent:

\`\`\`html
<ocr-tabs>
  <ocr-tab tabId="tab1" label="Overview" [badge]="5">
    <ng-template>
      <p>Tab content here</p>
    </ng-template>
  </ocr-tab>
</ocr-tabs>
\`\`\`

## Inputs
| Input | Type | Default | Description |
|-------|------|---------|-------------|
| tabId | string | required | Unique identifier |
| label | string | required | Tab button text |
| icon | string | undefined | Icon name |
| disabled | boolean | false | Prevents selection |
| badge | string/number | undefined | Badge content |
        `,
      },
    },
  },
};

export default meta;
type Story = StoryObj<TabComponent>;

export const Default: Story = {
  render: (args) => ({
    props: args,
    template: `
      <ocr-tabs>
        <ocr-tab
          [tabId]="tabId"
          [label]="label"
          [disabled]="disabled"
          [badge]="badge"
        >
          <ng-template>
            <div style="padding: 1rem; background: #f9fafb; border-radius: 0.375rem;">
              <h3 style="margin: 0 0 0.5rem 0; font-size: 1rem; font-weight: 600;">{{ label }}</h3>
              <p style="margin: 0; color: #6b7280;">
                Dies ist der Inhalt des Tabs. Der Inhalt wird erst gerendert,
                wenn der Tab zum ersten Mal ausgewählt wird.
              </p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="other" label="Anderer Tab">
          <ng-template>
            <div style="padding: 1rem; background: #f9fafb; border-radius: 0.375rem;">
              <p style="margin: 0; color: #6b7280;">Ein weiterer Tab zum Vergleich.</p>
            </div>
          </ng-template>
        </ocr-tab>
      </ocr-tabs>
    `,
  }),
  args: {
    tabId: 'demo-tab',
    label: 'Beispiel Tab',
    disabled: false,
    badge: undefined,
  },
};

export const WithBadge: Story = {
  render: () => ({
    template: `
      <ocr-tabs>
        <ocr-tab tabId="inbox" label="Posteingang" [badge]="12">
          <ng-template>
            <div style="padding: 1rem;">
              <p style="margin: 0; color: #6b7280;">12 ungelesene Nachrichten.</p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="sent" label="Gesendet" [badge]="0">
          <ng-template>
            <div style="padding: 1rem;">
              <p style="margin: 0; color: #6b7280;">Keine neuen gesendeten Nachrichten.</p>
            </div>
          </ng-template>
        </ocr-tab>
      </ocr-tabs>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Tabs can display badges with counts or labels.',
      },
    },
  },
};

export const Disabled: Story = {
  render: () => ({
    template: `
      <ocr-tabs>
        <ocr-tab tabId="active" label="Aktiver Tab">
          <ng-template>
            <div style="padding: 1rem;">
              <p style="margin: 0; color: #6b7280;">Dieser Tab ist auswählbar.</p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="disabled" label="Deaktiviert" [disabled]="true">
          <ng-template>
            <div style="padding: 1rem;">
              <p style="margin: 0; color: #6b7280;">Dieser Inhalt ist nicht erreichbar.</p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="another" label="Weiterer Tab">
          <ng-template>
            <div style="padding: 1rem;">
              <p style="margin: 0; color: #6b7280;">Ein weiterer aktiver Tab.</p>
            </div>
          </ng-template>
        </ocr-tab>
      </ocr-tabs>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'A disabled tab cannot be selected by clicking or keyboard.',
      },
    },
  },
};

export const AllFeatures: Story = {
  render: () => ({
    template: `
      <ocr-tabs>
        <ocr-tab tabId="overview" label="Übersicht">
          <ng-template>
            <div style="padding: 1rem; background: #f0fdf4; border-radius: 0.375rem;">
              <p style="margin: 0; color: #166534;">Übersicht ohne Badge oder Icon.</p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="notifications" label="Benachrichtigungen" [badge]="5">
          <ng-template>
            <div style="padding: 1rem; background: #fef3c7; border-radius: 0.375rem;">
              <p style="margin: 0; color: #92400e;">5 neue Benachrichtigungen!</p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="premium" label="Premium" [disabled]="true" badge="PRO">
          <ng-template>
            <div style="padding: 1rem;">
              <p style="margin: 0; color: #6b7280;">Premium-Inhalt (deaktiviert).</p>
            </div>
          </ng-template>
        </ocr-tab>
      </ocr-tabs>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Demonstration of various tab configurations in one view.',
      },
    },
  },
};

export const VehicleDetailsTabs: Story = {
  render: () => ({
    template: `
      <div style="border: 1px solid #e5e7eb; border-radius: 0.5rem; padding: 1rem; max-width: 600px;">
        <h3 style="margin: 0 0 1rem 0; font-size: 1.125rem; font-weight: 600;">BMW X5 Details</h3>
        <ocr-tabs>
          <ocr-tab tabId="specs" label="Technische Daten">
            <ng-template>
              <div style="display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; padding: 1rem 0;">
                <div>
                  <span style="font-size: 0.75rem; color: #6b7280;">Motor</span>
                  <p style="margin: 0.25rem 0 0 0; font-weight: 500;">3.0L Turbo</p>
                </div>
                <div>
                  <span style="font-size: 0.75rem; color: #6b7280;">Leistung</span>
                  <p style="margin: 0.25rem 0 0 0; font-weight: 500;">340 PS</p>
                </div>
                <div>
                  <span style="font-size: 0.75rem; color: #6b7280;">Getriebe</span>
                  <p style="margin: 0.25rem 0 0 0; font-weight: 500;">8-Gang Automatik</p>
                </div>
                <div>
                  <span style="font-size: 0.75rem; color: #6b7280;">Verbrauch</span>
                  <p style="margin: 0.25rem 0 0 0; font-weight: 500;">8.5L/100km</p>
                </div>
              </div>
            </ng-template>
          </ocr-tab>
          <ocr-tab tabId="bookings" label="Buchungen" [badge]="3">
            <ng-template>
              <div style="padding: 1rem 0;">
                <p style="margin: 0; color: #6b7280;">3 aktive Buchungen für dieses Fahrzeug.</p>
              </div>
            </ng-template>
          </ocr-tab>
          <ocr-tab tabId="history" label="Wartung">
            <ng-template>
              <div style="padding: 1rem 0;">
                <p style="margin: 0 0 0.5rem 0; color: #6b7280;">Letzte Inspektion: 15.12.2025</p>
                <p style="margin: 0; color: #6b7280;">Nächste Inspektion: 15.06.2026</p>
              </div>
            </ng-template>
          </ocr-tab>
        </ocr-tabs>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Real-world example: Tabs used in a vehicle details view.',
      },
    },
  },
};
