using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace APIWithGraph.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly GraphServiceClient _graphServiceClient;

    public ProfileController(GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
    }

    [HttpGet("mobile-greeting")]
    [AuthorizeForScopes(Scopes = new[] { "User.Read" })]
    public async Task<ActionResult<string>> GetMobilePhoneGreeting()
    {
        try
        {
            var user = await _graphServiceClient.Me.Request()
                .Select("givenName,surname,mobilePhone")
                .GetAsync();

            var firstName = user?.GivenName ?? "(unknown)";
            var lastName = user?.Surname ?? "";
            var mobile = user?.MobilePhone ?? "(not set)";

            return Ok($"Hello {firstName} {lastName}. Is {mobile} the best number?");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving user info: {ex.Message}");
        }
    }
}
