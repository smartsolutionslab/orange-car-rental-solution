import type { Meta, StoryObj } from '@storybook/angular';
import { moduleMetadata, applicationConfig } from '@storybook/angular';
import { provideRouter } from '@angular/router';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { NavigationComponent } from './navigation.component';
import type { NavItem } from '@orange-car-rental/shared';
import { ICON_CAR, ICON_CALENDAR, ICON_LOCATION, ICON_EMAIL, ICON_PERSON } from '@orange-car-rental/util';

const mockNavItems: NavItem[] = [
  { path: '/', label: 'Fahrzeuge', icon: ICON_CAR, exactMatch: true },
  { path: '/bookings', label: 'Buchungen', icon: ICON_CALENDAR },
  { path: '/locations', label: 'Standorte', icon: ICON_LOCATION },
  { path: '/contact', label: 'Kontakt', icon: ICON_EMAIL },
];

const mockCallCenterNavItems: NavItem[] = [
  { path: '/', label: 'Fahrzeuge', icon: ICON_CAR, exactMatch: true },
  { path: '/reservations', label: 'Reservierungen', icon: ICON_CALENDAR },
  { path: '/customers', label: 'Kunden', icon: ICON_PERSON },
  { path: '/locations', label: 'Standorte', icon: ICON_LOCATION },
];

const meta: Meta<NavigationComponent> = {
  title: 'Components/Navigation/Navigation',
  component: NavigationComponent,
  tags: ['autodocs'],
  decorators: [
    applicationConfig({
      providers: [provideRouter([])],
    }),
    moduleMetadata({
      imports: [TranslateModule.forRoot()],
      providers: [
        {
          provide: TranslateService,
          useValue: {
            instant: (key: string) => {
              const translations: Record<string, string> = {
                'common.actions.login': 'Anmelden',
                'common.actions.logout': 'Abmelden',
              };
              return translations[key] || key;
            },
          },
        },
      ],
    }),
  ],
  argTypes: {
    title: {
      control: 'text',
      description: 'Application title',
    },
    isAuthenticated: {
      control: 'boolean',
      description: 'Whether user is authenticated',
    },
    username: {
      control: 'text',
      description: 'Username to display',
    },
  },
  parameters: {
    docs: {
      description: {
        component: `
A shared navigation component used across all Orange Car Rental portals.

## Features
- Consistent branding with customizable title
- Dynamic navigation items with icon support
- Authentication state handling (login/logout buttons)
- Language switcher integration
- Responsive layout for mobile devices
- Active link highlighting

## Usage
\`\`\`typescript
// In your component
protected readonly navItems = computed(() => {
  return this.allNavItems.filter(item => {
    if (item.requiresAuth && !this.isAuthenticated()) return false;
    return true;
  });
});
\`\`\`

\`\`\`html
<ocr-navigation
  [title]="'Orange Car Rental'"
  [navItems]="navItems()"
  [isAuthenticated]="isAuthenticated()"
  [username]="username()"
  (loginClick)="login()"
  (logoutClick)="logout()"
/>
\`\`\`
        `,
      },
    },
  },
};

export default meta;
type Story = StoryObj<NavigationComponent>;

export const Default: Story = {
  args: {
    title: 'Orange Car Rental',
    navItems: mockNavItems,
    isAuthenticated: false,
    username: '',
  },
};

export const Authenticated: Story = {
  args: {
    title: 'Orange Car Rental',
    navItems: mockNavItems,
    isAuthenticated: true,
    username: 'max.mustermann',
  },
  parameters: {
    docs: {
      description: {
        story: 'Navigation showing authenticated state with username and logout button.',
      },
    },
  },
};

export const CallCenterPortal: Story = {
  args: {
    title: 'Orange Car Rental - Call Center',
    navItems: mockCallCenterNavItems,
    isAuthenticated: true,
    username: 'agent.smith',
  },
  parameters: {
    docs: {
      description: {
        story: 'Navigation configured for the call center portal with different menu items.',
      },
    },
  },
};

export const MinimalNav: Story = {
  args: {
    title: 'Orange Car Rental',
    navItems: [
      { path: '/', label: 'Start', icon: ICON_CAR, exactMatch: true },
      { path: '/contact', label: 'Kontakt', icon: ICON_EMAIL },
    ],
    isAuthenticated: false,
    username: '',
  },
  parameters: {
    docs: {
      description: {
        story: 'Minimal navigation with only essential items.',
      },
    },
  },
};
