import { Component, inject, signal, computed } from '@angular/core';
import type { OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ReservationStatus, isCustomerId } from '@orange-car-rental/reservation-api';
import { logError } from '@orange-car-rental/util';
import {
  SelectReservationStatusComponent,
  SelectLocationComponent,
  StatusBadgeComponent,
  ModalComponent,
  LoadingStateComponent,
  EmptyStateComponent,
  ErrorStateComponent,
  getReservationStatusClass,
  StatCardComponent,
  PaginationComponent,
  SelectPageSizeComponent,
  getReservationStatusLabel,
  formatDateDE,
  formatPriceDE,
  SuccessAlertComponent,
  ErrorAlertComponent,
} from '@orange-car-rental/ui-components';
import { ReservationService } from '../../services/reservation.service';
import { UI_TIMING, DEFAULT_PAGE_SIZE } from '../../constants/app.constants';
import type { Reservation, ReservationSearchQuery } from '../../types';

type GroupBy = 'none' | 'status' | 'pickupDate' | 'location';

type GroupedReservations = Record<string, Reservation[]>;

/**
 * Reservations management page for call center
 * Displays booking management tools with advanced filtering, sorting, and grouping
 */
@Component({
  selector: 'app-reservations',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    SelectReservationStatusComponent,
    SelectLocationComponent,
    StatusBadgeComponent,
    ModalComponent,
    LoadingStateComponent,
    EmptyStateComponent,
    ErrorStateComponent,
    StatCardComponent,
    PaginationComponent,
    SelectPageSizeComponent,
    SuccessAlertComponent,
    ErrorAlertComponent,
  ],
  templateUrl: './reservations.component.html',
  styleUrl: './reservations.component.css'
})
export class ReservationsComponent implements OnInit {
  private readonly reservationService = inject(ReservationService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  protected readonly reservations = signal<Reservation[]>([]);
  protected readonly loading = signal(false);
  protected readonly error = signal<string | null>(null);
  protected readonly selectedReservation = signal<Reservation | null>(null);
  protected readonly showDetails = signal(false);
  protected readonly showCancelModal = signal(false);
  protected readonly cancelReason = signal<string>('');
  protected readonly actionInProgress = signal(false);
  protected readonly successMessage = signal<string | null>(null);

  // Filter controls
  protected readonly searchStatus = signal<string>('');
  protected readonly searchCustomerId = signal<string>('');
  protected readonly searchPickupDateFrom = signal<string>('');
  protected readonly searchPickupDateTo = signal<string>('');
  protected readonly searchLocation = signal<string>('');
  protected readonly searchMinPrice = signal<number | null>(null);
  protected readonly searchMaxPrice = signal<number | null>(null);

  // Sorting controls
  protected readonly sortBy = signal<'PickupDate' | 'Price' | 'Status' | 'CreatedDate'>('PickupDate');
  protected readonly sortOrder = signal<'asc' | 'desc'>('desc');

  // Grouping controls
  protected readonly groupBy = signal<GroupBy>('none');

  // Pagination
  protected readonly currentPage = signal(1);
  protected readonly pageSize = signal<number>(DEFAULT_PAGE_SIZE.RESERVATIONS);
  protected readonly totalCount = signal(0);
  protected readonly totalPages = computed(() =>
    Math.ceil(this.totalCount() / this.pageSize())
  );

  // Stats
  protected readonly todayReservations = signal(0);
  protected readonly activeReservations = signal(0);
  protected readonly pendingReservations = signal(0);

  // Active filters count
  protected readonly activeFiltersCount = computed(() => {
    let count = 0;
    if (this.searchStatus()) count++;
    if (this.searchCustomerId()) count++;
    if (this.searchPickupDateFrom()) count++;
    if (this.searchPickupDateTo()) count++;
    if (this.searchLocation()) count++;
    if (this.searchMinPrice() !== null) count++;
    if (this.searchMaxPrice() !== null) count++;
    return count;
  });

  // Grouped reservations
  protected readonly groupedReservations = computed<GroupedReservations>(() => {
    const reservations = this.reservations();
    const grouping = this.groupBy();

    if (grouping === 'none') {
      return { 'all': reservations };
    }

    const grouped: GroupedReservations = {};

    reservations.forEach(reservation => {
      let key: string;

      switch (grouping) {
        case 'status':
          key = reservation.status || 'Unknown';
          break;
        case 'pickupDate':
          key = this.getDateGroup(reservation.pickupDate);
          break;
        case 'location':
          key = reservation.pickupLocationCode || 'Unknown';
          break;
        default:
          key = 'all';
      }

      if (!grouped[key]) {
        grouped[key] = [];
      }
      grouped[key].push(reservation);
    });

    return grouped;
  });

  protected readonly groupKeys = computed(() => Object.keys(this.groupedReservations()));

  // Sort options
  protected readonly sortOptions = [
    { value: 'PickupDate', label: 'Abholdatum' },
    { value: 'Price', label: 'Preis' },
    { value: 'Status', label: 'Status' },
    { value: 'CreatedDate', label: 'Erstellungsdatum' }
  ];

  // Group options
  protected readonly groupOptions = [
    { value: 'none', label: 'Keine Gruppierung' },
    { value: 'status', label: 'Nach Status' },
    { value: 'pickupDate', label: 'Nach Abholdatum' },
    { value: 'location', label: 'Nach Standort' }
  ];

  ngOnInit(): void {
    // Load filters from URL parameters
    this.loadFiltersFromUrl();
    this.loadReservations();
  }

  /**
   * Load filter values from URL query parameters
   */
  private loadFiltersFromUrl(): void {
    this.route.queryParams.subscribe(params => {
      if (params['status']) this.searchStatus.set(params['status']);
      if (params['customerId']) this.searchCustomerId.set(params['customerId']);
      if (params['dateFrom']) this.searchPickupDateFrom.set(params['dateFrom']);
      if (params['dateTo']) this.searchPickupDateTo.set(params['dateTo']);
      if (params['location']) this.searchLocation.set(params['location']);
      if (params['minPrice']) this.searchMinPrice.set(Number(params['minPrice']));
      if (params['maxPrice']) this.searchMaxPrice.set(Number(params['maxPrice']));
      if (params['sortBy']) this.sortBy.set(params['sortBy'] as 'PickupDate' | 'Price' | 'Status' | 'CreatedDate');
      if (params['sortOrder']) this.sortOrder.set(params['sortOrder'] as 'asc' | 'desc');
      if (params['groupBy']) this.groupBy.set(params['groupBy'] as GroupBy);
      if (params['page']) this.currentPage.set(Number(params['page']));
      if (params['pageSize']) this.pageSize.set(Number(params['pageSize']));
    });
  }

  /**
   * Update URL with current filter values
   */
  private updateUrl(): void {
    const queryParams: {
      status?: string;
      customerId?: string;
      dateFrom?: string;
      dateTo?: string;
      location?: string;
      minPrice?: number | null;
      maxPrice?: number | null;
      sortBy?: string;
      sortOrder?: string;
      groupBy?: string;
      page?: number;
      pageSize?: number;
    } = {};

    if (this.searchStatus()) queryParams.status = this.searchStatus();
    if (this.searchCustomerId()) queryParams.customerId = this.searchCustomerId();
    if (this.searchPickupDateFrom()) queryParams.dateFrom = this.searchPickupDateFrom();
    if (this.searchPickupDateTo()) queryParams.dateTo = this.searchPickupDateTo();
    if (this.searchLocation()) queryParams.location = this.searchLocation();
    if (this.searchMinPrice() !== null) queryParams.minPrice = this.searchMinPrice();
    if (this.searchMaxPrice() !== null) queryParams.maxPrice = this.searchMaxPrice();
    if (this.sortBy() !== 'PickupDate') queryParams.sortBy = this.sortBy();
    if (this.sortOrder() !== 'desc') queryParams.sortOrder = this.sortOrder();
    if (this.groupBy() !== 'none') queryParams.groupBy = this.groupBy();
    if (this.currentPage() !== 1) queryParams.page = this.currentPage();
    if (this.pageSize() !== DEFAULT_PAGE_SIZE.RESERVATIONS) queryParams.pageSize = this.pageSize();

    this.router.navigate([], {
      relativeTo: this.route,
      queryParams,
      queryParamsHandling: 'merge'
    });
  }

  /**
   * Load reservations with current filters
   */
  protected loadReservations(): void {
    this.loading.set(true);
    this.error.set(null);

    const customerIdValue = this.searchCustomerId();
    const query: ReservationSearchQuery = {
      status: (this.searchStatus() || undefined) as ReservationStatus | undefined,
      customerId: customerIdValue && isCustomerId(customerIdValue) ? customerIdValue : undefined,
      pickupDateFrom: this.searchPickupDateFrom() || undefined,
      pickupDateTo: this.searchPickupDateTo() || undefined,
      locationCode: this.searchLocation() || undefined,
      minPrice: this.searchMinPrice() ?? undefined,
      maxPrice: this.searchMaxPrice() ?? undefined,
      sortBy: this.sortBy(),
      sortOrder: this.sortOrder(),
      pageNumber: this.currentPage(),
      pageSize: this.pageSize()
    };

    this.reservationService.searchReservations(query).subscribe({
      next: (result) => {
        this.reservations.set(result.reservations);
        this.totalCount.set(result.totalCount);
        this.calculateStats(result.reservations);
        this.loading.set(false);
        this.updateUrl();
      },
      error: (err) => {
        logError('ReservationsComponent', 'Error loading reservations', err);
        this.error.set('Fehler beim Laden der Reservierungen');
        this.loading.set(false);
      }
    });
  }

  /**
   * Calculate statistics from reservations
   */
  private calculateStats(reservations: Reservation[]): void {
    const today = new Date().toISOString().split('T')[0];

    this.todayReservations.set(
      reservations.filter(r => r.createdAt.startsWith(today)).length
    );

    this.activeReservations.set(
      reservations.filter(r => r.status === ReservationStatus.Confirmed || r.status === ReservationStatus.Active).length
    );

    this.pendingReservations.set(
      reservations.filter(r => r.status === ReservationStatus.Pending).length
    );
  }

  /**
   * Group reservations by date
   */
  private getDateGroup(dateString: string): string {
    const date = new Date(dateString);
    const today = new Date();
    const tomorrow = new Date(today);
    tomorrow.setDate(tomorrow.getDate() + 1);
    const nextWeek = new Date(today);
    nextWeek.setDate(nextWeek.getDate() + 7);

    const dateOnly = date.toISOString().split('T')[0];
    const todayOnly = today.toISOString().split('T')[0];
    const tomorrowOnly = tomorrow.toISOString().split('T')[0];

    if (dateOnly === todayOnly) return 'Heute';
    if (dateOnly === tomorrowOnly) return 'Morgen';
    if (date <= nextWeek) return 'Diese Woche';
    return 'Später';
  }

  /**
   * Apply search filters
   */
  protected applyFilters(): void {
    this.currentPage.set(1); // Reset to first page
    this.loadReservations();
  }

  /**
   * Clear all search filters
   */
  protected clearFilters(): void {
    this.searchStatus.set('');
    this.searchCustomerId.set('');
    this.searchPickupDateFrom.set('');
    this.searchPickupDateTo.set('');
    this.searchLocation.set('');
    this.searchMinPrice.set(null);
    this.searchMaxPrice.set(null);
    this.sortBy.set('PickupDate');
    this.sortOrder.set('desc');
    this.groupBy.set('none');
    this.currentPage.set(1);
    this.loadReservations();
  }

  /**
   * Change sort field
   */
  protected changeSortBy(field: 'PickupDate' | 'Price' | 'Status' | 'CreatedDate'): void {
    if (this.sortBy() === field) {
      // Toggle sort order
      this.sortOrder.set(this.sortOrder() === 'asc' ? 'desc' : 'asc');
    } else {
      this.sortBy.set(field);
      this.sortOrder.set('desc');
    }
    this.loadReservations();
  }

  /**
   * Change grouping
   */
  protected changeGroupBy(group: GroupBy): void {
    this.groupBy.set(group);
    this.updateUrl();
  }

  /**
   * Pagination
   */
  protected goToPage(page: number): void {
    if (page < 1 || page > this.totalPages()) return;
    this.currentPage.set(page);
    this.loadReservations();
  }

  protected nextPage(): void {
    this.goToPage(this.currentPage() + 1);
  }

  protected previousPage(): void {
    this.goToPage(this.currentPage() - 1);
  }

  /**
   * View reservation details
   */
  protected viewDetails(reservation: Reservation): void {
    this.selectedReservation.set(reservation);
    this.showDetails.set(true);
  }

  /**
   * Close details view
   */
  protected closeDetails(): void {
    this.showDetails.set(false);
    this.selectedReservation.set(null);
  }

  /**
   * Format date for display
   */
  protected formatDate = formatDateDE;

  /**
   * Format price for display
   */
  protected formatPrice = formatPriceDE;

  /**
   * Math helper for templates
   */
  protected readonly Math = Math;

  /**
   * Get status badge class
   */
  protected getStatusClass = getReservationStatusClass;

  /**
   * Get status label in German
   */
  protected getStatusLabel = getReservationStatusLabel;

  /**
   * Check if reservation can be confirmed
   */
  protected canConfirm(reservation: Reservation): boolean {
    return reservation.status === ReservationStatus.Pending;
  }

  /**
   * Check if reservation can be cancelled
   */
  protected canCancel(reservation: Reservation): boolean {
    return reservation.status === ReservationStatus.Pending || reservation.status === ReservationStatus.Confirmed;
  }

  /**
   * Confirm a reservation
   */
  protected confirmReservation(reservation: Reservation): void {
    if (!this.canConfirm(reservation)) {
      return;
    }

    if (!confirm(`Möchten Sie die Reservierung ${reservation.reservationId.substring(0, 8)} wirklich bestätigen?`)) {
      return;
    }

    this.actionInProgress.set(true);
    this.error.set(null);
    this.successMessage.set(null);

    this.reservationService.confirmReservation(reservation.reservationId).subscribe({
      next: () => {
        this.actionInProgress.set(false);
        this.successMessage.set('Reservierung erfolgreich bestätigt');
        this.loadReservations();
        this.closeDetails();

        // Clear success message after timeout
        setTimeout(() => this.successMessage.set(null), UI_TIMING.SUCCESS_MESSAGE_DURATION);
      },
      error: (err) => {
        logError('ReservationsComponent', 'Error confirming reservation', err);
        this.actionInProgress.set(false);
        this.error.set('Fehler beim Bestätigen der Reservierung');
      }
    });
  }

  /**
   * Show cancel reservation modal
   */
  protected showCancelDialog(reservation: Reservation): void {
    if (!this.canCancel(reservation)) {
      return;
    }

    this.selectedReservation.set(reservation);
    this.cancelReason.set('');
    this.showCancelModal.set(true);
  }

  /**
   * Close cancel modal
   */
  protected closeCancelModal(): void {
    this.showCancelModal.set(false);
    this.cancelReason.set('');
  }

  /**
   * Cancel a reservation
   */
  protected cancelReservation(): void {
    const reservation = this.selectedReservation();
    const reason = this.cancelReason().trim();

    if (!reservation || !reason) {
      this.error.set('Bitte geben Sie einen Stornierungsgrund ein');
      return;
    }

    this.actionInProgress.set(true);
    this.error.set(null);
    this.successMessage.set(null);

    this.reservationService.cancelReservation(reservation.reservationId, reason).subscribe({
      next: () => {
        this.actionInProgress.set(false);
        this.successMessage.set('Reservierung erfolgreich storniert');
        this.closeCancelModal();
        this.closeDetails();
        this.loadReservations();

        // Clear success message after timeout
        setTimeout(() => this.successMessage.set(null), UI_TIMING.SUCCESS_MESSAGE_DURATION);
      },
      error: (err) => {
        logError('ReservationsComponent', 'Error cancelling reservation', err);
        this.actionInProgress.set(false);
        this.error.set('Fehler beim Stornieren der Reservierung');
      }
    });
  }
}
