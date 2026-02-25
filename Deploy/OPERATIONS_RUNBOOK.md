# Operations Runbook

This document contains infrastructure operations procedures for the TrashMob platform. For development setup and coding standards, see the root [CLAUDE.md](../CLAUDE.md).

## Azure Resources

| Environment | Container App | Environment | Resource Group | Custom Domain |
|-------------|---------------|-------------|----------------|---------------|
| **Production** | `ca-tm-pr-westus2` | `cae-tm-pr-westus2` | `rg-trashmob-pr-westus2` | `www.trashmob.eco` |
| **Development** | `ca-tm-dev-westus2` | `cae-tm-dev-westus2` | `rg-trashmob-dev-westus2` | `dev.trashmob.eco` |

## Custom Domain & SSL Certificate

Both sites use Azure-managed SSL certificates bound to Container Apps. Managed certificates **auto-renew automatically** - no manual intervention required.

**Verify current certificate binding:**
```bash
# Production
az containerapp hostname list --name ca-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2

# Development
az containerapp hostname list --name ca-tm-dev-westus2 --resource-group rg-trashmob-dev-westus2
```

**Check certificate status:**
```bash
# Production
az containerapp env certificate list --name cae-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2 --query "[?name=='trashmob-eco-cert']"

# Development
az containerapp env certificate list --name cae-tm-dev-westus2 --resource-group rg-trashmob-dev-westus2 --query "[?name=='dev-trashmob-eco-cert']"
```

**If certificate needs to be recreated** (rare - only if deleted or corrupted):
```bash
# 1. Add TXT record for domain verification (get token from Azure Portal or CLI error message)
#    Name: asuid.www.trashmob.eco
#    Value: <verification-token>

# 2. Add hostname to container app
az containerapp hostname add \
  --name ca-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --hostname www.trashmob.eco

# 3. Create managed certificate (takes 2-3 minutes to provision)
az containerapp env certificate create \
  --name cae-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --hostname www.trashmob.eco \
  --certificate-name trashmob-eco-cert \
  --validation-method CNAME

# 4. Bind certificate to hostname
az containerapp hostname bind \
  --name ca-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --hostname www.trashmob.eco \
  --certificate trashmob-eco-cert \
  --environment cae-tm-pr-westus2
```

**DNS Requirements:**

| Environment | Record Type | Name | Value |
|-------------|-------------|------|-------|
| Production | CNAME | `www` | `ca-tm-pr-westus2.greenground-fd8fc385.westus2.azurecontainerapps.io` |
| Production | TXT | `asuid.www` | domain verification token (initial setup only) |
| Development | CNAME | `dev` | `ca-tm-dev-westus2.ashypebble-059d2628.westus2.azurecontainerapps.io` |
| Development | TXT | `asuid.dev` | domain verification token (initial setup only) |

**DNS Management:** DNS records for trashmob.eco are managed in [Microsoft 365 Admin Center](https://admin.cloud.microsoft) under Domains.

See `CUSTOM_DOMAIN_MIGRATION.md` for full migration documentation.

## Apex Domain (trashmob.eco) with Azure Front Door

Azure Container Apps doesn't support apex/root domains with managed certificates directly. To handle both `trashmob.eco` and `www.trashmob.eco`, use Azure Front Door:

**Deploy Front Door:**
```bash
# Deploy Front Door for production
az deployment group create \
  --resource-group rg-trashmob-pr-westus2 \
  --template-file Deploy/frontDoor.bicep \
  --parameters \
    environment=pr \
    containerAppFqdn=ca-tm-pr-westus2.greenground-fd8fc385.westus2.azurecontainerapps.io \
    primaryDomain=www.trashmob.eco \
    apexDomain=trashmob.eco
```

**DNS Configuration for Front Door:**

| Record Type | Name | Value |
|-------------|------|-------|
| CNAME | `www` | `fde-tm-pr.azurefd.net` (Front Door endpoint) |
| ALIAS/ANAME | `@` (apex) | `fde-tm-pr.azurefd.net` (requires Azure DNS or Cloudflare) |
| TXT | `_dnsauth.www` | validation token from Azure Portal |
| TXT | `_dnsauth` | validation token from Azure Portal |

**Note:** Microsoft 365 DNS doesn't support ALIAS records for apex domains. Options:
1. Migrate DNS to Azure DNS (supports alias records to Front Door)
2. Use Cloudflare DNS (free, supports CNAME flattening for apex)
3. Keep current setup with only `www.trashmob.eco` working

**Bicep template:** `frontDoor.bicep`

## Azure DNS Migration (for Apex Domain Support)

To support the apex domain (`trashmob.eco`) with Azure Front Door, migrate DNS from Microsoft 365 to Azure DNS:

**Step 1: Deploy Azure DNS Zone**
```bash
az deployment group create \
  --resource-group rg-trashmob-pr-westus2 \
  --template-file Deploy/dnsZone.bicep \
  --parameters \
    zoneName=trashmob.eco \
    environment=pr \
    frontDoorEndpointHostname=fde-tm-pr.azurefd.net \
    frontDoorEndpointId=/subscriptions/<sub-id>/resourceGroups/rg-trashmob-pr-westus2/providers/Microsoft.Cdn/profiles/fd-tm-pr/afdEndpoints/fde-tm-pr \
    useFrontDoor=true
```

**Step 2: Note Azure Nameservers**
The deployment outputs Azure's nameservers (e.g., `ns1-01.azure-dns.com`). You'll need all 4.

**Step 3: Update Domain Registrar**
Go to your domain registrar (where trashmob.eco was purchased) and update nameservers to Azure's:
- `ns1-01.azure-dns.com`
- `ns2-01.azure-dns.net`
- `ns3-01.azure-dns.org`
- `ns4-01.azure-dns.info`

**Step 4: Wait for Propagation**
DNS propagation can take 24-48 hours. Use `dig` or `nslookup` to verify.

**Step 5: Update Validation Tokens**
After Front Door is deployed, update the `_dnsauth` TXT records with actual validation tokens from Azure Portal.

**Included Records:**
- WWW CNAME -> Front Door
- Apex ALIAS -> Front Door (Azure DNS alias record)
- Dev CNAME -> Dev Container App
- MX, SPF, DKIM -> Microsoft 365 email
- Autodiscover CNAME -> Outlook

**Bicep template:** `dnsZone.bicep`

## Deployment Rollback Procedures

Azure Container Apps keeps multiple revisions available. Use these procedures to roll back a deployment.

**View available revisions:**
```bash
# Production
az containerapp revision list --name ca-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2 --output table

# Development
az containerapp revision list --name ca-tm-dev-westus2 --resource-group rg-trashmob-dev-westus2 --output table
```

**Roll back to a previous revision:**
```bash
# Production - activate a previous revision (replace <revision-name> with actual name from list)
az containerapp revision activate --name ca-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2 --revision <revision-name>

# Then route 100% traffic to that revision
az containerapp ingress traffic set --name ca-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2 --revision-weight <revision-name>=100
```

**Roll back using a previous container image:**
```bash
# List images in Azure Container Registry
az acr repository show-tags --name crtmprwestus2 --repository trashmob --orderby time_desc --output table

# Deploy a specific image tag (e.g., a previous git SHA)
az containerapp update --name ca-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2 --image crtmprwestus2.azurecr.io/trashmob:<previous-tag>
```

**Rollback strategy:**
- Keep 3 previous container images in ACR (automatic via GitHub Actions tagging)
- Each deployment creates a new revision; old revisions remain available
- Target rollback time: <= 5 minutes
- Always verify rollback success by checking application health

**Emergency rollback checklist:**
1. Identify the issue (check Application Insights, user reports)
2. List available revisions or images
3. Execute rollback command
4. Verify application is healthy (check `/health` endpoint or Swagger)
5. Notify team of rollback
6. Investigate root cause before redeploying

## Database Backups & Restore

Azure SQL databases have automated backup with the following retention policies:

| Backup Type | Retention | Purpose |
|-------------|-----------|---------|
| **Point-in-Time (PITR)** | 14 days | Restore to any point within the retention window |
| **Weekly LTR** | 4 weeks | Weekly full backups for longer-term recovery |
| **Monthly LTR** | 12 months | Monthly backups for compliance and audit |

**Backup configuration:** `sqlDatabase.bicep` and `sqlDatabaseStrapi.bicep`
**Alerting:** `backupAlerts.bicep` - Monitors database health and sends alerts to `info@trashmob.eco`

**View current backup settings:**
```bash
# Short-term retention
az sql db str-policy show --name db-tm-pr-westus2 --server sql-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2

# Long-term retention
az sql db ltr-policy show --name db-tm-pr-westus2 --server sql-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2
```

**Point-in-Time Restore (within 14 days):**
```bash
# Restore to a specific time (creates new database)
az sql db restore --dest-name db-tm-pr-westus2-restored \
  --name db-tm-pr-westus2 --server sql-tm-pr-westus2 \
  --resource-group rg-trashmob-pr-westus2 \
  --time "2026-01-25T12:00:00Z"
```

**Long-Term Retention Restore:**
```bash
# List available LTR backups
az sql db ltr-backup list --location westus2 \
  --server sql-tm-pr-westus2 --database db-tm-pr-westus2

# Restore from LTR backup
az sql db ltr-backup restore --dest-database db-tm-pr-westus2-restored \
  --dest-server sql-tm-pr-westus2 \
  --dest-resource-group rg-trashmob-pr-westus2 \
  --backup-id "<backup-id-from-list>"
```

**Recovery objectives:**
- **RPO (Recovery Point Objective):** < 1 hour
- **RTO (Recovery Time Objective):** < 4 hours

See Project 32 (Database Backups) for full documentation and runbook.

## Key Vault Access (RBAC Authorization)

TrashMob uses Azure Key Vault with **RBAC authorization** (not access policies). This means:

1. **Managed Identities** (Container Apps, Container App Jobs) are granted the `Key Vault Secrets User` role to read secrets at runtime
2. **Developers** need the `Key Vault Secrets Officer` role to create/update secrets
3. **GitHub Actions** deployment workflows automatically grant RBAC roles to managed identities

**Check your Key Vault permissions:**
```bash
# List your role assignments on the Key Vault
az role assignment list --scope $(az keyvault show --name kv-tm-dev-westus2 --query id -o tsv) --assignee $(az account show --query user.name -o tsv) -o table
```

**Request access if needed:**
Contact the engineering lead to be granted `Key Vault Secrets Officer` role for development environments.

See `keyVault.bicep` and Project 26 documentation for technical details on the RBAC model.

## Strapi CMS Infrastructure

The Strapi CMS runs as a separate Container App with external ingress for admin access. It uses **SQLite** with persistent Azure Files storage for the database (Strapi v5 dropped MSSQL support). Media uploads are stored on a separate Azure Files share.

| Environment | Container App | Storage Account |
|-------------|---------------|-----------------|
| **Development** | `ca-strapi-tm-dev-westus2` | `stortmdevwestus2` |
| **Production** | `ca-strapi-tm-pr-westus2` | `stortmprwestus2` |

### Storage Architecture

| Azure Files Share | Mount Path | Purpose |
|-------------------|------------|---------|
| `strapi-data` | `/app/data` | SQLite database (`strapi.db`) — persistent across restarts |
| `strapi-uploads` | `/app/public/uploads` | Media uploads (images, files) |

**Important:** `maxReplicas` is set to 1 in the Bicep template because SQLite does not support concurrent writes from multiple replicas. Do not increase this without migrating to PostgreSQL.

### Bootstrap (Auto-Configuration)

On every startup, `Strapi/src/index.ts` runs a bootstrap function that:
1. **Configures public read permissions** for news-posts, news-categories, hero-section, what-is-trashmob, and getting-started content types
2. **Seeds default news categories** (Announcements, Community Stories, Tips & Guides) if they don't exist

This ensures the CMS API is functional immediately after deployment without manual admin configuration.

### Admin Setup

After a fresh deployment (new persistent storage), register the first admin at:
```
POST https://<strapi-fqdn>/admin/register-admin
```

Or visit the admin panel in a browser at `https://<strapi-fqdn>/admin` and follow the registration wizard.

### Key Vault Secrets

**Note:** You need the `Key Vault Secrets Officer` RBAC role to create secrets (see Key Vault Access section above).

| Secret Name | Purpose |
|-------------|---------|
| `strapi-admin-jwt-secret` | JWT secret for admin panel authentication |
| `strapi-api-token-salt` | Salt for API token generation |
| `strapi-app-keys` | Application keys (comma-separated) |
| `strapi-transfer-token-salt` | Salt for transfer tokens |

**Create secrets for a new environment:**
```bash
# Replace kv-tm-dev-westus2 with appropriate Key Vault name
az keyvault secret set --vault-name kv-tm-dev-westus2 --name strapi-admin-jwt-secret --value "$(openssl rand -base64 32)"
az keyvault secret set --vault-name kv-tm-dev-westus2 --name strapi-api-token-salt --value "$(openssl rand -base64 32)"
az keyvault secret set --vault-name kv-tm-dev-westus2 --name strapi-app-keys --value "$(openssl rand -base64 32),$(openssl rand -base64 32)"
az keyvault secret set --vault-name kv-tm-dev-westus2 --name strapi-transfer-token-salt --value "$(openssl rand -base64 32)"
```

### Key Files

- Bicep template: `Deploy/containerAppStrapi.bicep`
- Database config: `Strapi/config/database.ts`
- Bootstrap / seed: `Strapi/src/index.ts`
- Dev workflow: `.github/workflows/container_strapi-tm-dev-westus2.yml`
- Prod workflow: `.github/workflows/release_strapi-tm-pr-westus2.yml`
- Source: `Strapi/`

For local development setup, see `Strapi/README.md`.
