@description('Azure region for the Front Door profile')
param region string = 'global'

@description('Environment name (dev, pr)')
param environment string

@description('The FQDN of the Container App backend')
param containerAppFqdn string

@description('Primary custom domain (e.g., www.trashmob.eco)')
param primaryDomain string

@description('Apex domain for redirect (e.g., trashmob.eco)')
param apexDomain string = ''

@description('Front Door SKU')
@allowed(['Standard_AzureFrontDoor', 'Premium_AzureFrontDoor'])
param skuName string = 'Standard_AzureFrontDoor'

// Naming convention
var frontDoorProfileName = 'fd-tm-${environment}'
var endpointName = 'fde-tm-${environment}'
var originGroupName = 'og-containerapp'
var originName = 'origin-containerapp'
var routeName = 'route-default'
var ruleSetName = 'ApexRedirect'

// Front Door Profile
resource frontDoorProfile 'Microsoft.Cdn/profiles@2024-02-01' = {
  name: frontDoorProfileName
  location: region
  sku: {
    name: skuName
  }
  properties: {
    originResponseTimeoutSeconds: 60
  }
  tags: {
    environment: environment
  }
}

// Endpoint
resource endpoint 'Microsoft.Cdn/profiles/afdEndpoints@2024-02-01' = {
  parent: frontDoorProfile
  name: endpointName
  location: region
  properties: {
    enabledState: 'Enabled'
  }
}

// Origin Group
resource originGroup 'Microsoft.Cdn/profiles/originGroups@2024-02-01' = {
  parent: frontDoorProfile
  name: originGroupName
  properties: {
    loadBalancingSettings: {
      sampleSize: 4
      successfulSamplesRequired: 3
      additionalLatencyInMilliseconds: 50
    }
    healthProbeSettings: {
      probePath: '/health/live'
      probeRequestType: 'HEAD'
      probeProtocol: 'Https'
      probeIntervalInSeconds: 100
    }
    sessionAffinityState: 'Disabled'
  }
}

// Origin (Container App)
resource origin 'Microsoft.Cdn/profiles/originGroups/origins@2024-02-01' = {
  parent: originGroup
  name: originName
  properties: {
    hostName: containerAppFqdn
    httpPort: 80
    httpsPort: 443
    originHostHeader: containerAppFqdn
    priority: 1
    weight: 1000
    enabledState: 'Enabled'
    enforceCertificateNameCheck: true
  }
}

// Rule Set for apex domain redirect
resource ruleSet 'Microsoft.Cdn/profiles/ruleSets@2024-02-01' = if (apexDomain != '') {
  parent: frontDoorProfile
  name: ruleSetName
}

// Rule to redirect apex to www
resource redirectRule 'Microsoft.Cdn/profiles/ruleSets/rules@2024-02-01' = if (apexDomain != '') {
  parent: ruleSet
  name: 'RedirectApexToWww'
  properties: {
    order: 1
    conditions: [
      {
        name: 'HostName'
        parameters: {
          typeName: 'DeliveryRuleHostNameConditionParameters'
          operator: 'Equal'
          matchValues: [apexDomain]
          transforms: ['Lowercase']
          negateCondition: false
        }
      }
    ]
    actions: [
      {
        name: 'UrlRedirect'
        parameters: {
          typeName: 'DeliveryRuleUrlRedirectActionParameters'
          redirectType: 'PermanentRedirect'
          destinationProtocol: 'Https'
          customHostname: primaryDomain
        }
      }
    ]
    matchProcessingBehavior: 'Stop'
  }
}

// Route for primary domain (www)
resource route 'Microsoft.Cdn/profiles/afdEndpoints/routes@2024-02-01' = {
  parent: endpoint
  name: routeName
  properties: {
    originGroup: {
      id: originGroup.id
    }
    originPath: '/'
    ruleSets: apexDomain != '' ? [
      {
        id: ruleSet.id
      }
    ] : []
    supportedProtocols: ['Http', 'Https']
    patternsToMatch: ['/*']
    forwardingProtocol: 'HttpsOnly'
    linkToDefaultDomain: 'Enabled'
    httpsRedirect: 'Enabled'
    enabledState: 'Enabled'
  }
  dependsOn: [
    origin
    redirectRule
  ]
}

// Custom domain for www (primary)
resource customDomainWww 'Microsoft.Cdn/profiles/customDomains@2024-02-01' = if (primaryDomain != '') {
  parent: frontDoorProfile
  name: replace(primaryDomain, '.', '-')
  properties: {
    hostName: primaryDomain
    tlsSettings: {
      certificateType: 'ManagedCertificate'
      minimumTlsVersion: 'TLS12'
    }
  }
}

// Custom domain for apex (redirect only)
resource customDomainApex 'Microsoft.Cdn/profiles/customDomains@2024-02-01' = if (apexDomain != '') {
  parent: frontDoorProfile
  name: replace(apexDomain, '.', '-')
  properties: {
    hostName: apexDomain
    tlsSettings: {
      certificateType: 'ManagedCertificate'
      minimumTlsVersion: 'TLS12'
    }
  }
}

// Outputs
output frontDoorId string = frontDoorProfile.id
output frontDoorEndpointHostName string = endpoint.properties.hostName
output frontDoorProfileName string = frontDoorProfile.name

// DNS Configuration Instructions (output as metadata)
// For www subdomain: Create CNAME record pointing to the endpoint hostname
// For apex domain: Create ALIAS/ANAME record pointing to the endpoint hostname
//                  Or use Azure DNS with alias record to Front Door endpoint
output dnsInstructions string = '''
DNS Configuration Required:
1. For ${primaryDomain}: Create CNAME record -> ${endpoint.properties.hostName}
2. For ${apexDomain}: Create ALIAS/ANAME record -> ${endpoint.properties.hostName}
   (Microsoft 365 DNS may not support ALIAS records - consider Azure DNS or Cloudflare)
3. For domain validation, create TXT record: _dnsauth.${primaryDomain} with the validation token from Azure Portal
'''
