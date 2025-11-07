import { setupPgLite } from './pgLiteSetup'

export const setup = async () => {
  console.log('Setup in-memory data store')
  await setupPgLite()
  console.log('In-memory data store is ready')
}

export const teardown = () => {
  console.log('Teardown in-memory data store')
}
