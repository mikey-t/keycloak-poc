using Microsoft.AspNetCore.Mvc;
using WebServer.Auth;
using WebServer.Logic;
using WebServer.Model.Auth;

namespace WebServer.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Role.USER)]
public class ProtectedController : ControllerBase
{
    private readonly ILogger<ProtectedController> _logger;
    private readonly IKeycloakTokenService _keycloakTokenService;
    
    public ProtectedController(ILogger<ProtectedController> logger, IKeycloakTokenService keycloakTokenService)
    {
        _logger = logger;
        _keycloakTokenService = keycloakTokenService;
    }
    
    [HttpGet("test")]
    public string Test()
    {
        return "test content from a protected API endpoint";
    }

    [HttpGet("keycloak-token")]
    public async Task<string> GetKeycloakToken()
    {
        var token = await _keycloakTokenService.GetBasicAccessToken();
        // var token = await _keycloakTokenService.GetAccessTokenFromJwtAssertion();
        return token;
    }

    [HttpGet("oidc-sso")]
    public async Task<string> InitiateOidcSso()
    {
        var res = await _keycloakTokenService.InitiateOidcSso();
        return res;
    }
}
