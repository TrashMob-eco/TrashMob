# Project 1 — Auth Revamp (Azure B2C ? Entra External ID)

| Attribute | Value |
|-----------|-------|
| **Status** | Planning |
| **Priority** | High |
| **Risk** | Very High |
| **Size** | Large |
| **Dependencies** | Projects 10 (Community Pages), 23 (Minors) |

---

## Business Rationale

Modernize authentication system to enable minors (13+) participation, improve brand consistency, ensure legal compliance, and future-proof identity management. Current Azure B2C implementation lacks flexibility for parental consent workflows and partner SSO requirements.

---

## Objectives

### Primary Goals
- **Migrate** from Azure B2C to Entra External ID with minimal user disruption
- **Enable** parental consent flow for minors (13-17)
- **Refresh** sign-in UI with new branding and improved UX
- **Streamline** ToS/Privacy consent with versioning and re-consent support
- **Implement** optional partner SSO for community administrators
- **Add** profile photo upload (opt-in)

### Secondary Goals
- Set home location during sign-up flow
- Reduce auth-related support tickets
- Improve signup completion rate

---

## Decision Point: Entra External ID vs Azure B2C

**Status:** Pending (Q1 2026)

### Entra External ID Advantages
- Unified identity platform with Azure AD
- Better enterprise SSO integration for partners
- Modern UI customization options
- Future-proof as Microsoft's strategic direction
- Better developer experience and documentation

### Azure B2C Advantages
- Already implemented and working
- Known cost structure ($0/month current usage)
- Established user flows
- Zero migration risk
- Team familiarity

### Decision Criteria
1. Feature gap analysis for minors support
2. Total cost comparison (development + runtime)
3. Migration complexity and rollback plan
4. Privo.com integration compatibility
5. Partner SSO requirements
6. Community feedback from pilot users

**Decision Timeline:** Before implementation begins

---

## Scope

### Phase 1 - Planning
- ? Entra External ID vs B2C decision
- ? Architecture design and migration plan
- ? Privo.com integration design
- ? Legal review of consent workflows
- ? UI/UX mockups for new flows
- ? Rollback strategy

### Phase 2 - Implementation
- ? Set up Entra External ID tenant (if chosen)
- ? Implement minors consent flow with Privo.com
- ? Rebrand sign-in pages
- ? ToS/Privacy versioning system
- ? Home location capture during signup
- ? Profile photo upload feature
- ? Partner SSO configuration
- ? Migration scripts for existing users

### Phase 3 - Migration
- ? Shadow deployment and testing
- ? Canary rollout (5% users)
- ? Monitor and iterate
- ? Full rollout with hot rollback capability
- ? Update mobile apps (if needed)

---

## Out-of-Scope

- ? Automated billing for community subscriptions (manual for 2026)
- ? Multi-factor authentication (Phase 2)
- ? Social login consolidation (e.g., link Google + Microsoft accounts)
- ? Enterprise directory sync for large partners

---

## Success Metrics

### Quantitative
- **Signup completion rate:** Increase by 15%
- **Minors successfully onboarded:** 500+ in 2026
- **Auth-related support tickets:** Decrease by 40%
- **Migration success rate:** 99%+ of users migrated without issues
- **Rollback events:** 0 (successful migration without needing rollback)

### Qualitative
- Mobile app store updates published and approved
- Legal sign-off on consent workflows
- Zero security incidents related to auth changes
- Positive user feedback on new sign-in experience

---

## Dependencies

### Blockers
- **Legal review:** COPPA compliance and consent artifacts retention
- **Privo.com contract:** Age verification vendor agreement
- **Security audit:** Entra External ID configuration review

### Enablers for Other Projects
- **Project 10 (Community Pages):** Requires partner SSO
- **Project 23 (Minors):** Requires parental consent infrastructure
- **Project 8 (Waivers V3):** Benefits from improved auth flows

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Scarce Entra expertise** | Medium | High | Contract SME; create detailed migration plan; extensive documentation |
| **User confusion during migration** | High | Medium | Clear communication; in-app notifications; help center updates |
| **Privo.com integration issues** | Low | High | Early integration testing; fallback to manual verification |
| **Mobile app store approval delays** | Medium | Medium | Submit updates early; have rollback plan |
| **Data loss during migration** | Low | Critical | Comprehensive backups; dry-run migrations; validation scripts |
| **Performance degradation** | Low | Medium | Load testing; caching strategies; CDN for auth pages |

---

## Implementation Plan

### Data Model Changes
- **User table additions:**
  - `ConsentVersion` (int)
  - `ConsentDate` (DateTimeOffset)
  - `ConsentIPAddress` (string)
  - `IsMinor` (bool)
  - `ParentEmail` (string, nullable)
  - `ParentConsentDate` (DateTimeOffset, nullable)
  - `ProfilePhotoUrl` (string, nullable)
  - `PreferredLocationLat` (double, nullable)
  - `PreferredLocationLng` (double, nullable)

- **New table: UserConsents**
  - `UserId` (Guid, FK)
  - `ConsentType` (enum: ToS, Privacy, Parental)
  - `Version` (string)
  - `AcceptedDate` (DateTimeOffset)
  - `IPAddress` (string)
  - `UserAgent` (string)

### API Changes
- **Auth endpoints updates:**
  - `/api/auth/register` - Add consent parameters
  - `/api/auth/consent/accept` - Record consent acceptance
  - `/api/auth/profile-photo` - Upload/delete profile photo
  - `/api/auth/location` - Set preferred location

- **Claims modifications:**
  - Add `IsMinor` claim
  - Add `ConsentVersion` claim
  - Add `ProfilePhotoUrl` claim

### Web UX Changes
- **Rebrand sign-in pages:**
  - New logo and color scheme
  - Mobile-responsive design
  - Accessibility improvements (WCAG 2.2 AA)

- **New onboarding flow:**
  1. Email/password or social login
  2. Accept ToS/Privacy (with version tracking)
  3. Age verification (if under 18, trigger Privo flow)
  4. Set preferred location (map picker)
  5. Optional profile photo upload
  6. Welcome message and tour

- **Return-to-page after auth:**
  - Store intended destination before redirecting to login
  - Return after successful authentication

### Mobile App Changes
- Update MSAL configuration for new tenant (if Entra)
- Align mobile UI with new consent states
- Handle profile photo display
- Sync auth flow with web

---

## Implementation Phases

### Phase 1: Preparation
- Finalize decision (B2C vs Entra)
- Complete legal review
- Set up new environment
- Create migration scripts

### Phase 2: Development
- Implement consent flows
- Rebrand auth pages
- API updates
- Mobile app updates

### Phase 3: Testing
- Unit and integration tests
- Security audit
- Load testing
- User acceptance testing (UAT)

### Phase 4: Shadow Deployment
- Deploy to production but don't route traffic
- Validate configuration
- Monitor metrics

### Phase 5: Canary Rollout
- Route 5% of new signups to new system
- Monitor error rates and user feedback
- A/B test signup completion rates

### Phase 6: Gradual Rollout
- Increase to 25%, 50%, 75% based on metrics
- Migrate existing users in batches
- 24/7 monitoring during migration

### Phase 7: Full Deployment
- 100% traffic to new system
- Decommission old auth flow (keep available for rollback)
- Update documentation

### Phase 8: Stabilization
- Address bugs and feedback
- Optimize performance
- Document lessons learned

**Note:** Phases are sequential. Each phase gate requires sign-off before proceeding.

---

## Open Questions

1. **Entra External ID vs B2C final decision?**  
   **Owner:** Security Engineer + Product Lead  
   **Due:** Before implementation starts

2. **What exact documents/data prove parental consent?**  
   **Owner:** Legal team  
   **Due:** Before implementation starts

3. **How long must we retain consent artifacts?**  
   **Owner:** Legal team  
   **Due:** Before implementation starts

4. **Can we grandfather existing users or require re-consent?**  
   **Owner:** Product team with legal review  
   **Due:** Before implementation starts

5. **What's the fallback if Privo.com is unavailable?**  
   **Owner:** Engineering team  
   **Due:** During implementation

6. **Do we need multi-factor auth for admins?**  
   **Owner:** Security team  
   **Due:** Can be Phase 2 if needed

---

## Related Documents

- **[Project 23 - Parental Consent](./Project_23_Parental_Consent.md)** - Minors registration details
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Partner SSO requirements
- **[Auth Migration Technical Design](../TechnicalDesigns/Auth_Migration.md)** - (To be created)
- **[Privo.com Integration Guide](../TechnicalDesigns/Privo_Integration.md)** - (To be created)

---

**Last Updated:** January 24, 2026  
**Owner:** Security Engineer + Product Lead  
**Status:** Ready for Decision  
**Next Review:** Before implementation phase begins
