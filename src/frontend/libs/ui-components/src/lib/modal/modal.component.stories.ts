import type { Meta, StoryObj } from "@storybook/angular";
import { moduleMetadata } from "@storybook/angular";
import { ModalComponent } from "./modal.component";

const meta: Meta<ModalComponent> = {
  title: "Components/Modal",
  component: ModalComponent,
  tags: ["autodocs"],
  decorators: [
    moduleMetadata({
      imports: [ModalComponent],
    }),
  ],
  argTypes: {
    size: {
      control: "select",
      options: ["sm", "md", "lg", "xl"],
      description: "Size of the modal dialog",
    },
    showFooter: {
      control: "boolean",
      description: "Whether to show the footer section",
    },
    closeOnEscape: {
      control: "boolean",
      description: "Close modal when Escape key is pressed",
    },
    closeOnOverlayClick: {
      control: "boolean",
      description: "Close modal when clicking the overlay",
    },
  },
  parameters: {
    docs: {
      description: {
        component:
          "A reusable modal dialog with overlay, header, body, and footer sections. Supports keyboard navigation and accessible ARIA attributes.",
      },
    },
  },
};

export default meta;
type Story = StoryObj<ModalComponent>;

export const Default: Story = {
  render: (args) => ({
    props: {
      ...args,
      isOpen: true,
      onClose: () => console.log("Modal closed"),
    },
    template: `
      <ui-modal
        [isOpen]="isOpen"
        [title]="title"
        [size]="size"
        [showFooter]="showFooter"
        (close)="onClose()"
      >
        <ng-container modal-body>
          <p>This is the modal body content. You can put any content here.</p>
        </ng-container>
        <ng-container modal-footer>
          <button style="padding: 0.5rem 1rem; background: #e5e7eb; border: none; border-radius: 0.375rem; cursor: pointer;">Cancel</button>
          <button style="padding: 0.5rem 1rem; background: #f97316; color: white; border: none; border-radius: 0.375rem; cursor: pointer;">Save</button>
        </ng-container>
      </ui-modal>
    `,
  }),
  args: {
    title: "Edit Vehicle",
    size: "md",
    showFooter: true,
    closeOnEscape: true,
    closeOnOverlayClick: true,
  },
};

export const SmallModal: Story = {
  render: (args) => ({
    props: {
      ...args,
      isOpen: true,
    },
    template: `
      <ui-modal [isOpen]="isOpen" [title]="title" size="sm" [showFooter]="false">
        <ng-container modal-body>
          <p>A small confirmation modal.</p>
          <p style="margin-top: 1rem;">Are you sure you want to proceed?</p>
        </ng-container>
      </ui-modal>
    `,
  }),
  args: {
    title: "Confirm Action",
  },
};

export const LargeModal: Story = {
  render: (args) => ({
    props: {
      ...args,
      isOpen: true,
    },
    template: `
      <ui-modal [isOpen]="isOpen" [title]="title" size="lg" [showFooter]="true">
        <ng-container modal-body>
          <h3 style="margin: 0 0 1rem 0; font-size: 1rem; font-weight: 600;">Vehicle Details</h3>
          <div style="display: grid; grid-template-columns: repeat(2, 1fr); gap: 1rem;">
            <div>
              <label style="display: block; font-size: 0.875rem; color: #6b7280; margin-bottom: 0.25rem;">Make</label>
              <p style="margin: 0; font-weight: 500;">BMW</p>
            </div>
            <div>
              <label style="display: block; font-size: 0.875rem; color: #6b7280; margin-bottom: 0.25rem;">Model</label>
              <p style="margin: 0; font-weight: 500;">X5</p>
            </div>
            <div>
              <label style="display: block; font-size: 0.875rem; color: #6b7280; margin-bottom: 0.25rem;">Year</label>
              <p style="margin: 0; font-weight: 500;">2024</p>
            </div>
            <div>
              <label style="display: block; font-size: 0.875rem; color: #6b7280; margin-bottom: 0.25rem;">License Plate</label>
              <p style="margin: 0; font-weight: 500;">M-AB 1234</p>
            </div>
          </div>
        </ng-container>
        <ng-container modal-footer>
          <button style="padding: 0.5rem 1rem; background: #e5e7eb; border: none; border-radius: 0.375rem; cursor: pointer;">Close</button>
          <button style="padding: 0.5rem 1rem; background: #f97316; color: white; border: none; border-radius: 0.375rem; cursor: pointer;">Edit</button>
        </ng-container>
      </ui-modal>
    `,
  }),
  args: {
    title: "Vehicle Information",
  },
};

export const WithoutFooter: Story = {
  render: (args) => ({
    props: {
      ...args,
      isOpen: true,
    },
    template: `
      <ui-modal [isOpen]="isOpen" [title]="title" [showFooter]="false">
        <ng-container modal-body>
          <p>This modal has no footer section. Use this variant for informational dialogs.</p>
        </ng-container>
      </ui-modal>
    `,
  }),
  args: {
    title: "Information",
  },
};

export const AllSizes: Story = {
  render: () => ({
    template: `
      <div style="display: flex; flex-direction: column; gap: 2rem;">
        <div>
          <h3 style="margin-bottom: 1rem;">Small (sm) - 24rem max-width</h3>
          <div style="position: relative; height: 200px; background: rgba(0,0,0,0.1); border-radius: 0.5rem; display: flex; align-items: center; justify-content: center;">
            <div style="background: white; border-radius: 0.5rem; box-shadow: 0 4px 6px rgba(0,0,0,0.1); width: 100%; max-width: 24rem; padding: 1rem;">
              <div style="border-bottom: 1px solid #e5e7eb; padding-bottom: 0.5rem; margin-bottom: 0.5rem; font-weight: 600;">Small Modal</div>
              <p style="margin: 0; color: #6b7280;">Content area</p>
            </div>
          </div>
        </div>
        <div>
          <h3 style="margin-bottom: 1rem;">Medium (md) - 32rem max-width (default)</h3>
          <div style="position: relative; height: 200px; background: rgba(0,0,0,0.1); border-radius: 0.5rem; display: flex; align-items: center; justify-content: center;">
            <div style="background: white; border-radius: 0.5rem; box-shadow: 0 4px 6px rgba(0,0,0,0.1); width: 100%; max-width: 32rem; padding: 1rem;">
              <div style="border-bottom: 1px solid #e5e7eb; padding-bottom: 0.5rem; margin-bottom: 0.5rem; font-weight: 600;">Medium Modal</div>
              <p style="margin: 0; color: #6b7280;">Content area</p>
            </div>
          </div>
        </div>
        <div>
          <h3 style="margin-bottom: 1rem;">Large (lg) - 48rem max-width</h3>
          <div style="position: relative; height: 200px; background: rgba(0,0,0,0.1); border-radius: 0.5rem; display: flex; align-items: center; justify-content: center;">
            <div style="background: white; border-radius: 0.5rem; box-shadow: 0 4px 6px rgba(0,0,0,0.1); width: 100%; max-width: 48rem; padding: 1rem;">
              <div style="border-bottom: 1px solid #e5e7eb; padding-bottom: 0.5rem; margin-bottom: 0.5rem; font-weight: 600;">Large Modal</div>
              <p style="margin: 0; color: #6b7280;">Content area</p>
            </div>
          </div>
        </div>
      </div>
    `,
  }),
  parameters: {
    docs: {
      description: {
        story:
          "Visual comparison of all modal sizes. The actual modals are rendered as overlays, these are static representations.",
      },
    },
  },
};
