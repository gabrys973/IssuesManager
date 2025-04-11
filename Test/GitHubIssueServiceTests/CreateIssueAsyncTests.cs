using Core.Exceptions;
using Core.Models;
using Core.Models.GitHub;
using Core.Services.GitHub;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Test.GitHubIssueServiceTests;

public class CreateIssueAsyncTests : GitHubIssueServiceTestBase
{
    [Test]
    public async Task CreateIssueAsync_CorrectRequest_Success()
    {
        // Arrange
        var owner = "owner";
        var repository = "repository";
        var requestBody = new IssueRequest("title", "decription");

        var responseBody = new GitHubIssueResponse(123, "title", "decription", "open", $"{BaseUrl}/repos/{owner}/{repository}/issues/123");

        var content = JsonSerializer.Serialize(responseBody);

        var response = new HttpResponseMessage
        {
            Content = new StringContent(content, Encoding.UTF8, "application/json"),
            StatusCode = HttpStatusCode.OK
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"{BaseUrl}/repos/{owner}/{repository}/issues")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var client = new HttpClient(_handlerMock.Object);
        client.DefaultRequestHeaders.Add("Authorization", "Basic username:password");
        var service = new GitHubIssueService(client);

        // Act
        var result = await service.CreateIssueAsync(owner, repository, requestBody);

        // Assert
        Assert.That(result.Title, Is.EqualTo(responseBody.Title));
        Assert.That(result.State, Is.EqualTo(responseBody.State));
        Assert.That(result.Description, Is.EqualTo(responseBody.Body));
        Assert.That(result.Url, Is.EqualTo(responseBody.HtmlUrl));
        Assert.That(result.Id, Is.EqualTo(responseBody.Number.ToString()));
    }

    [Test]
    public void CreateIssueAsync_BadRoute_ExceptionThrowed()
    {
        // Arrange
        var owner = "owner";
        var repository = "badRepositoryName";
        var requestBody = new IssueRequest("title", "decription");

        var responseBody = new GitHubIssueResponse(123, "title", "decription", "open", $"{BaseUrl}/repos/{owner}/{repository}/issues/123");

        var content = JsonSerializer.Serialize(responseBody);

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };

        _handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(x => x.RequestUri == new Uri($"{BaseUrl}/repos/{owner}/{repository}/issues")),
            ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(response);

        var client = new HttpClient(_handlerMock.Object);
        client.DefaultRequestHeaders.Add("Authorization", "Basic username:password");
        var service = new GitHubIssueService(client);

        // Act
        var ex = Assert.ThrowsAsync<ExternalErrorException>(async () => await service.CreateIssueAsync(owner, repository, requestBody));

        // Assert
        Assert.That(ex.Message, Is.EqualTo("External GitHub service threw an exception."));
    }

    [Test]
    public void CreateIssueAsync_AuthorizationHeaderIsNull_ExceptionThrowed()
    {
        // Arrange
        var owner = "owner";
        var repository = "repository";
        var requestBody = new IssueRequest("title", "decription");

        var responseBody = new GitHubIssueResponse(123, "title", "decription", "open", $"{BaseUrl}/repos/{owner}/{repository}/issues/123");

        var content = JsonSerializer.Serialize(responseBody);

        var response = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.OK
        };

        var client = new HttpClient();
        var service = new GitHubIssueService(client);

        // Act
        var ex = Assert.ThrowsAsync<TokenEmptyException>(async () => await service.CreateIssueAsync(owner, repository, requestBody));

        // Assert
        Assert.That(ex.Message, Is.EqualTo("Token for GitHub service cannot be empty or null."));
    }
}