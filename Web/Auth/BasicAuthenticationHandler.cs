using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Web.Auth;

internal sealed class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock)
        : base(options, logger, encoder, clock)
    {
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if(!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Unauthorized");
        }

        string authHeader = Request.Headers["Authorization"];
        if(string.IsNullOrEmpty(authHeader))
        {
            return AuthenticateResult.Fail("Unauthorized");
        }

        if(!authHeader.StartsWith("basic ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("Unauthorized");
        }

        var token = authHeader.Substring(6);
        var authBase64String = Encoding.UTF8.GetString(Convert.FromBase64String(token));

        var authSplit = authBase64String.Split(":");
        if(authSplit?.Length != 2)
        {
            return AuthenticateResult.Fail("Unauthorized");
        }

        var username = authSplit[0];
        var password = authSplit[1];

        if(username != "myIssueManagerUser" || password != "7a!JR589DR7v8Z9LF(k}h")
        {
            return AuthenticateResult.Fail("Authentication failed");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username)
        };

        var identity = new ClaimsIdentity(claims, "Basic");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }
}