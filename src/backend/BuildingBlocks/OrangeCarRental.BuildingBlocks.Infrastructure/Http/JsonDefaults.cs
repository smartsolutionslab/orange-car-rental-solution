using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Http;

/// <summary>
///     Centralized JSON serialization options for HTTP communication.
/// </summary>
public static class JsonDefaults
{
    /// <summary>
    ///     Default JSON options for HTTP communication between services.
    ///     Thread-safe and reusable across all HTTP client operations.
    /// </summary>
    public static JsonSerializerOptions HttpOptions { get; } = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        WriteIndented = false
    };
}
