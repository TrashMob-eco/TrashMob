# The goal of this script is to both allow a new environment to be set up from scratch, and to bring existing environments up to date
# If a feature is no longer used, it will not remove it
# Note that a sendGridApiKey of "x" tells the email handler to not try to send an email
param ([Parameter(Mandatory=$true)] [String]$environment, 
       [Parameter(Mandatory=$true)] [String]$region, 
       [Parameter(Mandatory=$true)] [String]$subscriptionId, 
       [Parameter(Mandatory=$true)] [String]$sqlAdminPassword, 
       [Parameter(Mandatory=$false)] [String]$sendGridApiKey = "x")


$rgName = "rg-trashmob-$environment-$region"
$keyVaultName = "kv-tm-$environment-$region"
$azureMapsName = "map-tm-$environment-$region"
$storageAccountName = "stortm$environment$region"
$appInsightsName = "ai-tm-${environment}-${region}"
$appServiceName = "as-tm-${environment}-${region}"
$appServicePlanName = "asp-tm-${environment}-${region}"
$functionAppName = "fa-tm-${environment}-${region}"

# Make sure we are logged in to Azure and in to the correct subscription id
az login
az account set --subscription $subscriptionId

# Create the Resource Group Name
az group create --location $region --name $rgName

# Set up the Assets needed with bicep files
az deployment group create --template-file ".\azureMaps.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create --template-file ".\storageAccount.bicep" -g $rgName --parameters storageAccountName=$storageAccountName region=$region
az deployment group create --template-file ".\sqlServer.bicep" -g $rgName --parameters environment=$environment region=$region subscriptionId=$subscriptionId sqlAdminPassword=$sqlAdminPassword
az deployment group create --template-file ".\sqlDatabase.bicep" -g $rgName --parameters environment=$environment region=$region subscriptionId=$subscriptionId
az deployment group create --template-file ".\keyVault.bicep" -g $rgName --parameters env=$environment region=$region
az deployment group create --template-file ".\appServicePlan.bicep" -g $rgName --parameters appServicePlanName=$appServicePlanName region=$region
az deployment group create --template-file ".\appService.bicep" -g $rgName --parameters appServicePlanName=$appServicePlanName appServiceName=$appServiceName region=$region subscriptionId=$subscriptionId rgName=$rgName
az deployment group create --template-file ".\logAnalyticsWorkspace.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create --template-file ".\appInsights.bicep" -g $rgName --parameters appInsightsName=$appInsightsName environment=$environment region=$region subscriptionId=$subscriptionId rgName=$rgName

# Need to get some values back to use later
$appInsightsKey = az monitor app-insights component show -g $rgName -a $appInsightsName --query 'instrumentationKey' -o tsv
$storageKey = (az storage account keys list --account-name $storageAccountName -g $rgName --query "[?keyName=='key1']" | ConvertFrom-Json)[0].value

# Create function app
az deployment group create --template-file ".\functionApp.bicep" -g $rgName --parameters region=$region functionAppName=$functionAppName serverFarmId=$appServicePlanName appInsightsInstrumentationKey=$appInsightsKey storageAccountName=$storageAccountName storageAccountKey=$storageKey

# Add secrets to KeyVault
$mapKey = az maps account keys list --name $azureMapsName --resource-group $rgName --query primaryKey
$sqlKey = "Server=tcp:sql-tm-$environment-$region.database.windows.net,1433;Database=db-tm-$environment-$region;User ID=dbadmin;Password=$sqlAdminPassword;Encrypt=true;Connection Timeout=30;"

az keyvault secret set --vault-name $keyVaultName --name AzureMapsKey --value $mapKey
az keyvault secret set --vault-name $keyVaultName --name TMDBServerConnectionString --value $sqlKey
az keyvault secret set --vault-name $keyVaultName --name StorageAccountKey --value $storageKey
az keyvault secret set --vault-name $keyVaultName --name sendGridApiKey --value $sendGridApiKey

# Add current IP Address to SQL Server Firewall
$ipAddress = (Invoke-WebRequest -uri "http://ifconfig.me/ip").Content
az sql server firewall-rule create --name devRule --resource-group $rgName --server sql-tm-$environment-$region --start-ip-address $ipAddress --end-ip-address $ipAddress

# Get the System Managed Identity for the FunctionApp to allow the Azure Function to access KeyVault
$principal = az functionapp identity show --name $functionAppName --subscription $subscriptionId --resource-group $rgName | ConvertFrom-Json
$principalId = $principal.principalId
az keyvault set-policy --name $keyVaultName --object-id $principalId --secret-permissions "get"

# Add the Policy for the App Service Id to the KeyVault
# az keyvault set-policy --name $keyVaultName --object-id $principalId --secret-permissions "get","list"

# Set the secret in the App Settings for the function. Need to update this to use KeyVault directly in the future, but couldn't get the function app to work on first few attempts
az functionapp config appsettings set --name $functionAppName --subscription $subscriptionId --resource-group $rgName --settings "DbConnectionString=$sqlKey SendGridApiKey=$sendGridApiKey InstanceName=$appServiceName"
