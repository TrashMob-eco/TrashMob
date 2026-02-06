param environment string = ''
param region string = ''

// Allowed origins for CORS - restricts which domains can make requests to Azure Maps Data API
// Note: This only affects the Data API. For Search/Render APIs, consider migrating to Azure AD auth
// See: https://learn.microsoft.com/en-us/azure/azure-maps/how-to-secure-spa-users
var allowedOrigins = environment == 'pr' ? [
  'https://www.trashmob.eco'
  'https://trashmob.eco'
] : [
  'https://dev.trashmob.eco'
  'http://localhost:3000'
]

var account_map_name = 'map-tm-${environment}-${region}'

resource accounts_map_trashmobdev_name_resource 'Microsoft.Maps/accounts@2023-06-01' = {
  name: account_map_name
  location: 'global'
  sku: {
    name: 'G2'
  }
  kind: 'Gen2'
  properties: {
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

// Security note (Issue #182):
// The CORS setting above only restricts Azure Maps Data API access.
// The subscription key used for Search/Geocoding APIs is still exposed to the browser.
//
// For better security, consider:
// 1. Proxying Azure Maps calls through the backend API (medium effort, high security)
// 2. Migrating to Azure AD authentication with managed identity (high effort, highest security)
//
// Current mitigation: CORS restricts Data API to trashmob.eco domains only.
