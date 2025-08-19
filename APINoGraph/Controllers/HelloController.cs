using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace APINoGraph.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HelloController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("Hello from APINoGraph.");
    }
}