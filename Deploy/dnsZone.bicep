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

// Domain-validation tokens for the Front Door managed certificates on
// the apex + www custom domains. These are public DNS values (anyone
// can query `_dnsauth[.www].trashmob.eco TXT`) so they are not
// secrets, but they DO rotate whenever a custom-domain binding is
// stripped and re-attached — Bicep re-applies of this file that don't
// pass the current tokens will strip the TXT records and force a full
// re-validation loop (see Deploy/APEX_FIRST_VISIT_INVESTIGATION.md
// change log entry for 2026-07-05 "late evening" for the outage that
// this parameterization is designed to prevent).
//
// Fetch the current expected values with:
//   az afd custom-domain show \
//     --resource-group rg-trashmob-pr-westus2 \
//     --profile-name fd-tm-pr \
//     --custom-domain-name trashmob-eco \
//     --query validationProperties.validationToken
//   az afd custom-domain show \
//     --resource-group rg-trashmob-pr-westus2 \
//     --profile-name fd-tm-pr \
//     --custom-domain-name www-trashmob-eco \
//     --query validationProperties.validationToken
//
// Pass them on the command line, e.g.:
//   az deployment group create --template-file .\dnsZone.bicep -g rg-trashmob-pr-westus2 \
//     --parameters zoneName=trashmob.eco environment=pr \
//                  apexDnsAuthToken=_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx \
//                  wwwDnsAuthToken=_yyyyyyyyyyyyyyyyyyyyyyyyyyyyyyyy
//
// If left empty, the TXT records are NOT declared in this deploy —
// ARM Incremental will leave any existing records in Azure DNS
// untouched, which is the safe default for re-applies that only mean
// to update non-DNS-auth records.
@description('Front Door managed-cert validation token for the apex custom-domain. Leave empty on re-apply to preserve the existing DNS record.')
param apexDnsAuthToken string = ''

@description('Front Door managed-cert validation token for the www custom-domain. Leave empty on re-apply to preserve the existing DNS record.')
param wwwDnsAuthToken string = ''

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

// WWW CNAME record - points to Front Door or Container App.
// Guarded so a re-apply that doesn't pass frontDoorEndpointHostname /
// containerAppFqdn (both default to '') doesn't corrupt the record by
// setting cname to an empty string. Same trap as the _dnsauth
// parameterization: silent property reset by ARM Incremental when the
// template computes an empty value.
resource wwwRecord 'Microsoft.Network/dnsZones/CNAME@2023-07-01-preview' = if ((useFrontDoor && frontDoorEndpointHostname != '') || (!useFrontDoor && containerAppFqdn != '')) {
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

// Domain validation TXT records for Front Door managed certificates.
// Guarded by the token params — if not supplied, the resource is not
// declared in this template and ARM Incremental will not touch any
// pre-existing DNS record. See the param declarations above for why.
resource dnsAuthWww 'Microsoft.Network/dnsZones/TXT@2023-07-01-preview' = if (wwwDnsAuthToken != '') {
  parent: dnsZone
  name: '_dnsauth.www'
  properties: {
    TTL: 3600
    TXTRecords: [
      {
        value: [wwwDnsAuthToken]
      }
    ]
  }
}

resource dnsAuthApex 'Microsoft.Network/dnsZones/TXT@2023-07-01-preview' = if (useFrontDoor && apexDnsAuthToken != '') {
  parent: dnsZone
  name: '_dnsauth'
  properties: {
    TTL: 3600
    TXTRecords: [
      {
        value: [apexDnsAuthToken]
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
1. Deploy this template to create the Azure DNS zone (omit apexDnsAuthToken / wwwDnsAuthToken on first deploy — Front Door has not issued tokens yet).
2. Note the nameServers output - these are Azure's DNS servers
3. Update domain registrar (likely where trashmob.eco was purchased) to use Azure nameservers:
   ${dnsZone.properties.nameServers}
4. Wait for DNS propagation (can take 24-48 hours)
5. Deploy Front Door (frontDoor.bicep) — this creates the custom-domain resources which issue DNS-validation tokens.
6. Fetch the tokens via `az afd custom-domain show ... --query validationProperties.validationToken` and re-run this template with apexDnsAuthToken / wwwDnsAuthToken set to those values.
7. Verify email still works (MX, SPF, DKIM records included)
'''
