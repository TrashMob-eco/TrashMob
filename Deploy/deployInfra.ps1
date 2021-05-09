param ($environment, $region)

$rgName = "rg-trashmob-$environment"

az deployment group create -f ".\azureMaps.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create -f ".\sqlServer.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create -f ".\sqlDatabase.bicep" -g $rgName --parameters environment=$environment region=$region
az deployment group create -f ".\keyVault.bicep" -g $rgName --parameters environment=$environment region=$region
