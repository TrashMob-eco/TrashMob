export default ({ env }) => {
  const client = env('DATABASE_CLIENT', 'sqlite');

  const connections = {
    sqlite: {
      connection: {
        filename: env('DATABASE_FILENAME', '.tmp/data.db'),
      },
      useNullAsDefault: true,
      pool: {
        min: 1,
        max: 1, // SQLite only supports single connection for writes
        afterCreate: (conn, done) => {
          // Enable WAL mode and set busy timeout for network storage
          // better-sqlite3 uses pragma() method
          conn.pragma('journal_mode = WAL');
          conn.pragma('busy_timeout = 30000');
          conn.pragma('synchronous = NORMAL');
          done();
        },
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
