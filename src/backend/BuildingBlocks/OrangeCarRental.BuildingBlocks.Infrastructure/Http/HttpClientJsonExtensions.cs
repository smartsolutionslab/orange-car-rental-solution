using System.Net.Http.Json;
using System.Text.Json;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Http;

/// <summary>
///     Extension methods for HttpClient JSON operations with standardized error handling.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class HttpClientJsonExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for HttpClient.
    /// </summary>
    extension(HttpClient client)
    {
        /// <summary>
        ///     Sends a POST request with JSON content and deserializes the response.
        /// </summary>
        /// <typeparam name="TRequest">The request body type.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="request">The request body to serialize.</param>
        /// <param name="serviceName">The name of the service for error messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The deserialized response.</returns>
        public async Task<TResponse> PostJsonAsync<TRequest, TResponse>(
            string requestUri,
            TRequest request,
            string serviceName,
            CancellationToken cancellationToken = default)
        {
            var response = await client.PostAsJsonAsync(requestUri, request, JsonDefaults.HttpOptions, cancellationToken);
            return await HandleResponseAsync<TResponse>(response, serviceName, requestUri, cancellationToken);
        }

        /// <summary>
        ///     Sends a POST request with JSON content without expecting a response body.
        /// </summary>
        /// <typeparam name="TRequest">The request body type.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="request">The request body to serialize.</param>
        /// <param name="serviceName">The name of the service for error messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task PostJsonAsync<TRequest>(
            string requestUri,
            TRequest request,
            string serviceName,
            CancellationToken cancellationToken = default)
        {
            var response = await client.PostAsJsonAsync(requestUri, request, JsonDefaults.HttpOptions, cancellationToken);
            await EnsureSuccessAsync(response, serviceName, requestUri, cancellationToken);
        }

        /// <summary>
        ///     Sends a GET request and deserializes the JSON response.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="serviceName">The name of the service for error messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The deserialized response.</returns>
        public async Task<TResponse> GetJsonAsync<TResponse>(
            string requestUri,
            string serviceName,
            CancellationToken cancellationToken = default)
        {
            var response = await client.GetAsync(requestUri, cancellationToken);
            return await HandleResponseAsync<TResponse>(response, serviceName, requestUri, cancellationToken);
        }

        /// <summary>
        ///     Sends a GET request and deserializes the JSON response, returning null on 404.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="serviceName">The name of the service for error messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The deserialized response, or null if not found.</returns>
        public async Task<TResponse?> GetJsonOrDefaultAsync<TResponse>(
            string requestUri,
            string serviceName,
            CancellationToken cancellationToken = default)
            where TResponse : class
        {
            var response = await client.GetAsync(requestUri, cancellationToken);

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

            return await HandleResponseAsync<TResponse>(response, serviceName, requestUri, cancellationToken);
        }

        /// <summary>
        ///     Sends a PUT request with JSON content and deserializes the response.
        /// </summary>
        /// <typeparam name="TRequest">The request body type.</typeparam>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="request">The request body to serialize.</param>
        /// <param name="serviceName">The name of the service for error messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The deserialized response.</returns>
        public async Task<TResponse> PutJsonAsync<TRequest, TResponse>(
            string requestUri,
            TRequest request,
            string serviceName,
            CancellationToken cancellationToken = default)
        {
            var response = await client.PutAsJsonAsync(requestUri, request, JsonDefaults.HttpOptions, cancellationToken);
            return await HandleResponseAsync<TResponse>(response, serviceName, requestUri, cancellationToken);
        }

        /// <summary>
        ///     Sends a PUT request with JSON content without expecting a response body.
        /// </summary>
        /// <typeparam name="TRequest">The request body type.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="request">The request body to serialize.</param>
        /// <param name="serviceName">The name of the service for error messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task PutJsonAsync<TRequest>(
            string requestUri,
            TRequest request,
            string serviceName,
            CancellationToken cancellationToken = default)
        {
            var response = await client.PutAsJsonAsync(requestUri, request, JsonDefaults.HttpOptions, cancellationToken);
            await EnsureSuccessAsync(response, serviceName, requestUri, cancellationToken);
        }

        /// <summary>
        ///     Sends a DELETE request and ensures success.
        /// </summary>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="serviceName">The name of the service for error messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        public async Task DeleteAsync(
            string requestUri,
            string serviceName,
            CancellationToken cancellationToken = default)
        {
            var response = await client.DeleteAsync(requestUri, cancellationToken);
            await EnsureSuccessAsync(response, serviceName, requestUri, cancellationToken);
        }

        /// <summary>
        ///     Sends a DELETE request and deserializes the response.
        /// </summary>
        /// <typeparam name="TResponse">The expected response type.</typeparam>
        /// <param name="requestUri">The request URI.</param>
        /// <param name="serviceName">The name of the service for error messages.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The deserialized response.</returns>
        public async Task<TResponse> DeleteJsonAsync<TResponse>(
            string requestUri,
            string serviceName,
            CancellationToken cancellationToken = default)
        {
            var response = await client.DeleteAsync(requestUri, cancellationToken);
            return await HandleResponseAsync<TResponse>(response, serviceName, requestUri, cancellationToken);
        }
    }

    private static async Task<TResponse> HandleResponseAsync<TResponse>(
        HttpResponseMessage response,
        string serviceName,
        string requestUri,
        CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpServiceException(serviceName, requestUri, response.StatusCode, errorContent);
        }

        try
        {
            var result = await response.Content.ReadFromJsonAsync<TResponse>(JsonDefaults.HttpOptions, cancellationToken);
            return result ?? throw new HttpServiceException(
                serviceName,
                requestUri,
                response.StatusCode,
                "Response body was null or could not be deserialized");
        }
        catch (JsonException ex)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpServiceException(
                serviceName,
                requestUri,
                response.StatusCode,
                $"Failed to deserialize response: {content}",
                ex);
        }
    }

    private static async Task EnsureSuccessAsync(
        HttpResponseMessage response,
        string serviceName,
        string requestUri,
        CancellationToken cancellationToken)
    {
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new HttpServiceException(serviceName, requestUri, response.StatusCode, errorContent);
        }
    }
}
