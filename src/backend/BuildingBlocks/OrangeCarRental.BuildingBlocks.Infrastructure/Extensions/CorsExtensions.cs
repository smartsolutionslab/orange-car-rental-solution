using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;

/// <summary>
///     CORS policy names for the Orange Car Rental application.
/// </summary>
public static class CorsPolicies
{
    /// <summary>
    ///     Policy allowing the Public Portal (customer-facing).
    ///     Origins: Shell (4300), Public Portal (4301)
    /// </summary>
    public const string PublicPortal = "AllowPublicPortal";

    /// <summary>
    ///     Policy allowing the Call Center Portal (staff-facing).
    ///     Origins: Shell (4300), Call Center Portal (4302)
    /// </summary>
    public const string CallCenterPortal = "AllowCallCenterPortal";

    /// <summary>
    ///     Policy allowing all frontend applications.
    ///     Origins: Shell (4300), Public Portal (4301), Call Center Portal (4302)
    /// </summary>
    public const string AllFrontends = "AllowAllFrontends";
}

/// <summary>
///     Extension methods for configuring CORS policies.
/// </summary>
public static class CorsExtensions
{
    private static readonly string[] PublicPortalOrigins =
    [
        "http://localhost:4300",
        "https://localhost:4300",
        "http://localhost:4301",
        "https://localhost:4301"
    ];

    private static readonly string[] CallCenterPortalOrigins =
    [
        "http://localhost:4300",
        "https://localhost:4300",
        "http://localhost:4302",
        "https://localhost:4302"
    ];

    private static readonly string[] AllFrontendOrigins =
    [
        "http://localhost:4300",
        "https://localhost:4300",
        "http://localhost:4301",
        "https://localhost:4301",
        "http://localhost:4302",
        "https://localhost:4302"
    ];

    /// <summary>
    ///     Adds CORS policies for Orange Car Rental frontends.
    ///     Configures three policies: PublicPortal, CallCenterPortal, and AllFrontends.
    /// </summary>
    public static IServiceCollection AddOrangeCarRentalCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicies.PublicPortal, policy =>
            {
                policy.WithOrigins(PublicPortalOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            options.AddPolicy(CorsPolicies.CallCenterPortal, policy =>
            {
                policy.WithOrigins(CallCenterPortalOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });

            options.AddPolicy(CorsPolicies.AllFrontends, policy =>
            {
                policy.WithOrigins(AllFrontendOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    ///     Adds CORS policy for the Public Portal only.
    ///     Use this for services that only serve customer-facing operations.
    /// </summary>
    public static IServiceCollection AddPublicPortalCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicies.PublicPortal, policy =>
            {
                policy.WithOrigins(PublicPortalOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    ///     Adds CORS policy for the Call Center Portal only.
    ///     Use this for services that only serve internal staff operations.
    /// </summary>
    public static IServiceCollection AddCallCenterPortalCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicies.CallCenterPortal, policy =>
            {
                policy.WithOrigins(CallCenterPortalOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    /// <summary>
    ///     Uses the Public Portal CORS policy.
    /// </summary>
    public static IApplicationBuilder UsePublicPortalCors(this IApplicationBuilder app)
    {
        return app.UseCors(CorsPolicies.PublicPortal);
    }

    /// <summary>
    ///     Uses the Call Center Portal CORS policy.
    /// </summary>
    public static IApplicationBuilder UseCallCenterPortalCors(this IApplicationBuilder app)
    {
        return app.UseCors(CorsPolicies.CallCenterPortal);
    }

    /// <summary>
    ///     Uses the AllFrontends CORS policy.
    /// </summary>
    public static IApplicationBuilder UseAllFrontendsCors(this IApplicationBuilder app)
    {
        return app.UseCors(CorsPolicies.AllFrontends);
    }
}