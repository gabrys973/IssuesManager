using Core.Models;
using Core.Models.GitHub;

namespace Core.Mappers.GitHub;

internal static class GitHubMapper
{
    public static IssueResponse MapToIssueResponse(this GitHubIssueResponse issue)
    {
        return new IssueResponse(issue.Number.ToString(), issue.Title, issue.Body, issue.State, issue.HtmlUrl);
    }
}