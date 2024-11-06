import { spawnAsync, spawnAsyncLongRunning } from '@mikeyt23/node-cli-utils'
import { log } from 'console'

const acmePath = './acme'
const acmeServerPath = `${acmePath}/server/src/WebServer`
const acmeClientPath = `${acmePath}/client`
const keycloakPath = './keycloak'
const acmeApiPath = './acme-api'
const acmeApiProjectPath = `${acmeApiPath}/AcmeApi`
const apiGatewayPath = './api-gateway'
const apiGatewayProjectPath = `${apiGatewayPath}/ApiGateway`

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
  await spawnAsync('swig', ['dockerDown'], { cwd: keycloakPath })
  log('- stopping acme database')
  await spawnAsync('swig', ['dockerDown'], { cwd: acmePath })
}

export async function keycloakUpAttached() {
  log('- starting keycloak in attached mode')
  await spawnAsync('swig', ['dockerUpAttached'], { cwd: keycloakPath })
}

export async function runServer() {
  log('- starting acme server')
  await spawnAsyncLongRunning('dotnet', ['watch', '--project', acmeServerPath])
}

export async function runClient() {
  log('- starting acme client')
  await spawnAsyncLongRunning('pnpm', ['run', 'dev'], acmeClientPath)
}

export async function runApi() {
  log('- starting acme-api service')
  await spawnAsyncLongRunning('dotnet', ['watch', '--project', acmeApiProjectPath])
}

export async function runGateway() {
  log('- starting API gateway')
  await spawnAsyncLongRunning('dotnet', ['watch', '--project', apiGatewayProjectPath])
}
