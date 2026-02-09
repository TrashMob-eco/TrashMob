# Project 1 — Auth Revamp (Azure B2C → Entra External ID)

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phase 0) |
| **Priority** | Critical |
| **Risk** | High |
| **Size** | Large |
| **Dependencies** | None (Project 23 depends on this) |

---

## Business Rationale

Migrate from Azure B2C to Microsoft Entra External ID to fix critical auth issues, modernize the sign-in experience, and enable Privo.com age verification for minors (Project 23).

**Why now (not later):**
- **B2C is broken:** Sign-up hangs for existing emails (#2321), profile edit not working, branding outdated
- **B2C is end-of-life:** Microsoft stopped selling B2C to new customers (May 2025). No new features, security patches only. Supported until at least May 2030.
- **Privo sponsorship:** TrashMob is the first organization to integrate Entra External ID with Privo.com. Privo is asking us to generate documentation on the process for sharing with their other customers.
- **Profile photos:** Users want profile photos; social providers already have them — migration gives us this for free

**Decision: Entra External ID** (resolved February 2026)

---

## Objectives

### Primary Goals
- **Migrate** from Azure B2C to Entra External ID with minimal user disruption
- **Fix** broken sign-up flow (existing email hangs), profile edit, and branding
- **Add** profile photo support (auto-populate from social IDP + manual upload)
- **Enable** Privo.com age verification via Custom Authentication Extensions (Project 23)
- **Document** the Entra External ID + Privo integration process (sponsorship deliverable)

### Secondary Goals
- Set home location during sign-up flow
- ToS/Privacy consent with versioning and re-consent support
- Optional partner SSO for community administrators
- Reduce auth-related support tickets
- Improve signup completion rate

---

## Current B2C Implementation (What Must Be Replaced)

### Known Issues
- **Sign-up for existing email hangs** (#2321) — B2C custom policy bug, no resolution path
- **Profile edit not working** — B2C_1A_TM_PROFILEEDIT policy broken
- **Branding outdated** — Custom HTML/CSS pages need refresh
- **B2C maintenance mode** — No new features from Microsoft

### Current Architecture
- **3 custom policies (IEF XML):** B2C_1A_TM_SIGNUP_SIGNIN, B2C_1A_TM_DEREGISTER, B2C_1A_TM_PROFILEEDIT
- **6 client IDs:** 2 web (dev/prod), 2 mobile (dev/prod), 2 backend (dev/prod)
- **User lookup by email** — auth handlers extract email from JWT, query database
- **MSAL on all platforms** — React (@azure/msal-react), MAUI (Microsoft.Identity.Client), Backend (Microsoft.Identity.Web)

### Entra External ID Replacements

| B2C Feature | Entra External ID Replacement |
|-------------|-------------------------------|
| B2C_1A_TM_SIGNUP_SIGNIN (IEF policy) | User flow (GUI-configured) + Custom Authentication Extension for Privo |
| B2C_1A_TM_PROFILEEDIT (broken) | In-app profile edit via Microsoft Graph API (more control) |
| B2C_1A_TM_DEREGISTER | In-app account deletion via Graph API |
| Custom HTML/CSS branding | Built-in branding (logo, colors, background, layout) |

### Branding Limitation
Custom CSS is **not available** — Entra External ID restricted custom CSS to tenants created before January 5, 2026, and our tenant has not been created yet. Built-in branding options (logo, colors, background image, layout templates) cover the sign-in page. All other UX is in-app and fully controlled by us.

**Mitigation:** Request exception from Microsoft given Privo sponsorship. Fall back to built-in branding if denied — it covers 90% of sign-in page needs.

---

## Scope

### Phase 0 — Tenant & Groundwork
- [ ] Create Entra External ID external tenant (dev)
- [ ] Register app registrations (web, mobile, backend) for dev
- [ ] Configure social identity providers (Google, Microsoft, Apple, Facebook)
- [ ] Configure built-in branding (logo, colors, background)
- [ ] Request custom CSS exception from Microsoft (given Privo sponsorship)
- [ ] Document tenant setup process (Privo documentation deliverable)

### Phase 1 — Sign-Up/Sign-In + Profile Photos
- [ ] Set up sign-up/sign-in user flow with custom attributes
- [ ] Configure MSAL on web (React) to point to new tenant
- [ ] Update backend JWT validation (authority URL, audience)
- [ ] Auto-populate `ProfilePhotoUrl` from social provider `picture` claim on sign-up
- [ ] Add `ProfilePhotoUrl` to User model (migration)
- [ ] Display profile photos in UserNav, event attendees, leaderboards
- [ ] Update `/api/config` endpoint with new tenant details
- [ ] Document sign-up flow for Privo

### Phase 2 — In-App Profile Edit + Photo Upload
- [ ] Build in-app profile edit page (replaces broken B2C policy)
- [ ] Update profile fields via Microsoft Graph API
- [ ] Add profile photo upload to Azure Blob Storage
- [ ] Photo display across web and mobile (avatars, attendee lists, etc.)
- [ ] In-app account deletion (replaces B2C deregister policy)
- [ ] Home location capture during sign-up or profile edit

### Phase 3 — Privo Age Verification (→ Project 23 Phase 1-2)
- [ ] Build Custom Authentication Extension (Azure Function) for age gate
- [ ] Integrate with Privo API on `OnAttributeCollectionSubmit` event
- [ ] Under-13 block, 13-17 minor flow trigger, 18+ standard flow
- [ ] Implement Privo VPC (Verifiable Parental Consent) webhook
- [ ] Consent status tracking in database (ParentalConsent entity)
- [ ] Pending account limitations for minors awaiting consent
- [ ] Document Custom Authentication Extension + Privo integration (sponsorship deliverable)

### Phase 4 — User Migration + Testing
- [ ] Run MS migration tool to export B2C users → import to Entra External ID
- [ ] Configure JIT (Just-In-Time) password migration for first-login
- [ ] Run both systems in parallel (coexistence period)
- [ ] Smoke test with real accounts on dev
- [ ] Security audit of new configuration
- [ ] Load testing

### Phase 5 — Production Cutover (Web + Backend)
- [ ] Create production Entra External ID tenant
- [ ] Register production app registrations
- [ ] Run production user migration
- [ ] Update production MSAL config + backend JWT validation
- [ ] Switch all web traffic to new system
- [ ] Monitor for auth failures, have hot rollback to B2C
- [ ] 24/7 monitoring during migration window

### Phase 6 — Mobile App Update
- [ ] Update MAUI MSAL config (AuthConstants.cs) for new tenant
- [ ] Test sign-in/sign-up on iOS and Android
- [ ] Push app update to stores
- [ ] Force update flow for users on old version
- [ ] Monitor crash-free rate
- [ ] Decommission B2C (after coexistence period)

### Phase 7 — Minor Protections (→ Project 23 Phase 3)
- [ ] Communication restrictions for minors
- [ ] Limited profile visibility (first name + last initial)
- [ ] Adult presence enforcement at events
- [ ] Parent notification system
- [ ] Complete Privo documentation package

---

## Out-of-Scope

- Automated billing for community subscriptions (manual for 2026)
- Multi-factor authentication (future enhancement)
- Social login consolidation (link Google + Microsoft accounts)
- Enterprise directory sync for large partners
- Custom CSS on hosted sign-in pages (cutoff passed; use built-in branding)

---

## Success Metrics

### Quantitative
- **Signup completion rate:** Increase by 15% (fix broken existing-email hang)
- **Auth-related support tickets:** Decrease by 40%
- **Migration success rate:** 99%+ of B2C users migrated without issues
- **Rollback events:** 0 (successful migration without needing rollback)
- **Profile photo adoption:** 30%+ of users with social IDP photos within 3 months

### Qualitative
- Mobile app store updates published and approved
- Zero security incidents related to auth changes
- Positive user feedback on new sign-in experience
- Privo documentation package delivered to sponsor

---

## Dependencies

### Blockers
- **Privo.com contract:** Required before Phase 3 (age verification)
- **Microsoft Entra External ID tenant:** Must be created (Phase 0)

### Enablers for Other Projects
- **Project 23 (Parental Consent):** Phases 3 and 7 of this project implement Project 23's core work
- **Project 10 (Community Pages):** Partner SSO is nice-to-have
- **Project 8 (Waivers V3):** Benefits from improved auth flows

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Scarce Entra External ID expertise** | Medium | High | TrashMob is Privo's first Entra External ID partner; document everything; lean on Microsoft and Privo support |
| **Custom CSS not available** | Confirmed | Low | Built-in branding covers sign-in page; all other UX is in-app; request MS exception via Privo sponsorship |
| **User confusion during migration** | High | Medium | Clear communication; in-app notifications; help center updates |
| **Mobile app store approval delays** | Medium | Medium | Submit updates early; have rollback plan |
| **Data loss during migration** | Low | Critical | Comprehensive backups; dry-run migrations; validation scripts |
| **Privo integration delays** | Medium | Medium | Phases 0-2 can proceed without Privo; Phase 3 is gated on Privo contract |
| **JIT password migration failures** | Low | High | Pre-migration dry runs; fallback to password reset flow |

---

## Implementation Plan

### Data Model Changes
- **User table additions:**
  - `ProfilePhotoUrl` (string, nullable) — auto-populated from social IDP `picture` claim

### API Changes
- **Config endpoint:** Update `/api/config` with new Entra External ID tenant details
- **Auth endpoints:** Update JWT validation (authority URL, audience, claims mapping)
- **Profile photo:** `/api/auth/profile-photo` — Upload/delete profile photo (Phase 2)
- **Profile edit:** In-app profile update via Microsoft Graph API (Phase 2)
- **Account deletion:** In-app via Graph API (replaces B2C deregister policy)

### Web UX Changes
- **Sign-in page:** Uses Entra External ID built-in branding (logo, colors, background)
- **Profile photos:** Display in UserNav, event attendees, leaderboards (Phase 1)
- **Profile edit page:** New in-app page replacing broken B2C policy (Phase 2)
- **Photo upload:** Azure Blob Storage upload with avatar cropping (Phase 2)

### Mobile App Changes
- Update MAUI MSAL config (`AuthConstants.cs`) for new Entra External ID tenant
- Handle profile photo display in app
- Coordinate app store updates with web cutover

---

## Open Questions

1. ~~**Does Entra External ID support all required social login providers?**~~
   **Decision:** Yes — Google, Microsoft, Apple, and Facebook are all supported via built-in social identity provider configuration.
   **Status:** ✅ Resolved

2. ~~**Entra External ID vs B2C final decision?**~~
   **Decision:** Entra External ID — B2C is maintenance-mode, multiple B2C policies are broken, and Privo sponsorship specifically targets Entra External ID.
   **Status:** ✅ Resolved (February 2026)

3. **Can we grandfather existing users or require re-consent for ToS?**
   **Owner:** Product team with legal review
   **Due:** Before Phase 4 (migration)

4. ~~**Do we need multi-factor auth for admins?**~~
   **Decision:** Not for initial launch — can be added later as Entra External ID supports MFA natively.
   **Status:** ✅ Resolved

5. ~~**How do we handle existing user sessions during migration cutover?**~~
   **Decision:** Not a concern — traffic volume is low enough that session disruption during migration is acceptable.
   **Status:** ✅ Resolved

6. **What is the strategy for users with duplicate accounts across identity providers?**
   **Recommendation:** Detect during migration; prompt users to link accounts or choose primary; preserve data from both.
   **Owner:** Product Lead + Engineering
   **Due:** Before Phase 4 (migration)

7. ~~**How will auth changes impact API authentication for potential third-party integrations?**~~
   **Decision:** Not a concern — there are no third-party consumer apps using the API.
   **Status:** ✅ Resolved

8. ~~**Should we implement device/session management (view active sessions, logout all devices)?**~~
   **Decision:** Not needed — user base is not large enough to warrant this feature.
   **Status:** ✅ Resolved

9. ~~**Can we get custom CSS for the Entra External ID sign-in page?**~~
   **Decision:** Custom CSS cutoff was January 5, 2026; our tenant hasn't been created yet. Request exception from Microsoft given Privo sponsorship. Fall back to built-in branding if denied.
   **Status:** ✅ Resolved (workaround identified)

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#2242](https://github.com/trashmob/TrashMob/issues/2242)** - Project 1: Revamp Sign Up / Sign-In Process (tracking issue)
- **[#2253](https://github.com/trashmob/TrashMob/issues/2253)** - Investigate Converting from AzureB2C to Entra External ID
- **[#2254](https://github.com/trashmob/TrashMob/issues/2254)** - Update the Graphics on the Sign In / Sign Up page
- **[#2255](https://github.com/trashmob/TrashMob/issues/2255)** - Add/Test current 3rd party integrations to EEID interface
- **[#2256](https://github.com/trashmob/TrashMob/issues/2256)** - Add integration with existing TrashMob APIs to Sign In Process
- **[#2257](https://github.com/trashmob/TrashMob/issues/2257)** - User can select home location during sign-up process
- **[#2258](https://github.com/trashmob/TrashMob/issues/2258)** - User can allow TrashMob to use their profile photo from 3rd party IDP
- **[#2259](https://github.com/trashmob/TrashMob/issues/2259)** - Allow user to upload a profile photo to TrashMob
- **[#2260](https://github.com/trashmob/TrashMob/issues/2260)** - Allow user to update/delete profile photo from TrashMob
- **[#2261](https://github.com/trashmob/TrashMob/issues/2261)** - Migrate current users from B2C to EEID
- **[#2262](https://github.com/trashmob/TrashMob/issues/2262)** - Allow TrashMob to set up Single Sign On with Partners
- **[#677](https://github.com/trashmob/TrashMob/issues/677)** - Add User Profile Images
- **[#948](https://github.com/trashmob/TrashMob/issues/948)** - Clean up B2C Redirects in prod
- **[#1015](https://github.com/trashmob/TrashMob/issues/1015)** - Convert to use Azure Graph for User Details
- **[#2321](https://github.com/trashmob/TrashMob/issues/2321)** - When trying to sign up with an id that already exists, the workflow hangs

---

## Related Documents

- **[Project 23 - Parental Consent](./Project_23_Parental_Consent.md)** - Age verification and minor protections (Phases 3 & 7 of this project)
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Partner SSO (nice-to-have)
- **[Project 8 - Waivers V3](./Project_08_Waivers_V3.md)** - Minor waiver handling
- **[Auth Migration Technical Design](../TechnicalDesigns/Auth_Migration.md)** - (To be created)

---

**Last Updated:** February 8, 2026
**Owner:** Security Engineer + Product Lead
**Status:** In Progress (Phase 0)
**Next Review:** After Phase 0 tenant setup
