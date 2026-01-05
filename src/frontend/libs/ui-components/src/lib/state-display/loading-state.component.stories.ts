import type { Meta, StoryObj } from '@storybook/angular';
import { LoadingStateComponent } from './loading-state.component';

const meta: Meta<LoadingStateComponent> = {
  title: 'Components/Feedback/LoadingState',
  component: LoadingStateComponent,
  tags: ['autodocs'],
  argTypes: {
    message: {
      control: 'text',
      description: 'Loading message to display below the spinner',
    },
    size: {
      control: 'select',
      options: ['sm', 'md', 'lg'],
      description: 'Size of the loading spinner',
    },
  },
  parameters: {
    docs: {
      description: {
        component: 'Displays a spinner with customizable loading message. Uses the Orange brand color for the spinner accent.',
      },
    },
  },
};

export default meta;
type Story = StoryObj<LoadingStateComponent>;

export const Default: Story = {
  args: {
    message: 'Laden...',
    size: 'md',
  },
};

export const Small: Story = {
  args: {
    message: 'Laden...',
    size: 'sm',
  },
};

export const Large: Story = {
  args: {
    message: 'Laden...',
    size: 'lg',
  },
};

export const CustomMessage: Story = {
  args: {
    message: 'Lade Fahrzeuge...',
    size: 'md',
  },
};

export const NoMessage: Story = {
  args: {
    message: '',
    size: 'md',
  },
};

export const AllSizes: Story = {
  render: () => ({
    template: `
      <div style="display: flex; gap: 3rem; align-items: flex-start;">
        <div style="text-align: center;">
          <ui-loading-state message="Small" size="sm"></ui-loading-state>
        </div>
        <div style="text-align: center;">
          <ui-loading-state message="Medium" size="md"></ui-loading-state>
        </div>
        <div style="text-align: center;">
          <ui-loading-state message="Large" size="lg"></ui-loading-state>
        </div>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Comparison of all spinner sizes: small (1.5rem), medium (2.5rem), and large (3.5rem).',
      },
    },
  },
};

export const InContext: Story = {
  render: () => ({
    template: `
      <div style="border: 1px solid #e5e7eb; border-radius: 0.5rem; padding: 1rem; max-width: 400px;">
        <h3 style="margin: 0 0 1rem 0; font-size: 1rem; font-weight: 600;">Fahrzeuge</h3>
        <ui-loading-state message="Lade Fahrzeuge..." size="md"></ui-loading-state>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story: 'Example of the loading state within a card context.',
      },
    },
  },
};
