using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Payments.Api.Requests;
using SmartSolutionsLab.OrangeCarRental.Payments.Api.Shared;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Commands;
using SmartSolutionsLab.OrangeCarRental.Payments.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;
using SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Api.Endpoints;

public static class InvoiceEndpoints
{
    public static IEndpointRouteBuilder MapInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        var invoices = app.MapGroup("/api/invoices")
            .WithTags("Invoices");

        // Generate invoice for a reservation
        invoices.MapPost("/generate", async Task<Results<Ok<GenerateInvoiceResult>, BadRequest<ProblemDetails>>> (
                GenerateInvoiceRequest request,
                ICommandHandler<GenerateInvoiceCommand, GenerateInvoiceResult> handler,
                CancellationToken cancellationToken) =>
            {
                try
                {
                    var command = new GenerateInvoiceCommand(
                        ReservationId.From(request.ReservationId),
                        CustomerId.From(request.CustomerId),
                        PersonName.Of(request.CustomerName),
                        Street.From(request.CustomerStreet),
                        PostalCode.From(request.CustomerPostalCode),
                        City.From(request.CustomerCity),
                        Country.From(request.CustomerCountry),
                        VatId.FromNullable(request.CustomerVatId),
                        request.VehicleDescription,
                        request.RentalDays,
                        Money.Euro(request.DailyRateNet),
                        request.PickupDate,
                        request.ReturnDate,
                        request.PaymentId.HasValue ? PaymentIdentifier.From(request.PaymentId.Value) : null);

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
                    .GetByReservationIdAsync(ReservationId.From(reservationId), cancellationToken);

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
                    .GetByCustomerIdAsync(CustomerId.From(customerId), cancellationToken);

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
            CustomerId: invoice.Customer.CustomerId.Value,
            CustomerName: invoice.Customer.Name,
            ReservationId: invoice.ReservationId.Value,
            LineItems: invoice.LineItems.Select(li => new InvoiceLineItemDto(
                Position: li.Position,
                Description: li.Description,
                Quantity: li.Quantity.Value,
                Unit: li.Quantity.Unit,
                UnitPriceNet: li.UnitPrice.NetAmount,
                VatRate: li.VatRate.Value,
                TotalNet: li.TotalNet,
                TotalGross: li.TotalGross)).ToList(),
            TotalNet: invoice.TotalNet,
            TotalVat: invoice.TotalVat,
            TotalGross: invoice.TotalGross,
            CurrencyCode: invoice.Currency.Code,
            CreatedAt: invoice.CreatedAt,
            SentAt: invoice.SentAt,
            PaidAt: invoice.PaidAt);
    }
}
