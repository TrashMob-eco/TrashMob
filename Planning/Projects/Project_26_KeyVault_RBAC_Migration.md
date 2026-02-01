# Project 26 � KeyVault RBAC Migration

| Attribute | Value |
|-----------|-------|
| **Status** | Planning |
| **Priority** | High |
| **Risk** | Moderate |
| **Size** | Small |
| **Dependencies** | None |

---

## Business Rationale

Azure KeyVault access policies are being deprecated in favor of Azure Role-Based Access Control (RBAC). Migrating to RBAC provides better security governance, more granular permissions, and aligns with Azure's recommended security practices. This change ensures TrashMob remains compliant with Azure's evolving security model and reduces technical debt before access policies are fully deprecated.

---

## Objectives

### Primary Goals
- **Convert KeyVault access model** from access policies to Azure RBAC
- **Assign appropriate RBAC roles** to applications and service principals
- **Validate access** for all services (web app, function apps, pipelines)
- **Update infrastructure documentation** to reflect RBAC model

### Secondary Goals (Nice-to-Have)
- Implement principle of least privilege with granular role assignments
- Set up monitoring for KeyVault access patterns
- Review and clean up unused secrets during migration

---

## Scope

### Phase 1 - Assessment and Planning
- ✅ Audit current KeyVault access policies and their permissions
- ✅ Identify all applications and service principals with KeyVault access
- ✅ Map access policies to equivalent RBAC roles
- ✅ Document current secrets and their usage across applications
- ✅ Create rollback plan

### Phase 2 - RBAC Configuration
- ✅ Enable RBAC permission model on KeyVault
- ✅ Assign "Key Vault Secrets User" role to web application managed identity
- ✅ Assign "Key Vault Secrets User" role to function apps managed identities
- ✅ Assign "Key Vault Secrets Officer" role to deployment pipelines/service principals
- ✅ Assign "Key Vault Administrator" role to infrastructure administrators
- ✅ Remove legacy access policies after RBAC validation

### Phase 3 - Testing and Validation
- ✅ Test secret retrieval from web application
- ✅ Test secret retrieval from hourly jobs function app
- ✅ Test secret retrieval from daily jobs function app
- ✅ Validate Azure B2C secrets access
- ✅ Test deployment pipeline secret management
- ✅ Monitor KeyVault diagnostic logs for access issues

### Phase 4 - Documentation and Cleanup
- ✅ Update infrastructure documentation with RBAC model
- ✅ Update deployment scripts and IaC templates (Bicep)
- ✅ Document RBAC role assignments and their purposes
- ✅ Remove deprecated access policy configurations
- ✅ Update developer onboarding documentation

---

## Out-of-Scope

- ❌ Migrating to Azure Key Vault Managed HSM (standard KeyVault is sufficient)
- ❌ Implementing Key Vault Private Link/Private Endpoint (separate project)
- ❌ Certificate rotation automation (separate security project)
- ❌ Multi-region KeyVault replication

---

## Success Metrics

### Quantitative
- **Access policy migration:** 100% of access policies converted to RBAC roles
- **Service availability:** Zero downtime during migration
- **RBAC role assignments:** All applications and pipelines functioning with RBAC
- **Secret access errors:** Zero unauthorized access attempts after migration

### Qualitative
- All services continue to access secrets without interruption
- Infrastructure team comfortable managing RBAC assignments
- Clearer audit trail for KeyVault access
- Reduced security risk from overly permissive access policies

---

## Dependencies

### Blockers (Must be complete before this project starts)
None - This is an infrastructure modernization that can proceed independently

### Enablers for Other Projects (What this unlocks)
- **Project 05 - Deployment Pipelines:** Ensures pipelines use modern RBAC for secret access
- **Project 01 - Auth Revamp:** Supports secure access to Azure B2C secrets via RBAC

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Service disruption during migration** | Medium | High | Implement dual-mode access (policies + RBAC) during transition, test thoroughly in dev/staging first |
| **Missing permissions after migration** | Medium | High | Create detailed permission mapping before migration, maintain access policies as backup during validation period |
| **Pipeline deployment failures** | Low | Critical | Test pipeline secret access in non-production environment first, have rollback script ready |
| **Managed identity misconfiguration** | Low | High | Document all managed identities and their assignments, verify each identity before removing access policies |

---

## Implementation Plan

### Infrastructure Changes
**Azure KeyVault Configuration:**
- Enable RBAC authorization on KeyVault
- Maintain access policies temporarily during migration
- Remove access policies after RBAC validation complete

**Azure RBAC Role Assignments:**

```bash
# Example Azure CLI commands for RBAC assignment

# Assign secrets user role to web app managed identity
az role assignment create \
  --role "Key Vault Secrets User" \
  --assignee <web-app-managed-identity-id> \
  --scope /subscriptions/<sub-id>/resourceGroups/<rg>/providers/Microsoft.KeyVault/vaults/<kv-name>

# Assign secrets user role to function apps
az role assignment create \
  --role "Key Vault Secrets User" \
  --assignee <function-app-managed-identity-id> \
  --scope /subscriptions/<sub-id>/resourceGroups/<rg>/providers/Microsoft.KeyVault/vaults/<kv-name>

# Assign secrets officer role to deployment service principal
az role assignment create \
  --role "Key Vault Secrets Officer" \
  --assignee <deployment-sp-id> \
  --scope /subscriptions/<sub-id>/resourceGroups/<rg>/providers/Microsoft.KeyVault/vaults/<kv-name>
```

### Bicep/IaC Changes
Update Bicep templates to include RBAC role assignments:

```bicep
// Example Bicep for RBAC role assignment
resource keyVaultSecretsUser 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(keyVault.id, webApp.id, 'Key Vault Secrets User')
  scope: keyVault
  properties: {
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '4633458b-17de-408a-b874-0445c86b69e6') // Key Vault Secrets User
    principalId: webApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}
```

### Application Code Changes
No application code changes required - applications already use Azure SDK/DefaultAzureCredential which supports both access policies and RBAC.

### Validation Steps
1. Enable RBAC on KeyVault (keep access policies enabled)
2. Assign RBAC roles to all service identities
3. Test each application/service individually
4. Monitor KeyVault diagnostic logs for 24-48 hours
5. Disable access policies
6. Monitor for 1 week
7. Remove access policy configuration permanently

---

## Implementation Phases

### Phase 1: Discovery and Preparation
- Audit all current KeyVault access policies
- Document all managed identities and service principals
- Identify all secrets and their consuming applications
- Prepare RBAC role mapping document
- Create rollback plan

### Phase 2: Dev/Test Environment Migration
- Enable RBAC on dev/test KeyVault
- Assign RBAC roles in dev/test
- Test all applications in dev/test environment
- Validate B2C secret access
- Document any issues and resolutions

### Phase 3: Production Migration
- Enable RBAC on production KeyVault (dual-mode)
- Assign RBAC roles to production identities
- Validate all services are functioning
- Monitor diagnostic logs
- Disable access policies after validation
- Final monitoring period

### Phase 4: Cleanup and Documentation
- Remove access policy configurations
- Update IaC templates and scripts
- Update documentation
- Share knowledge with team

**Note:** Phases are sequential. Each phase must be validated before proceeding to the next.

---

## Decisions

1. **Should we implement Key Vault Private Endpoint during this migration?**
   **Decision:** No - keep as separate project to reduce migration complexity and risk

2. **What RBAC roles should developers have for troubleshooting?**
   **Decision:** Developers get "Key Vault Secrets User" in dev/test only; read-only audit access in production

3. **Should we audit and clean up unused secrets during migration?**
   **Decision:** Yes, document cleanup separately during Phase 1 to avoid scope creep

---

## Related Documents

- **[Project 05 - Deployment Pipelines](./Project_05_Deployment_Pipelines.md)** - Will use RBAC for secret access in CI/CD
- **[Project 01 - Auth Revamp](./Project_01_Auth_Revamp.md)** - Relies on secure B2C secret access via KeyVault
- **[Azure KeyVault RBAC Documentation](https://learn.microsoft.com/en-us/azure/key-vault/general/rbac-guide)** - Official Microsoft guidance
- **Infrastructure as Code Templates** - Bicep templates will be updated with RBAC assignments

---

**Last Updated:** January 31, 2026
**Owner:** Infrastructure Team
**Status:** Planning
**Next Review:** After team review and before implementation starts

---

## Changelog

- **2026-01-31:** Converted open questions to decisions; confirmed all scope items
