# Container Deployment Guide for TrashMob

This guide explains the container-based deployment infrastructure for TrashMob applications.

## Overview

The TrashMob infrastructure has been enhanced to support containerized deployments using Azure Container Apps. This includes:

1. **Azure Container Registry (ACR)** - Stores Docker images
2. **Azure Container Apps Environment** - Hosting environment for containers
3. **Azure Container App** - Runs the TrashMob web application
4. **Azure Container App Job** - Runs TrashMobJobs on a schedule

## New Bicep Modules

### 1. containerRegistry.bicep
Creates an Azure Container Registry to store Docker images.

**Parameters:**
- `region` - Azure region
- `containerRegistryName` - Name of the registry (e.g., acrtmdevwestus2)
- `environment` - Environment tag (dev, staging, prod)

### 2. containerAppsEnvironment.bicep
Creates the Container Apps Environment with Log Analytics integration.

**Parameters:**
- `region` - Azure region
- `containerAppsEnvironmentName` - Name of the environment
- `logAnalyticsWorkspaceName` - Existing Log Analytics workspace name
- `environment` - Environment tag

### 3. containerApp.bicep
Deploys the TrashMob web application as a Container App.

**Parameters:**
- `region` - Azure region
- `containerAppName` - Name of the container app
- `containerAppsEnvironmentId` - Resource ID of the Container Apps Environment
- `containerRegistryName` - ACR name
- `containerImage` - Full image name (e.g., acrtmdevwestus2.azurecr.io/trashmob:latest)
- `keyVaultName` - Key Vault name for secrets
- `environment` - Environment tag
- `minReplicas` - Minimum number of replicas (default: 1)
- `maxReplicas` - Maximum number of replicas (default: 3)

**Features:**
- Auto-scaling based on HTTP requests
- System-assigned managed identity
- Key Vault integration for secrets
- Exposes port 8080 with HTTPS

### 4. containerAppJob.bicep
Deploys TrashMobJobs as a scheduled Container App Job.

**Parameters:**
- `region` - Azure region
- `containerAppJobName` - Name of the container app job
- `containerAppsEnvironmentId` - Resource ID of the Container Apps Environment
- `containerRegistryName` - ACR name
- `containerImage` - Full image name
- `keyVaultName` - Key Vault name
- `azureMapsName` - Azure Maps account name
- `storageAccountName` - Storage account name
- `environment` - Environment tag
- `cronExpression` - Cron schedule (default: every 6 hours)

**Features:**
- Scheduled execution via cron
- System-assigned managed identity
- Key Vault integration
- 30-minute timeout per execution

## Dockerfiles

### TrashMob Web App ([TrashMob/Dockerfile](../TrashMob/Dockerfile))
Multi-stage Docker build:
1. **Build stage** - Installs Node.js, builds React app, compiles .NET app
2. **Publish stage** - Creates production build
3. **Runtime stage** - Uses minimal ASP.NET runtime image

**Exposed Ports:** 8080, 8081

### TrashMobJobs ([TrashMobJobs/Dockerfile](../TrashMobJobs/Dockerfile))
Multi-stage Docker build for the jobs service.

## GitHub Actions Workflows

### Development Workflows

#### Web App: container_ca-tm-dev-westus2.yml
Builds and deploys the TrashMob web application as a container to the **dev** environment.

**Triggers:**
- Push to `main` branch affecting TrashMob files
- Manual workflow dispatch

**Environment:** dev
**Resources:** ca-tm-dev-westus2, acrtmdevwestus2
**Scaling:** 1-3 replicas

**Steps:**
1. Checkout code with GitVersion
2. Login to Azure using OIDC
3. Build and push Docker image to ACR (with version tag and latest)
4. Deploy to Container App using bicep template
5. Output Container App URL

#### Jobs: container_caj-tm-dev-westus2.yml
Builds and deploys TrashMobJobs as a scheduled container job to the **dev** environment.

**Triggers:**
- Push to `main` branch affecting TrashMobJobs files
- Manual workflow dispatch

**Environment:** dev
**Resources:** caj-tm-dev-westus2, acrtmdevwestus2
**Schedule:** Every 6 hours

**Steps:**
1. Checkout code with GitVersion
2. Login to Azure using OIDC
3. Build and push Docker image to ACR
4. Deploy to Container App Job using bicep template
5. Verify job configuration

### Production Workflows

#### Web App: release_ca-tm-pr-westus2.yml
Builds and deploys the TrashMob web application as a container to the **production** environment.

**Triggers:**
- Push to `release` branch affecting TrashMob files
- Manual workflow dispatch

**Environment:** pr (production)
**Resources:** ca-tm-pr-westus2, acrtmprwestus2
**Scaling:** 2-10 replicas (higher capacity for production)

**Steps:**
1. Checkout code with GitVersion
2. Login to Azure using OIDC
3. Build and push Docker image to ACR (with version tag and latest)
4. Deploy to Container App using bicep template
5. Output Container App URL

#### Jobs: release_caj-tm-pr-westus2.yml
Builds and deploys TrashMobJobs as a scheduled container job to the **production** environment.

**Triggers:**
- Push to `release` branch affecting TrashMobJobs files
- Manual workflow dispatch

**Environment:** pr (production)
**Resources:** caj-tm-pr-westus2, acrtmprwestus2
**Schedule:** Every 6 hours

**Steps:**
1. Checkout code with GitVersion
2. Login to Azure using OIDC
3. Build and push Docker image to ACR
4. Deploy to Container App Job using bicep template
5. Verify job configuration

## Deployment Instructions

### First-Time Setup

1. **Deploy Infrastructure**

   Run the updated `deployInfra.ps1` script:
   ```powershell
   cd Deploy
   .\deployInfra.ps1 -environment dev -region westus2 -subscriptionId <your-subscription-id> -sqlAdminPassword "<password>" -alwaysOn $False
   ```

   This will create:
   - All existing resources (SQL, Key Vault, Storage, etc.)
   - Azure Container Registry
   - Azure Container Apps Environment

2. **Configure GitHub Secrets**

   Ensure these secrets are configured in your GitHub repository:
   - `AZURE_CLIENT_ID` - Service principal client ID
   - `AZURE_TENANT_ID` - Azure AD tenant ID
   - `AZURE_SUBSCRIPTION_ID` - Azure subscription ID

3. **Deploy Containers**

   The GitHub Actions workflows will automatically deploy when you push to main, or you can manually trigger them:
   - Go to Actions tab in GitHub
   - Select the workflow
   - Click "Run workflow"

### Naming Convention

For the `dev` environment in `westus2` region:

| Resource Type | Dev Environment | Production Environment |
|--------------|-----------------|------------------------|
| Container Registry | `acrtmdevwestus2` | `acrtmprwestus2` |
| Container Apps Environment | `cae-tm-dev-westus2` | `cae-tm-pr-westus2` |
| Container App (Web) | `ca-tm-dev-westus2` | `ca-tm-pr-westus2` |
| Container App Job | `caj-tm-dev-westus2` | `caj-tm-pr-westus2` |
| Resource Group | `rg-trashmob-dev-westus2` | `rg-trashmob-pr-westus2` |
| Key Vault | `kv-tm-dev-westus2` | `kv-tm-pr-westus2` |
| Log Analytics | `log-tm-dev-westus2` | `log-tm-pr-westus2` |

**Note:** Container Registry names cannot contain hyphens, hence the different naming pattern.

### Environment Variables

Both containers use these environment variables:

- `ASPNETCORE_ENVIRONMENT=Production`
- `VaultUri=https://<keyvault-name>.vault.azure.net`
- `StorageAccountUri=https://<storage-account-name>.blob.core.windows.net/`
- `ApplicationInsights__ConnectionString` - Retrieved from the environment-specific Application Insights resource
- `ASPNETCORE_HTTP_PORTS=8080` (Web App only)
- `EnableSwagger=true/false` (Web App only, based on environment)
- `InstanceName=as-tm-<env>-<region>` (Jobs only)

Azure AD B2C configuration (public values, set based on environment parameter in Bicep):
- `AzureAdB2C__Instance` - B2C login endpoint (e.g., https://trashmobdev.b2clogin.com/)
- `AzureAdB2C__ClientId` - Backend API client ID for JWT validation
- `AzureAdB2C__FrontendClientId` - Frontend SPA client ID for MSAL
- `AzureAdB2C__Domain` - B2C tenant domain (e.g., trashmobdev.onmicrosoft.com)
- `AzureAdB2C__SignUpSignInPolicyId` - B2C policy ID

Secrets are retrieved from Key Vault using managed identity:
- `AzureMapsKey`
- `TMDBServerConnectionString`
- `StorageAccountKey`
- `sendGridApiKey`

## Scaling and Performance

### Web App
- **Min Replicas:** 1
- **Max Replicas:** 3
- **Scaling Rule:** HTTP concurrent requests (100 requests per instance)
- **Resources per instance:** 0.5 CPU, 1GB memory

### Jobs
- **Execution:** Scheduled (every 6 hours by default)
- **Parallelism:** 1 (one execution at a time)
- **Timeout:** 30 minutes
- **Retry Limit:** 1
- **Resources:** 0.5 CPU, 1GB memory

## Cost Optimization

Azure Container Apps pricing is based on:
- vCPU-seconds and GiB-seconds consumed
- Number of HTTP requests (web app)
- Number of executions (jobs)

The current configuration uses minimal resources (0.5 CPU, 1GB RAM) which is cost-effective for development environments.

## Monitoring and Logs

All containers send logs to the Log Analytics workspace configured in the Container Apps Environment.

**View logs:**
```bash
# Web App logs
az containerapp logs show --name ca-tm-dev-westus2 --resource-group rg-trashmob-dev-westus2 --follow

# Job execution history
az containerapp job execution list --name caj-tm-dev-westus2 --resource-group rg-trashmob-dev-westus2
```

## Troubleshooting

### Container fails to start
1. Check container logs in Log Analytics
2. Verify Key Vault permissions for managed identity
3. Ensure environment variables are correct

### Build fails in GitHub Actions
1. Verify Dockerfile paths are correct
2. Check that all project references are included
3. Ensure Node.js dependencies can be resolved

### Job doesn't execute
1. Verify cron expression is correct
2. Check job execution history
3. Review container logs for errors

## Migration from App Service/Function App

The original App Service and Function App deployments remain in the infrastructure. You can:

1. **Run both in parallel** - Test containers while keeping existing deployments
2. **Gradual migration** - Move traffic from App Service to Container App gradually
3. **Complete switch** - Update DNS/routing once containers are validated

To switch traffic, update your DNS or Front Door configuration to point to the Container App FQDN instead of the App Service URL.

## Next Steps

1. Run `deployInfra.ps1` to create the container infrastructure
2. Trigger the GitHub Actions workflows to build and deploy containers
3. Test the Container App URL to verify the web app works
4. Monitor the Container App Job executions
5. Consider removing App Service/Function App resources once containers are validated

## Post-Production Deployment Checklist

After deploying to production, complete these manual steps:

1. **Upload TrashMob.eco Waiver** - Log into the production admin portal and upload/create the global TrashMob.eco waiver document in the Waiver Management section. This is required for users to sign waivers before attending events.

2. Verify database migrations ran successfully
3. Test user authentication flow
4. Verify email notifications are working
5. Check Application Insights for any startup errors
6. **Review Google Search Console** - Visit [Google Search Console](https://search.google.com/search-console?resource_id=https://www.trashmob.eco/) to check for any remaining site issues (mobile usability, indexing problems, etc.)

## Additional Resources

- [Azure Container Apps documentation](https://learn.microsoft.com/azure/container-apps/)
- [Bicep documentation](https://learn.microsoft.com/azure/azure-resource-manager/bicep/)
- [Docker best practices](https://docs.docker.com/develop/dev-best-practices/)
