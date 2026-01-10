import { Component, input, output } from "@angular/core";
import { CommonModule } from "@angular/common";
import { IconComponent } from "../icon";

export type ButtonVariant = "primary" | "secondary" | "danger" | "success" | "ghost";
export type ButtonSize = "sm" | "md" | "lg";

/**
 * Reusable Button Component
 *
 * Provides consistent button styling across the application with
 * multiple variants, sizes, and states.
 *
 * @example
 * <ocr-button variant="primary" (clicked)="onSave()">Save</ocr-button>
 * <ocr-button variant="danger" [loading]="isDeleting()">Delete</ocr-button>
 * <ocr-button variant="secondary" icon="x-mark" iconOnly>Close</ocr-button>
 */
@Component({
  selector: "ocr-button",
  standalone: true,
  imports: [CommonModule, IconComponent],
  template: `
    <button
      [type]="type()"
      [disabled]="disabled() || loading()"
      [class]="buttonClasses()"
      (click)="onClick($event)"
    >
      @if (loading()) {
        <span class="btn-spinner"></span>
      } @else if (icon() && iconPosition() === 'left') {
        <lib-icon [name]="icon()!" variant="outline" [size]="iconSize()" />
      }

      @if (!iconOnly()) {
        <span class="btn-text">
          <ng-content></ng-content>
        </span>
      }

      @if (!loading() && icon() && iconPosition() === 'right') {
        <lib-icon [name]="icon()!" variant="outline" [size]="iconSize()" />
      }
    </button>
  `,
  styles: [
    `
      :host {
        display: inline-block;
      }

      :host(.block) {
        display: block;
        width: 100%;
      }

      button {
        display: inline-flex;
        align-items: center;
        justify-content: center;
        gap: 0.5rem;
        font-weight: 500;
        border-radius: 0.375rem;
        border: 1px solid transparent;
        cursor: pointer;
        transition: all 0.15s ease;
        white-space: nowrap;
      }

      button:disabled {
        opacity: 0.6;
        cursor: not-allowed;
      }

      /* Size variants */
      .btn-sm {
        padding: 0.375rem 0.75rem;
        font-size: 0.75rem;
      }

      .btn-md {
        padding: 0.625rem 1rem;
        font-size: 0.875rem;
      }

      .btn-lg {
        padding: 0.75rem 1.5rem;
        font-size: 1rem;
      }

      /* Icon-only buttons */
      .btn-icon-only.btn-sm {
        padding: 0.375rem;
      }

      .btn-icon-only.btn-md {
        padding: 0.625rem;
      }

      .btn-icon-only.btn-lg {
        padding: 0.75rem;
      }

      /* Primary variant */
      .btn-primary {
        background-color: #f97316;
        color: white;
        border-color: #f97316;
      }

      .btn-primary:hover:not(:disabled) {
        background-color: #ea580c;
        border-color: #ea580c;
      }

      .btn-primary:focus {
        box-shadow: 0 0 0 3px rgba(249, 115, 22, 0.3);
      }

      /* Secondary variant */
      .btn-secondary {
        background-color: white;
        color: #374151;
        border-color: #d1d5db;
      }

      .btn-secondary:hover:not(:disabled) {
        background-color: #f9fafb;
        border-color: #9ca3af;
      }

      .btn-secondary:focus {
        box-shadow: 0 0 0 3px rgba(107, 114, 128, 0.2);
      }

      /* Danger variant */
      .btn-danger {
        background-color: #ef4444;
        color: white;
        border-color: #ef4444;
      }

      .btn-danger:hover:not(:disabled) {
        background-color: #dc2626;
        border-color: #dc2626;
      }

      .btn-danger:focus {
        box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.3);
      }

      /* Success variant */
      .btn-success {
        background-color: #22c55e;
        color: white;
        border-color: #22c55e;
      }

      .btn-success:hover:not(:disabled) {
        background-color: #16a34a;
        border-color: #16a34a;
      }

      .btn-success:focus {
        box-shadow: 0 0 0 3px rgba(34, 197, 94, 0.3);
      }

      /* Ghost variant */
      .btn-ghost {
        background-color: transparent;
        color: #374151;
        border-color: transparent;
      }

      .btn-ghost:hover:not(:disabled) {
        background-color: #f3f4f6;
      }

      .btn-ghost:focus {
        box-shadow: 0 0 0 3px rgba(107, 114, 128, 0.2);
      }

      /* Block width */
      .btn-block {
        width: 100%;
      }

      /* Loading spinner */
      .btn-spinner {
        width: 1rem;
        height: 1rem;
        border: 2px solid currentColor;
        border-right-color: transparent;
        border-radius: 50%;
        animation: spin 0.75s linear infinite;
      }

      @keyframes spin {
        to {
          transform: rotate(360deg);
        }
      }

      .btn-text {
        display: inline-flex;
        align-items: center;
      }
    `,
  ],
})
export class ButtonComponent {
  /** Button type attribute */
  readonly type = input<"button" | "submit" | "reset">("button");

  /** Visual variant */
  readonly variant = input<ButtonVariant>("primary");

  /** Button size */
  readonly size = input<ButtonSize>("md");

  /** Whether button is disabled */
  readonly disabled = input(false);

  /** Whether button is in loading state */
  readonly loading = input(false);

  /** Whether button should take full width */
  readonly block = input(false);

  /** Icon name to display */
  readonly icon = input<string>();

  /** Icon position */
  readonly iconPosition = input<"left" | "right">("left");

  /** Whether this is an icon-only button */
  readonly iconOnly = input(false);

  /** Click event */
  readonly clicked = output<MouseEvent>();

  /** Computed button classes */
  buttonClasses(): string {
    const classes = [
      `btn-${this.variant()}`,
      `btn-${this.size()}`,
    ];

    if (this.block()) {
      classes.push("btn-block");
    }

    if (this.iconOnly()) {
      classes.push("btn-icon-only");
    }

    return classes.join(" ");
  }

  /** Computed icon size based on button size */
  iconSize(): "xs" | "sm" | "md" {
    const size = this.size();
    if (size === "sm") return "xs";
    if (size === "lg") return "md";
    return "sm";
  }

  onClick(event: MouseEvent): void {
    if (!this.disabled() && !this.loading()) {
      this.clicked.emit(event);
    }
  }
}
