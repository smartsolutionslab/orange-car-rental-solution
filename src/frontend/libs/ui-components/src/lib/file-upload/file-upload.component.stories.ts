import type { Meta, StoryObj } from '@storybook/angular';
import { FileUploadComponent } from './file-upload.component';

const meta: Meta<FileUploadComponent> = {
  title: 'Forms/File Upload',
  component: FileUploadComponent,
  parameters: {
    layout: 'centered',
    docs: {
      description: {
        component: `
A drag-and-drop file upload component with file preview, multiple file support,
and file type/size validation.

## Features
- Drag and drop support
- Click to select files
- Image preview
- File type validation
- File size validation
- Multiple file support
- File list with remove option

## Usage

\`\`\`html
<!-- Basic upload -->
<ocr-file-upload
  [label]="'Upload files'"
  (filesChange)="onFilesSelected($event)"
/>

<!-- With validation -->
<ocr-file-upload
  [label]="'Upload documents'"
  [accept]="'.pdf,.doc,.docx'"
  [maxSize]="5242880"
  [multiple]="true"
  (filesChange)="onFilesSelected($event)"
  (fileRejected)="onFileRejected($event)"
/>
\`\`\`
        `,
      },
    },
  },
  tags: ['autodocs'],
  decorators: [
    (story) => ({
      ...story,
      styles: ['div { width: 400px; }'],
    }),
  ],
};

export default meta;
type Story = StoryObj<FileUploadComponent>;

/**
 * Default file upload
 */
export const Default: Story = {
  args: {
    label: 'Upload files',
  },
};

/**
 * With accepted file types
 */
export const WithAcceptedTypes: Story = {
  args: {
    label: 'Upload documents',
    accept: '.pdf,.doc,.docx',
    hint: 'Only PDF and Word documents are accepted',
  },
};

/**
 * Images only
 */
export const ImagesOnly: Story = {
  args: {
    label: 'Upload images',
    accept: 'image/*',
    hint: 'Drag and drop images here',
  },
};

/**
 * With size limit
 */
export const WithSizeLimit: Story = {
  args: {
    label: 'Upload file',
    maxSize: 5242880, // 5MB
    hint: 'Maximum file size is 5MB',
  },
};

/**
 * Multiple files
 */
export const MultipleFiles: Story = {
  args: {
    label: 'Upload documents',
    accept: '.pdf,.doc,.docx',
    maxSize: 10485760, // 10MB
    multiple: true,
  },
};

/**
 * Required field
 */
export const Required: Story = {
  args: {
    label: 'Required upload',
    required: true,
    accept: 'image/*',
  },
};

/**
 * With error
 */
export const WithError: Story = {
  args: {
    label: 'Upload file',
    error: 'Please upload at least one file',
  },
};

/**
 * With hint
 */
export const WithHint: Story = {
  args: {
    label: 'Profile picture',
    accept: 'image/jpeg,image/png',
    maxSize: 2097152, // 2MB
    hint: 'Upload a JPG or PNG image (max 2MB)',
  },
};

/**
 * Disabled state
 */
export const Disabled: Story = {
  args: {
    label: 'Upload file',
    disabled: true,
  },
};

/**
 * Without label
 */
export const WithoutLabel: Story = {
  args: {
    accept: 'image/*',
    hint: 'Drop images here or click to browse',
  },
};

/**
 * Driver's license upload example
 */
export const DriversLicenseUpload: Story = {
  args: {
    label: "Driver's License",
    accept: 'image/jpeg,image/png,.pdf',
    maxSize: 5242880,
    required: true,
    hint: 'Upload a clear photo or scan of your driver\'s license',
  },
};

/**
 * Vehicle photos example
 */
export const VehiclePhotos: Story = {
  args: {
    label: 'Vehicle Photos',
    accept: 'image/*',
    maxSize: 10485760,
    multiple: true,
    hint: 'Upload photos of vehicle damage (if any)',
  },
};
