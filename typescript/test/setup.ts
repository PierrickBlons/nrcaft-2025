import { setup, teardown } from './datastore/initializeInMemoryDataStore'

beforeAll(async () => {
  await setup()
})

afterAll(() => {
  teardown()
})
