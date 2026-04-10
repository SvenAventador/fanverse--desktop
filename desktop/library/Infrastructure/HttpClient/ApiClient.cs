using library.Core.Interfaces;
using library.Core.Models.Common;
using library.Infrastructure.Storage;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace library.Infrastructure.HttpClient
{
    public class ApiClient : IApiClient, IDisposable
    {
        private readonly System.Net.Http.HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private bool _disposed;

        public ApiClient(string baseUrl)
        {
            _httpClient = new System.Net.Http.HttpClient
            {
                BaseAddress = new Uri(baseUrl),
                Timeout = TimeSpan.FromSeconds(30)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DictionaryKeyPolicy = JsonNamingPolicy.CamelCase
            };

            SetupAuthHeader();
        }

        private void SetupAuthHeader()
        {
            var token = SecureStorage.GetToken();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        public async Task<TResponse> GetAsync<TResponse>(string endpoint,
                                                         CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            return await HandleResponse<TResponse>(response, cancellationToken);
        }

        public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint,
                                               TRequest data,
                                               CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);
            return await HandleResponse<TResponse>(response, cancellationToken);
        }

        public async Task<HttpResponseMessage> PostMultipartAsync(string endpoint,
                                                                  MultipartFormDataContent content,
                                                                  CancellationToken cancellationToken = default)
             => await _httpClient.PostAsync(endpoint, content, cancellationToken);

        public async Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint,
                                                                   TRequest data,
                                                                   CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync(endpoint, content, cancellationToken);
            return await HandleResponse<TResponse>(response, cancellationToken);
        }

        public async Task<TResponse> DeleteAsync<TResponse>(string endpoint,
                                                            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            return await HandleResponse<TResponse>(response, cancellationToken);
        }

        public async Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.DeleteAsync(endpoint, cancellationToken);
            return response.IsSuccessStatusCode;
        }

        private async Task<TResponse> HandleResponse<TResponse>(HttpResponseMessage response,
                                                                CancellationToken cancellationToken)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                if (typeof(TResponse) == typeof(string))
                    return (TResponse)(object)content;

                if (typeof(TResponse) == typeof(byte[]))
                    return (TResponse)(object)Encoding.UTF8.GetBytes(content);

                return JsonSerializer.Deserialize<TResponse>(content, _jsonOptions)!;
            }

            try
            {
                var error = JsonSerializer.Deserialize<ApiError>(content, _jsonOptions);
                throw new HttpRequestException(error?.ToString() ?? $"Ошибка: {(int)response.StatusCode}");
            }
            catch (JsonException)
            {
                throw new HttpRequestException($"Ошибка сервера: {(int)response.StatusCode} - {response.ReasonPhrase}");
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _httpClient?.Dispose();
                _disposed = true;
            }
        }
    }
}