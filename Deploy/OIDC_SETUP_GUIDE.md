# GitHub OIDC Authentication Setup for Multi-Subscription Deployment

This guide explains how to configure GitHub OIDC authentication for deploying to multiple Azure subscriptions (dev and production).

## Overview

TrashMob uses two different Azure subscriptions:
- **Dev Subscription**: Used by `main` branch workflows
- **Production Subscription**: Used by `release` branch workflows

GitHub OIDC allows workflows to authenticate to Azure without storing credentials as secrets. Instead, we use:
- A single Azure AD App Registration
- Federated credentials for different branches
- Role assignments in both subscriptions
- GitHub Environments to store subscription-specific settings

## Architecture

```
GitHub Workflows
├── main branch → test environment → Dev Subscription
└── release branch → production environment → Production Subscription

Azure AD App Registration (shared)
├── Federated Credential: main branch
├── Federated Credential: release branch
├── Service Principal
    ├── Contributor role in Dev Subscription
    └── Contributor role in Production Subscription
```

## Step 1: Create Azure AD App Registration

This app registration will be shared across both subscriptions:

```bash
# Login to Azure (use an account with access to both subscriptions)
az login

# Create app registration
APP_NAME="GitHub-TrashMob-OIDC"
APP_ID=$(az ad app create --display-name "$APP_NAME" --query appId -o tsv)
echo "Application (Client) ID: $APP_ID"

# Save this - you'll need it for GitHub secrets
echo $APP_ID > app-id.txt

# Create service principal
az ad sp create --id $APP_ID
```

## Step 2: Configure Federated Credentials

Add federated credentials for both the main and release branches:

```bash
# Federated credential for main branch (dev deployments)
az ad app federated-credential create --id $APP_ID --parameters '{
  "name": "GitHub-TrashMob-Main",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:TrashMob-eco/TrashMob:ref:refs/heads/main",
  "audiences": ["api://AzureADTokenExchange"]
}'

# Federated credential for release branch (production deployments)
az ad app federated-credential create --id $APP_ID --parameters '{
  "name": "GitHub-TrashMob-Release",
  "issuer": "https://token.actions.githubusercontent.com",
  "subject": "repo:TrashMob-eco/TrashMob:ref:refs/heads/release",
  "audiences": ["api://AzureADTokenExchange"]
}'

# Verify credentials were created
az ad app federated-credential list --id $APP_ID --query "[].{name:name, subject:subject}" -o table
```

## Step 3: Assign Roles in Dev Subscription

```bash
# Switch to dev subscription
DEV_SUBSCRIPTION_ID="<your-dev-subscription-id>"
az account set --subscription $DEV_SUBSCRIPTION_ID

# Assign Contributor role
az role assignment create \
  --assignee $APP_ID \
  --role Contributor \
  --scope /subscriptions/$DEV_SUBSCRIPTION_ID

# Verify assignment
az role assignment list \
  --assignee $APP_ID \
  --subscription $DEV_SUBSCRIPTION_ID \
  --query "[].{role:roleDefinitionName, scope:scope}" -o table
```

## Step 4: Assign Roles in Production Subscription

```bash
# Switch to production subscription
PROD_SUBSCRIPTION_ID="<your-prod-subscription-id>"
az account set --subscription $PROD_SUBSCRIPTION_ID

# Assign Contributor role
az role assignment create \
  --assignee $APP_ID \
  --role Contributor \
  --scope /subscriptions/$PROD_SUBSCRIPTION_ID

# Verify assignment
az role assignment list \
  --assignee $APP_ID \
  --subscription $PROD_SUBSCRIPTION_ID \
  --query "[].{role:roleDefinitionName, scope:scope}" -o table
```

## Step 5: Get Tenant ID

```bash
# Get your Azure AD tenant ID
TENANT_ID=$(az account show --query tenantId -o tsv)
echo "Tenant ID: $TENANT_ID"
```

## Step 6: Configure GitHub Repository Secrets

These secrets are shared across all environments and should be set at the **repository level**:

1. Go to: `https://github.com/TrashMob-eco/TrashMob/settings/secrets/actions`
2. Click "New repository secret"
3. Add these three secrets:

| Secret Name | Value | Description |
|-------------|-------|-------------|
| `AZURE_CLIENT_ID` | `<APP_ID from Step 1>` | The Application (client) ID |
| `AZURE_TENANT_ID` | `<TENANT_ID from Step 5>` | Your Azure AD tenant ID |

**Important:** Do NOT add `AZURE_SUBSCRIPTION_ID` at the repository level - it will be environment-specific.

## Step 7: Configure GitHub Environments

GitHub Environments allow different subscriptions for dev vs production.

### Configure Test Environment

1. Go to: `https://github.com/TrashMob-eco/TrashMob/settings/environments`
2. The `test` environment should already exist
3. Click on `test` to configure it
4. Click "Configure environment"
5. Under "Environment secrets", add:
   - Name: `AZURE_SUBSCRIPTION_ID`
   - Value: `<your-dev-subscription-id>`

### Create Production Environment

1. In the same Environments page, click "New environment"
2. Name: `production`
3. Click "Configure environment"
4. **Optional:** Add protection rules:
   - ✅ Required reviewers (recommended for production)
   - ✅ Wait timer
   - ✅ Deployment branches (limit to `release` branch)
5. Under "Environment secrets", add:
   - Name: `AZURE_SUBSCRIPTION_ID`
   - Value: `<your-prod-subscription-id>`

## Step 8: Verify Configuration

Run this verification script to check everything is configured correctly:

```bash
#!/bin/bash

APP_ID="<your-app-id>"
DEV_SUB="<dev-subscription-id>"
PROD_SUB="<prod-subscription-id>"
TENANT_ID="<tenant-id>"

echo "=== OIDC Configuration Verification ==="
echo ""

# Check app exists
echo "1. Checking App Registration..."
az ad app show --id $APP_ID > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ App Registration exists"
else
    echo "❌ App Registration not found"
    exit 1
fi

# Check service principal
echo "2. Checking Service Principal..."
az ad sp show --id $APP_ID > /dev/null 2>&1
if [ $? -eq 0 ]; then
    echo "✅ Service Principal exists"
else
    echo "❌ Service Principal not found"
    exit 1
fi

# Check federated credentials
echo "3. Checking Federated Credentials..."
CREDS=$(az ad app federated-credential list --id $APP_ID -o json)
MAIN_CRED=$(echo $CREDS | jq -r '.[] | select(.subject=="repo:TrashMob-eco/TrashMob:ref:refs/heads/main") | .name')
RELEASE_CRED=$(echo $CREDS | jq -r '.[] | select(.subject=="repo:TrashMob-eco/TrashMob:ref:refs/heads/release") | .name')

if [ -n "$MAIN_CRED" ]; then
    echo "✅ Main branch credential: $MAIN_CRED"
else
    echo "❌ Main branch credential missing"
fi

if [ -n "$RELEASE_CRED" ]; then
    echo "✅ Release branch credential: $RELEASE_CRED"
else
    echo "❌ Release branch credential missing"
fi

# Check dev subscription role
echo "4. Checking Dev Subscription Role Assignment..."
az account set --subscription $DEV_SUB
DEV_ROLE=$(az role assignment list --assignee $APP_ID --query "[?roleDefinitionName=='Contributor']" -o json)
if [ "$(echo $DEV_ROLE | jq '. | length')" -gt 0 ]; then
    echo "✅ Contributor role assigned in Dev subscription"
else
    echo "❌ No Contributor role in Dev subscription"
fi

# Check prod subscription role
echo "5. Checking Production Subscription Role Assignment..."
az account set --subscription $PROD_SUB
PROD_ROLE=$(az role assignment list --assignee $APP_ID --query "[?roleDefinitionName=='Contributor']" -o json)
if [ "$(echo $PROD_ROLE | jq '. | length')" -gt 0 ]; then
    echo "✅ Contributor role assigned in Production subscription"
else
    echo "❌ No Contributor role in Production subscription"
fi

echo ""
echo "=== GitHub Secrets Configuration ==="
echo "Set these at repository level:"
echo "  AZURE_CLIENT_ID = $APP_ID"
echo "  AZURE_TENANT_ID = $TENANT_ID"
echo ""
echo "Set these in 'test' environment:"
echo "  AZURE_SUBSCRIPTION_ID = $DEV_SUB"
echo ""
echo "Set these in 'production' environment:"
echo "  AZURE_SUBSCRIPTION_ID = $PROD_SUB"
echo ""
echo "=== Verification Complete ==="
```

Save as `verify-oidc.sh`, update the values, and run:
```bash
chmod +x verify-oidc.sh
./verify-oidc.sh
```

## Workflow Structure

### Dev Workflows (main branch)
- **Branch**: `main`
- **Environment**: `test`
- **Subscription**: Dev subscription (from environment secret)
- **Workflows**:
  - `container_ca-tm-dev-westus2.yml`
  - `container_caj-tm-dev-westus2.yml`

### Production Workflows (release branch)
- **Branch**: `release`
- **Environment**: `production`
- **Subscription**: Production subscription (from environment secret)
- **Workflows**:
  - `release_ca-tm-pr-westus2.yml`
  - `release_caj-tm-pr-westus2.yml`

## Troubleshooting

### Error: "No subscriptions found"
**Cause**: Service principal doesn't have access to the subscription

**Fix**:
```bash
az role assignment create \
  --assignee <APP_ID> \
  --role Contributor \
  --scope /subscriptions/<SUBSCRIPTION_ID>
```

### Error: "AADSTS70021: No matching federated identity record found"
**Cause**: Federated credential subject doesn't match

**Fix**: Verify the subject format is exactly:
- Main: `repo:TrashMob-eco/TrashMob:ref:refs/heads/main`
- Release: `repo:TrashMob-eco/TrashMob:ref:refs/heads/release`

### Error: "Login failed with exit code 1"
**Possible causes**:
1. GitHub secrets not set correctly
2. Federated credential missing or incorrect
3. Service principal doesn't have subscription access
4. Wrong environment being used

**Fix**: Run the verification script above to identify the issue

### Environment Protection Rules
If you enable protection rules on the production environment:
- Deployments will require manual approval
- Only specified branches can deploy
- Adds an audit trail for production deployments

## Security Best Practices

1. **Limit federated credentials** to specific branches (`main` and `release` only)
2. **Use environment protection rules** for production
3. **Regular audit** of role assignments
4. **Principle of least privilege** - only grant Contributor role where needed
5. **Monitor deployments** through GitHub Actions logs

## Additional Resources

- [Azure Workload Identity Federation](https://learn.microsoft.com/azure/active-directory/develop/workload-identity-federation)
- [GitHub OIDC with Azure](https://docs.github.com/actions/deployment/security-hardening-your-deployments/configuring-openid-connect-in-azure)
- [GitHub Environments](https://docs.github.com/actions/deployment/targeting-different-environments/using-environments-for-deployment)
