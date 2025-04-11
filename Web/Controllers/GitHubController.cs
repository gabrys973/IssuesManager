using Core.Exceptions;
using Core.Models;
using Core.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers;

[ApiController]
[Route("api/github")]
public class GitHubController : ControllerBase
{
    private readonly GitHubIssueService _gitHubIssueService;
    private readonly IValidator<IssueRequest> _validator;

    public GitHubController(GitHubIssueService gitHubIssueService, IValidator<IssueRequest> validator)
    {
        _gitHubIssueService = gitHubIssueService;
        _validator = validator;
    }

    [HttpPost("{owner}/{repo}")]
    [Authorize]
    public async Task<ActionResult<IssueResponse>> CreateIssue([FromRoute] string owner, [FromRoute] string repo, [FromBody] IssueRequest request)
    {
        var validationResult = _validator.Validate(request);
        if(!validationResult.IsValid)
            throw new ValidationErrorException(validationResult.ToDictionary());

        var issue = await _gitHubIssueService.CreateIssueAsync(owner, repo, request);
        return Ok(issue);
    }

    [HttpPatch("{owner}/{repo}/{issueId}/close")]
    [Authorize]
    public async Task<ActionResult<IssueResponse>> CloseIssue([FromRoute] string owner, [FromRoute] string repo, [FromRoute] string issueId)
    {
        var issue = await _gitHubIssueService.CloseIssueAsync(owner, repo, issueId);
        return Ok(issue);
    }
}