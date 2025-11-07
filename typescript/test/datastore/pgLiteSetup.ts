import {
  interpolationSafeWindowsPath,
  runShellCommand,
} from '@augment-vir/node'
import { PGlite } from '@electric-sql/pglite'
import { readFile } from 'node:fs/promises'
import { PrismaPGlite } from 'pglite-prisma-adapter'
import { PrismaService } from '../../src/shared/infrastructure/sql/prisma-service'

declare global {
  var prismaService: PrismaService
}

const generateDbInitSql = async ({
  prismaSchemaPath,
  migrationOutputPath,
}: {
  prismaSchemaPath: string
  migrationOutputPath: string
}): Promise<string> => {
  await runShellCommand(
    `npx prisma migrate diff --from-empty --to-schema-datamodel ${interpolationSafeWindowsPath(prismaSchemaPath)} --script > ${interpolationSafeWindowsPath(migrationOutputPath)}`,
    {
      rejectOnError: true,
    },
  )

  return String(await readFile(migrationOutputPath))
}

export const testContext: {
  db: PGlite | undefined
} = {
  db: undefined,
}

export const setupPgLite = async () => {
  testContext.db = new PGlite({ dataDir: 'memory://' })
  await testContext.db.exec(
    await generateDbInitSql({
      prismaSchemaPath: 'prisma/schema.prisma',
      migrationOutputPath: 'prisma/migration.sql',
    }),
  )

  global.prismaService = new PrismaService({
    adapter: new PrismaPGlite(testContext.db),
  })
}
