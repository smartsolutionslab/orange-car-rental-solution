import {
  Component,
  input,
  output,
  signal,
  contentChildren,
  effect,
  AfterContentInit,
} from '@angular/core';
import { CommonModule, NgTemplateOutlet } from '@angular/common';
import { AccordionItemComponent } from './accordion-item.component';

/**
 * Event emitted when accordion item expansion changes
 */
export interface AccordionChangeEvent {
  /** ID of the item that changed */
  itemId: string;
  /** Whether the item is now expanded */
  expanded: boolean;
  /** All currently expanded item IDs */
  expandedIds: string[];
}

/**
 * Accordion Component
 *
 * A collapsible content panel component.
 * Supports single or multiple expansion modes.
 *
 * @example
 * <ocr-accordion [multiple]="false" (itemChange)="onItemChange($event)">
 *   <ocr-accordion-item itemId="item1" title="Section 1">
 *     <ng-template>Content 1</ng-template>
 *   </ocr-accordion-item>
 *   <ocr-accordion-item itemId="item2" title="Section 2">
 *     <ng-template>Content 2</ng-template>
 *   </ocr-accordion-item>
 * </ocr-accordion>
 */
@Component({
  selector: 'ocr-accordion',
  standalone: true,
  imports: [CommonModule, NgTemplateOutlet],
  template: `
    <div class="accordion" role="presentation">
      @for (item of items(); track item.itemId(); let i = $index) {
        <div
          class="accordion__item"
          [class.accordion__item--expanded]="item.isExpanded()"
          [class.accordion__item--disabled]="item.disabled()"
        >
          <button
            class="accordion__header"
            [id]="'accordion-header-' + item.itemId()"
            [attr.aria-expanded]="item.isExpanded()"
            [attr.aria-controls]="'accordion-panel-' + item.itemId()"
            [disabled]="item.disabled()"
            (click)="toggle(item.itemId())"
            (keydown)="onKeydown($event, i)"
          >
            <div class="accordion__header-content">
              <span class="accordion__title">{{ item.title() }}</span>
              @if (item.subtitle()) {
                <span class="accordion__subtitle">{{ item.subtitle() }}</span>
              }
            </div>
            <span class="accordion__icon" [class.accordion__icon--expanded]="item.isExpanded()">
              <svg viewBox="0 0 24 24" fill="currentColor" aria-hidden="true">
                <path d="M7.41 8.59L12 13.17l4.59-4.58L18 10l-6 6-6-6 1.41-1.41z"/>
              </svg>
            </span>
          </button>
          <div
            class="accordion__panel"
            [id]="'accordion-panel-' + item.itemId()"
            role="region"
            [attr.aria-labelledby]="'accordion-header-' + item.itemId()"
            [class.accordion__panel--expanded]="item.isExpanded()"
            [attr.hidden]="!item.isExpanded() ? true : null"
          >
            <div class="accordion__panel-content">
              @if (item.isExpanded() || item.hasOpened()) {
                @if (item.contentTemplate()) {
                  <ng-container *ngTemplateOutlet="item.contentTemplate()!"></ng-container>
                }
              }
            </div>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .accordion {
      border: 1px solid #e5e7eb;
      border-radius: 0.5rem;
      overflow: hidden;
    }

    .accordion__item {
      border-bottom: 1px solid #e5e7eb;
    }

    .accordion__item:last-child {
      border-bottom: none;
    }

    .accordion__header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      width: 100%;
      padding: 1rem;
      background: white;
      border: none;
      cursor: pointer;
      text-align: left;
      transition: background-color 0.15s ease;
    }

    .accordion__header:hover:not(:disabled) {
      background-color: #f9fafb;
    }

    .accordion__header:focus-visible {
      outline: 2px solid #f97316;
      outline-offset: -2px;
    }

    .accordion__header:disabled {
      cursor: not-allowed;
      opacity: 0.5;
    }

    .accordion__item--expanded .accordion__header {
      background-color: #f9fafb;
    }

    .accordion__header-content {
      display: flex;
      flex-direction: column;
      gap: 0.25rem;
    }

    .accordion__title {
      font-size: 0.875rem;
      font-weight: 500;
      color: #111827;
    }

    .accordion__subtitle {
      font-size: 0.75rem;
      color: #6b7280;
    }

    .accordion__icon {
      display: flex;
      align-items: center;
      justify-content: center;
      width: 1.5rem;
      height: 1.5rem;
      color: #6b7280;
      transition: transform 0.2s ease;
      flex-shrink: 0;
    }

    .accordion__icon svg {
      width: 1.25rem;
      height: 1.25rem;
    }

    .accordion__icon--expanded {
      transform: rotate(180deg);
    }

    .accordion__panel {
      display: grid;
      grid-template-rows: 0fr;
      transition: grid-template-rows 0.2s ease;
    }

    .accordion__panel--expanded {
      grid-template-rows: 1fr;
    }

    .accordion__panel-content {
      overflow: hidden;
    }

    .accordion__panel--expanded .accordion__panel-content {
      padding: 0 1rem 1rem 1rem;
    }
  `]
})
export class AccordionComponent implements AfterContentInit {
  /**
   * Allow multiple items to be expanded at once
   */
  readonly multiple = input(false);

  /**
   * Keep content in DOM after collapse (for performance)
   */
  readonly keepAlive = input(true);

  /**
   * Emitted when an item's expansion state changes
   */
  readonly itemChange = output<AccordionChangeEvent>();

  /**
   * Accordion items from content projection
   */
  readonly items = contentChildren(AccordionItemComponent);

  /**
   * Set of currently expanded item IDs
   */
  readonly expandedIds = signal<Set<string>>(new Set());

  constructor() {
    // Sync initial expanded states
    effect(() => {
      const items = this.items();
      const expanded = new Set<string>();

      items.forEach(item => {
        if (item.expanded() && !item.disabled()) {
          expanded.add(item.itemId());
          item.isExpanded.set(true);
          item.hasOpened.set(true);
        }
      });

      this.expandedIds.set(expanded);
    }, { allowSignalWrites: true });
  }

  ngAfterContentInit(): void {
    // Initialize expansion states
    const items = this.items();
    const expanded = new Set<string>();

    items.forEach(item => {
      if (item.expanded() && !item.disabled()) {
        expanded.add(item.itemId());
        item.isExpanded.set(true);
        item.hasOpened.set(true);
      }
    });

    this.expandedIds.set(expanded);
  }

  /**
   * Toggle an item's expansion state
   */
  toggle(itemId: string): void {
    const items = this.items();
    const item = items.find(i => i.itemId() === itemId);

    if (!item || item.disabled()) return;

    const isCurrentlyExpanded = item.isExpanded();
    const newExpanded = !isCurrentlyExpanded;

    // In single mode, collapse other items
    if (newExpanded && !this.multiple()) {
      items.forEach(i => {
        if (i.itemId() !== itemId) {
          i.isExpanded.set(false);
        }
      });
    }

    // Update the target item
    item.isExpanded.set(newExpanded);
    if (newExpanded) {
      item.hasOpened.set(true);
    }

    // Update expanded IDs
    const expandedSet = new Set(
      items.filter(i => i.isExpanded()).map(i => i.itemId())
    );
    this.expandedIds.set(expandedSet);

    // Emit change event
    this.itemChange.emit({
      itemId,
      expanded: newExpanded,
      expandedIds: Array.from(expandedSet),
    });
  }

  /**
   * Expand a specific item
   */
  expand(itemId: string): void {
    const item = this.items().find(i => i.itemId() === itemId);
    if (item && !item.isExpanded() && !item.disabled()) {
      this.toggle(itemId);
    }
  }

  /**
   * Collapse a specific item
   */
  collapse(itemId: string): void {
    const item = this.items().find(i => i.itemId() === itemId);
    if (item && item.isExpanded()) {
      this.toggle(itemId);
    }
  }

  /**
   * Expand all items (only works in multiple mode)
   */
  expandAll(): void {
    if (!this.multiple()) return;

    this.items().forEach(item => {
      if (!item.disabled()) {
        item.isExpanded.set(true);
        item.hasOpened.set(true);
      }
    });

    this.updateExpandedIds();
  }

  /**
   * Collapse all items
   */
  collapseAll(): void {
    this.items().forEach(item => {
      item.isExpanded.set(false);
    });

    this.expandedIds.set(new Set());
  }

  /**
   * Handle keyboard navigation
   */
  onKeydown(event: KeyboardEvent, currentIndex: number): void {
    const items = this.items();
    const enabledItems = items.filter(i => !i.disabled());

    let targetIndex: number | null = null;

    if (event.key === 'ArrowUp') {
      event.preventDefault();
      targetIndex = this.findPreviousEnabledIndex(currentIndex);
    } else if (event.key === 'ArrowDown') {
      event.preventDefault();
      targetIndex = this.findNextEnabledIndex(currentIndex);
    } else if (event.key === 'Home') {
      event.preventDefault();
      const first = enabledItems[0];
      if (first) {
        targetIndex = items.findIndex(i => i === first);
      }
    } else if (event.key === 'End') {
      event.preventDefault();
      const last = enabledItems[enabledItems.length - 1];
      if (last) {
        targetIndex = items.findIndex(i => i === last);
      }
    }

    if (targetIndex !== null && targetIndex >= 0) {
      const targetItem = items[targetIndex];
      const header = document.getElementById('accordion-header-' + targetItem.itemId());
      header?.focus();
    }
  }

  private findPreviousEnabledIndex(currentIndex: number): number {
    const items = this.items();
    let index = currentIndex - 1;

    while (index >= 0) {
      if (!items[index].disabled()) {
        return index;
      }
      index--;
    }

    // Wrap to end
    index = items.length - 1;
    while (index > currentIndex) {
      if (!items[index].disabled()) {
        return index;
      }
      index--;
    }

    return currentIndex;
  }

  private findNextEnabledIndex(currentIndex: number): number {
    const items = this.items();
    let index = currentIndex + 1;

    while (index < items.length) {
      if (!items[index].disabled()) {
        return index;
      }
      index++;
    }

    // Wrap to start
    index = 0;
    while (index < currentIndex) {
      if (!items[index].disabled()) {
        return index;
      }
      index++;
    }

    return currentIndex;
  }

  private updateExpandedIds(): void {
    const expandedSet = new Set(
      this.items().filter(i => i.isExpanded()).map(i => i.itemId())
    );
    this.expandedIds.set(expandedSet);
  }
}
