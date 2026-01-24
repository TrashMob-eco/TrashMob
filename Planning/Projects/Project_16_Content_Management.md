# Project 16 — Page Content Management (CMS via Strapi)

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (PR #2364) |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

Allow non-developers to update home page and partner content with preview, scheduling, and versioning/rollback capabilities. Currently, any content change requires developer involvement and deployment.

---

## Objectives

### Primary Goals
- **CMS tool & workflow** using Strapi headless CMS
- **Preview & scheduled publish** for content changes
- **Versioning/rollback** for content safety
- **Non-developer content editing** for marketing/admin team

### Secondary Goals
- Multi-language content support (future)
- Content approval workflows
- A/B testing support
- Content analytics

---

## Scope

### Phase 1 - Infrastructure
- ✅ Deploy Strapi as Azure Container App
- ✅ Configure Azure SQL database for Strapi
- ✅ Set up internal-only ingress (security)
- ✅ GitHub Actions deployment workflow

### Phase 2 - Home Page Content
- ✅ Hero section (tagline, buttons, background)
- ✅ What is TrashMob section
- ✅ Getting Started section
- ✅ React frontend integration

### Phase 3 - Admin Integration
- ✅ "Manage Content" tab in site admin
- ✅ Link to Strapi admin panel
- ✅ Content type documentation

### Phase 4 - Expansion (Future)
- ❓ Community page content
- ❓ Team page branding
- ❓ Partner page customization
- ❓ News/announcements

---

## Out-of-Scope

- ❌ Full website CMS (blog, etc.)
- ❌ User-generated content management
- ❌ Event content (managed through app)
- ❌ Multi-tenant CMS for communities (future)

---

## Success Metrics

### Quantitative
- **Content update time:** < 5 minutes for simple changes
- **Developer involvement:** 0 for content-only changes
- **Uptime:** ≥ 99.9% (with fallback to defaults)

### Qualitative
- Marketing team comfortable using CMS
- No content-related incidents
- Positive feedback on admin experience

---

## Dependencies

### Blockers
None - independent infrastructure

### Enables
- **Project 2 (Home Page):** Dynamic content for news, partners
- **Project 10 (Community Pages):** Community-specific content
- **Marketing velocity:** Faster content iterations

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Strapi downtime affects site** | Low | Medium | Fallback to hardcoded defaults; caching |
| **Content mistakes go live** | Medium | Medium | Preview environment; rollback capability |
| **Security exposure** | Low | High | Internal-only ingress; proxy through ASP.NET |
| **Learning curve for editors** | Medium | Low | Documentation; training; simple content types |

---

## Implementation Status (PR #2364)

### Completed Infrastructure

**Database:**
- `Deploy/sqlDatabaseStrapi.bicep` - Strapi database on existing Azure SQL Server (Basic tier, 5 DTU)

**Container App:**
- `Deploy/containerAppStrapi.bicep` - Internal-only Strapi container with:
  - Port 1337 internal ingress (not internet accessible)
  - Health checks on `/_health`
  - 0.5 CPU, 1Gi memory
  - Environment variables for database and secrets

**Deployment:**
- `.github/workflows/container_strapi-tm-dev-westus2.yml` - Build and deploy workflow

### Completed Backend

**ASP.NET Core:**
```csharp
// Program.cs - HttpClient registration
builder.Services.AddHttpClient("Strapi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["StrapiBaseUrl"] ?? "http://localhost:1337");
});

// CmsController.cs - Proxy endpoints
[HttpGet("api/cms/hero-section")]
public async Task<ActionResult<HeroSectionDto>> GetHeroSection()
{
    // Fetch from Strapi, return to frontend
}

[HttpGet("api/cms/what-is-trashmob")]
public async Task<ActionResult<WhatIsTrashMobDto>> GetWhatIsTrashMob()
{
    // Fetch from Strapi, return to frontend
}

[HttpGet("api/cms/getting-started")]
public async Task<ActionResult<GettingStartedDto>> GetGettingStarted()
{
    // Fetch from Strapi, return to frontend
}

[Authorize(Roles = "SiteAdmin")]
[HttpGet("api/cms/admin-url")]
public ActionResult<string> GetAdminUrl()
{
    // Return Strapi admin URL for redirect
}
```

### Completed Frontend

**CMS Service:**
```typescript
// services/cms.ts
export interface HeroSection {
  tagline: string;
  primaryButtonText: string;
  primaryButtonLink: string;
  googlePlayUrl: string;
  appStoreUrl: string;
  backgroundImage?: string;
}

export const GetHeroSection = () => ({
  key: ['cms', 'hero-section'],
  service: async (): Promise<HeroSection> => {
    const response = await fetch('/api/cms/hero-section');
    if (!response.ok) throw new Error('Failed to fetch');
    return response.json();
  },
});
```

**Updated Components:**
- `hero-section.tsx` - Fetches CMS content with React Query
- `whatistrashmob-section.tsx` - Fetches CMS content
- `getting-start-section.tsx` - Fetches CMS content

**Admin Page:**
- `pages/siteadmin/content.tsx` - Links to Strapi admin

### Strapi Project Structure

```
Strapi/
├── Dockerfile              # Multi-stage Node 20 Alpine build
├── package.json           # Strapi v5 dependencies
├── config/
│   ├── database.ts        # MS SQL connection
│   ├── server.ts          # Server config
│   └── admin.ts           # Admin panel config
└── src/
    └── api/
        ├── hero-section/         # Single type
        ├── what-is-trashmob/     # Single type
        └── getting-started/      # Single type
```

### Content Types

**Hero Section (Single Type):**
- `tagline` (string)
- `primaryButtonText` (string)
- `primaryButtonLink` (string)
- `googlePlayUrl` (string)
- `appStoreUrl` (string)
- `backgroundImage` (media)

**What Is TrashMob (Single Type):**
- `heading` (string)
- `description` (text)
- `youtubeVideoUrl` (string)
- `primaryButtonText`, `primaryButtonLink` (strings)
- `secondaryButtonText`, `secondaryButtonLink` (strings)

**Getting Started (Single Type):**
- `heading` (string)
- `subheading` (string)
- `requirements` (repeatable component: icon, label)
- `ctaButtonText`, `ctaButtonLink` (strings)

---

## Security Notes

- Strapi container has **internal-only ingress** - not accessible from internet
- All CMS API access proxied through ASP.NET Core
- Admin endpoints require `isSiteAdmin` authorization
- Strapi secrets stored in Azure Key Vault:
  - `strapi-db-password`
  - `strapi-admin-jwt-secret`
  - `strapi-api-token-salt`
  - `strapi-app-keys`
  - `strapi-transfer-token-salt`

---

## Implementation Phases

### Phase 1: Infrastructure ✅
- Bicep templates
- GitHub workflow
- Database setup

### Phase 2: Integration ✅
- CmsController
- React hooks
- Home page sections

### Phase 3: Admin UX ✅
- Admin content page
- Documentation

### Phase 4: Expansion (Future)
- Community content
- Partner content
- News/announcements

---

## Future Phases

This CMS foundation enables:
- **Community Pages (Project 10):** Custom content per community
- **Teams (Project 9):** Team branding and descriptions
- **Partner Pages:** Partner-managed content
- **Dynamic News:** Home page announcements

---

## Open Questions

1. **Content preview environment?**
   **Recommendation:** Use Strapi draft/publish; add preview API endpoint
   **Owner:** Engineering
   **Due:** Phase 4

2. **Multi-language support?**
   **Recommendation:** Strapi i18n plugin when needed
   **Owner:** Product Lead
   **Due:** When international expansion planned

3. **Content approval workflow?**
   **Recommendation:** Simple for now; Strapi workflows for enterprise needs
   **Owner:** Product Lead
   **Due:** When team grows

---

## Related Documents

- **[Project 2 - Home Page](./Project_02_Home_Page.md)** - Uses CMS content
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Future CMS expansion
- **PR #2364** - Implementation pull request

---

**Last Updated:** January 24, 2026
**Owner:** Engineering Team
**Status:** In Progress (PR #2364)
**Next Review:** After PR merge
