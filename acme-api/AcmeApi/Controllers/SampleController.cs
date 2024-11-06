using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AcmeApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SampleController : ControllerBase
{
    [HttpGet("protected")]
    [Authorize]
    public IActionResult GetProtectedResource()
    {
        return Ok("This is a protected resource.");
    }
}
