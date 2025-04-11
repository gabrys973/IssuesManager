using Core.Exceptions;
using Core.Models;
using Core.Services.GitLab;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers.GitLab;

[ApiController]
[Route("api/gitlab")]
public class GitLabController : ControllerBase
{
    private readonly GitLabIssueService _gitLabIssueService;
    private readonly IValidator<IssueRequest> _validator;

    public GitLabController(GitLabIssueService gitLabIssueService, IValidator<IssueRequest> validator)
    {
        _gitLabIssueService = gitLabIssueService;
        _validator = validator;
    }

    [HttpPost("{projectId}")]
    [Authorize]
    public async Task<ActionResult<IssueResponse>> CreateIssue([FromRoute] string projectId, [FromBody] IssueRequest request)
    {
        var validationResult = _validator.Validate(request);
        if(!validationResult.IsValid)
            throw new ValidationErrorException(validationResult.ToDictionary());

        var issue = await _gitLabIssueService.CreateIssueAsync(null, projectId, request);
        return Ok(issue);
    }

    [HttpPatch("{projectId}/issues/{issueId}/update")]
    [Authorize]
    public async Task<ActionResult<IssueResponse>> UpdateIssue([FromRoute] string projectId, [FromRoute] string issueId, [FromBody] IssueRequest request)
    {
        var validationResult = _validator.Validate(request);
        if(!validationResult.IsValid)
            throw new ValidationErrorException(validationResult.ToDictionary());

        var issue = await _gitLabIssueService.UpdateIssueAsync(null, projectId, issueId, request);
        return Ok(issue);
    }

    [HttpPatch("{projectId}/issues/{issueId}/close")]
    [Authorize]
    public async Task<ActionResult<IssueResponse>> CloseIssue([FromRoute] string projectId, [FromRoute] string issueId)
    {
        var issue = await _gitLabIssueService.CloseIssueAsync(null, projectId, issueId);
        return Ok(issue);
    }
}