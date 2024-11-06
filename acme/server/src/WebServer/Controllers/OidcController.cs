using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MikeyT.EnvironmentSettingsNS.Interface;
using WebServer.Auth;
using WebServer.Data;
using WebServer.Model.Auth;

namespace WebServer.Controllers;

[ApiController]
[Route("api/connect")]
public class OidcController : ControllerBase
{
    private readonly ILogger<OidcController> _logger;
    private readonly IAccountRepository _accountRepository;
    private readonly IEnvironmentSettings _environmentSettings;

    public OidcController(ILogger<OidcController> logger, IEnvironmentSettings environmentSettings, IAccountRepository accountRepository)
    {
        _logger = logger;
        _environmentSettings = environmentSettings;
        _accountRepository = accountRepository;
    }

    [Route("temp")]
    [HttpGet]
    public IActionResult Temp()
    {
        return Ok("hello world");
    }

    [Route("openid-configuration")]
    [HttpGet]
    public IActionResult OpenIdConfiguration()
    {
        return Ok(new
        {
            issuer = "https://local.acme.mikeyt.net",
            token_endpoint = "https://local.acme.mikeyt.net/api/connect/token",
            userinfo_endpoint = "https://local.acme.mikeyt.net/api/connect/user-info"
            // jwks_uri = "https://your-monolith.com/.well-known/jwks.json" // If you're using JSON Web Keys
        });
    }

    [Route("token")]
    [HttpPost]
    // public IActionResult Token([FromForm] TokenRequestModel model)
    public async Task<IActionResult> Token()
    {
        // return Ok("hello world");

        // TODO
        // Authenticate the client (Keycloak)
        // if (model.ClientId != "your-client-id" || model.ClientSecret != "your-client-secret")
        // {
        //     return Unauthorized(new { error = "invalid_client" });
        // }

        var account = await _accountRepository.GetAccountByEmail("admin@test.com"); // TODO: get from request instead
        if (account == null)
        {
            return Unauthorized(new { error = "invalid_grant" });
        }

        var token = GenerateJwtString(account);

        return Ok(new
        {
            access_token = token,
            token_type = "Bearer",
            expires_in = 3600
        });
    }

    [HttpGet]
    [Route("user-info")]
    public async Task<IActionResult> UserInfo()
    {
        // TODO: validate keycloak client token since the request is coming from keycloak and not the end user
        // if (???)
        // {
        //     return Unauthorized();
        // }

        var account = await _accountRepository.GetAccountByEmail("admin@test.com");
        if (account == null)
        {
            _logger.LogWarning("user not found");
            return Unauthorized();
        }

        return Ok(new
        {
            sub = account.Id,
            name = "John Doe",
            email = account.Email,
            favorite_color = "blue"
        });
    }

    [HttpGet("jwks")]
    public IActionResult GetJwks()
    {
        var jwks = new
        {
            keys = new List<object>
            {
                new
                {
                    kty = "RSA",
                    kid = "unique-key-id",
                    use = "sig",
                    alg = "RS256",
                    n = "modulus-in-base64-url",
                    e = "AQAB" // common exponent (65537 in base64url)
                }
            }
        };

        return Ok(jwks);
    }

    private string GenerateJwtString(Account account)
    {
        var jwtIssuer = _environmentSettings.GetString(GlobalSettings.JWT_ISSUER);
        var jwtKey = _environmentSettings.GetString(GlobalSettings.JWT_KEY);

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        if (account.Id == null)
        {
            throw new Exception("Cannot generate JWT - account Id is null");
        }

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, account.Id.Value.ToString()),
            new(JwtRegisteredClaimNames.Email, account.Email),
            new("role", Roles.ADMIN),
            new("role", Roles.USER)
        };

        claims.AddRange(account.Roles.Select(role => new Claim("role", role)));

        var token = new JwtSecurityToken(jwtIssuer,
            jwtIssuer,
            claims.ToArray(),
            expires: DateTime.UtcNow.AddDays(10),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class TokenRequestModel
{
    [FromForm(Name = "grant_type")]
    public string GrantType { get; set; } // e.g., "password" or "client_credentials"

    [FromForm(Name = "client_id")]
    public string ClientId { get; set; }

    [FromForm(Name = "client_secret")]
    public string ClientSecret { get; set; }

    [FromForm(Name = "username")]
    public string Username { get; set; } // Needed for password grant (not client credentials)

    [FromForm(Name = "password")]
    public string Password { get; set; } // Needed for password grant (not client credentials)

    [FromForm(Name = "scope")]
    public string Scope { get; set; } // Optional
}
