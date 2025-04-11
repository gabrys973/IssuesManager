using Core.Services;
using Microsoft.Extensions.Options;
using Web.Configutarions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<IssueServicesConfiguration>()
    .BindConfiguration("IssueServices");

builder.Services.AddHttpClient<GitHubIssueService>((serviceProvider, httpClient) =>
{
    var issueServicesConfiguration = serviceProvider.GetRequiredService<IOptions<IssueServicesConfiguration>>().Value;

    httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
    httpClient.DefaultRequestHeaders.Add("Authorization", issueServicesConfiguration.GitHubToken);
    httpClient.DefaultRequestHeaders.Add("User-Agent", "IssuesManager");
});

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();