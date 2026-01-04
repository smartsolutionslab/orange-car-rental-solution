using System.Text.Json;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.DependencyInjection;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;

/// <summary>
///     Extension methods for configuring JSON serialization in Minimal APIs.
/// </summary>
public static class JsonSerializerExtensions
{
    /// <summary>
    ///     Configures JSON serialization options for Minimal APIs.
    ///     Enables case-insensitive property matching for incoming JSON requests.
    /// </summary>
    public static IServiceCollection AddOrangeCarRentalJsonOptions(this IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = true;
            options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        });

        return services;
    }
}
