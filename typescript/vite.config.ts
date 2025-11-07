/// <reference types='vitest' />
import { defineConfig } from 'vitest/config'

export default defineConfig({
  test: {
    globals: true,
    include: ['**/*.test.ts'],
    setupFiles: ['./test/setup.ts'],
    globalSetup: ['./test/datastore/initializeInMemoryDataStore.ts'],
    coverage: {
      reportsDirectory: '../../coverage/apps/talent-management',
      provider: 'v8',
    },
  },
})
