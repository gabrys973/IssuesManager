using Core.Services;
using Moq;

namespace Test.GitHubIssueServiceTests;

public abstract class GitHubIssueServiceTestBase
{
    protected const string BaseUrl = "https://api.github.com";
    protected readonly Mock<HttpMessageHandler> _handlerMock;
    protected readonly GitHubIssueService _service;
    protected readonly HttpClient _httpClient;

    public GitHubIssueServiceTestBase()
    {
        _handlerMock = new();
        _httpClient = new(_handlerMock.Object);
        _service = new GitHubIssueService(_httpClient);
    }
}