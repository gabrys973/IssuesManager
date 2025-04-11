using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/github")]
public class GitHubController : ControllerBase
{
    private readonly GitHubIssueService _gitHubIssueService;

    public GitHubController(GitHubIssueService gitHubIssueService)
    {
        _gitHubIssueService = gitHubIssueService;
    }

    [HttpPost("{owner}/{repo}")]
    public async Task<ActionResult<IssueResponse>> CreateIssue([FromRoute] string owner, [FromRoute] string repo, [FromBody] IssueRequest request)
    {
        var issue = await _gitHubIssueService.CreateIssueAsync(owner, repo, request);
        return Ok(issue);
    }
}