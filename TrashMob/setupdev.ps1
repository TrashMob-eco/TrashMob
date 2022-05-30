param ($subscription, $environment, $region)

az account set --subscription $subscription

$keyVaultName = "kv-tm-$environment-$region"

$mapKey = az keyvault secret show --vault-name $keyVaultName --name AzureMapsKey --query value 
$sqlConnection = az keyvault secret show --vault-name $keyVaultName --name TMDBServerConnectionString --query value
 
# Get the Secrets for local use
dotnet user-secrets init
dotnet user-secrets set "AzureMapsKey" $mapKey
dotnet user-secrets set "TMDBServerConnectionString" $sqlConnection
dotnet user-secrets set "sendGridApiKey" "x"
dotnet user-secrets set "DocusignImpersonatedUserId" "x"
dotnet user-secrets set "DocusignClientId" "x"
dotnet user-secrets set "DocusignAuthServer" "account-d.docusign.com"
dotnet user-secrets set "DocusignAccountId" "x"
dotnet user-secrets set "DocusignPrivateKey" "x"
dotnet user-secrets set "DocusignBasePath" "https://demo.docusign.net/restapi"
dotnet user-secrets set "DocusignRedirectHome" "https://localhost:44332/waivers"

# Build the project
dotnet build -c Debug

# Don't need to do this when running against the generic dev environment
if ($environment -ne "dev")
{
    dotnet ef database update
}