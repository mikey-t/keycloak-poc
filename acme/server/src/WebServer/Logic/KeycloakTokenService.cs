using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebServer.Logic;

public interface IKeycloakTokenService {
    public Task<string> GetAccessTokenAsync();
}

public class KeycloakTokenService : IKeycloakTokenService
{
    private readonly HttpClient _httpClient;

    public KeycloakTokenService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> GetAccessTokenAsync()
    {
        var tokenEndpoint = "http://localhost:8080/realms/acme/protocol/openid-connect/token";
        var clientId = "acme-monolith";
        var clientSecret = "FXg72GNHCUzanKWU1k52mVy1OEN1C3qs";

        var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
        var content = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        });
        request.Content = content;

        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var responseBody = await response.Content.ReadAsStringAsync();
        var token = Newtonsoft.Json.Linq.JObject.Parse(responseBody)["access_token"].ToString();

        return token;
    }
}
