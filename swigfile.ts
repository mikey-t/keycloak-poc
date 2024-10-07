import { log } from 'console'

export async function remind() {
  log('Reminders for each terminal:')
  log('  - cd keycloak, swig dockerUp')
  log('  - cd acme, swig dockerUp, swig server')
  log('  - cd acme, swig client')
  log('  - cd acme-api, swig run')
  log('  - cd api-gateway, swig run')
}
