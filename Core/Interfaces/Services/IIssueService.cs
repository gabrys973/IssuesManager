using Core.Models;

namespace Core.Interfaces.Services;

public interface IIssueService
{
    Task<IssueResponse> CreateIssueAsync(string? owner, string repository, IssueRequest request);

    Task<IssueResponse> UpdateIssueAsync(string? owner, string repository, string issueId, IssueRequest request);

    Task<IssueResponse> CloseIssueAsync(string? owner, string repository, string issueId);
}