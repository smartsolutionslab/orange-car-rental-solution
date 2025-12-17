import { Component, input, output, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';

/**
 * Modal Wrapper Component
 * A reusable modal dialog with overlay, header, body, and footer
 *
 * @example
 * <ui-modal [isOpen]="showModal()" [title]="'Edit Vehicle'" (close)="closeModal()">
 *   <ng-container modal-body>
 *     <p>Modal content here</p>
 *   </ng-container>
 *   <ng-container modal-footer>
 *     <button (click)="save()">Save</button>
 *   </ng-container>
 * </ui-modal>
 */
@Component({
  selector: 'ui-modal',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (isOpen()) {
      <div
        class="modal-overlay"
        (click)="onOverlayClick()"
        role="dialog"
        aria-modal="true"
        [attr.aria-labelledby]="titleId"
      >
        <div
          class="modal-content"
          [class.modal-sm]="size() === 'sm'"
          [class.modal-md]="size() === 'md'"
          [class.modal-lg]="size() === 'lg'"
          [class.modal-xl]="size() === 'xl'"
          (click)="$event.stopPropagation()"
          (keydown)="$event.stopPropagation()"
          role="document"
        >
          <div class="modal-header">
            <h2 [id]="titleId">{{ title() }}</h2>
            <button class="btn-close" (click)="onClose()" type="button" aria-label="Schließen">
              <svg viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
                <path d="M19 6.41L17.59 5 12 10.59 6.41 5 5 6.41 10.59 12 5 17.59 6.41 19 12 13.41 17.59 19 19 17.59 13.41 12z"/>
              </svg>
            </button>
          </div>
          <div class="modal-body">
            <ng-content select="[modal-body]"></ng-content>
          </div>
          @if (showFooter()) {
            <div class="modal-footer">
              <ng-content select="[modal-footer]"></ng-content>
            </div>
          }
        </div>
      </div>
    }
  `,
  styles: [`
    .modal-overlay {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      background-color: rgba(0, 0, 0, 0.5);
      display: flex;
      align-items: center;
      justify-content: center;
      z-index: 1000;
      padding: 1rem;
      animation: fadeIn 0.2s ease-out;
    }

    @keyframes fadeIn {
      from {
        opacity: 0;
      }
      to {
        opacity: 1;
      }
    }

    .modal-content {
      background-color: white;
      border-radius: 0.5rem;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.1), 0 10px 10px -5px rgba(0, 0, 0, 0.04);
      max-height: 90vh;
      overflow: hidden;
      display: flex;
      flex-direction: column;
      animation: slideIn 0.2s ease-out;
    }

    @keyframes slideIn {
      from {
        transform: translateY(-10px);
        opacity: 0;
      }
      to {
        transform: translateY(0);
        opacity: 1;
      }
    }

    .modal-sm {
      width: 100%;
      max-width: 24rem;
    }

    .modal-md {
      width: 100%;
      max-width: 32rem;
    }

    .modal-lg {
      width: 100%;
      max-width: 48rem;
    }

    .modal-xl {
      width: 100%;
      max-width: 64rem;
    }

    .modal-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 1rem 1.5rem;
      border-bottom: 1px solid #e5e7eb;
    }

    .modal-header h2 {
      margin: 0;
      font-size: 1.25rem;
      font-weight: 600;
      color: #111827;
    }

    .btn-close {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 2rem;
      height: 2rem;
      padding: 0;
      background: none;
      border: none;
      border-radius: 0.25rem;
      cursor: pointer;
      color: #6b7280;
      transition: background-color 0.15s ease, color 0.15s ease;
    }

    .btn-close:hover {
      background-color: #f3f4f6;
      color: #111827;
    }

    .btn-close svg {
      width: 1.25rem;
      height: 1.25rem;
    }

    .modal-body {
      padding: 1.5rem;
      overflow-y: auto;
      flex: 1;
    }

    .modal-footer {
      display: flex;
      align-items: center;
      justify-content: flex-end;
      gap: 0.75rem;
      padding: 1rem 1.5rem;
      border-top: 1px solid #e5e7eb;
      background-color: #f9fafb;
    }
  `]
})
export class ModalComponent {
  readonly isOpen = input.required<boolean>();
  readonly title = input.required<string>();
  readonly size = input<'sm' | 'md' | 'lg' | 'xl'>('md');
  readonly showFooter = input(true);
  readonly closeOnEscape = input(true);
  readonly closeOnOverlayClick = input(true);

  readonly close = output<void>();

  protected readonly titleId = `modal-title-${Math.random().toString(36).slice(2, 9)}`;

  @HostListener('document:keydown.escape')
  onEscapeKey(): void {
    if (this.isOpen() && this.closeOnEscape()) {
      this.onClose();
    }
  }

  protected onOverlayClick(): void {
    if (this.closeOnOverlayClick()) {
      this.onClose();
    }
  }

  protected onClose(): void {
    this.close.emit();
  }
}
