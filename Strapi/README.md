# Strapi CMS

Content management system for TrashMob, built on [Strapi v5](https://strapi.io/).

## Architecture

- **Database:** SQLite (both local and deployed)
- **Deployed storage:** Azure Files mounts for persistent SQLite database (`/app/data/strapi.db`) and media uploads (`/app/public/uploads`)
- **Single replica:** Required because SQLite doesn't support concurrent writes. Do not increase `maxReplicas` without migrating to PostgreSQL.
- **Bootstrap:** `src/index.ts` auto-configures public read permissions and seeds default news categories on every startup.

## Content Types

| Content Type | API Endpoint | Description |
|--------------|-------------|-------------|
| News Post | `/api/news-posts` | Blog articles with title, slug, excerpt, body (blocks), author, category, tags, cover image |
| News Category | `/api/news-categories` | Categories for organizing posts (seeded: Announcements, Community Stories, Tips & Guides) |
| Hero Section | `/api/hero-section` | Home page hero content |
| What Is TrashMob | `/api/what-is-trashmob` | Home page "What is TrashMob" section |
| Getting Started | `/api/getting-started` | Home page "Getting Started" section |

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

**Note:** Local development uses SQLite at `.tmp/data.db` (ephemeral). The bootstrap function seeds categories and permissions on startup, so you get a working CMS immediately. Data created locally does not sync to deployed environments.

## Deployed Environments

| Environment | Container App | Admin URL |
|-------------|---------------|-----------|
| **Development** | `ca-strapi-tm-dev-westus2` | `https://ca-strapi-tm-dev-westus2.ashypebble-059d2628.westus2.azurecontainerapps.io/admin` |
| **Production** | `ca-strapi-tm-pr-westus2` | See Azure Portal for FQDN |

See `Deploy/OPERATIONS_RUNBOOK.md` for full infrastructure details (storage, secrets, bootstrap, admin setup).

## Adding Content via API

After registering an admin account, create an API token via the admin panel or API:

```bash
STRAPI_URL="https://<strapi-fqdn>"

# Login
ADMIN_TOKEN=$(curl -s -X POST "$STRAPI_URL/admin/login" \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"yourpassword"}' | jq -r '.data.token')

# Create full-access API token
API_TOKEN=$(curl -s -X POST "$STRAPI_URL/admin/api-tokens" \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"My Token","type":"full-access","lifespan":null}' | jq -r '.data.accessKey')

# Create a news post
curl -X POST "$STRAPI_URL/api/news-posts" \
  -H "Authorization: Bearer $API_TOKEN" \
  -H "Content-Type: application/json" \
  -d @article.json
```

Article JSON format: see `Planning/NewsArticles/strapi-upload.json` for an example with Strapi blocks body format.
