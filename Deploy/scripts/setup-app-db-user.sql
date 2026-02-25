-- ============================================================================
-- Least-Privilege Database User Setup for TrashMob
-- ============================================================================
-- Creates a restricted SQL login and database user for application runtime.
-- The app user gets db_datareader + db_datawriter (SELECT/INSERT/UPDATE/DELETE)
-- but NO schema modification, user management, or backup permissions.
--
-- This script is idempotent — safe to run multiple times.
--
-- Usage (two-step — must run against master first, then app database):
--
--   sqlcmd -S <server> -d master -U dbadmin -P <admin-pw> \
--     -v AppUserPassword="<password>" -i setup-app-db-user.sql -b
--
--   sqlcmd -S <server> -d <app-db> -U dbadmin -P <admin-pw> \
--     -v AppUserPassword="<password>" -i setup-app-db-user.sql -b
--
-- The script detects which database it's running in and executes the
-- appropriate statements (login creation in master, user setup in app db).
-- ============================================================================

-- Step 1: Create server-level login (only runs in master)
IF DB_NAME() = 'master'
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.server_principals WHERE name = 'trashmob_app')
    BEGIN
        CREATE LOGIN [trashmob_app] WITH PASSWORD = N'$(AppUserPassword)';
        PRINT 'Created login [trashmob_app]';
    END
    ELSE
    BEGIN
        -- Update password to match the provided value (idempotent)
        ALTER LOGIN [trashmob_app] WITH PASSWORD = N'$(AppUserPassword)';
        PRINT 'Login [trashmob_app] already exists — password updated';
    END
END
GO

-- Step 2: Create database user and grant roles (only runs in app database)
IF DB_NAME() != 'master'
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.database_principals WHERE name = 'trashmob_app')
    BEGIN
        CREATE USER [trashmob_app] FOR LOGIN [trashmob_app];
        PRINT 'Created database user [trashmob_app]';
    END
    ELSE
        PRINT 'Database user [trashmob_app] already exists';

    -- Grant data-level permissions (idempotent — adding an existing member is a no-op)
    ALTER ROLE db_datareader ADD MEMBER [trashmob_app];
    ALTER ROLE db_datawriter ADD MEMBER [trashmob_app];
    PRINT 'Granted db_datareader and db_datawriter roles to [trashmob_app]';
END
GO
