using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using MikeyT.EnvironmentSettingsNS.Interface;
using WebServer.Auth;
using WebServer.Data;
using WebServer.Model.Auth;

namespace WebServer.Logic;

public interface IKeycloakTokenService
{
    public Task<string> GetBasicAccessToken();
    public Task<string> GetAccessTokenFromJwtAssertion();
    public Task<string> InitiateOidcSso();
}

public class KeycloakTokenService : IKeycloakTokenService
{
    private readonly ILogger<KeycloakTokenService> _logger;
    private readonly HttpClient _httpClient;
    private readonly IAccountRepository _accountRepository;
    private readonly IEnvironmentSettings _environmentSettings;

    public KeycloakTokenService(ILogger<KeycloakTokenService> logger, HttpClient httpClient, IEnvironmentSettings environmentSettings, IAccountRepository accountRepository)
    {
        _logger = logger;
        _httpClient = httpClient;
        _environmentSettings = environmentSettings;
        _accountRepository = accountRepository;
    }

    public async Task<string> GetBasicAccessToken()
    {
        var tokenEndpoint = "http://localhost:8080/realms/acme/protocol/openid-connect/token";

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", "acme-monolith"),
            new KeyValuePair<string, string>("client_secret", "Ftoym7YziUoy11VlU77OBVBT8spTpEpS"),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        ]);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var tokenObj = Newtonsoft.Json.Linq.JObject.Parse(responseBody)["access_token"];

        return tokenObj != null ? tokenObj.ToString() : "null";
    }

    public async Task<string> GetAccessTokenFromJwtAssertion()
    {
        var tokenEndpoint = "http://localhost:8080/realms/acme/protocol/openid-connect/token";

        var account = await _accountRepository.GetAccountByEmail("admin@test.com");

        if (account == null)
        {
            throw new Exception("account is null");
        }

        var jwtString = GenerateJwtString(account);

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
        var content = new FormUrlEncodedContent(
        [
            new KeyValuePair<string, string>("client_id", "acme-monolith"),
            new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
            new KeyValuePair<string, string>("assertion", jwtString)
        ]);
        request.Content = content;

        var response = await _httpClient.SendAsync(request);

        _logger.LogInformation("Response code: {ResponseCode}", (int)response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("Response content: {ResponseContent}", responseBody);

        var tokenObj = Newtonsoft.Json.Linq.JObject.Parse(responseBody)["access_token"];

        return tokenObj != null ? tokenObj.ToString() : "null";
    }

    // Keycloak doesn't seem to support non-interactive OIDC interactions, so this just initiates OIDC SSO redirect instead of getting a token
    public async Task<string> InitiateOidcSso()
    {
        var tokenEndpoint = "http://localhost:8080/realms/acme/protocol/openid-connect/auth?client_id=acme-monolith&response_type=code&scope=openid&redirect_uri=http://localhost:8080/realms/acme/broker/MonolithOidc/endpoint";

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);

        var response = await _httpClient.SendAsync(request);

        _logger.LogInformation("Response code: {ResponseCode}", (int)response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();

        _logger.LogInformation("Response content: {ResponseContent}", responseBody);

        response.EnsureSuccessStatusCode();

        if (responseBody == null)
        {
            return "null";
        }
        var tokenObj = Newtonsoft.Json.Linq.JObject.Parse(responseBody)["access_token"];

        return tokenObj != null ? tokenObj.ToString() : "null";
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

    // private string GenerateJwtString(Account account)
    // {
    //     var jwtIssuer = _environmentSettings.GetString(GlobalSettings.JWT_ISSUER);

    //     // Load the private RSA key
    //     var privateKey = File.ReadAllText("path-to-private-key.pem");
    //     var rsa = RSA.Create();
    //     rsa.ImportFromPem(privateKey);

    //     var securityKey = new RsaSecurityKey(rsa);
    //     var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);

    //     if (account.Id == null)
    //     {
    //         throw new Exception("Cannot generate JWT - account Id is null");
    //     }

    //     var claims = new List<Claim>
    // {
    //     new(JwtRegisteredClaimNames.Sub, account.Id.Value.ToString()),
    //     new(JwtRegisteredClaimNames.Email, account.Email),
    //     new("role", Roles.ADMIN),
    //     new("role", Roles.USER)
    // };

    //     claims.AddRange(account.Roles.Select(role => new Claim("role", role)));

    //     var token = new JwtSecurityToken(jwtIssuer,
    //         jwtIssuer,
    //         claims.ToArray(),
    //         expires: DateTime.UtcNow.AddDays(10),
    //         signingCredentials: credentials);

    //     return new JwtSecurityTokenHandler().WriteToken(token);
    // }
}
