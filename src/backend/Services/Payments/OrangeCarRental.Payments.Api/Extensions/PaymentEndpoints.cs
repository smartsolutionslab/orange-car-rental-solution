using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Api.Extensions;

public static class PaymentEndpoints
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        var payments = app.MapGroup("/api/payments")
            .WithTags("Payments")
            .WithOpenApi();

        payments.MapPost("/process", async Task<Results<Ok<ProcessPaymentResult>, BadRequest<ProblemDetails>>> (
                ProcessPaymentCommand command,
                ProcessPaymentCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
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
            .WithSummary("Process a payment for a reservation");

        payments.MapPost("/{paymentId:guid}/refund", async Task<Results<Ok<RefundPaymentResult>, BadRequest<ProblemDetails>, NotFound>> (
                Guid paymentId,
                RefundPaymentCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new RefundPaymentCommand(paymentId);
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
            .WithSummary("Refund a captured payment");

        return app;
    }
}
