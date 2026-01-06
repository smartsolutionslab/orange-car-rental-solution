using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Payments.Api.Requests;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Api.Endpoints;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var payments = app.MapGroup("/api/payments")
            .WithTags("Payments");

        payments.MapPost("/process", async Task<Results<Ok<ProcessPaymentResult>, BadRequest<ProblemDetails>>> (
                ProcessPaymentRequest request,
                ICommandHandler<ProcessPaymentCommand, ProcessPaymentResult> handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    // Parse payment method
                    if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, ignoreCase: true, out var paymentMethod))
                    {
                        return TypedResults.BadRequest(new ProblemDetails
                        {
                            Title = "Invalid payment method",
                            Detail = $"'{request.PaymentMethod}' is not a valid payment method",
                            Status = StatusCodes.Status400BadRequest
                        });
                    }

                    // Create command with value objects
                    var currency = Currency.From(request.Currency);
                    var amount = Money.FromGross(request.Amount, 0.19m, currency);

                    var command = new ProcessPaymentCommand(
                        ReservationIdentifier.From(request.ReservationId),
                        CustomerIdentifier.From(request.CustomerId),
                        amount,
                        paymentMethod);

                    var result = await handler.HandleAsync(command, cancellationToken);
                    return TypedResults.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return TypedResults.BadRequest(new ProblemDetails
                    {
                        Title = "Invalid request",
                        Detail = ex.Message,
                        Status = StatusCodes.Status400BadRequest
                    });
                }
                catch (InvalidOperationException ex)
                {
                    return TypedResults.BadRequest(new ProblemDetails
                    {
                        Title = "Payment processing failed",
                        Detail = ex.Message,
                        Status = StatusCodes.Status400BadRequest
                    });
                }
            })
            .WithName("ProcessPayment")
            .WithSummary("Process a payment for a reservation")
            .RequireAuthorization("CustomerOrCallCenterOrAdminPolicy");

        payments.MapPost("/{paymentId:guid}/refund", async Task<Results<Ok<RefundPaymentResult>, BadRequest<ProblemDetails>, NotFound>> (
                Guid paymentId,
                ICommandHandler<RefundPaymentCommand, RefundPaymentResult> handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new RefundPaymentCommand(PaymentIdentifier.From(paymentId));
                    var result = await handler.HandleAsync(command, cancellationToken);
                    return TypedResults.Ok(result);
                }
                catch (InvalidOperationException ex)
                {
                    if (ex.Message.Contains("not found"))
                    {
                        return TypedResults.NotFound();
                    }

                    return TypedResults.BadRequest(new ProblemDetails
                    {
                        Title = "Refund processing failed",
                        Detail = ex.Message,
                        Status = StatusCodes.Status400BadRequest
                    });
                }
            })
            .WithName("RefundPayment")
            .WithSummary("Refund a captured payment")
            .RequireAuthorization("CallCenterOrAdminPolicy");

        return app;
    }
}
