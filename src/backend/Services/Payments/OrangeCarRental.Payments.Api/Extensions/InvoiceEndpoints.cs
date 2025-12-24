using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Api.Extensions;

public static class InvoiceEndpoints
{
    public static IEndpointRouteBuilder MapInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        var invoices = app.MapGroup("/api/invoices")
            .WithTags("Invoices");

        // Generate invoice for a reservation
        invoices.MapPost("/generate", async Task<Results<Ok<GenerateInvoiceResult>, BadRequest<ProblemDetails>>> (
                GenerateInvoiceCommand command,
                GenerateInvoiceCommandHandler handler,
                CancellationToken cancellationToken) =>
            {
                var result = await handler.HandleAsync(command, cancellationToken);

                if (!result.Success)
                {
                    return TypedResults.BadRequest(new ProblemDetails
                    {
                        Title = "Invoice generation failed",
                        Detail = result.ErrorMessage,
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                return TypedResults.Ok(result);
            })
            .WithName("GenerateInvoice")
            .WithSummary("Generate an invoice for a reservation")
            .RequireAuthorization("CallCenterOrAdminPolicy");

        // Get invoice by ID
        invoices.MapGet("/{invoiceId:guid}", async Task<Results<Ok<InvoiceDto>, NotFound>> (
                Guid invoiceId,
                IPaymentsUnitOfWork unitOfWork,
                CancellationToken cancellationToken) =>
            {
                var invoice = await unitOfWork.Invoices
                    .GetByIdAsync(InvoiceIdentifier.From(invoiceId), cancellationToken);

                if (invoice == null)
                    return TypedResults.NotFound();

                return TypedResults.Ok(MapToDto(invoice));
            })
            .WithName("GetInvoice")
            .WithSummary("Get invoice details by ID")
            .RequireAuthorization("CustomerOrCallCenterOrAdminPolicy");

        // Get invoice PDF by ID
        invoices.MapGet("/{invoiceId:guid}/pdf", async Task<Results<FileContentHttpResult, NotFound>> (
                Guid invoiceId,
                IPaymentsUnitOfWork unitOfWork,
                CancellationToken cancellationToken) =>
            {
                var invoice = await unitOfWork.Invoices
                    .GetByIdAsync(InvoiceIdentifier.From(invoiceId), cancellationToken);

                if (invoice?.PdfDocument == null)
                    return TypedResults.NotFound();

                return TypedResults.File(
                    invoice.PdfDocument,
                    "application/pdf",
                    $"Rechnung_{invoice.InvoiceNumber.Value}.pdf");
            })
            .WithName("GetInvoicePdf")
            .WithSummary("Download invoice PDF")
            .RequireAuthorization("CustomerOrCallCenterOrAdminPolicy");

        // Get invoice by reservation ID
        invoices.MapGet("/by-reservation/{reservationId:guid}", async Task<Results<Ok<InvoiceDto>, NotFound>> (
                Guid reservationId,
                IPaymentsUnitOfWork unitOfWork,
                CancellationToken cancellationToken) =>
            {
                var invoice = await unitOfWork.Invoices
                    .GetByReservationIdAsync(reservationId, cancellationToken);

                if (invoice == null)
                    return TypedResults.NotFound();

                return TypedResults.Ok(MapToDto(invoice));
            })
            .WithName("GetInvoiceByReservation")
            .WithSummary("Get invoice by reservation ID")
            .RequireAuthorization("CustomerOrCallCenterOrAdminPolicy");

        // Get all invoices for a customer
        invoices.MapGet("/by-customer/{customerId:guid}", async Task<Results<Ok<IReadOnlyList<InvoiceDto>>, NotFound>> (
                Guid customerId,
                IPaymentsUnitOfWork unitOfWork,
                CancellationToken cancellationToken) =>
            {
                var invoices = await unitOfWork.Invoices
                    .GetByCustomerIdAsync(customerId, cancellationToken);

                return TypedResults.Ok(invoices.Select(MapToDto).ToList() as IReadOnlyList<InvoiceDto>);
            })
            .WithName("GetInvoicesByCustomer")
            .WithSummary("Get all invoices for a customer")
            .RequireAuthorization("CustomerOrCallCenterOrAdminPolicy");

        // Mark invoice as sent
        invoices.MapPost("/{invoiceId:guid}/send", async Task<Results<Ok<InvoiceDto>, NotFound, BadRequest<ProblemDetails>>> (
                Guid invoiceId,
                IPaymentsUnitOfWork unitOfWork,
                CancellationToken cancellationToken) =>
            {
                var invoice = await unitOfWork.Invoices
                    .GetByIdAsync(InvoiceIdentifier.From(invoiceId), cancellationToken);

                if (invoice == null)
                    return TypedResults.NotFound();

                try
                {
                    var updatedInvoice = invoice.MarkAsSent();
                    await unitOfWork.Invoices.UpdateAsync(updatedInvoice, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    return TypedResults.Ok(MapToDto(updatedInvoice));
                }
                catch (InvalidOperationException ex)
                {
                    return TypedResults.BadRequest(new ProblemDetails
                    {
                        Title = "Cannot mark invoice as sent",
                        Detail = ex.Message,
                        Status = StatusCodes.Status400BadRequest
                    });
                }
            })
            .WithName("MarkInvoiceSent")
            .WithSummary("Mark invoice as sent to customer")
            .RequireAuthorization("CallCenterOrAdminPolicy");

        // Mark invoice as paid
        invoices.MapPost("/{invoiceId:guid}/paid", async Task<Results<Ok<InvoiceDto>, NotFound, BadRequest<ProblemDetails>>> (
                Guid invoiceId,
                IPaymentsUnitOfWork unitOfWork,
                CancellationToken cancellationToken) =>
            {
                var invoice = await unitOfWork.Invoices
                    .GetByIdAsync(InvoiceIdentifier.From(invoiceId), cancellationToken);

                if (invoice == null)
                    return TypedResults.NotFound();

                try
                {
                    var updatedInvoice = invoice.MarkAsPaid();
                    await unitOfWork.Invoices.UpdateAsync(updatedInvoice, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    return TypedResults.Ok(MapToDto(updatedInvoice));
                }
                catch (InvalidOperationException ex)
                {
                    return TypedResults.BadRequest(new ProblemDetails
                    {
                        Title = "Cannot mark invoice as paid",
                        Detail = ex.Message,
                        Status = StatusCodes.Status400BadRequest
                    });
                }
            })
            .WithName("MarkInvoicePaid")
            .WithSummary("Mark invoice as paid")
            .RequireAuthorization("CallCenterOrAdminPolicy");

        // Send invoice via email
        invoices.MapPost("/{invoiceId:guid}/email", async Task<Results<Ok<SendEmailResultDto>, NotFound, BadRequest<ProblemDetails>>> (
                Guid invoiceId,
                SendInvoiceEmailRequest request,
                IPaymentsUnitOfWork unitOfWork,
                IInvoiceEmailSender emailSender,
                CancellationToken cancellationToken) =>
            {
                var invoice = await unitOfWork.Invoices
                    .GetByIdAsync(InvoiceIdentifier.From(invoiceId), cancellationToken);

                if (invoice == null)
                    return TypedResults.NotFound();

                var result = await emailSender.SendInvoiceAsync(invoice, request.RecipientEmail, cancellationToken);

                if (!result.Success)
                {
                    return TypedResults.BadRequest(new ProblemDetails
                    {
                        Title = "Failed to send invoice email",
                        Detail = result.ErrorMessage,
                        Status = StatusCodes.Status400BadRequest
                    });
                }

                // Mark invoice as sent if not already
                if (invoice.Status == InvoiceStatus.Created)
                {
                    var updatedInvoice = invoice.MarkAsSent();
                    await unitOfWork.Invoices.UpdateAsync(updatedInvoice, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                }

                return TypedResults.Ok(new SendEmailResultDto(
                    Success: true,
                    ProviderMessageId: result.ProviderMessageId,
                    Message: "Invoice email sent successfully."));
            })
            .WithName("SendInvoiceEmail")
            .WithSummary("Send invoice to customer via email")
            .RequireAuthorization("CallCenterOrAdminPolicy");

        return app;
    }

    private static InvoiceDto MapToDto(Invoice invoice)
    {
        return new InvoiceDto(
            Id: invoice.Id.Value,
            InvoiceNumber: invoice.InvoiceNumber.Value,
            InvoiceDate: invoice.InvoiceDate,
            ServiceDate: invoice.ServiceDate,
            DueDate: invoice.DueDate,
            Status: invoice.Status.ToString(),
            CustomerId: invoice.CustomerId,
            CustomerName: invoice.CustomerName,
            ReservationId: invoice.ReservationId,
            LineItems: invoice.LineItems.Select(li => new InvoiceLineItemDto(
                Position: li.Position,
                Description: li.Description,
                Quantity: li.Quantity,
                Unit: li.Unit,
                UnitPriceNet: li.UnitPriceNet,
                VatRate: li.VatRate,
                TotalNet: li.TotalNet,
                TotalGross: li.TotalGross)).ToList(),
            TotalNet: invoice.TotalNet,
            TotalVat: invoice.TotalVat,
            TotalGross: invoice.TotalGross,
            CurrencyCode: invoice.CurrencyCode,
            CreatedAt: invoice.CreatedAt,
            SentAt: invoice.SentAt,
            PaidAt: invoice.PaidAt);
    }
}

/// <summary>
///     DTO for invoice data.
/// </summary>
public sealed record InvoiceDto(
    Guid Id,
    string InvoiceNumber,
    DateOnly InvoiceDate,
    DateOnly ServiceDate,
    DateOnly DueDate,
    string Status,
    Guid CustomerId,
    string CustomerName,
    Guid ReservationId,
    IReadOnlyList<InvoiceLineItemDto> LineItems,
    decimal TotalNet,
    decimal TotalVat,
    decimal TotalGross,
    string CurrencyCode,
    DateTime CreatedAt,
    DateTime? SentAt,
    DateTime? PaidAt);

/// <summary>
///     DTO for invoice line item.
/// </summary>
public sealed record InvoiceLineItemDto(
    int Position,
    string Description,
    int Quantity,
    string Unit,
    decimal UnitPriceNet,
    decimal VatRate,
    decimal TotalNet,
    decimal TotalGross);

/// <summary>
///     Request to send invoice via email.
/// </summary>
/// <param name="RecipientEmail">Customer's email address.</param>
public sealed record SendInvoiceEmailRequest(string RecipientEmail);

/// <summary>
///     Result of sending invoice email.
/// </summary>
/// <param name="Success">Whether the email was sent successfully.</param>
/// <param name="ProviderMessageId">Message ID from the email provider.</param>
/// <param name="Message">Success or error message.</param>
public sealed record SendEmailResultDto(
    bool Success,
    string? ProviderMessageId,
    string Message);
