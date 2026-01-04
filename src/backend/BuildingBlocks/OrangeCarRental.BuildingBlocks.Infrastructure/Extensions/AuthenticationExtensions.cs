using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;

/// <summary>
/// Extension methods for configuring JWT authentication with Keycloak.
/// </summary>
public static class AuthenticationExtensions
{
    /// <summary>
    /// Adds JWT Bearer authentication configured for Keycloak.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">Application configuration</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddJwtAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var keycloakConfig = configuration.GetSection("Authentication:Keycloak");

        var authority = keycloakConfig["Authority"]
            ?? throw new InvalidOperationException("Keycloak Authority is not configured");
        var audience = keycloakConfig["Audience"]
            ?? throw new InvalidOperationException("Keycloak Audience is not configured");
        var requireHttpsMetadata = keycloakConfig.GetValue<bool>("RequireHttpsMetadata", true);
        var validateIssuer = keycloakConfig.GetValue<bool>("ValidateIssuer", true);
        var validateAudience = keycloakConfig.GetValue<bool>("ValidateAudience", true);
        var validateLifetime = keycloakConfig.GetValue<bool>("ValidateLifetime", true);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.Authority = authority;
            options.Audience = audience;
            options.RequireHttpsMetadata = requireHttpsMetadata;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = validateIssuer,
                ValidateAudience = validateAudience,
                ValidateLifetime = validateLifetime,
                ValidateIssuerSigningKey = true,
                ValidAudience = audience,
                ValidIssuer = authority,
                ClockSkew = TimeSpan.FromMinutes(5), // Allow 5 minutes clock skew
                RoleClaimType = "roles" // Match Keycloak's role claim name
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.Headers["Token-Expired"] = "true";
                    }
                    return Task.CompletedTask;
                }
            };
        });

        return services;
    }

    /// <summary>
    /// Adds authorization policies for Orange Car Rental roles.
    /// </summary>
    /// <param name="services">The service collection</param>
    /// <returns>The service collection for chaining</returns>
    public static IServiceCollection AddOrangeCarRentalAuthorization(
        this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Customer policy - can access customer-facing features
            options.AddPolicy("CustomerPolicy", policy =>
                policy.RequireRole("customer"));

            // Call Center policy - can manage reservations and customers
            options.AddPolicy("CallCenterPolicy", policy =>
                policy.RequireRole("call_center"));

            // Fleet Manager policy - can manage vehicles and locations
            options.AddPolicy("FleetManagerPolicy", policy =>
                policy.RequireRole("fleet_manager"));

            // Admin policy - full system access
            options.AddPolicy("AdminPolicy", policy =>
                policy.RequireRole("admin"));

            // Combined policies for common scenarios
            options.AddPolicy("CallCenterOrAdminPolicy", policy =>
                policy.RequireRole("call_center", "admin"));

            options.AddPolicy("FleetManagerOrAdminPolicy", policy =>
                policy.RequireRole("fleet_manager", "admin"));

            options.AddPolicy("CustomerOrCallCenterOrAdminPolicy", policy =>
                policy.RequireRole("customer", "call_center", "admin"));

            options.AddPolicy("AuthenticatedUserPolicy", policy =>
                policy.RequireAuthenticatedUser());
        });

        return services;
    }
}
