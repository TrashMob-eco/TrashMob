param ($environment, $region, $subscriptionId, $sqlAdminPassword)

$rgName = "rg-trashmob-$environment"

az group create --location $region --name $rgName

az deployment group create --template-file ".\azureMaps.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create --template-file ".\sqlServer.bicep" -g $rgName --parameters environment=$environment region=$region subscriptionId=$subscriptionId sqlAdminPassword=$sqlAdminPassword
az deployment group create --template-file ".\sqlDatabase.bicep" -g $rgName --parameters environment=$environment region=$region subscriptionId=$subscriptionId rgName=$rgName
az deployment group create --template-file ".\keyVault.bicep" -g $rgName --parameters environment=$environment region=$region
