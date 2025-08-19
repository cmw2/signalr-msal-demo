using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web;

namespace APIWithGraph.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ManagerController : ControllerBase
{
    private readonly GraphServiceClient _graphServiceClient;

    public ManagerController(GraphServiceClient graphServiceClient)
    {
        _graphServiceClient = graphServiceClient;
    }

    [HttpGet]
    [AuthorizeForScopes(Scopes = new[] { "User.Read", "User.Read.All" })]
    public async Task<ActionResult<string>> GetManager()
    {
        try
        {
            var manager = await _graphServiceClient.Me.Manager.GetAsync();
            
            if (manager is Microsoft.Graph.Models.User managerUser)
            {
                return Ok(managerUser.DisplayName ?? "Manager display name not available");
            }
            
            return Ok("Manager information not available");
        }
        catch (Exception ex)
        {
            return BadRequest($"Error retrieving manager: {ex.Message}");
        }
    }
}