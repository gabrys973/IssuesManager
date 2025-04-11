using Moq;

namespace Test.GitHubIssueServiceTests;

public abstract class GitHubIssueServiceTestBase
{
    protected const string BaseUrl = "https://api.github.com";
    protected readonly Mock<HttpMessageHandler> _handlerMock;

    public GitHubIssueServiceTestBase()
    {
        _handlerMock = new();
    }
}