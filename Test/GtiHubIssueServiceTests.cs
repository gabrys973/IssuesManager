using Core.Exceptions;
using Core.Models;
using Core.Models.GitHub;
using Core.Services;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Test;

public class GtiHubIssueServiceTests
{
    private const string BaseUrl = "https://api.github.com";
    private readonly Mock<HttpMessageHandler> _handlerMock;
    private readonly GitHubIssueService _service;
    private readonly HttpClient _httpClient;

    public GtiHubIssueServiceTests()
    {
        _handlerMock = new();
        _httpClient = new(_handlerMock.Object);
        _service = new GitHubIssueService(_httpClient);
    }

    [Test]
    public async Task CreateIssueAsync_CorrectRequest_Success()
    {
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

        var result = await _service.CreateIssueAsync(owner, repository, requestBody);

        Assert.That(responseBody.Title, Is.EqualTo(result.Title));
        Assert.That(responseBody.State, Is.EqualTo(result.State));
        Assert.That(responseBody.Body, Is.EqualTo(result.Description));
        Assert.That(responseBody.HtmlUrl, Is.EqualTo(result.Url));
        Assert.That(responseBody.Number.ToString(), Is.EqualTo(result.Id));
    }

    [Test]
    public void CreateIssueAsync_BadRoute_ExceptionThrowed()
    {
        var owner = "owner";
        var repository = "repository";
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

        var ex = Assert.ThrowsAsync<ExternalErrorException>(async () => await _service.CreateIssueAsync(owner, repository, requestBody));

        Assert.That(ex.Message, Is.EqualTo("External service GitHub throw an exception."));
    }
}