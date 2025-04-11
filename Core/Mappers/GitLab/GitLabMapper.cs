using Core.Models;
using Core.Models.GitLab;

namespace Core.Mappers.GitLab;

internal static class GitLabMapper
{
    public static IssueResponse MapToIssueResponse(this GitLabIssueResponse issue)
    {
        return new IssueResponse(issue.Iid.ToString(), issue.Title, issue.Description, issue.State, issue.WebUrl);
    }
}