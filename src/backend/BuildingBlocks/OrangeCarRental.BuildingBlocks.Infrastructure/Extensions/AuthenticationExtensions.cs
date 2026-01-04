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

        // Disable default claim type mapping to preserve JWT claim names as-is
        // Without this, ASP.NET Core maps claim types like "roles" to different names
        System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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

            // Also disable mapping via MapInboundClaims
            options.MapInboundClaims = false;

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
                    Console.WriteLine($"[JWT] Authentication failed: {context.Exception.GetType().Name} - {context.Exception.Message}");
                    if (context.Exception is SecurityTokenExpiredException)
                    {
                        context.Response.Headers["Token-Expired"] = "true";
                    }
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    // CRITICAL: Ensure the ClaimsIdentity uses "roles" as the RoleClaimType
                    // The new JsonWebTokenHandler in .NET 9 may not respect TokenValidationParameters.RoleClaimType
                    // when creating the ClaimsIdentity, so we need to manually fix it here.
                    if (context.Principal != null)
                    {
                        var existingIdentity = context.Principal.Identity as System.Security.Claims.ClaimsIdentity;
                        if (existingIdentity != null && existingIdentity.RoleClaimType != "roles")
                        {
                            Console.WriteLine($"[JWT] Identity RoleClaimType was '{existingIdentity.RoleClaimType}', recreating with 'roles'");

                            // Create new identity with the correct RoleClaimType
                            var newIdentity = new System.Security.Claims.ClaimsIdentity(
                                existingIdentity.Claims,
                                existingIdentity.AuthenticationType,
                                existingIdentity.NameClaimType,
                                "roles"); // Set RoleClaimType to "roles"

                            context.Principal = new System.Security.Claims.ClaimsPrincipal(newIdentity);
                        }
                    }

                    var claims = context.Principal?.Claims.Select(c => $"{c.Type}={c.Value}").ToList();
                    Console.WriteLine($"[JWT] Token validated. Claims: {string.Join(", ", claims ?? [])}");

                    // Log roles specifically
                    var roles = context.Principal?.FindAll("roles").Select(c => c.Value).ToList();
                    Console.WriteLine($"[JWT] Roles from 'roles' claim: {string.Join(", ", roles ?? [])}");

                    // Also check for role claim type
                    var roleClaimRoles = context.Principal?.FindAll(context.Principal?.Identities.First().RoleClaimType ?? "roles").Select(c => c.Value).ToList();
                    Console.WriteLine($"[JWT] Roles from RoleClaimType '{context.Principal?.Identities.First().RoleClaimType}': {string.Join(", ", roleClaimRoles ?? [])}");

                    return Task.CompletedTask;
                },
                OnMessageReceived = context =>
                {
                    var hasAuth = context.Request.Headers.ContainsKey("Authorization");
                    Console.WriteLine($"[JWT] Message received. Has Authorization header: {hasAuth}");
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    Console.WriteLine($"[JWT] Challenge issued. Error: {context.Error}, Description: {context.ErrorDescription}");
                    return Task.CompletedTask;
                },
                OnForbidden = context =>
                {
                    Console.WriteLine($"[JWT] Forbidden. User: {context.Principal?.Identity?.Name}, IsAuthenticated: {context.Principal?.Identity?.IsAuthenticated}");
                    var roles = context.Principal?.FindAll("roles").Select(c => c.Value).ToList();
                    Console.WriteLine($"[JWT] User roles: {string.Join(", ", roles ?? [])}");
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
