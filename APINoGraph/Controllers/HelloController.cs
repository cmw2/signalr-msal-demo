using Microsoft.AspNetCore.Mvc;

namespace APINoGraph.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HelloController : ControllerBase
{
    [HttpGet]
    public ActionResult<string> Get()
    {
        return Ok("Hello from APINoGraph.");
    }
}