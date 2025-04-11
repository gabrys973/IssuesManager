using Core.Exceptions;

namespace Core.Services;

public abstract class IssueService
{
    protected readonly HttpClient _httpClient;
    protected readonly string BaseUrl;
    protected readonly string _serviceName;

    protected IssueService(HttpClient httpClient, string baseUrl, string serviceName)
    {
        _httpClient = httpClient;
        BaseUrl = baseUrl;
        _serviceName = serviceName;
    }

    protected async Task ThrowExternalException(HttpResponseMessage response)
    {
        var message = await response.Content.ReadAsStringAsync();
        throw new ExternalErrorException(_serviceName, message);
    }

    protected void CheckIfTokenEmptyOrNull()
    {
        _httpClient.DefaultRequestHeaders.TryGetValues("Authorization", out var authHeader);

        if(string.IsNullOrEmpty(authHeader?.FirstOrDefault()))
            throw new TokenEmptyException(_serviceName);
    }
}