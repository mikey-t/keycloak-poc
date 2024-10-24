import { log } from 'console'
import { spawnAsync, spawnAsyncLongRunning } from '@mikeyt23/node-cli-utils'

const acmePath = './acme'
const keycloakPath = './keycloak'
const acmeApiPath = './acme-api'
const apiGatewayPath = './api-gateway'

export async function remind() {
  log('\n(see readme for more info)')

  log('\nStartup keycloak and acme DB:')
  log(' - swig depsUp')

  log('\nIn separate terminals:')
  log(' - swig runServer')
  log(' - swig runClient')
  log(' - swig runApi')
  log(' - swig runGateway')

  log('\nLocations:')
  log(' - Acme site URL: https://local.acme.mikeyt.net:3000')
  log(' - Keycloak URL: http://localhost:8080 (admin/admin)\n')

  log('\nMisc:')
  log(' - Postman collection name: KeycloakPoc\n')
}

export async function depsUp() {
  log('- starting keycloak')
  await spawnAsync('swig', ['dockerUp'], { cwd: keycloakPath })
  log('- starting acme database')
  await spawnAsync('swig', ['dockerUp'], { cwd: acmePath })
}

export async function depsDown() {
  log('- stopping keycloak')
  await spawnAsync('swig', ['dockerUp'], { cwd: keycloakPath })
  log('- stopping acme database')
  await spawnAsync('swig', ['dockerUp'], { cwd: acmePath })
}

export async function runServer() {
  log('- starting acme server')
  await spawnAsyncLongRunning('swig', ['server'], acmePath)
}

export async function runClient() {
  log('- starting acme client')
  await spawnAsyncLongRunning('swig', ['client'], acmePath)
}

export async function runApi() {
  log('- starting acme-api service')
  await spawnAsyncLongRunning('swig', ['run'], acmeApiPath)
}

export async function runGateway() {
  log('- starting API gateway')
  await spawnAsyncLongRunning('swig', ['run'], apiGatewayPath)
}
