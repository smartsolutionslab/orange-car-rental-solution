import {
  Component,
  input,
  output,
  signal,
  computed,
  ElementRef,
  viewChild,
} from "@angular/core";
import { CommonModule } from "@angular/common";
import { IconComponent } from "../icon";

export interface UploadedFile {
  file: File;
  name: string;
  size: number;
  type: string;
  preview?: string;
}

/**
 * File Upload Component
 *
 * A drag-and-drop file upload component with file preview,
 * multiple file support, and file type/size validation.
 *
 * @example
 * <ocr-file-upload
 *   [label]="'Upload documents'"
 *   [accept]="'.pdf,.doc,.docx'"
 *   [maxSize]="5242880"
 *   [multiple]="true"
 *   (filesChange)="onFilesSelected($event)"
 * />
 */
@Component({
  selector: "ocr-file-upload",
  standalone: true,
  imports: [CommonModule, IconComponent],
  template: `
    <div class="file-upload-wrapper" [class.disabled]="disabled()">
      @if (label()) {
        <label class="file-upload-label">
          {{ label() }}
          @if (required()) {
            <span class="required-marker">*</span>
          }
        </label>
      }

      <div
        class="dropzone"
        [class.dragging]="isDragging()"
        [class.has-error]="error()"
        [class.has-files]="files().length > 0"
        (dragover)="onDragOver($event)"
        (dragleave)="onDragLeave($event)"
        (drop)="onDrop($event)"
        (click)="openFileDialog()"
      >
        <input
          #fileInput
          type="file"
          class="file-input"
          [accept]="accept()"
          [multiple]="multiple()"
          [disabled]="disabled()"
          (change)="onFileSelected($event)"
        />

        <div class="dropzone-content">
          <div class="dropzone-icon">
            <lib-icon name="upload-cloud" variant="outline" size="lg" />
          </div>
          <div class="dropzone-text">
            <span class="dropzone-primary">
              @if (isDragging()) {
                Drop files here
              } @else {
                <span class="dropzone-link">Click to upload</span> or drag and
                drop
              }
            </span>
            <span class="dropzone-secondary">
              @if (accept()) {
                {{ acceptedTypesText() }}
              }
              @if (maxSize()) {
                (max {{ formatSize(maxSize()!) }})
              }
            </span>
          </div>
        </div>
      </div>

      @if (files().length > 0) {
        <ul class="file-list">
          @for (file of files(); track file.name; let i = $index) {
            <li class="file-item">
              @if (file.preview) {
                <img
                  [src]="file.preview"
                  [alt]="file.name"
                  class="file-preview"
                />
              } @else {
                <div class="file-icon">
                  <lib-icon name="file" variant="outline" size="sm" />
                </div>
              }
              <div class="file-info">
                <span class="file-name">{{ file.name }}</span>
                <span class="file-size">{{ formatSize(file.size) }}</span>
              </div>
              <button
                type="button"
                class="file-remove"
                (click)="removeFile(i, $event)"
                aria-label="Remove file"
              >
                <lib-icon name="x" variant="outline" size="sm" />
              </button>
            </li>
          }
        </ul>
      }

      @if (error()) {
        <span class="upload-error">{{ error() }}</span>
      } @else if (hint()) {
        <span class="upload-hint">{{ hint() }}</span>
      }
    </div>
  `,
  styles: [
    `
      .file-upload-wrapper {
        display: flex;
        flex-direction: column;
        gap: 0.5rem;
        width: 100%;
      }

      .file-upload-wrapper.disabled {
        opacity: 0.6;
        pointer-events: none;
      }

      .file-upload-label {
        font-size: 0.875rem;
        font-weight: 500;
        color: #374151;
      }

      .required-marker {
        color: #ef4444;
        margin-left: 0.125rem;
      }

      .dropzone {
        position: relative;
        display: flex;
        flex-direction: column;
        align-items: center;
        justify-content: center;
        min-height: 10rem;
        padding: 2rem;
        border: 2px dashed #d1d5db;
        border-radius: 0.5rem;
        background-color: #f9fafb;
        cursor: pointer;
        transition: all 0.15s ease;
      }

      .dropzone:hover {
        border-color: #f97316;
        background-color: #fff7ed;
      }

      .dropzone.dragging {
        border-color: #f97316;
        background-color: #fff7ed;
        border-style: solid;
      }

      .dropzone.has-error {
        border-color: #ef4444;
        background-color: #fef2f2;
      }

      .file-input {
        position: absolute;
        width: 0;
        height: 0;
        opacity: 0;
        overflow: hidden;
      }

      .dropzone-content {
        display: flex;
        flex-direction: column;
        align-items: center;
        gap: 0.75rem;
        text-align: center;
      }

      .dropzone-icon {
        color: #9ca3af;
      }

      .dropzone.dragging .dropzone-icon,
      .dropzone:hover .dropzone-icon {
        color: #f97316;
      }

      .dropzone-text {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
      }

      .dropzone-primary {
        font-size: 0.875rem;
        color: #374151;
      }

      .dropzone-link {
        color: #f97316;
        font-weight: 500;
        text-decoration: underline;
      }

      .dropzone-secondary {
        font-size: 0.75rem;
        color: #6b7280;
      }

      /* File list */
      .file-list {
        list-style: none;
        margin: 0;
        padding: 0;
        display: flex;
        flex-direction: column;
        gap: 0.5rem;
      }

      .file-item {
        display: flex;
        align-items: center;
        gap: 0.75rem;
        padding: 0.75rem;
        background-color: #f9fafb;
        border: 1px solid #e5e7eb;
        border-radius: 0.375rem;
      }

      .file-preview {
        width: 2.5rem;
        height: 2.5rem;
        object-fit: cover;
        border-radius: 0.25rem;
      }

      .file-icon {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 2.5rem;
        height: 2.5rem;
        background-color: #e5e7eb;
        border-radius: 0.25rem;
        color: #6b7280;
      }

      .file-info {
        flex: 1;
        min-width: 0;
        display: flex;
        flex-direction: column;
        gap: 0.125rem;
      }

      .file-name {
        font-size: 0.875rem;
        font-weight: 500;
        color: #374151;
        white-space: nowrap;
        overflow: hidden;
        text-overflow: ellipsis;
      }

      .file-size {
        font-size: 0.75rem;
        color: #6b7280;
      }

      .file-remove {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 1.75rem;
        height: 1.75rem;
        padding: 0;
        background: none;
        border: none;
        border-radius: 0.25rem;
        color: #6b7280;
        cursor: pointer;
        transition: all 0.15s ease;
      }

      .file-remove:hover {
        background-color: #fee2e2;
        color: #ef4444;
      }

      /* Error and hint */
      .upload-error {
        font-size: 0.75rem;
        color: #ef4444;
      }

      .upload-hint {
        font-size: 0.75rem;
        color: #6b7280;
      }
    `,
  ],
})
export class FileUploadComponent {
  /** Component label */
  readonly label = input<string>();

  /** Accepted file types (e.g., '.pdf,.doc' or 'image/*') */
  readonly accept = input<string>();

  /** Maximum file size in bytes */
  readonly maxSize = input<number>();

  /** Allow multiple files */
  readonly multiple = input(false);

  /** Whether component is required */
  readonly required = input(false);

  /** Whether component is disabled */
  readonly disabled = input(false);

  /** Error message */
  readonly error = input<string | null>(null);

  /** Hint text */
  readonly hint = input<string>();

  /** Files change event */
  readonly filesChange = output<UploadedFile[]>();

  /** File rejected event (validation failed) */
  readonly fileRejected = output<{ file: File; reason: string }>();

  /** Reference to file input element */
  private readonly fileInput =
    viewChild<ElementRef<HTMLInputElement>>("fileInput");

  /** Uploaded files */
  readonly files = signal<UploadedFile[]>([]);

  /** Drag state */
  readonly isDragging = signal(false);

  /** Computed accepted types text */
  readonly acceptedTypesText = computed(() => {
    const accept = this.accept();
    if (!accept) return "";

    const types = accept.split(",").map((t) => t.trim());
    return types.join(", ").toUpperCase();
  });

  openFileDialog(): void {
    this.fileInput()?.nativeElement.click();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files) {
      this.processFiles(Array.from(input.files));
    }
    // Reset input so same file can be selected again
    input.value = "";
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    if (!this.disabled()) {
      this.isDragging.set(true);
    }
  }

  onDragLeave(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(false);
  }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    event.stopPropagation();
    this.isDragging.set(false);

    if (this.disabled()) return;

    const files = event.dataTransfer?.files;
    if (files) {
      this.processFiles(Array.from(files));
    }
  }

  removeFile(index: number, event: Event): void {
    event.stopPropagation();
    const currentFiles = this.files();
    const file = currentFiles[index];

    // Revoke preview URL if exists
    if (file.preview) {
      URL.revokeObjectURL(file.preview);
    }

    const newFiles = currentFiles.filter((_, i) => i !== index);
    this.files.set(newFiles);
    this.filesChange.emit(newFiles);
  }

  private processFiles(fileList: File[]): void {
    const validFiles: UploadedFile[] = [];

    for (const file of fileList) {
      // Validate file type
      if (this.accept() && !this.isValidType(file)) {
        this.fileRejected.emit({ file, reason: "Invalid file type" });
        continue;
      }

      // Validate file size
      if (this.maxSize() && file.size > this.maxSize()!) {
        this.fileRejected.emit({
          file,
          reason: `File exceeds maximum size of ${this.formatSize(this.maxSize()!)}`,
        });
        continue;
      }

      const uploadedFile: UploadedFile = {
        file,
        name: file.name,
        size: file.size,
        type: file.type,
      };

      // Create preview for images
      if (file.type.startsWith("image/")) {
        uploadedFile.preview = URL.createObjectURL(file);
      }

      validFiles.push(uploadedFile);
    }

    if (validFiles.length > 0) {
      const currentFiles = this.multiple() ? this.files() : [];
      const newFiles = [...currentFiles, ...validFiles];
      this.files.set(newFiles);
      this.filesChange.emit(newFiles);
    }
  }

  private isValidType(file: File): boolean {
    const accept = this.accept();
    if (!accept) return true;

    const types = accept.split(",").map((t) => t.trim().toLowerCase());

    for (const type of types) {
      // Check for wildcard types like 'image/*'
      if (type.endsWith("/*")) {
        const prefix = type.slice(0, -2);
        if (file.type.toLowerCase().startsWith(prefix)) {
          return true;
        }
      }
      // Check for extension like '.pdf'
      else if (type.startsWith(".")) {
        if (file.name.toLowerCase().endsWith(type)) {
          return true;
        }
      }
      // Check for MIME type
      else if (file.type.toLowerCase() === type) {
        return true;
      }
    }

    return false;
  }

  formatSize(bytes: number): string {
    if (bytes === 0) return "0 B";
    const k = 1024;
    const sizes = ["B", "KB", "MB", "GB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(1)) + " " + sizes[i];
  }

  /** Clear all files */
  clear(): void {
    // Revoke all preview URLs
    for (const file of this.files()) {
      if (file.preview) {
        URL.revokeObjectURL(file.preview);
      }
    }
    this.files.set([]);
    this.filesChange.emit([]);
  }
}
