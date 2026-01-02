using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace SmartSolutionsLab.OrangeCarRental.IntegrationTests.Infrastructure;

/// <summary>
/// Provides OAuth tokens from Keycloak for integration tests.
/// </summary>
public class KeycloakTokenProvider
{
    private readonly HttpClient _httpClient;
    private readonly string _tokenEndpoint;
    private readonly string _clientId;
    private readonly Dictionary<string, CachedToken> _tokenCache = new();

    public KeycloakTokenProvider(string keycloakBaseUrl, string realm = "orange-car-rental", string clientId = "orange-car-rental-api")
    {
        _httpClient = new HttpClient();
        _tokenEndpoint = $"{keycloakBaseUrl}/realms/{realm}/protocol/openid-connect/token";
        _clientId = clientId;
    }

    /// <summary>
    /// Gets an access token for a test user using Resource Owner Password Credentials grant.
    /// Tokens are cached until they expire.
    /// </summary>
    public async Task<string> GetTokenAsync(string username, string password)
    {
        var cacheKey = $"{username}:{password}";

        if (_tokenCache.TryGetValue(cacheKey, out var cached) && !cached.IsExpired)
        {
            return cached.AccessToken;
        }

        var tokenRequest = new Dictionary<string, string>
        {
            ["grant_type"] = "password",
            ["client_id"] = _clientId,
            ["username"] = username,
            ["password"] = password
        };

        var response = await _httpClient.PostAsync(_tokenEndpoint, new FormUrlEncodedContent(tokenRequest));

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new InvalidOperationException($"Failed to get token from Keycloak: {response.StatusCode} - {error}");
        }

        var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();

        if (tokenResponse?.AccessToken == null)
        {
            throw new InvalidOperationException("Token response did not contain an access token");
        }

        _tokenCache[cacheKey] = new CachedToken(tokenResponse.AccessToken, tokenResponse.ExpiresIn);

        return tokenResponse.AccessToken;
    }

    /// <summary>
    /// Gets a token for a customer test user.
    /// </summary>
    public Task<string> GetCustomerTokenAsync() => GetTokenAsync("test-customer", "test123");

    /// <summary>
    /// Gets a token for a call center test user.
    /// </summary>
    public Task<string> GetCallCenterTokenAsync() => GetTokenAsync("test-callcenter", "test123");

    /// <summary>
    /// Gets a token for a fleet manager test user.
    /// </summary>
    public Task<string> GetFleetManagerTokenAsync() => GetTokenAsync("test-fleetmanager", "test123");

    /// <summary>
    /// Gets a token for an admin test user.
    /// </summary>
    public Task<string> GetAdminTokenAsync() => GetTokenAsync("test-admin", "test123");

    private class TokenResponse
    {
        [JsonPropertyName("access_token")]
        public string? AccessToken { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
    }

    private class CachedToken
    {
        public string AccessToken { get; }
        public DateTime ExpiresAt { get; }

        public CachedToken(string accessToken, int expiresInSeconds)
        {
            AccessToken = accessToken;
            // Subtract 60 seconds buffer to ensure token is still valid when used
            ExpiresAt = DateTime.UtcNow.AddSeconds(expiresInSeconds - 60);
        }

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    }
}
