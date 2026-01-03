using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Diagnostics;

/// <summary>
/// Centralized metrics for Orange Car Rental application.
/// Provides counters and histograms for business KPIs.
/// </summary>
public static class OrangeCarRentalMetrics
{
    /// <summary>
    /// The meter name used for all Orange Car Rental metrics.
    /// Must match the meter name registered in ServiceDefaults.
    /// </summary>
    public const string MeterName = "OrangeCarRental";

    /// <summary>
    /// The activity source name for custom spans.
    /// </summary>
    public const string ActivitySourceName = "OrangeCarRental";

    private static readonly Meter Meter = new(MeterName, "1.0.0");
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName, "1.0.0");

    // Reservation metrics
    private static readonly Counter<long> ReservationsCreatedCounter =
        Meter.CreateCounter<long>(
            "orangecarrental.reservations.created",
            unit: "{reservation}",
            description: "Total number of reservations created");

    private static readonly Counter<long> ReservationsCancelledCounter =
        Meter.CreateCounter<long>(
            "orangecarrental.reservations.cancelled",
            unit: "{reservation}",
            description: "Total number of reservations cancelled");

    private static readonly Histogram<double> ReservationDurationHistogram =
        Meter.CreateHistogram<double>(
            "orangecarrental.reservations.duration",
            unit: "days",
            description: "Distribution of reservation durations in days");

    private static readonly Histogram<double> ReservationValueHistogram =
        Meter.CreateHistogram<double>(
            "orangecarrental.reservations.value",
            unit: "EUR",
            description: "Distribution of reservation total values");

    // Payment metrics
    private static readonly Counter<long> PaymentsProcessedCounter =
        Meter.CreateCounter<long>(
            "orangecarrental.payments.processed",
            unit: "{payment}",
            description: "Total number of payments processed");

    private static readonly Counter<long> PaymentsFailedCounter =
        Meter.CreateCounter<long>(
            "orangecarrental.payments.failed",
            unit: "{payment}",
            description: "Total number of failed payments");

    private static readonly Histogram<double> PaymentAmountHistogram =
        Meter.CreateHistogram<double>(
            "orangecarrental.payments.amount",
            unit: "EUR",
            description: "Distribution of payment amounts");

    // Customer metrics
    private static readonly Counter<long> CustomersRegisteredCounter =
        Meter.CreateCounter<long>(
            "orangecarrental.customers.registered",
            unit: "{customer}",
            description: "Total number of customers registered");

    // Vehicle metrics
    private static readonly Counter<long> VehiclesAddedCounter =
        Meter.CreateCounter<long>(
            "orangecarrental.vehicles.added",
            unit: "{vehicle}",
            description: "Total number of vehicles added to fleet");

    private static readonly Counter<long> VehicleStatusChangesCounter =
        Meter.CreateCounter<long>(
            "orangecarrental.vehicles.status_changes",
            unit: "{change}",
            description: "Total number of vehicle status changes");

    /// <summary>
    /// Records a new reservation creation.
    /// </summary>
    public static void RecordReservationCreated(string categoryCode, string locationCode)
    {
        ReservationsCreatedCounter.Add(1,
            new KeyValuePair<string, object?>("category", categoryCode),
            new KeyValuePair<string, object?>("location", locationCode));
    }

    /// <summary>
    /// Records a reservation cancellation.
    /// </summary>
    public static void RecordReservationCancelled(string reason)
    {
        ReservationsCancelledCounter.Add(1,
            new KeyValuePair<string, object?>("reason", reason));
    }

    /// <summary>
    /// Records the duration of a reservation.
    /// </summary>
    public static void RecordReservationDuration(double durationDays, string categoryCode)
    {
        ReservationDurationHistogram.Record(durationDays,
            new KeyValuePair<string, object?>("category", categoryCode));
    }

    /// <summary>
    /// Records the total value of a reservation.
    /// </summary>
    public static void RecordReservationValue(decimal totalValue, string categoryCode)
    {
        ReservationValueHistogram.Record((double)totalValue,
            new KeyValuePair<string, object?>("category", categoryCode));
    }

    /// <summary>
    /// Records a successful payment.
    /// </summary>
    public static void RecordPaymentProcessed(decimal amount, string paymentMethod)
    {
        PaymentsProcessedCounter.Add(1,
            new KeyValuePair<string, object?>("method", paymentMethod));
        PaymentAmountHistogram.Record((double)amount,
            new KeyValuePair<string, object?>("method", paymentMethod));
    }

    /// <summary>
    /// Records a failed payment.
    /// </summary>
    public static void RecordPaymentFailed(string reason)
    {
        PaymentsFailedCounter.Add(1,
            new KeyValuePair<string, object?>("reason", reason));
    }

    /// <summary>
    /// Records a new customer registration.
    /// </summary>
    public static void RecordCustomerRegistered(string country)
    {
        CustomersRegisteredCounter.Add(1,
            new KeyValuePair<string, object?>("country", country));
    }

    /// <summary>
    /// Records a vehicle being added to the fleet.
    /// </summary>
    public static void RecordVehicleAdded(string categoryCode, string locationCode)
    {
        VehiclesAddedCounter.Add(1,
            new KeyValuePair<string, object?>("category", categoryCode),
            new KeyValuePair<string, object?>("location", locationCode));
    }

    /// <summary>
    /// Records a vehicle status change.
    /// </summary>
    public static void RecordVehicleStatusChange(string fromStatus, string toStatus)
    {
        VehicleStatusChangesCounter.Add(1,
            new KeyValuePair<string, object?>("from_status", fromStatus),
            new KeyValuePair<string, object?>("to_status", toStatus));
    }

    /// <summary>
    /// Starts a new activity (span) for custom tracing.
    /// </summary>
    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        return ActivitySource.StartActivity(name, kind);
    }
}
