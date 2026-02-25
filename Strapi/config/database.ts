export default ({ env }) => {
  const client = env('DATABASE_CLIENT', 'sqlite');

  const connections = {
    sqlite: {
      connection: {
        // Deployed: DATABASE_FILENAME=/app/data/strapi.db (persistent Azure Files mount)
        // Local dev: defaults to /app/.tmp/data.db (ephemeral)
        // maxReplicas=1 in Bicep avoids SQLite locking issues with SMB
        filename: env('DATABASE_FILENAME', '/app/.tmp/data.db'),
      },
      useNullAsDefault: true,
      pool: {
        min: 1,
        max: 1, // SQLite only supports single connection for writes
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
