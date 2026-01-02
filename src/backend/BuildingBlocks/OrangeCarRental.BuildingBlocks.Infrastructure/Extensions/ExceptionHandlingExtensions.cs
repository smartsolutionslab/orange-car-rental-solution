using Microsoft.AspNetCore.Builder;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Middleware;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;

/// <summary>
///     Extension methods for configuring exception handling.
/// </summary>
public static class ExceptionHandlingExtensions
{
    /// <summary>
    ///     Adds the exception handling middleware to the pipeline.
    ///     This middleware catches exceptions and converts them to appropriate HTTP responses:
    ///     <list type="bullet">
    ///         <item><description>ValidationException, ArgumentException → 400 Bad Request</description></item>
    ///         <item><description>EntityNotFoundException → 404 Not Found</description></item>
    ///         <item><description>ConflictException → 409 Conflict</description></item>
    ///         <item><description>InvalidOperationException with "not found" → 404 Not Found</description></item>
    ///         <item><description>InvalidOperationException (other) → 400 Bad Request</description></item>
    ///         <item><description>UnauthorizedAccessException → 401 Unauthorized</description></item>
    ///         <item><description>Other exceptions → 500 Internal Server Error</description></item>
    ///     </list>
    /// </summary>
    public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
