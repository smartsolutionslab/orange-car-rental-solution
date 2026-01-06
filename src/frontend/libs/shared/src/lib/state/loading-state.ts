import { signal, computed } from "@angular/core";
import type { Signal, WritableSignal } from "@angular/core";

/**
 * State manager for loading, error, and success states.
 * Reduces boilerplate for common async operation patterns.
 *
 * @example
 * ```typescript
 * // In component
 * protected readonly state = new LoadingState();
 *
 * loadData() {
 *   this.state.startLoading();
 *   this.service.getData().subscribe({
 *     next: (data) => {
 *       this.data.set(data);
 *       this.state.setSuccess('Daten erfolgreich geladen');
 *     },
 *     error: (err) => {
 *       this.state.setError('Fehler beim Laden der Daten');
 *     }
 *   });
 * }
 *
 * // In template
 * @if (state.isLoading()) {
 *   <loading-spinner />
 * } @else if (state.error(); as error) {
 *   <error-message [message]="error" />
 * } @else {
 *   <!-- content -->
 * }
 * ```
 */
export class LoadingState {
  private readonly _isLoading: WritableSignal<boolean>;
  private readonly _error: WritableSignal<string | null>;
  private readonly _successMessage: WritableSignal<string | null>;

  constructor() {
    this._isLoading = signal(false);
    this._error = signal<string | null>(null);
    this._successMessage = signal<string | null>(null);
  }

  /** Whether an async operation is in progress */
  readonly isLoading: Signal<boolean> = computed(() => this._isLoading());

  /** Current error message (null if no error) */
  readonly error: Signal<string | null> = computed(() => this._error());

  /** Current success message (null if no success message) */
  readonly successMessage: Signal<string | null> = computed(() =>
    this._successMessage(),
  );

  /** Whether there is an error */
  readonly hasError: Signal<boolean> = computed(() => this._error() !== null);

  /** Whether there is a success message */
  readonly hasSuccess: Signal<boolean> = computed(
    () => this._successMessage() !== null,
  );

  /**
   * Start a loading operation, clearing any previous error/success
   */
  startLoading(): void {
    this._isLoading.set(true);
    this._error.set(null);
    this._successMessage.set(null);
  }

  /**
   * Stop loading (without setting error or success)
   */
  stopLoading(): void {
    this._isLoading.set(false);
  }

  /**
   * Set an error message and stop loading
   * @param message - The error message to display
   */
  setError(message: string): void {
    this._error.set(message);
    this._successMessage.set(null);
    this._isLoading.set(false);
  }

  /**
   * Set a success message and stop loading
   * @param message - The success message to display (optional)
   */
  setSuccess(message?: string): void {
    this._successMessage.set(message ?? null);
    this._error.set(null);
    this._isLoading.set(false);
  }

  /**
   * Clear only the error message
   */
  clearError(): void {
    this._error.set(null);
  }

  /**
   * Clear only the success message
   */
  clearSuccess(): void {
    this._successMessage.set(null);
  }

  /**
   * Clear all messages (error and success)
   */
  clearMessages(): void {
    this._error.set(null);
    this._successMessage.set(null);
  }

  /**
   * Reset all state to initial values
   */
  reset(): void {
    this._isLoading.set(false);
    this._error.set(null);
    this._successMessage.set(null);
  }
}

/**
 * Extended loading state with action-specific loading indicators.
 * Useful when a component has multiple async operations.
 *
 * @example
 * ```typescript
 * type Actions = 'load' | 'save' | 'delete';
 * protected readonly state = new ActionLoadingState<Actions>();
 *
 * loadData() {
 *   this.state.startAction('load');
 *   // ...
 *   this.state.completeAction('load');
 * }
 *
 * // In template - check specific action
 * @if (state.isActionLoading('save')) {
 *   <button disabled>Speichern...</button>
 * }
 *
 * // Or check if any action is loading
 * @if (state.isAnyLoading()) {
 *   <loading-overlay />
 * }
 * ```
 */
export class ActionLoadingState<K extends string> extends LoadingState {
  private readonly _activeActions: WritableSignal<Set<K>>;

  constructor() {
    super();
    this._activeActions = signal(new Set<K>());
  }

  /** Set of currently active action names */
  readonly activeActions: Signal<Set<K>> = computed(() =>
    this._activeActions(),
  );

  /**
   * Check if a specific action is loading
   * @param action - The action name to check
   */
  isActionLoading(action: K): boolean {
    return this._activeActions().has(action);
  }

  /**
   * Check if any action is currently loading
   */
  isAnyLoading(): boolean {
    return this._activeActions().size > 0;
  }

  /**
   * Start a specific action
   * @param action - The action name
   * @param clearMessages - Whether to clear error/success messages (default: true)
   */
  startAction(action: K, clearMessages = true): void {
    this._activeActions.update((actions) => {
      const newActions = new Set(actions);
      newActions.add(action);
      return newActions;
    });
    if (clearMessages) {
      this.clearMessages();
    }
  }

  /**
   * Complete a specific action (removes from active set)
   * @param action - The action name
   */
  completeAction(action: K): void {
    this._activeActions.update((actions) => {
      const newActions = new Set(actions);
      newActions.delete(action);
      return newActions;
    });
  }

  /**
   * Complete an action with an error
   * @param action - The action name
   * @param message - The error message
   */
  completeActionWithError(action: K, message: string): void {
    this.completeAction(action);
    this.setError(message);
  }

  /**
   * Complete an action with success
   * @param action - The action name
   * @param message - Optional success message
   */
  completeActionWithSuccess(action: K, message?: string): void {
    this.completeAction(action);
    this.setSuccess(message);
  }

  /**
   * Clear all active actions
   */
  clearAllActions(): void {
    this._activeActions.set(new Set());
  }

  /**
   * Reset all state including actions
   */
  override reset(): void {
    super.reset();
    this._activeActions.set(new Set());
  }
}
