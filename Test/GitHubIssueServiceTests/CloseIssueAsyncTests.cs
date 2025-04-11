using Core.Exceptions;
using Core.Models;
using Core.Models.GitHub;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Test.GitHubIssueServiceTests;

public class CloseIssueAsyncTests : GitHubIssueServiceTestBase
{
    [Test]
    public async Task CloseIssueAsync_CorrectRequest_StatusChanged()
    {
        var owner = "owner";
        var repository = "repository";
        var issueId = "issueId";
        var requestBody = new IssueRequest("title", "decription");

        var responseBody = new GitHubIssueResponse(123, "title", "decription", "closed", $"{BaseUrl}/repos/{owner}/{repository}/issues/123");

        var content = JsonSerializer.Serialize(responseBody);

        var response = new HttpResponseMessage
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json"),
            StatusCode = HttpStatusCode.OK
        };

        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic username:password");

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"{BaseUrl}/repos/{owner}/{repository}/issues/{issueId}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var result = await _service.CloseIssueAsync(owner, repository, issueId);

        Assert.That(result.Title, Is.EqualTo(responseBody.Title));
        Assert.That(result.State, Is.EqualTo(responseBody.State));
        Assert.That(result.Description, Is.EqualTo(responseBody.Body));
        Assert.That(result.Url, Is.EqualTo(responseBody.HtmlUrl));
        Assert.That(result.Id, Is.EqualTo(responseBody.Number.ToString()));
    }

    [Test]
    public void CloseIssueAsync_BadRoute_ExceptionThrowed()
    {
        var owner = "owner";
        var repository = "badRepositoryName";
        var issueId = "issueId";
        var requestBody = new IssueRequest("title", "decription");

        var responseBody = new GitHubIssueResponse(123, "title", "decription", "open", $"{BaseUrl}/repos/{owner}/{repository}/issues/123");

        var content = JsonSerializer.Serialize(responseBody);

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };

        _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Basic username:password");

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"{BaseUrl}/repos/{owner}/{repository}/issues/{issueId}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var ex = Assert.ThrowsAsync<ExternalErrorException>(async () => await _service.CloseIssueAsync(owner, repository, issueId));

        Assert.That(ex.Message, Is.EqualTo("External GitHub service threw an exception."));
    }

    [Test]
    public void CloseIssueAsync_AuthorizationHeaderIsNull_ExceptionThrowed()
    {
        var owner = "owner";
        var repository = "repository";
        var issueId = "issueId";
        var requestBody = new IssueRequest("title", "decription");

        var responseBody = new GitHubIssueResponse(123, "title", "decription", "open", $"{BaseUrl}/repos/{owner}/{repository}/issues/123");

        var content = JsonSerializer.Serialize(responseBody);

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        };

        _httpClient.DefaultRequestHeaders.Remove("Authorization");

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"{BaseUrl}/repos/{owner}/{repository}/issues/{issueId}")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var ex = Assert.ThrowsAsync<TokenEmptyException>(async () => await _service.CloseIssueAsync(owner, repository, issueId));

        Assert.That(ex.Message, Is.EqualTo("Token for GitHub service cannot be empty or null."));
    }
}