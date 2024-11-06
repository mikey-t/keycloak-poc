# keycloak-poc

This POC is a playground to experiment with keycloak functionality in a split monolith microservices scenario.

Goals:
- 2 interconnected apps + API Gateway + Keycloak instance
  - Keycloak instance, use federated identity provider concept (login using IdP info external to keycloak instance)
  - Relying party, in this case a simulated monolith that will also be the IdP
  - Resource server, in this case an API that accepts JWT generated by keycloak, simulating an internal microservice
- Basic client credentials interaction between monolith and keycloak
- OIDC SSO from monolith to keycloak for a shared token that can be utilized by a microservice
- Wire up API gateway as another bearer-only keycloak client in order to also parse/check JWT

## Acme Site Setup

See readme in acme site for info on setup.

## Running Locally

Reminders for manually running each component in separate terminals, starting from project root:
- Starts both keycloak and acme database: `swig depsUp`
- Start acme server: `swig runServer`
- Start acme react client: `swig runClient`
- Start acme-api microservice: `swig runApi`
- Start api-gateway service: `swig runGateway`

Access reminders:
- Keycloak runs at http://localhost:8080, login with credentials admin/admin
- Login to acme site at https://local.acme.mikeyt.net:3000/ with credentials admin@test.com/Abc1234!
- Postman collection "KeycloakPoc" has various requests, some are direct, other go through api-gateway

Port reminders:
- keycloak: 8080
- acme: 3000
- acme backend: 5001
- api-gateway: 5202
- acme-api: 5163


## Keycloak Setup

See https://www.keycloak.org/docs/latest/server_admin/index.html.

Setup acme-monolith client:
- Login with admin/admin
- Create new realm "acme"
- Realm settings -> General -> set "Require SSL" to false
- Create acme-monolith client in acme realm
  - Client ID: acme-monolith
  - Client authentication: enabled
  - Create "acme-user" role within acme-monolith client ("roles" tab)
  - Everything else default, save client

Setup acme-api bearer only client:
- Within acme realm, create new client called "acme-api"
- Disable authentication and authorization capabilities
- Disable "consent required" and leave URLs blank (no user interactivity for the bearer-only scenario)

Setup client scope for audience validation:
- Within acme realm, click on "Client Scopes"
- Create new client scope called acme-audience
  - Type: Default
  - Disable "Display on consent screen"
  - Enable "Include in token scope"
- Add new mapper to client scope ("by configuration"), select "Audience" type, call it "acme-api-audience"
  - From "Included Client Audience" dropdown, select "acme-api" client
  - Disable "Add to ID token"
  - Enable "Add to access token"
  - Other settings, leave default
- Add the new client scope to the acme-monolith client
  - Within acme realm, go to acme-monolith client
  - Under "Client Scopes", add "acme-audience" to default scopes

## Demo Functionality

(these instructions will change a lot after some project simplification)

Login to monolith at https://local.acme.mikeyt.net:3000 with credentials admin@test.com, navigate to "Account" page

Click "Get keycloak token"

View decoded token at jwt.io

Copy token to postman request "API Keycloak Protected Sample - direct to api" to demo utilization of this shared keycloak token with the acme-api.

# acme-api

Port: 5163

Local swagger URL: http://localhost:5163/swagger/index.html

Weather forecast API GET URL: http://localhost:5163/weatherforecast

Weather forecast URL through API gateway (API gateway URL + "/api" prefix): http://localhost:5202/api/weatherforecast

## acme-api Setup

Steps taken to setup acme-api:

- Change dir to AcmeApi
- `dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer`
- Add authentication middleware to startup logic
- Create sample controller with `[Authorize]` attribute

# api-gateway

URL: http://localhost:5202

Simple status endpoint (returns hello world message): http://localhost:5202/status

# TODO

## General TODO

- Clean and simplify solution
  - De-dupe JWT generation logic in acme solution
  - Consolidate most of the demo functionality into a single UI with multiple buttons for each scenario
  - Write additional docs explaining which parts of the acme solution are specifically part of the keycloak interactions (vs part of the template solution that was used to generate the project)
  - Simplify each demo scenario so it doesn't require any postman usage
- Wire up API gateway usage
- Javascript client usage of shared keycloak token
  - Refresh token utilization
  - Or, manual expiration/refresh check
- Research ability to export/import keycloak settings
- Move settings (especially client_id/client_secret) to environment settings
- More specific info on setting up acme monolith

## Additional Keycloak Research

- Finish wiring up the OIDC SSO (login directly to keycloak with a redirect to the monolith as the identity provider)
  - Implement authorization endpoint
- Try out token exchange. It's in "preview" state, but might be a good choice for adding additional claims to the shared keycloak token. See [token-exchange](https://www.keycloak.org/securing-apps/token-exchange) and https://github.com/keycloak/keycloak/discussions/26502.
- Administrative access
  - Active Directory (LDAP) integration
  - Lower privileged admin setup and usage (accounts that can manage a single keycloak client, for example)
- Lightweight tokens
- Encryption (JWE)
- Refresh token usage
- Research best way to ensure shared keycloak token does not outlive the monolith session (or at least, does not extend very far past it)
- Explicit logout (logout of monolith -> trigger logout of shared keycloak session)

# Misc

## Dotnet Auth Debug Logging

Modify appSettings.json:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore.Authentication.JwtBearer": "Debug",
      "Microsoft.AspNetCore.Authorization": "Debug",
      "Microsoft.AspNetCore.Authentication": "Debug"
    }
  }
}

```

## Noteworthy Code Locations

TODO
- React UI component with buttons to trigger demos
- KeycloakTokenService class in acme
- acme-api middleware wire-up and sample controller with `[Authorize]`
- Keycloak docker-compose
- ?
