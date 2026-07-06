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

// Rule Set for apex + www HTTPS canonicalisation. See
// Deploy/APEX_FIRST_VISIT_INVESTIGATION.md for background — the route
// used to have httpsRedirect: Enabled, which auto-307'd HTTP-apex
// traffic to HTTPS-apex before the rule set could see it, causing a
// two-hop redirect chain (http://apex → 307 → https://apex → 308 →
// https://www) with two consecutive TLS handshakes on cold browser
// sessions. Now the route delegates HTTPS enforcement to this rule
// set so http://apex resolves to https://www in a single 308.
resource ruleSet 'Microsoft.Cdn/profiles/ruleSets@2024-02-01' = if (apexDomain != '') {
  parent: frontDoorProfile
  name: ruleSetName
}

// Rule to redirect apex traffic (either HTTP or HTTPS) straight to
// https://www.trashmob.eco/ in a single hop. Matches both schemes on
// purpose so cold browsers never pay the http-apex → https-apex
// interstitial handshake.
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

// Rule to redirect http://www.trashmob.eco/ traffic to
// https://www.trashmob.eco/ (same host, upgrade scheme). Previously
// route.httpsRedirect handled this, but we disabled it above so the
// apex rule could catch HTTP-apex directly. This rule preserves the
// prior HTTP→HTTPS behavior for www without adding a second hop.
resource redirectWwwHttpToHttpsRule 'Microsoft.Cdn/profiles/ruleSets/rules@2024-02-01' = if (apexDomain != '') {
  parent: ruleSet
  name: 'RedirectWwwHttpToHttps'
  properties: {
    order: 2
    conditions: [
      {
        name: 'HostName'
        parameters: {
          typeName: 'DeliveryRuleHostNameConditionParameters'
          operator: 'Equal'
          matchValues: [primaryDomain]
          transforms: ['Lowercase']
          negateCondition: false
        }
      }
      {
        name: 'RequestScheme'
        parameters: {
          typeName: 'DeliveryRuleRequestSchemeConditionParameters'
          operator: 'Equal'
          matchValues: ['HTTP']
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
        }
      }
    ]
    matchProcessingBehavior: 'Stop'
  }
  dependsOn: [
    redirectRule
  ]
}

// Route for both custom domains (www + apex).
//
// customDomains MUST be listed here — Bicep re-deploys are Incremental
// but ARM still resets Route.customDomains to whatever the template
// declares. The initial cutover set the associations manually via the
// Azure portal, which left the Bicep silent on this property; the
// 2026-07-05 Fix A re-deploy consequently stripped the associations
// and produced a production outage on trashmob.eco (Front Door
// returning 404 because no route was bound to that host). Never leave
// this array implicit again.
resource route 'Microsoft.Cdn/profiles/afdEndpoints/routes@2024-02-01' = {
  parent: endpoint
  name: routeName
  properties: {
    customDomains: concat(
      primaryDomain != '' ? [{ id: customDomainWww.id }] : [],
      apexDomain != '' ? [{ id: customDomainApex.id }] : []
    )
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
    // Disabled on purpose — HTTPS enforcement now lives in the rule
    // set (RedirectApexToWww + RedirectWwwHttpToHttps). Route-level
    // httpsRedirect would fire *before* the rule set, forcing
    // http://apex to bounce through https://apex on the way to
    // https://www and doubling the cold-visit TLS handshakes.
    httpsRedirect: 'Disabled'
    enabledState: 'Enabled'
    cacheConfiguration: {
      // Required by the AFD Standard/Premium schema whenever
      // cacheConfiguration is set. Matches the effective behavior of
      // the historic config that was accepted under an older API
      // version — Front Door treats a URL's cache key without regard
      // to query string, which is correct for our SPA (all client
      // routing lives in the app; the same index.html is served for
      // any path). Origin Cache-Control headers still control whether
      // any given response is actually cached.
      queryStringCachingBehavior: 'IgnoreQueryString'
      compressionSettings: {
        isCompressionEnabled: true
        contentTypesToCompress: [
          'application/json'
          'text/plain'
          'text/html'
          'application/javascript'
          'text/css'
          'application/xml'
        ]
      }
    }
  }
  dependsOn: [
    origin
    redirectRule
    redirectWwwHttpToHttpsRule
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
