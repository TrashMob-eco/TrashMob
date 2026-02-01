export default ({ env }) => {
  const client = env('DATABASE_CLIENT', 'sqlite');

  const connections = {
    sqlite: {
      connection: {
        // Use local ephemeral storage for SQLite (Azure Files SMB has locking issues)
        // Database is small and rebuilt on container restart
        // Content types are defined in code, only data needs re-entry
        filename: env('DATABASE_FILENAME', '/app/.tmp/data.db'),
      },
      useNullAsDefault: true,
      pool: {
        min: 1,
        max: 1, // SQLite only supports single connection for writes
      },
    },
    mssql: {
      connection: {
        host: env('DATABASE_HOST'),
        port: env.int('DATABASE_PORT', 1433),
        database: env('DATABASE_NAME'),
        user: env('DATABASE_USERNAME'),
        password: env('DATABASE_PASSWORD'),
        options: {
          encrypt: true, // Required for Azure SQL
          trustServerCertificate: false,
        },
      },
      pool: {
        min: env.int('DATABASE_POOL_MIN', 2),
        max: env.int('DATABASE_POOL_MAX', 10),
      },
    },
  };

  return {
    connection: {
      client,
      ...connections[client],
      acquireConnectionTimeout: env.int('DATABASE_CONNECTION_TIMEOUT', 60000),
    },
  };
};
