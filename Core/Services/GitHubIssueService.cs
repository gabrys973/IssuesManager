using Core.Exceptions;
using Core.Interfaces.Services;
using Core.Mappers.GitHub;
using Core.Models;
using Core.Models.GitHub;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Core.Services;

public sealed class GitHubIssueService : IIssueService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.github.com";
    private readonly string _serviceName = "GitHub";

    public GitHubIssueService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IssueResponse> CreateIssueAsync(string owner, string repository, IssueRequest request)
    {
        var url = $"{BaseUrl}/repos/{owner}/{repository}/issues";

        var requestBody = JsonSerializer.Serialize(new
        {
            title = request.Title,
            body = request.Description
        });

        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if(!response.IsSuccessStatusCode)
        {
            await ThrowExternalException(response);
        }

        var githubIssue = await response.Content.ReadFromJsonAsync<GitHubIssueResponse>();
        return githubIssue.MapToIssueResponse();
    }

    public async Task<IssueResponse> UpdateIssueAsync(string owner, string repository, string issueId, IssueRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<IssueResponse> CloseIssueAsync(string owner, string repository, string issueId)
    {
        throw new NotImplementedException();
    }

    private async Task ThrowExternalException(HttpResponseMessage response)
    {
        var message = await response.Content.ReadAsStringAsync();
        throw new ExternalErrorException(_serviceName, message);
    }
}