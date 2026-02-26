export default ({ env }) => {
  const client = env('DATABASE_CLIENT', 'sqlite');

  const connections = {
    sqlite: {
      connection: {
        // Deployed: docker-entrypoint.sh sets DATABASE_FILENAME=/app/.tmp/strapi.db (local ephemeral)
        // and handles persistence by copying to/from Azure Files mount at /app/data/
        // Local dev: defaults to /app/.tmp/data.db
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
