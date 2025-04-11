using System.Text.Json.Serialization;

namespace Core.Models.GitHub;
internal record GitHubIssueResponse
{
    public GitHubIssueResponse(int number, string title, string body, string state, string htmlUrl)
    {
        Number = number;
        Title = title;
        Body = body;
        State = state;
        HtmlUrl = htmlUrl;
    }

    public int Number { get; }
    public string Title { get; }
    public string Body { get; }
    public string State { get; }
    [JsonPropertyName("html_url")]
    public string HtmlUrl { get; }
}