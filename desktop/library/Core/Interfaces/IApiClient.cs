namespace library.Core.Interfaces
{
    public interface IApiClient
    {
        Task<TResponse> GetAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
        Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken cancellationToken = default);
        Task<TResponse> PutAsync<TRequest, TResponse>(string endpoint, TRequest data, CancellationToken cancellationToken = default);
        Task<TResponse> DeleteAsync<TResponse>(string endpoint, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
    }
}