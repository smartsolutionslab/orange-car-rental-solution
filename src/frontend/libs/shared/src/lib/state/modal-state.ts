import { signal, computed } from '@angular/core';
import type { Signal, WritableSignal } from '@angular/core';

/**
 * Generic state manager for modal dialogs with selected item.
 * Reduces boilerplate for common modal state patterns.
 *
 * @example
 * ```typescript
 * // In component
 * protected readonly detailsModal = new ModalState<Vehicle>();
 * protected readonly addModal = new ModalState<void>();
 *
 * // In template
 * <ui-modal [isOpen]="detailsModal.isOpen()" (close)="detailsModal.close()">
 *   @if (detailsModal.item(); as vehicle) {
 *     <div>{{ vehicle.name }}</div>
 *   }
 * </ui-modal>
 *
 * // In methods
 * viewDetails(vehicle: Vehicle) {
 *   this.detailsModal.open(vehicle);
 * }
 * ```
 */
export class ModalState<T = void> {
  private readonly _isOpen: WritableSignal<boolean>;
  private readonly _item: WritableSignal<T | null>;

  constructor() {
    this._isOpen = signal(false);
    this._item = signal<T | null>(null);
  }

  /** Whether the modal is currently open */
  readonly isOpen: Signal<boolean> = computed(() => this._isOpen());

  /** The currently selected item (null when closed or no item) */
  readonly item: Signal<T | null> = computed(() => this._item());

  /**
   * Open the modal with an optional item
   * @param item - The item to display in the modal
   */
  open(item?: T): void {
    this._item.set(item ?? null);
    this._isOpen.set(true);
  }

  /**
   * Close the modal and clear the selected item
   */
  close(): void {
    this._isOpen.set(false);
    this._item.set(null);
  }

  /**
   * Toggle the modal open/closed state
   * @param item - Optional item to set when opening
   */
  toggle(item?: T): void {
    if (this._isOpen()) {
      this.close();
    } else {
      this.open(item);
    }
  }

  /**
   * Update the selected item without changing open state
   * @param item - The new item value
   */
  setItem(item: T | null): void {
    this._item.set(item);
  }
}

/**
 * Multiple modal state manager for components with several modals.
 * Ensures only one modal is open at a time.
 *
 * @example
 * ```typescript
 * type VehicleModals = 'details' | 'add' | 'status' | 'location';
 * protected readonly modals = new MultiModalState<Vehicle, VehicleModals>();
 *
 * viewDetails(vehicle: Vehicle) {
 *   this.modals.open('details', vehicle);
 * }
 *
 * closeAll() {
 *   this.modals.closeAll();
 * }
 * ```
 */
export class MultiModalState<T, K extends string> {
  private readonly _activeModal: WritableSignal<K | null>;
  private readonly _item: WritableSignal<T | null>;

  constructor() {
    this._activeModal = signal<K | null>(null);
    this._item = signal<T | null>(null);
  }

  /** The currently active modal name (null if all closed) */
  readonly activeModal: Signal<K | null> = computed(() => this._activeModal());

  /** The currently selected item */
  readonly item: Signal<T | null> = computed(() => this._item());

  /**
   * Check if a specific modal is open
   * @param modal - The modal name to check
   */
  isOpen(modal: K): boolean {
    return this._activeModal() === modal;
  }

  /**
   * Open a specific modal with an optional item
   * @param modal - The modal name to open
   * @param item - Optional item to display
   */
  open(modal: K, item?: T): void {
    this._item.set(item ?? null);
    this._activeModal.set(modal);
  }

  /**
   * Close the currently active modal
   */
  close(): void {
    this._activeModal.set(null);
    this._item.set(null);
  }

  /**
   * Close all modals (alias for close)
   */
  closeAll(): void {
    this.close();
  }

  /**
   * Update the selected item without changing modal state
   * @param item - The new item value
   */
  setItem(item: T | null): void {
    this._item.set(item);
  }
}
