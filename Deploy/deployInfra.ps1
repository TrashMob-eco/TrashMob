param ($environment, $region, $subscriptionId, $sqlAdminPassword)

$rgName = "rg-trashmob-$environment-$region"
$keyVaultName = "kv-tm-$environment-$region"
$azureMapsName = "map-tm-$environment-$region"
$storageAccountName = "stortm$environment$region"
$appInsightsName = "as-tm-${environment}-${region}"
$appServicePlanName = "asp-tm-${environment}-${region}"
$functionAppName = "fa-tm-${environment}-${region}"

az group create --location $region --name $rgName

az deployment group create --template-file ".\azureMaps.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create --template-file ".\storageAccount.bicep" -g $rgName --parameters storageAccountName=$storageAccountName region=$region
az deployment group create --template-file ".\sqlServer.bicep" -g $rgName --parameters environment=$environment region=$region subscriptionId=$subscriptionId sqlAdminPassword=$sqlAdminPassword
az deployment group create --template-file ".\sqlDatabase.bicep" -g $rgName --parameters environment=$environment region=$region subscriptionId=$subscriptionId
az deployment group create --template-file ".\keyVault.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create --template-file ".\appServicePlan.bicep" -g $rgName --parameters appServicePlanName=$appServicePlanName region=$region
az deployment group create --template-file ".\appService.bicep" -g $rgName --parameters appServicePlanName=$appServicePlanName environment=$environment region=$region subscriptionId=$subscriptionId rgName=$rgName
az deployment group create --template-file ".\logAnalyticsWorkspace.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create --template-file ".\appInsights.bicep" -g $rgName --parameters appInsightsName=$appInsightsName environment=$environment region=$region subscriptionId=$subscriptionId rgName=$rgName

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

# We default this to "x" to prevent email from being sent from developer instances accidentally
az keyvault secret set --vault-name $keyVaultName --name sendGridApiKey --value "x"

# Add current IP Address to SQL Server Firewall
$ipAddress = (Invoke-WebRequest -uri "http://ifconfig.me/ip").Content
az sql server firewall-rule create --name devRule --resource-group $rgName --server sql-tm-$environment-$region --start-ip-address $ipAddress --end-ip-address $ipAddress

# Get the System Managed Identity for the FunctionApp to allow the Azure Function to access KeyVault
$principal = az functionapp identity show --name $functionAppName --subscription $subscriptionId --resource-group $rgName | ConvertFrom-Json
$principalId = $principal.principalId
az keyvault set-policy --name $keyVaultName --object-id $principalId --secret-permissions "get"

$secret = az keyvault secret show --vault-name $keyVaultName --name TMDBServerConnectionString | ConvertFrom-Json

# Set the secret in the App Settings for the function. Need to update this to use KeyVault directly in the future, but couldn't get it to work on first few attempts
$secretValue = $secret.value
az functionapp config appsettings set --name $functionAppName --subscription $subscriptionId --resource-group $rgName --settings "DbConnectionString=$secretValue"