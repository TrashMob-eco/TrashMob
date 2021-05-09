param ($subscriptionId, $environment, $region)

$rgName = "rg-trashmob-$environment"

az deployment group create -f ".\azureMaps.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create -f ".\sqlServer.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create -f ".\sqlDatabase.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create -f ".\keyVault.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create -f ".\appServicePlan.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create -f ".\appService.bicep" -g $rgName --parameters environment=$environment region=$region subscription=$subscriptionId rgName=$rgName
az deployment group create -f ".\logAnalyticsWorkspace.bicep" -g $rgName --parameters environment=$environment region=$region 
az deployment group create -f ".\appInsights.bicep" -g $rgName --parameters environment=$environment region=$region subscription=$subscriptionId rgName=$rgName
