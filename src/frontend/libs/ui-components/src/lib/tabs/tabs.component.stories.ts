import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata } from '@storybook/angular';
import { TabsComponent } from './tabs.component';
import { TabComponent } from './tab.component';

const meta: Meta<TabsComponent> = {
  title: 'Components/Navigation/Tabs',
  component: TabsComponent,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      imports: [TabsComponent, TabComponent],
    }),
  ],
  argTypes: {
    orientation: {
      control: 'select',
      options: ['horizontal', 'vertical'],
      description: 'Orientation of the tabs',
    },
    keepAlive: {
      control: 'boolean',
      description: 'Keep visited tab content in DOM',
    },
    ariaLabel: {
      control: 'text',
      description: 'Accessible label for the tab list',
    },
  },
  parameters: {
    docs: {
      description: {
        component: `
A tabbed interface component with keyboard navigation and lazy loading.

## Features
- Horizontal/vertical orientation
- Lazy content loading with optional keep-alive
- Full keyboard navigation (arrow keys, Home, End)
- WAI-ARIA tabs pattern for accessibility
- Active tab indicator animation
- Badge support for notifications

## Keyboard Navigation
- **Arrow Left/Right** (horizontal) or **Arrow Up/Down** (vertical): Move between tabs
- **Home**: Go to first tab
- **End**: Go to last tab
- **Tab**: Move focus to tab panel content
        `,
      },
    },
  },
};

export default meta;
type Story = StoryObj<TabsComponent>;

export const Default: Story = {
  render: (args) => ({
    props: {
      ...args,
      onTabChange: (event: unknown) => console.log('Tab changed:', event),
    },
    template: `
      <ocr-tabs
        [orientation]="orientation"
        [keepAlive]="keepAlive"
        [ariaLabel]="ariaLabel"
        (tabChange)="onTabChange($event)"
      >
        <ocr-tab tabId="overview" label="Übersicht">
          <ng-template>
            <div style="padding: 1rem; background: #f9fafb; border-radius: 0.375rem;">
              <h3 style="margin: 0 0 0.5rem 0; font-size: 1rem; font-weight: 600;">Übersicht</h3>
              <p style="margin: 0; color: #6b7280;">
                Dies ist der Inhalt des Übersicht-Tabs. Hier können allgemeine Informationen angezeigt werden.
              </p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="details" label="Details">
          <ng-template>
            <div style="padding: 1rem; background: #f9fafb; border-radius: 0.375rem;">
              <h3 style="margin: 0 0 0.5rem 0; font-size: 1rem; font-weight: 600;">Details</h3>
              <p style="margin: 0; color: #6b7280;">
                Detaillierte Informationen werden hier angezeigt.
              </p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="history" label="Verlauf">
          <ng-template>
            <div style="padding: 1rem; background: #f9fafb; border-radius: 0.375rem;">
              <h3 style="margin: 0 0 0.5rem 0; font-size: 1rem; font-weight: 600;">Verlauf</h3>
              <p style="margin: 0; color: #6b7280;">
                Der Verlauf der Aktivitäten wird hier dargestellt.
              </p>
            </div>
          </ng-template>
        </ocr-tab>
      </ocr-tabs>
    `,
  }),
  args: {
    orientation: 'horizontal',
    keepAlive: false,
    ariaLabel: 'Tabs',
  },
};

export const VerticalOrientation: Story = {
  render: () => ({
    template: `
      <div style="height: 300px;">
        <ocr-tabs orientation="vertical">
          <ocr-tab tabId="account" label="Konto">
            <ng-template>
              <div style="padding: 1rem;">
                <h3 style="margin: 0 0 1rem 0;">Kontoeinstellungen</h3>
                <p style="color: #6b7280;">Verwalten Sie Ihre Kontoinformationen.</p>
              </div>
            </ng-template>
          </ocr-tab>
          <ocr-tab tabId="security" label="Sicherheit">
            <ng-template>
              <div style="padding: 1rem;">
                <h3 style="margin: 0 0 1rem 0;">Sicherheitseinstellungen</h3>
                <p style="color: #6b7280;">Passwort und Zwei-Faktor-Authentifizierung.</p>
              </div>
            </ng-template>
          </ocr-tab>
          <ocr-tab tabId="notifications" label="Benachrichtigungen">
            <ng-template>
              <div style="padding: 1rem;">
                <h3 style="margin: 0 0 1rem 0;">Benachrichtigungseinstellungen</h3>
                <p style="color: #6b7280;">E-Mail- und Push-Benachrichtigungen konfigurieren.</p>
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
        story: 'Tabs displayed in vertical orientation, suitable for settings pages.',
      },
    },
  },
};

export const WithBadges: Story = {
  render: () => ({
    template: `
      <ocr-tabs>
        <ocr-tab tabId="inbox" label="Posteingang" [badge]="12">
          <ng-template>
            <div style="padding: 1rem;">
              <p>12 neue Nachrichten im Posteingang.</p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="sent" label="Gesendet" [badge]="0">
          <ng-template>
            <div style="padding: 1rem;">
              <p>Gesendete Nachrichten.</p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="drafts" label="Entwürfe" [badge]="3">
          <ng-template>
            <div style="padding: 1rem;">
              <p>3 Entwürfe gespeichert.</p>
            </div>
          </ng-template>
        </ocr-tab>
      </ocr-tabs>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Tabs with badge counts for notifications or item counts.',
      },
    },
  },
};

export const WithDisabledTab: Story = {
  render: () => ({
    template: `
      <ocr-tabs>
        <ocr-tab tabId="active" label="Aktiv">
          <ng-template>
            <div style="padding: 1rem;">
              <p>Dieser Tab ist aktiv und anklickbar.</p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="disabled" label="Deaktiviert" [disabled]="true">
          <ng-template>
            <div style="padding: 1rem;">
              <p>Dieser Tab ist deaktiviert.</p>
            </div>
          </ng-template>
        </ocr-tab>
        <ocr-tab tabId="another" label="Noch ein Tab">
          <ng-template>
            <div style="padding: 1rem;">
              <p>Ein weiterer aktiver Tab.</p>
            </div>
          </ng-template>
        </ocr-tab>
      </ocr-tabs>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Example with a disabled tab that cannot be selected.',
      },
    },
  },
};

export const VehicleDetails: Story = {
  render: () => ({
    template: `
      <div style="border: 1px solid #e5e7eb; border-radius: 0.5rem; padding: 1rem;">
        <h2 style="margin: 0 0 1rem 0; font-size: 1.25rem; font-weight: 600;">BMW X5 - M-AB 1234</h2>
        <ocr-tabs>
          <ocr-tab tabId="specs" label="Spezifikationen">
            <ng-template>
              <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 1rem; padding: 1rem 0;">
                <div>
                  <span style="font-size: 0.875rem; color: #6b7280;">Marke</span>
                  <p style="margin: 0; font-weight: 500;">BMW</p>
                </div>
                <div>
                  <span style="font-size: 0.875rem; color: #6b7280;">Modell</span>
                  <p style="margin: 0; font-weight: 500;">X5</p>
                </div>
                <div>
                  <span style="font-size: 0.875rem; color: #6b7280;">Baujahr</span>
                  <p style="margin: 0; font-weight: 500;">2024</p>
                </div>
                <div>
                  <span style="font-size: 0.875rem; color: #6b7280;">Getriebe</span>
                  <p style="margin: 0; font-weight: 500;">Automatik</p>
                </div>
              </div>
            </ng-template>
          </ocr-tab>
          <ocr-tab tabId="bookings" label="Buchungen" [badge]="5">
            <ng-template>
              <div style="padding: 1rem 0;">
                <p style="color: #6b7280;">5 aktive Buchungen für dieses Fahrzeug.</p>
              </div>
            </ng-template>
          </ocr-tab>
          <ocr-tab tabId="maintenance" label="Wartung">
            <ng-template>
              <div style="padding: 1rem 0;">
                <p style="color: #6b7280;">Letzte Wartung: 15.12.2025</p>
                <p style="color: #6b7280;">Nächste Wartung: 15.03.2026</p>
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
        story: 'Example of tabs used in a vehicle details view.',
      },
    },
  },
};
