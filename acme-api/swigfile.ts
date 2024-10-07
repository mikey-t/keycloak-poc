import { spawnAsyncLongRunning } from '@mikeyt23/node-cli-utils'

const apiPath = './AcmeApi'

export async function run() {
  await spawnAsyncLongRunning('dotnet', ['watch', '--project', apiPath])
}
