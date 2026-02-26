#!/bin/sh
# Entrypoint script that copies SQLite DB from Azure Files (persistent)
# to local ephemeral storage (where SQLite locking works properly),
# runs Strapi, and syncs the DB back periodically.
#
# Azure Files uses SMB which doesn't support the POSIX file locking
# that SQLite requires, causing "database is locked" errors.

PERSISTENT_DIR="/app/data"
LOCAL_DIR="/app/.tmp"
DB_NAME="strapi.db"

# Ensure local directory exists
mkdir -p "$LOCAL_DIR"

# Restore DB from persistent storage if it exists and is non-empty
if [ -f "$PERSISTENT_DIR/$DB_NAME" ] && [ -s "$PERSISTENT_DIR/$DB_NAME" ]; then
  echo "Restoring database from persistent storage..."
  cp "$PERSISTENT_DIR/$DB_NAME" "$LOCAL_DIR/$DB_NAME"
  echo "Database restored ($(wc -c < "$PERSISTENT_DIR/$DB_NAME") bytes)"
else
  echo "No existing database found, Strapi will create a fresh one."
fi

# Background sync: copy local DB back to persistent storage every 5 minutes
sync_db() {
  if [ -f "$LOCAL_DIR/$DB_NAME" ] && [ -s "$LOCAL_DIR/$DB_NAME" ]; then
    cp "$LOCAL_DIR/$DB_NAME" "$PERSISTENT_DIR/$DB_NAME" 2>/dev/null && \
      echo "$(date -u +%Y-%m-%dT%H:%M:%SZ) Database synced to persistent storage"
  fi
}

(
  while true; do
    sleep 300
    sync_db
  done
) &
SYNC_PID=$!

# Override DATABASE_FILENAME to use local storage
export DATABASE_FILENAME="$LOCAL_DIR/$DB_NAME"

# Start Strapi in the background so we can handle signals
echo "Starting Strapi with local database at $DATABASE_FILENAME"
npm run start &
STRAPI_PID=$!

# Trap shutdown signals to sync DB before exit
cleanup() {
  echo "Shutting down - syncing database to persistent storage..."
  sync_db
  kill $SYNC_PID 2>/dev/null
  kill $STRAPI_PID 2>/dev/null
  wait $STRAPI_PID 2>/dev/null
  exit 0
}
trap cleanup SIGTERM SIGINT

# Wait for Strapi to exit
wait $STRAPI_PID
EXIT_CODE=$?

# Sync on normal exit too
sync_db
kill $SYNC_PID 2>/dev/null

exit $EXIT_CODE
