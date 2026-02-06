param environment string = ''
param region string = ''
param containerAppPrincipalId string = ''

// Allowed origins for CORS - restricts which domains can make requests to Azure Maps Data API
var allowedOrigins = environment == 'pr' ? [
  'https://www.trashmob.eco'
  'https://trashmob.eco'
] : [
  'https://dev.trashmob.eco'
  'http://localhost:3000'
]

var account_map_name = 'map-tm-${environment}-${region}'

// Azure Maps Data Reader role - allows reading map data
// https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles#azure-maps-data-reader
var azureMapsDataReaderRoleId = '423170ca-a8f6-4b0f-8487-9e4eb8f49bfa'

resource accounts_map_trashmobdev_name_resource 'Microsoft.Maps/accounts@2023-06-01' = {
  name: account_map_name
  location: 'global'
  sku: {
    name: 'G2'
  }
  kind: 'Gen2'
  properties: {
    // Keep local auth enabled during migration, disable after fully migrated to managed identity
    disableLocalAuth: false
    cors: {
      corsRules: [
        {
          allowedOrigins: allowedOrigins
        }
      ]
    }
  }
}

// Grant the Container App's managed identity the Azure Maps Data Reader role
// This allows the app to access Azure Maps using DefaultAzureCredential
resource azureMapsRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = if (containerAppPrincipalId != '') {
  name: guid(accounts_map_trashmobdev_name_resource.id, containerAppPrincipalId, azureMapsDataReaderRoleId)
  scope: accounts_map_trashmobdev_name_resource
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', azureMapsDataReaderRoleId)
    principalId: containerAppPrincipalId
    principalType: 'ServicePrincipal'
  }
}

// Output the Azure Maps client ID for use with managed identity authentication
output azureMapsClientId string = accounts_map_trashmobdev_name_resource.properties.uniqueId
