# Project 1 � Auth Revamp (Azure B2C ? Entra External ID)

| Attribute | Value |
|-----------|-------|
| **Status** | Planning |
| **Priority** | High |
| **Risk** | Very High |
| **Size** | Large |
| **Dependencies** | None |

---

## Business Rationale

Modernize authentication system to improve brand consistency, simplify maintenance, and future-proof identity management. Evaluate whether Entra External ID provides benefits over current Azure B2C implementation for partner SSO requirements and long-term platform strategy.

---

## Objectives

### Primary Goals
- **Evaluate** Entra External ID vs staying on Azure B2C
- **Migrate** to Entra External ID if evaluation supports it, with minimal user disruption
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
1. **Social login support:** Research whether Entra External ID supports required social providers (Google, Microsoft, Facebook, Apple)
2. **Feature parity:** Ensure all current B2C capabilities are available in Entra External ID
3. **Migration complexity:** Assess effort to migrate existing users and configurations
4. **Total cost comparison:** Development + runtime costs
5. **Partner SSO requirements:** Enterprise federation capabilities
6. **Developer experience:** SDK support, documentation quality, debugging tools

**Decision Timeline:** Before implementation begins

---

## Scope

### Phase 1 - Planning
- ☐ Research Entra External ID feature support (social logins, custom UI, etc.)
- ☐ Entra External ID vs B2C decision
- ☐ Architecture design and migration plan
- ☐ UI/UX mockups for new flows
- ☐ Rollback strategy

### Phase 2 - Implementation
- ☐ Set up Entra External ID tenant (if chosen)
- ☐ Rebrand sign-in pages
- ☐ ToS/Privacy versioning system
- ☐ Home location capture during signup
- ☐ Profile photo upload feature
- ☐ Partner SSO configuration
- ☐ Migration scripts for existing users

### Phase 3 - Migration
- ☐ Shadow deployment and testing
- ☐ Full rollout with hot rollback capability
- ☐ Monitor and iterate
- ☐ Update mobile apps (if needed)

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
- **Auth-related support tickets:** Decrease by 40%
- **Migration success rate:** 99%+ of users migrated without issues
- **Rollback events:** 0 (successful migration without needing rollback)

### Qualitative
- Mobile app store updates published and approved
- Zero security incidents related to auth changes
- Positive user feedback on new sign-in experience

---

## Dependencies

### Blockers
- **Feature research:** Confirm Entra External ID supports all required features
- **Security audit:** Entra External ID configuration review (if migrating)

### Enablers for Other Projects
- **Project 10 (Community Pages):** Partner SSO is nice-to-have
- **Project 8 (Waivers V3):** Benefits from improved auth flows

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Scarce Entra expertise** | Medium | High | Contract SME; create detailed migration plan; extensive documentation |
| **User confusion during migration** | High | Medium | Clear communication; in-app notifications; help center updates |
| **Mobile app store approval delays** | Medium | Medium | Submit updates early; have rollback plan |
| **Data loss during migration** | Low | Critical | Comprehensive backups; dry-run migrations; validation scripts |
| **Performance degradation** | Low | Medium | Load testing; caching strategies; CDN for auth pages |
| **Entra missing required features** | Medium | High | Thorough research in Phase 1; stay on B2C if gaps found |

---

## Implementation Plan

### Data Model Changes
- **User table additions:**
  - `ProfilePhotoUrl` (string, nullable)

### API Changes
- **Auth endpoints updates:**
  - `/api/auth/profile-photo` - Upload/delete profile photo

- **Claims modifications:**
  - Add `ProfilePhotoUrl` claim

### Web UX Changes
- **Rebrand sign-in pages:**
  - New logo and color scheme
  - Mobile-responsive design
  - Accessibility improvements (WCAG 2.2 AA)

- **New onboarding flow:**
  1. Email/password or social login
  2. Accept ToS/Privacy (with version tracking)
  3. Set preferred location (map picker)
  4. Optional profile photo upload
  5. Welcome message and tour

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

### Phase 5: Full Deployment
- Switch all traffic to new system
- Migrate existing users
- 24/7 monitoring during migration

### Phase 6: Stabilization
- Address bugs and feedback
- Optimize performance
- Decommission old auth flow (keep available for rollback)
- Update documentation
- Document lessons learned

**Note:** Phases are sequential. Each phase gate requires sign-off before proceeding.

---

## Open Questions

1. **Does Entra External ID support all required social login providers?**
   **Owner:** Engineering team
   **Due:** Phase 1 research

2. **Entra External ID vs B2C final decision?**
   **Owner:** Security Engineer + Product Lead
   **Due:** Before implementation starts

3. **Can we grandfather existing users or require re-consent for ToS?**
   **Owner:** Product team with legal review
   **Due:** Before implementation starts

4. **Do we need multi-factor auth for admins?**
   **Owner:** Security team
   **Due:** Can be Phase 2 if needed

5. **How do we handle existing user sessions during migration cutover?**
   **Recommendation:** Implement session migration strategy; either invalidate all sessions (require re-login) or transfer session state to new system
   **Owner:** Engineering Team
   **Due:** Before Phase 5

6. **What is the strategy for users with duplicate accounts across identity providers?**
   **Recommendation:** Detect during migration; prompt users to link accounts or choose primary; preserve data from both
   **Owner:** Product Lead + Engineering
   **Due:** Before implementation

7. **How will auth changes impact API authentication for potential third-party integrations?**
   **Recommendation:** Document API auth changes; provide migration guide for any future third-party consumers
   **Owner:** Security Team
   **Due:** Before Phase 2

8. **Should we implement device/session management (view active sessions, logout all devices)?**
   **Recommendation:** Add to user profile settings; enables security for shared device scenarios
   **Owner:** Security Team
   **Due:** Can be Phase 2

---

## Related Documents

- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Partner SSO (nice-to-have)
- **[Project 23 - Parental Consent](./Project_23_Parental_Consent.md)** - Separate project for minors/consent
- **[Auth Migration Technical Design](../TechnicalDesigns/Auth_Migration.md)** - (To be created)

---

**Last Updated:** January 24, 2026  
**Owner:** Security Engineer + Product Lead  
**Status:** Ready for Decision  
**Next Review:** Before implementation phase begins
