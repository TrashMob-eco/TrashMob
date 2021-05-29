param ($environment, $region)

$keyVaultName = "kv-tm-$environment-$region"

$mapKey = az keyvault secret show --vault-name $keyVaultName --name AzureMapsKey --query value 
$sqlConnection = az keyvault secret show --vault-name $keyVaultName --name TMDBServerConnectionString --query value
 
# Get the Secrets for local use
dotnet user-secrets init
dotnet user-secrets set "AzureMapsKey" $mapKey
dotnet user-secrets set "TMDBServerConnectionString" $sqlConnection
dotnet user-secrets set "sendGridApiKey" "x"

# Build the project
dotnet build -c Debug

# Set up the Database
dotnet ef database update