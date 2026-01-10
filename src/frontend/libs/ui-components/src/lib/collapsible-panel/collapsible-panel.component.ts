import {
  Component,
  input,
  output,
  signal,
  computed,
  contentChild,
  TemplateRef,
} from "@angular/core";
import { CommonModule, NgTemplateOutlet } from "@angular/common";

/**
 * Event emitted when the panel's expanded state changes
 */
export interface CollapsiblePanelChangeEvent {
  /** Whether the panel is now expanded */
  expanded: boolean;
}

/**
 * Collapsible Panel Component
 *
 * A single collapsible content panel with a customizable header.
 * Supports smooth animations and custom trigger elements.
 *
 * @example
 * // Basic usage
 * <ocr-collapsible-panel
 *   title="Advanced Settings"
 *   [expanded]="false"
 *   (expandedChange)="onToggle($event)"
 * >
 *   <p>Panel content goes here...</p>
 * </ocr-collapsible-panel>
 *
 * @example
 * // With custom header
 * <ocr-collapsible-panel [expanded]="true">
 *   <ng-template #headerTemplate>
 *     <div class="custom-header">
 *       <span>Custom Header</span>
 *       <span class="badge">3 items</span>
 *     </div>
 *   </ng-template>
 *   <p>Panel content...</p>
 * </ocr-collapsible-panel>
 */
@Component({
  selector: "ocr-collapsible-panel",
  standalone: true,
  imports: [CommonModule, NgTemplateOutlet],
  template: `
    <div
      class="collapsible-panel"
      [class.collapsible-panel--expanded]="isExpanded()"
      [class.collapsible-panel--bordered]="bordered()"
    >
      <button
        type="button"
        class="collapsible-panel__header"
        [id]="headerId()"
        [attr.aria-expanded]="isExpanded()"
        [attr.aria-controls]="panelId()"
        [disabled]="disabled()"
        (click)="toggle()"
      >
        @if (headerTemplate()) {
          <ng-container *ngTemplateOutlet="headerTemplate()!"></ng-container>
        } @else {
          <div class="collapsible-panel__header-content">
            <span class="collapsible-panel__title">{{ title() }}</span>
            @if (subtitle()) {
              <span class="collapsible-panel__subtitle">{{ subtitle() }}</span>
            }
          </div>
        }
        @if (showIcon()) {
          <span
            class="collapsible-panel__icon"
            [class.collapsible-panel__icon--expanded]="isExpanded()"
          >
            <svg viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
              <path
                d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6 1.41-1.41z"
              />
            </svg>
          </span>
        }
      </button>
      <div
        class="collapsible-panel__content"
        [id]="panelId()"
        role="region"
        [attr.aria-labelledby]="headerId()"
        [class.collapsible-panel__content--expanded]="isExpanded()"
        [attr.hidden]="!isExpanded() ? true : null"
      >
        <div class="collapsible-panel__content-inner">
          @if (isExpanded() || hasOpened()) {
            <ng-content></ng-content>
          }
        </div>
      </div>
    </div>
  `,
  styles: [
    `
      .collapsible-panel {
        background: white;
        border-radius: 0.5rem;
      }

      .collapsible-panel--bordered {
        border: 1px solid #e5e7eb;
      }

      .collapsible-panel__header {
        display: flex;
        align-items: center;
        justify-content: space-between;
        width: 100%;
        padding: 1rem;
        background: transparent;
        border: none;
        cursor: pointer;
        text-align: left;
        transition: background-color 0.15s ease;
        border-radius: 0.5rem;
      }

      .collapsible-panel--bordered .collapsible-panel__header {
        border-radius: 0.5rem 0.5rem 0 0;
      }

      .collapsible-panel--bordered.collapsible-panel--expanded
        .collapsible-panel__header {
        border-bottom: 1px solid #e5e7eb;
      }

      .collapsible-panel__header:hover:not(:disabled) {
        background-color: #f9fafb;
      }

      .collapsible-panel__header:focus-visible {
        outline: 2px solid #f97316;
        outline-offset: -2px;
      }

      .collapsible-panel__header:disabled {
        cursor: not-allowed;
        opacity: 0.5;
      }

      .collapsible-panel__header-content {
        display: flex;
        flex-direction: column;
        gap: 0.25rem;
        flex: 1;
        min-width: 0;
      }

      .collapsible-panel__title {
        font-size: 0.9375rem;
        font-weight: 600;
        color: #111827;
      }

      .collapsible-panel__subtitle {
        font-size: 0.8125rem;
        color: #6b7280;
      }

      .collapsible-panel__icon {
        display: flex;
        align-items: center;
        justify-content: center;
        width: 1.5rem;
        height: 1.5rem;
        color: #6b7280;
        transition: transform 0.2s ease;
        flex-shrink: 0;
        margin-left: 0.75rem;
      }

      .collapsible-panel__icon svg {
        width: 1.25rem;
        height: 1.25rem;
      }

      .collapsible-panel__icon--expanded {
        transform: rotate(180deg);
      }

      .collapsible-panel__content {
        display: grid;
        grid-template-rows: 0fr;
        transition: grid-template-rows 0.2s ease;
      }

      .collapsible-panel__content--expanded {
        grid-template-rows: 1fr;
      }

      .collapsible-panel__content-inner {
        overflow: hidden;
      }

      .collapsible-panel__content--expanded .collapsible-panel__content-inner {
        padding: 1rem;
      }

      .collapsible-panel--bordered
        .collapsible-panel__content--expanded
        .collapsible-panel__content-inner {
        padding: 1rem;
      }
    `,
  ],
})
export class CollapsiblePanelComponent {
  /**
   * Unique identifier for the panel
   */
  readonly panelId = input<string>(`collapsible-panel-${Math.random().toString(36).slice(2, 9)}`);

  /**
   * Panel title (used if no headerTemplate is provided)
   */
  readonly title = input<string>("");

  /**
   * Optional subtitle text
   */
  readonly subtitle = input<string>("");

  /**
   * Initial expanded state
   */
  readonly expanded = input(false);

  /**
   * Whether the panel is disabled
   */
  readonly disabled = input(false);

  /**
   * Show border around the panel
   */
  readonly bordered = input(true);

  /**
   * Show the expand/collapse icon
   */
  readonly showIcon = input(true);

  /**
   * Keep content in DOM after collapse (for performance)
   */
  readonly keepAlive = input(true);

  /**
   * Emitted when expanded state changes
   */
  readonly expandedChange = output<CollapsiblePanelChangeEvent>();

  /**
   * Custom header template
   */
  readonly headerTemplate = contentChild<TemplateRef<unknown>>("headerTemplate");

  /**
   * Internal expanded state
   */
  readonly isExpanded = signal(false);

  /**
   * Track if panel has been opened at least once (for lazy content loading)
   */
  readonly hasOpened = signal(false);

  /**
   * Computed header ID
   */
  readonly headerId = computed(() => `${this.panelId()}-header`);

  constructor() {
    // Sync with initial expanded input
    const initialExpanded = this.expanded();
    if (initialExpanded) {
      this.isExpanded.set(true);
      this.hasOpened.set(true);
    }
  }

  /**
   * Toggle the panel's expanded state
   */
  toggle(): void {
    if (this.disabled()) return;

    const newExpanded = !this.isExpanded();
    this.isExpanded.set(newExpanded);

    if (newExpanded) this.hasOpened.set(true);

    this.expandedChange.emit({ expanded: newExpanded });
  }

  /**
   * Expand the panel
   */
  expand(): void {
    if (!this.isExpanded() && !this.disabled()) this.toggle();
  }

  /**
   * Collapse the panel
   */
  collapse(): void {
    if (this.isExpanded()) this.toggle();
  }
}
