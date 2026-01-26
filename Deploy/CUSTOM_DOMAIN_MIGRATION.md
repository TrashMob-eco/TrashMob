# Custom Domain Migration: App Service to Container App

This document outlines the process for migrating the custom domain (www.trashmob.eco) from the Azure App Service to the Azure Container App.

## Prerequisites

- Azure CLI installed and authenticated
- Access to the Azure subscription with Owner or Contributor role
- Access to the DNS provider (for updating DNS records)
- Production Container App deployed and verified working

## Current State

- **App Service**: `as-tm-pr-westus2` hosts www.trashmob.eco
- **Container App**: `ca-tm-pr-westus2` is the migration target
- **Resource Group**: `rg-trashmob-pr-westus2`
- **Container Apps Environment**: `cae-tm-pr-westus2`

## Migration Steps

### Phase 1: Preparation (Day -7)

1. **Lower DNS TTL**
   - Reduce TTL on www.trashmob.eco CNAME record to 300 seconds (5 minutes)
   - Wait for old TTL to expire before proceeding

2. **Verify Container App is Working**
   ```bash
   # Test the container app directly
   curl -I https://ca-tm-pr-westus2.<env-id>.westus2.azurecontainerapps.io
   ```

### Phase 2: Domain Verification (Day -1)

1. **Add Custom Hostname to Container App**
   ```bash
   az containerapp hostname add \
     --name ca-tm-pr-westus2 \
     --resource-group rg-trashmob-pr-westus2 \
     --hostname www.trashmob.eco
   ```

2. **Get Domain Verification Token**
   The previous command will output a TXT record that needs to be added to DNS.

   Example output:
   ```
   Add a TXT record to your DNS:
   Name: asuid.www.trashmob.eco
   Value: <verification-token>
   ```

3. **Add TXT Record at DNS Provider**
   - Record Type: TXT
   - Name: `asuid.www` (or `asuid.www.trashmob.eco` depending on provider)
   - Value: The verification token from step 2
   - TTL: 300

4. **Wait for DNS Propagation**
   ```bash
   # Verify TXT record is visible
   nslookup -type=TXT asuid.www.trashmob.eco
   ```

### Phase 3: Certificate Provisioning (Day 0)

1. **Create Managed Certificate**
   ```bash
   az containerapp env certificate create \
     --name cae-tm-pr-westus2 \
     --resource-group rg-trashmob-pr-westus2 \
     --hostname www.trashmob.eco \
     --certificate-name trashmob-eco-cert \
     --validation-method CNAME
   ```

2. **Bind Certificate to Hostname**
   ```bash
   az containerapp hostname bind \
     --name ca-tm-pr-westus2 \
     --resource-group rg-trashmob-pr-westus2 \
     --hostname www.trashmob.eco \
     --certificate trashmob-eco-cert
   ```

3. **Verify Binding**
   ```bash
   az containerapp hostname list \
     --name ca-tm-pr-westus2 \
     --resource-group rg-trashmob-pr-westus2
   ```

### Phase 4: DNS Cutover (Day 0)

1. **Get Container App FQDN**
   ```bash
   az containerapp show \
     --name ca-tm-pr-westus2 \
     --resource-group rg-trashmob-pr-westus2 \
     --query "properties.configuration.ingress.fqdn" \
     --output tsv
   ```

2. **Update CNAME Record**
   - Record Type: CNAME
   - Name: `www`
   - Value: `ca-tm-pr-westus2.<env-id>.westus2.azurecontainerapps.io`
   - TTL: 300 (can increase after verified)

3. **Monitor DNS Propagation**
   ```bash
   # Check from multiple locations
   nslookup www.trashmob.eco
   dig www.trashmob.eco CNAME
   ```

4. **Verify Site is Working**
   ```bash
   curl -I https://www.trashmob.eco
   ```

### Phase 5: Cleanup (Day +7)

1. **Remove Custom Domain from App Service**
   ```bash
   az webapp config hostname delete \
     --webapp-name as-tm-pr-westus2 \
     --resource-group rg-trashmob-pr-westus2 \
     --hostname www.trashmob.eco
   ```

2. **Delete Old Certificate from App Service** (if applicable)

3. **Increase DNS TTL**
   - Increase TTL back to normal (e.g., 3600 or 86400)

4. **Remove DNS Verification TXT Record**
   - Delete the `asuid.www.trashmob.eco` TXT record

## Bicep Template Updates

To make custom domains declarative in Bicep, add the following to `containerApp.bicep`:

```bicep
// Add parameters
param customDomainName string = ''
param managedCertificateId string = ''

// Update ingress configuration
configuration: {
  ingress: {
    external: true
    targetPort: 8080
    transport: 'auto'
    allowInsecure: false
    customDomains: customDomainName != '' ? [
      {
        name: customDomainName
        certificateId: managedCertificateId
        bindingType: 'SniEnabled'
      }
    ] : []
  }
  // ... rest of configuration
}
```

**Note**: The managed certificate must be created separately before it can be referenced in the Bicep template. This is typically done via Azure CLI or Portal first, then referenced in subsequent deployments.

## Rollback Plan

If issues occur after DNS cutover:

1. **Revert CNAME Record**
   - Point www.trashmob.eco back to `as-tm-pr-westus2.azurewebsites.net`

2. **Verify App Service is Still Running**
   - The App Service should still be functional as a fallback

## Monitoring

After migration, monitor:

- Application Insights for errors
- Container App logs: `az containerapp logs show --name ca-tm-pr-westus2 --resource-group rg-trashmob-pr-westus2`
- SSL certificate expiry (managed certificates auto-renew, but verify)

## Apex Domain (trashmob.eco without www)

If the apex domain needs to point to the Container App:

1. **Option A: Azure DNS**
   - Migrate DNS to Azure DNS
   - Use ALIAS record pointing to Container App

2. **Option B: DNS Provider with ALIAS/ANAME Support**
   - Cloudflare, DNSimple, etc. support ALIAS/ANAME records
   - Point apex to Container App FQDN

3. **Option C: Redirect**
   - Keep apex pointing elsewhere (or use A record to a redirect service)
   - Configure redirect from trashmob.eco to www.trashmob.eco

## References

- [Azure Container Apps Custom Domains](https://learn.microsoft.com/en-us/azure/container-apps/custom-domains-managed-certificates)
- [Azure CLI containerapp hostname commands](https://learn.microsoft.com/en-us/cli/azure/containerapp/hostname)
- [Managed Certificates in Container Apps](https://learn.microsoft.com/en-us/azure/container-apps/custom-domains-managed-certificates?tabs=azure-cli)
