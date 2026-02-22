# Strapi CMS

Content management system for TrashMob, built on [Strapi](https://strapi.io/).

## Local Development

```bash
cd Strapi
npm install

# Create .env file with required secrets
cat > .env << 'EOF'
ADMIN_JWT_SECRET=local-dev-jwt-secret-32chars-min
API_TOKEN_SALT=local-dev-api-token-salt-here
APP_KEYS=key1-for-local-dev,key2-for-local-dev
TRANSFER_TOKEN_SALT=local-dev-transfer-salt-here
DATABASE_CLIENT=sqlite
DATABASE_FILENAME=.tmp/data.db
EOF

# Create required directories
mkdir -p public/uploads .tmp

# Development mode (auto-reloads on changes)
npm run develop
# Strapi admin: http://localhost:1337/admin

# Production mode
npm run build
npm run start
```

**Note:** Local Strapi uses SQLite for simplicity. Deployed environments (dev/prod) use Azure SQL for persistent storage. Data created locally will not sync to deployed environments.

## Deployed Environments

See `Deploy/OPERATIONS_RUNBOOK.md` for infrastructure details (Container App, Azure SQL, Key Vault secrets).
