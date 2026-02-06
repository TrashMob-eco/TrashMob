# The goal of this script is to both allow a new environment to be set up from scratch, and to bring existing environments up to date
# If a feature is no longer used, it will not remove it
# Note that a sendGridApiKey of "x" tells the email handler to not try to send an email
param ([Parameter(Mandatory=$true)] [String]$environment, 
       [Parameter(Mandatory=$true)] [String]$region, 
       [Parameter(Mandatory=$true)] [String]$subscriptionId, 
       [Parameter(Mandatory=$true)] [String]$sqlAdminPassword, 
       [Parameter(Mandatory=$false)] [bool]$alwaysOn, 
       [Parameter(Mandatory=$false)] [String]$sendGridApiKey = "x")


$rgName = "rg-trashmob-$environment-$region"
$keyVaultName = "kv-tm-$environment-$region"
$azureMapsName = "map-tm-$environment-$region"
$storageAccountName = "stortm$environment$region"
$appInsightsName = "ai-tm-${environment}-${region}"
$appServiceName = "as-tm-${environment}-${region}"
$appServicePlanName = "asp-tm-${environment}-${region}"
$containerRegistryName = "acrtm${environment}${region}"
$containerAppsEnvironmentName = "cae-tm-${environment}-${region}"
$containerAppName = "ca-tm-${environment}-${region}"
$containerAppJobDailyName = "caj-tm-${environment}-${region}-daily"
$containerAppJobHourlyName = "caj-tm-${environment}-${region}-hourly"
$logAnalyticsWorkspaceName = "log-tm-${environment}-${region}"

# Make sure we are logged in to Azure and in to the correct subscription id
az login
az account set --subscription $subscriptionId

# Create the Resource Group Name
az group create --location $region --name $rgName

# Set up the Assets needed with bicep files
az deployment group create --template-file ".\azureMaps.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create --template-file ".\storageAccount.bicep" -g $rgName --parameters storageAccountName=$storageAccountName region=$region
az deployment group create --template-file ".\sqlServer.bicep" -g $rgName --parameters environment=$environment region=$region sqlAdminPassword=$sqlAdminPassword
az deployment group create --template-file ".\sqlDatabase.bicep" -g $rgName --parameters environment=$environment region=$region

# Get existing Key Vault access policies to preserve them during deployment
Write-Host "Checking for existing Key Vault access policies..." -ForegroundColor Yellow
$existingPolicies = az keyvault show --name $keyVaultName --resource-group $rgName --query "properties.accessPolicies" 2>$null
if ($existingPolicies) {
    Write-Host "Found existing access policies. Preserving them during deployment." -ForegroundColor Green
    # Save to temporary file to pass as parameter
    $existingPolicies | Out-File -FilePath ".\temp_policies.json" -Encoding UTF8
    az deployment group create --template-file ".\keyVault.bicep" -g $rgName --parameters environment=$environment region=$region accessPolicies="@temp_policies.json"
    Remove-Item ".\temp_policies.json" -ErrorAction SilentlyContinue
} else {
    Write-Host "No existing Key Vault found or no access policies. Creating new Key Vault." -ForegroundColor Yellow
    az deployment group create --template-file ".\keyVault.bicep" -g $rgName --parameters environment=$environment region=$region
}

az deployment group create --template-file ".\appServicePlan.bicep" -g $rgName --parameters appServicePlanName=$appServicePlanName region=$region
az deployment group create --template-file ".\appService.bicep" -g $rgName --parameters appServicePlanName=$appServicePlanName appServiceName=$appServiceName region=$region subscriptionId=$subscriptionId rgName=$rgName alwaysOn=$alwaysOn
az deployment group create --template-file ".\logAnalyticsWorkspace.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create --template-file ".\appInsights.bicep" -g $rgName --parameters appInsightsName=$appInsightsName environment=$environment region=$region subscriptionId=$subscriptionId rgName=$rgName

# Need to get some values back to use later
$storageKey = (az storage account keys list --account-name $storageAccountName -g $rgName --query "[?keyName=='key1']" | ConvertFrom-Json)[0].value

# Add secrets to KeyVault
$mapKey = az maps account keys list --name $azureMapsName --resource-group $rgName --query primaryKey
$sqlKey = "Server=tcp:sql-tm-$environment-$region.database.windows.net,1433;Database=db-tm-$environment-$region;User ID=dbadmin;Password=$sqlAdminPassword;Encrypt=true;Connection Timeout=30;"

az keyvault secret set --vault-name $keyVaultName --name AzureMapsKey --value $mapKey
az keyvault secret set --vault-name $keyVaultName --name TMDBServerConnectionString --value $sqlKey
az keyvault secret set --vault-name $keyVaultName --name StorageAccountKey --value $storageKey
az keyvault secret set --vault-name $keyVaultName --name sendGridApiKey --value $sendGridApiKey

# Note: Azure AD B2C configuration is now set via environment variables in containerApp.bicep
# The B2C settings are public configuration values (not secrets) and are determined by the 'environment' parameter
# This includes both backend (JWT validation) and frontend (MSAL) client IDs

# Set the webapp to the correct keyvaulturi
az webapp config appsettings set --name $appServiceName --resource-group $rgName --settings "VaultUri=https://$keyVaultName.vault.azure.net"

# Add current IP Address to SQL Server Firewall
$ipAddress = (Invoke-WebRequest -uri "http://ifconfig.me/ip").Content
az sql server firewall-rule create --name devRule --resource-group $rgName --server sql-tm-$environment-$region --start-ip-address $ipAddress --end-ip-address $ipAddress

# Add the Policy for the App Service Id to the KeyVault
$principal2 = az webapp identity show --name $appServiceName --resource-group $rgName | ConvertFrom-Json
$principal2Id = $principal2.principalId
az keyvault set-policy --name $keyVaultName --object-id $principal2Id --secret-permissions get list

# Deploy Container Infrastructure
Write-Host "Deploying Container Registry..." -ForegroundColor Green
az deployment group create --template-file ".\containerRegistry.bicep" -g $rgName --parameters containerRegistryName=$containerRegistryName region=$region environment=$environment

Write-Host "Deploying Container Apps Environment..." -ForegroundColor Green
az deployment group create --template-file ".\containerAppsEnvironment.bicep" -g $rgName --parameters containerAppsEnvironmentName=$containerAppsEnvironmentName region=$region logAnalyticsWorkspaceName=$logAnalyticsWorkspaceName environment=$environment

# Deploy Billing Alerts and Budget Caps
Write-Host "Deploying Billing Alerts..." -ForegroundColor Green
az deployment group create --template-file ".\billingAlerts.bicep" -g $rgName --parameters environment=$environment region=$region

# Note: Container App and Container App Job deployments will be done via GitHub Actions with actual container images
# Uncomment the following lines after container images are built and pushed to the registry
# Write-Host "Deploying Container App for Web Application..." -ForegroundColor Green
# $webContainerImage = "$containerRegistryName.azurecr.io/trashmob:latest"
# az deployment group create --template-file ".\containerApp.bicep" -g $rgName --parameters containerAppName=$containerAppName containerAppsEnvironmentId="/subscriptions/$subscriptionId/resourceGroups/$rgName/providers/Microsoft.App/managedEnvironments/$containerAppsEnvironmentName" containerRegistryName=$containerRegistryName containerImage=$webContainerImage keyVaultName=$keyVaultName region=$region environment=$environment

# Write-Host "Deploying Daily Container App Job..." -ForegroundColor Green
# $dailyJobContainerImage = "$containerRegistryName.azurecr.io/trashmobdailyjobs:latest"
# az deployment group create --template-file ".\containerAppJobDaily.bicep" -g $rgName --parameters containerAppJobName=$containerAppJobDailyName containerAppsEnvironmentId="/subscriptions/$subscriptionId/resourceGroups/$rgName/providers/Microsoft.App/managedEnvironments/$containerAppsEnvironmentName" containerRegistryName=$containerRegistryName containerImage=$dailyJobContainerImage keyVaultName=$keyVaultName azureMapsName=$azureMapsName storageAccountName=$storageAccountName region=$region environment=$environment

# Write-Host "Deploying Hourly Container App Job..." -ForegroundColor Green
# $hourlyJobContainerImage = "$containerRegistryName.azurecr.io/trashmobhourlyjobs:latest"
# az deployment group create --template-file ".\containerAppJobHourly.bicep" -g $rgName --parameters containerAppJobName=$containerAppJobHourlyName containerAppsEnvironmentId="/subscriptions/$subscriptionId/resourceGroups/$rgName/providers/Microsoft.App/managedEnvironments/$containerAppsEnvironmentName" containerRegistryName=$containerRegistryName containerImage=$hourlyJobContainerImage keyVaultName=$keyVaultName azureMapsName=$azureMapsName storageAccountName=$storageAccountName region=$region environment=$environment
