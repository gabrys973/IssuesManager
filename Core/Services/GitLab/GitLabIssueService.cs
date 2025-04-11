using Core.Interfaces.Services;
using Core.Mappers.GitLab;
using Core.Models;
using Core.Models.GitLab;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Core.Services.GitLab;

public class GitLabIssueService(HttpClient httpClient) : IssueService(httpClient, "https://gitlab.com/api/v4", "GitLab"), IIssueService
{
    public async Task<IssueResponse> CreateIssueAsync(string? owner, string projectId, IssueRequest request)
    {
        CheckIfTokenEmptyOrNull();

        var url = $"{BaseUrl}/projects/{projectId}/issues";

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

        var gitLabIssue = await response.Content.ReadFromJsonAsync<GitLabIssueResponse>();
        return gitLabIssue.MapToIssueResponse();
    }

    public async Task<IssueResponse> UpdateIssueAsync(string? owner, string projectId, string issueId, IssueRequest request)
    {
        CheckIfTokenEmptyOrNull();

        var url = $"{BaseUrl}/projects/{projectId}/issues/{issueId}";

        var requestBody = JsonSerializer.Serialize(new
        {
            title = request.Title,
            description = request.Description
        });

        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(url, content);

        if(!response.IsSuccessStatusCode)
        {
            await ThrowExternalException(response);
        }

        var gitLabIssue = await response.Content.ReadFromJsonAsync<GitLabIssueResponse>();
        return gitLabIssue.MapToIssueResponse();
    }

    public async Task<IssueResponse> CloseIssueAsync(string? owner, string projectId, string issueId)
    {
        CheckIfTokenEmptyOrNull();

        var url = $"{BaseUrl}/projects/{projectId}/issues/{issueId}";

        var requestBody = JsonSerializer.Serialize(new
        {
            state_event = "close"
        });

        var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(url, content);

        if(!response.IsSuccessStatusCode)
        {
            await ThrowExternalException(response);
        }

        var gitLabIssue = await response.Content.ReadFromJsonAsync<GitLabIssueResponse>();
        return gitLabIssue.MapToIssueResponse();
    }
}