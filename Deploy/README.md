# Deploy

Infrastructure-as-code (Bicep templates), deployment scripts, and operational documentation for TrashMob.

## Documentation

| Document | Description |
|----------|-------------|
| [OPERATIONS_RUNBOOK.md](./OPERATIONS_RUNBOOK.md) | Infrastructure ops: rollback, backups, DNS, Key Vault, SSL, Strapi |
| [CONTAINER_DEPLOYMENT_GUIDE.md](./CONTAINER_DEPLOYMENT_GUIDE.md) | Docker builds, Azure Container Apps deployment |
| [CUSTOM_DOMAIN_MIGRATION.md](./CUSTOM_DOMAIN_MIGRATION.md) | Custom domain and SSL certificate setup |
| [OIDC_SETUP_GUIDE.md](./OIDC_SETUP_GUIDE.md) | GitHub Actions OIDC federation for deployments |
| [COST_ALERT_RUNBOOK.md](./COST_ALERT_RUNBOOK.md) | Azure cost alerts and budget monitoring |

## Quick Start

```powershell
# Log in and set subscription
az login
az account set --subscription <subscriptionName>

# Deploy infrastructure for a new environment
.\deployInfra.ps1 -environment <env> -region <regionName> -subscriptionId <subscriptionId>
```

See [OPERATIONS_RUNBOOK.md](./OPERATIONS_RUNBOOK.md) for detailed procedures.
