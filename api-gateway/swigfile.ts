import { spawnAsyncLongRunning } from '@mikeyt23/node-cli-utils'

const apiGatewayPath = './ApiGateway'

export async function run() {
  await spawnAsyncLongRunning('dotnet', ['watch', '--project', apiGatewayPath])
}
