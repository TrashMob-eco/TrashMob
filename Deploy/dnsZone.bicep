@description('The DNS zone name (e.g., trashmob.eco)')
param zoneName string

@description('Environment name (dev, pr)')
param environment string

@description('Front Door endpoint hostname (e.g., fde-tm-pr.azurefd.net)')
param frontDoorEndpointHostname string = ''

@description('Container App FQDN for direct access (fallback if no Front Door)')
param containerAppFqdn string = ''

@description('Enable Front Door integration (uses explicit AFD anycast A records for apex)')
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

// E6 (custom auth domain) — dev-tier records for auth-dev.trashmob.eco.
//
// The DEV Front Door lives in the Sandbox subscription (rg-trashmob-dev-westus2)
// but the DNS zone is here in TrashMobProd. That cross-sub setup means we can't
// reference the dev AFD resource from a Bicep symbolic name in this file — the
// only auth-dev-specific value we need to pass in from outside is the AFD
// validation token (for the _dnsauth.auth-dev TXT). The routing record (the A
// record below) uses fixed anycast IPs and doesn't need any dev-side lookup.
//
// EXPLICIT A RECORDS, NOT A CNAME. See the parallel discussion on apexARecord
// below (Azure DNS alias-record inconsistency). The auth-dev case surfaced its
// own variant of the same problem on 2026-07-27 while validating E6:
//
//   1. `auth-dev.trashmob.eco` was originally a CNAME to the dev AFD endpoint
//      hostname `fde-tm-dev-<random>.z01.azurefd.net`.
//   2. After the Custom URL Domain association was made on the TrashMobEcoDev
//      Entra tenant, the AFD endpoint hostname's own upstream CNAME chain
//      (`mr-z01.tm-azurefd.net` → `www.tm.a.prd.aadg.akadns.net`) started
//      redirecting global recursive-resolver lookups to Microsoft's Entra AAD
//      Gateway (aadg) fleet IPs (20.190.x.x, 40.126.x.x) instead of AFD's
//      edge (150.171.x.x).
//   3. The aadg fleet serves a multi-tenant Microsoft platform cert
//      (`graph.windows.net`, `*.b2clogin.com`, etc.) that does NOT include
//      `auth-dev.trashmob.eco` in its SANs, so browser TLS fails with a
//      trust-relationship error even though AFD itself is serving the correct
//      managed cert on its own edge IPs.
//   4. Pinning A records directly to AFD's classic anycast pair sidesteps the
//      entire Microsoft-side CNAME chain and forces resolution to hit AFD.
//      AFD's edge terminates TLS with the right custom-domain cert
//      independent of profile SKU (Classic/Standard/Premium all share the
//      same anycast infrastructure).
//
// This is the same defensive pattern the apex uses. See apexARecord for the
// original apex-outage story that established the anycast-pair values.
//
// Fetch the current AFD validation token (from the Sandbox sub) with:
//   az account set --subscription <Sandbox>
//   az afd custom-domain show --resource-group rg-trashmob-dev-westus2 \
//     --profile-name fd-tm-dev --custom-domain-name auth-dev-trashmob-eco \
//     --query validationProperties.validationToken -o tsv
//
// Then pass it here:
//   az deployment group create --template-file .\dnsZone.bicep -g rg-trashmob-pr-westus2 \
//     --parameters zoneName=trashmob.eco environment=pr \
//                  authDevDnsAuthToken=_xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
//
// Leave empty on a re-apply that only means to update non-auth-dev records —
// ARM Incremental will preserve the existing token TXT, same as the apex/www
// _dnsauth pattern above.
@description('E6: Dev Front Door managed-cert validation token for the auth-dev custom-domain. Empty skips the _dnsauth.auth-dev TXT record.')
param authDevDnsAuthToken string = ''

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

// Apex A record: EXPLICIT Front Door anycast IPs, not an alias-to-endpoint.
//
// We used to declare this as `targetResource: { id: frontDoorEndpointId }`
// (an Azure DNS alias A record) — the standard "apex CNAME" pattern for
// AFD Standard/Premium. That produced weeks of intermittent user reports
// of a blue "This Container App is stopped or does not exist" 404 on
// first visit to trashmob.eco. See Deploy/APEX_FIRST_VISIT_INVESTIGATION.md
// change log entry for 2026-07-21 for the full autopsy.
//
// Root cause: Azure DNS alias resolution for AFD Standard/Premium
// endpoints was returning inconsistent answers across the four NSes
// (some correct AFD anycast, some 150.171.x.x AFD Std/Prem frontends,
// some — critically — the origin's Container Apps environment static
// IP 20.69.75.244, apparently by "walking" through the AFD origin
// group's origin hostName). Public resolvers (Cloudflare, Google,
// Quad9) all cached the ACA env IP, so a majority of first-visit
// users connected directly to ACA with Host: trashmob.eco, ACA had
// no cert for that hostname, and the connection died in TLS.
//
// Explicit A records pin apex traffic to Microsoft's classic Front
// Door anycast pair, which routes to any AFD tenant via SNI at the
// TLS layer regardless of profile SKU. If Microsoft ever changes
// these anycast IPs (they've been stable since 2016-ish), we update
// them here.
resource apexARecord 'Microsoft.Network/dnsZones/A@2023-07-01-preview' = if (useFrontDoor) {
  parent: dnsZone
  name: '@'
  properties: {
    TTL: 300
    ARecords: [
      { ipv4Address: '13.107.226.70' }
      { ipv4Address: '13.107.253.70' }
    ]
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

// E6 — auth-dev subdomain A records pinned to AFD's classic anycast pair.
// See the long E6 comment block above the params for the full story on why
// this is A records instead of a CNAME to the dev AFD endpoint. Short version:
// the CNAME chain from `fde-tm-<hash>.z01.azurefd.net` transits through
// `mr-z01.tm-azurefd.net` → `www.tm.a.prd.aadg.akadns.net` and lands on
// Microsoft's aadg fleet, which serves a platform SNI cert that doesn't
// include auth-dev.trashmob.eco. Pinning A records to the AFD anycast pair
// bypasses that chain entirely.
//
// Same anycast IPs as apexARecord below. Unconditional (no guard param) —
// values are static, no reason to skip on re-apply.
resource authDevRecord 'Microsoft.Network/dnsZones/A@2023-07-01-preview' = {
  parent: dnsZone
  name: 'auth-dev'
  properties: {
    TTL: 300
    ARecords: [
      { ipv4Address: '13.107.226.70' }
      { ipv4Address: '13.107.253.70' }
    ]
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

// E6 — Front Door managed-cert validation TXT for auth-dev.trashmob.eco.
// Guarded by the token param so a re-apply that doesn't pass a token
// preserves whatever record exists (same pattern as the apex/www TXTs).
resource dnsAuthAuthDev 'Microsoft.Network/dnsZones/TXT@2023-07-01-preview' = if (authDevDnsAuthToken != '') {
  parent: dnsZone
  name: '_dnsauth.auth-dev'
  properties: {
    TTL: 3600
    TXTRecords: [
      {
        value: [authDevDnsAuthToken]
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

// Apex TXT record set for trashmob.eco. Holds:
//   1. SPF for Outlook / M365 email authentication
//   2. `_mn729nlts6vg16hckcss91bxyub7qvz` — a domain-verification-shaped
//      token added out-of-band more than 3 months ago. Provenance is
//      unknown: the format matches an AFD custom-domain validation
//      token, but neither of our current AFD custom domains (apex,
//      www) matches this value — their live tokens are correctly at
//      `_dnsauth` and `_dnsauth.www`. Not referenced anywhere in this
//      repo. Preserved defensively because we don't know which service
//      (if any) relies on it, and it's been in prod for months without
//      issue.
//
// A prior version of this file declared only the SPF value, which
// would have caused ARM Incremental to strip the mystery token on any
// re-apply (discovered via `az deployment group what-if` on 2026-07-26
// while preparing to publish the E6 auth-dev records). If the mystery
// token's owner is ever identified and confirmed obsolete, delete the
// second array entry below and this comment block.
resource spfRecord 'Microsoft.Network/dnsZones/TXT@2023-07-01-preview' = {
  parent: dnsZone
  name: '@'
  properties: {
    TTL: 3600
    TXTRecords: [
      {
        value: ['v=spf1 include:spf.protection.outlook.com -all']
      }
      {
        value: ['_mn729nlts6vg16hckcss91bxyub7qvz']
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

E6 auth-dev subdomain:
8. Deploy dev Front Door via .github/workflows/container_frontdoor-tm-dev-westus2.yml.
9. From the Sandbox subscription, fetch the AFD custom-domain validation token:
   az afd custom-domain show --resource-group rg-trashmob-dev-westus2 \
     --profile-name fd-tm-dev --custom-domain-name auth-dev-trashmob-eco \
     --query validationProperties.validationToken -o tsv
10. Re-run this template with authDevDnsAuthToken set to that value. The auth-dev A records
    to AFD anycast (13.107.226.70, 13.107.253.70) deploy unconditionally — no lookup needed.
11. Wait a few minutes for the token TXT to propagate; the dev Front Door custom domain will flip from Pending -> Approved.
12. In the Entra admin center on the TrashMobEcoDev tenant, add auth-dev.trashmob.eco under
    Identity > Domain names > Custom domain names (requires TXT MS=<token> verification), then
    associate it under Entra ID > Domain names > Custom URL domains.
'''
