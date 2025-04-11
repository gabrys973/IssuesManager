using Core.Interfaces.Services;
using Core.Mappers.GitHub;
using Core.Models;
using Core.Models.GitHub;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Core.Services.GitHub;

public sealed class GitHubIssueService(HttpClient httpClient) : IssueService(httpClient, "https://api.github.com", "GitHub"), IIssueService
{
    public async Task<IssueResponse> CreateIssueAsync(string owner, string repository, IssueRequest request)
    {
        CheckIfTokenEmptyOrNull();

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
        CheckIfTokenEmptyOrNull();

        var url = $"{BaseUrl}/repos/{owner}/{repository}/issues/{issueId}";

        var requestBody = JsonSerializer.Serialize(new
        {
            title = request.Title,
            body = request.Description
        });

        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync(url, content);

        if(!response.IsSuccessStatusCode)
        {
            await ThrowExternalException(response);
        }

        var githubIssue = await response.Content.ReadFromJsonAsync<GitHubIssueResponse>();
        return githubIssue.MapToIssueResponse();
    }

    public async Task<IssueResponse> CloseIssueAsync(string owner, string repository, string issueId)
    {
        CheckIfTokenEmptyOrNull();

        var url = $"{BaseUrl}/repos/{owner}/{repository}/issues/{issueId}";

        var requestBody = JsonSerializer.Serialize(new
        {
            state = "closed"
        });

        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PatchAsync(url, content);

        if(!response.IsSuccessStatusCode)
        {
            await ThrowExternalException(response);
        }

        var githubIssue = await response.Content.ReadFromJsonAsync<GitHubIssueResponse>();
        return githubIssue.MapToIssueResponse();
    }
}