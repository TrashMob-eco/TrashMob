@description('The DNS zone name (e.g., trashmob.eco)')
param zoneName string

@description('Environment name (dev, pr)')
param environment string

@description('Front Door endpoint hostname (e.g., fde-tm-pr.azurefd.net)')
param frontDoorEndpointHostname string = ''

@description('Container App FQDN for direct access (fallback if no Front Door)')
param containerAppFqdn string = ''

@description('Front Door endpoint resource ID for alias record (e.g., /subscriptions/.../providers/Microsoft.Cdn/profiles/fd-tm-pr/afdEndpoints/fde-tm-pr)')
param frontDoorEndpointId string = ''

@description('Enable Front Door integration (uses alias records for apex)')
param useFrontDoor bool = true

// DNS Zone
resource dnsZone 'Microsoft.Network/dnsZones@2023-07-01-preview' = {
  name: zoneName
  location: 'global'
  properties: {
    zoneType: 'Public'
  }
  tags: {
    environment: environment
  }
}

// WWW CNAME record - points to Front Door or Container App
resource wwwRecord 'Microsoft.Network/dnsZones/CNAME@2023-07-01-preview' = {
  parent: dnsZone
  name: 'www'
  properties: {
    TTL: 3600
    CNAMERecord: {
      cname: useFrontDoor ? frontDoorEndpointHostname : containerAppFqdn
    }
  }
}

// Apex A record with alias to Front Door (only if using Front Door)
// This is the key record that enables apex domain support
resource apexAliasRecord 'Microsoft.Network/dnsZones/A@2023-07-01-preview' = if (useFrontDoor && frontDoorEndpointId != '') {
  parent: dnsZone
  name: '@'
  properties: {
    TTL: 3600
    targetResource: {
      id: frontDoorEndpointId
    }
  }
}

// Dev subdomain CNAME (for dev.trashmob.eco)
resource devRecord 'Microsoft.Network/dnsZones/CNAME@2023-07-01-preview' = if (environment == 'pr') {
  parent: dnsZone
  name: 'dev'
  properties: {
    TTL: 3600
    CNAMERecord: {
      cname: 'ca-tm-dev-westus2.ashypebble-059d2628.westus2.azurecontainerapps.io'
    }
  }
}

// Domain validation TXT records for Front Door managed certificates
resource dnsAuthWww 'Microsoft.Network/dnsZones/TXT@2023-07-01-preview' = {
  parent: dnsZone
  name: '_dnsauth.www'
  properties: {
    TTL: 3600
    TXTRecords: [
      {
        value: ['<ADD_VALIDATION_TOKEN_FROM_AZURE_PORTAL>']
      }
    ]
  }
}

resource dnsAuthApex 'Microsoft.Network/dnsZones/TXT@2023-07-01-preview' = if (useFrontDoor) {
  parent: dnsZone
  name: '_dnsauth'
  properties: {
    TTL: 3600
    TXTRecords: [
      {
        value: ['<ADD_VALIDATION_TOKEN_FROM_AZURE_PORTAL>']
      }
    ]
  }
}

// MX records for email (Microsoft 365)
// These values should be updated with actual M365 values
resource mxRecord 'Microsoft.Network/dnsZones/MX@2023-07-01-preview' = {
  parent: dnsZone
  name: '@'
  properties: {
    TTL: 3600
    MXRecords: [
      {
        preference: 0
        exchange: 'trashmob-eco.mail.protection.outlook.com'
      }
    ]
  }
}

// SPF record for email authentication
resource spfRecord 'Microsoft.Network/dnsZones/TXT@2023-07-01-preview' = {
  parent: dnsZone
  name: '@'
  properties: {
    TTL: 3600
    TXTRecords: [
      {
        value: ['v=spf1 include:spf.protection.outlook.com -all']
      }
    ]
  }
}

// Autodiscover CNAME for Outlook
resource autodiscoverRecord 'Microsoft.Network/dnsZones/CNAME@2023-07-01-preview' = {
  parent: dnsZone
  name: 'autodiscover'
  properties: {
    TTL: 3600
    CNAMERecord: {
      cname: 'autodiscover.outlook.com'
    }
  }
}

// DKIM selectors for Microsoft 365 (if configured)
resource dkim1Record 'Microsoft.Network/dnsZones/CNAME@2023-07-01-preview' = {
  parent: dnsZone
  name: 'selector1._domainkey'
  properties: {
    TTL: 3600
    CNAMERecord: {
      cname: 'selector1-trashmob-eco._domainkey.trashmob.onmicrosoft.com'
    }
  }
}

resource dkim2Record 'Microsoft.Network/dnsZones/CNAME@2023-07-01-preview' = {
  parent: dnsZone
  name: 'selector2._domainkey'
  properties: {
    TTL: 3600
    CNAMERecord: {
      cname: 'selector2-trashmob-eco._domainkey.trashmob.onmicrosoft.com'
    }
  }
}

// Outputs
output nameServers array = dnsZone.properties.nameServers
output zoneId string = dnsZone.id

output migrationInstructions string = '''
DNS Migration Steps:
1. Deploy this template to create the Azure DNS zone
2. Note the nameServers output - these are Azure's DNS servers
3. Update domain registrar (likely where trashmob.eco was purchased) to use Azure nameservers:
   ${dnsZone.properties.nameServers}
4. Wait for DNS propagation (can take 24-48 hours)
5. Update TXT validation records with actual tokens from Azure Portal
6. Verify email still works (MX, SPF, DKIM records included)
'''
