import { log } from 'console'

export async function remind() {
  log('\n(see readme for details)\n')
  log('Startup reminders:')
  log(' keycloak, dockerUp')
  log(' acme, dockerUp, server, client')
  log(' acme-api, run')
  log(' api-gateway, run')
  log('\nLocation reminders:')
  log(' Postman collection: KeycloakPoc')
  log(' Acme site URL: https://local.acme.mikeyt.net:3000')
  log(' Keycloak URL: http://localhost:8080 (admin/admin)\n')
}
