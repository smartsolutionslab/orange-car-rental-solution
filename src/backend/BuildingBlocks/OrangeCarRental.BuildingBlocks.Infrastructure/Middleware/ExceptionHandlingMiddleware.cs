using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Middleware;

/// <summary>
///     Middleware that handles exceptions and converts them to appropriate HTTP responses.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, problemDetails) = exception switch
        {
            ValidationException validationEx => (
                StatusCodes.Status400BadRequest,
                CreateProblemDetails(
                    StatusCodes.Status400BadRequest,
                    "Validation Error",
                    validationEx.Message,
                    context.Request.Path)),

            ArgumentException argEx => (
                StatusCodes.Status400BadRequest,
                CreateProblemDetails(
                    StatusCodes.Status400BadRequest,
                    "Invalid Request",
                    argEx.Message,
                    context.Request.Path)),

            EntityNotFoundException notFoundEx => (
                StatusCodes.Status404NotFound,
                CreateProblemDetails(
                    StatusCodes.Status404NotFound,
                    "Not Found",
                    notFoundEx.Message,
                    context.Request.Path)),

            ConflictException conflictEx => (
                StatusCodes.Status409Conflict,
                CreateProblemDetails(
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    conflictEx.Message,
                    context.Request.Path)),

            InvalidOperationException invalidOpEx when invalidOpEx.Message.Contains("not found", StringComparison.OrdinalIgnoreCase) => (
                StatusCodes.Status404NotFound,
                CreateProblemDetails(
                    StatusCodes.Status404NotFound,
                    "Not Found",
                    invalidOpEx.Message,
                    context.Request.Path)),

            InvalidOperationException invalidOpEx => (
                StatusCodes.Status400BadRequest,
                CreateProblemDetails(
                    StatusCodes.Status400BadRequest,
                    "Operation Failed",
                    invalidOpEx.Message,
                    context.Request.Path)),

            UnauthorizedAccessException => (
                StatusCodes.Status401Unauthorized,
                CreateProblemDetails(
                    StatusCodes.Status401Unauthorized,
                    "Unauthorized",
                    "You are not authorized to access this resource.",
                    context.Request.Path)),

            _ => (
                StatusCodes.Status500InternalServerError,
                CreateProblemDetails(
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred. Please try again later.",
                    context.Request.Path))
        };

        // Log the exception
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Unhandled exception occurred: {Message}", exception.Message);
        }
        else
        {
            _logger.LogWarning(exception, "Request failed with {StatusCode}: {Message}", statusCode, exception.Message);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/problem+json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(problemDetails, JsonOptions));
    }

    private static ProblemDetails CreateProblemDetails(
        int statusCode,
        string title,
        string detail,
        string instance)
    {
        return new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = instance
        };
    }
}
