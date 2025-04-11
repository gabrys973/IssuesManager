using System.Text.Json.Serialization;

namespace Core.Models.GitLab;
internal record GitLabIssueResponse
{
    public GitLabIssueResponse(int iid, string title, string description, string state, string webUrl)
    {
        Iid = iid;
        Title = title;
        Description = description;
        State = state;
        WebUrl = webUrl;
    }

    public int Iid { get; }
    public string Title { get; }
    public string Description { get; }
    public string State { get; }
    [JsonPropertyName("web_url")]
    public string WebUrl { get; }
}