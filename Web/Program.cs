using Core.Services;
using Core.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Globalization;
using Web.Auth;
using Web.Configutarions;
using Web.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "basic",
        In = ParameterLocation.Header,
        Description = "Basic Authorization header."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "basic"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddOptions<IssueServicesConfiguration>()
    .BindConfiguration("IssueServices");

builder.Services.AddHttpClient<GitHubIssueService>((serviceProvider, httpClient) =>
{
    var issueServicesConfiguration = serviceProvider.GetRequiredService<IOptions<IssueServicesConfiguration>>().Value;

    httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
    httpClient.DefaultRequestHeaders.Add("Authorization", issueServicesConfiguration.GitHubToken);
    httpClient.DefaultRequestHeaders.Add("User-Agent", "IssuesManager");
});

builder.Services.AddValidatorsFromAssembly(typeof(IssueRequestValidator).Assembly);
ValidatorOptions.Global.LanguageManager.Culture = new CultureInfo("en");

builder.Services.AddTransient<GlobalExceptionHandlerMiddleware>();

builder.Services.AddAuthentication("BasicAuthentication")
    .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.MapControllers();

app.Run();