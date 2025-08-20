using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebApp.Controllers;

[Route("[controller]/[action]")]
public class ConsentController : Controller
{
    private readonly IConfiguration _configuration;
    public ConsentController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IActionResult ApiWithGraph()
    {
        // Read the scope from configuration
        var scope = _configuration["DownstreamApis:APIWithGraph:Scopes"];
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Content("~/api-demo")
        };

        // Get the current user's login (UPN/email)
        var loginHint = User?.Identities?.FirstOrDefault()?.Claims
            ?.FirstOrDefault(c => c.Type == "preferred_username" || c.Type == "upn" || c.Type == "email")?.Value;

        if (!string.IsNullOrEmpty(loginHint))
        {
            props.SetParameter("login_hint", loginHint);
        }
        //props.SetParameter("domain_hint", "cmwexternal.onmicrosoft.com");
        props.SetParameter("prompt", "consent");
        props.SetParameter("scope", scope);
        return Challenge(props, "OpenIdConnect");
    }
}
