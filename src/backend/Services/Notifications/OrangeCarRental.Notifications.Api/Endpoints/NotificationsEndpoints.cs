using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Notifications.Application.Commands;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Api.Endpoints;

public static class NotificationsEndpoints
{
    public static IEndpointRouteBuilder MapNotificationsEndpoints(this IEndpointRouteBuilder app)
    {
        var notifications = app.MapGroup("/api/notifications")
            .WithTags("Notifications");

        // POST /api/notifications/email - Send email notification
        notifications.MapPost("/email", async (
            SendEmailCommand command,
            ICommandHandler<SendEmailCommand, SendEmailResult> handler,
            CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while sending the email");
                }
            })
            .WithName("SendEmail")
            .WithSummary("Send an email notification")
            .WithDescription("Sends an email notification to the specified recipient. The email will be tracked in the notifications database.")
            .Produces<SendEmailResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("AdminPolicy");

        // POST /api/notifications/sms - Send SMS notification
        notifications.MapPost("/sms", async (
            SendSmsCommand command,
            ICommandHandler<SendSmsCommand, SendSmsResult> handler,
            CancellationToken cancellationToken) =>
            {
                try
                {
                    var result = await handler.HandleAsync(command, cancellationToken);
                    return Results.Ok(result);
                }
                catch (ArgumentException ex)
                {
                    return Results.BadRequest(new { message = ex.Message });
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError,
                        title: "An error occurred while sending the SMS");
                }
            })
            .WithName("SendSms")
            .WithSummary("Send an SMS notification")
            .WithDescription("Sends an SMS notification to the specified phone number (German format +49). The SMS will be tracked in the notifications database.")
            .Produces<SendSmsResult>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .RequireAuthorization("AdminPolicy");

        return app;
    }
}
