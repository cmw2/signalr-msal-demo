using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APINoGraph.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "APINoGraph.ApiAccess")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("Hello from APINoGraph.");
    }
}