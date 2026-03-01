# Project 52 — Volunteer Rewards Program

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Low |
| **Risk** | High |
| **Size** | Large |
| **Dependencies** | Project 20 (Gamification), Project 51 (Contact Management) |

---

## Business Rationale

TrashMob's leaderboards, badges, and achievements (Project 20) provide intrinsic motivation — recognition and friendly competition. A Volunteer Rewards Program adds **extrinsic motivation** by offering tangible rewards from partner businesses — a free coffee, a discount at a local shop, branded merchandise — in recognition of volunteer effort.

This program serves multiple purposes:

- **Volunteer retention** — tangible incentives keep volunteers coming back, especially in the critical first few months
- **Partner engagement** — local businesses gain visibility with an environmentally conscious audience; TrashMob gains resources without cash outlay
- **Community building** — rewards tied to local partners strengthen the connection between TrashMob and the communities it serves
- **Growth catalyst** — "Clean up your neighborhood, earn free coffee" is a compelling recruitment message

This is a **future project** — not prioritized for immediate development. It should be considered as TrashMob grows its volunteer base and partner network to a scale where a rewards program becomes impactful and sustainable.

**Admin-facing features (partner sourcing, reward management) are Site Admin only. Volunteer-facing features (viewing/claiming rewards) are available to all authenticated users.**

---

## Objectives

### Primary Goals
- **Partner reward sourcing** — Admin tools to identify, recruit, and manage partners willing to contribute rewards
- **Reward criteria** — Define what volunteer actions earn rewards and at what thresholds
- **Fraud & abuse prevention** — Ensure rewards go to genuine volunteers for real cleanup activity
- **Reward distribution** — Mechanism to deliver rewards to qualifying volunteers
- **Volunteer awareness** — Notify users about available rewards and their progress toward earning them

### Secondary Goals
- Community-specific rewards from local partner businesses
- Seasonal or campaign-specific reward promotions
- Sponsor recognition for reward-contributing partners
- Impact reporting for reward partners (ROI on their contribution)
- Integration with existing gamification milestones and badges

---

## Scope

### Phase 1 — Reward Data Model & Admin Foundation

- ❌ Create `RewardPartner` entity — businesses/organizations contributing rewards (may link to existing Partner)
- ❌ Create `Reward` entity — a specific reward offering (description, quantity, eligibility criteria, expiration)
- ❌ Create `RewardClaim` entity — tracks when a volunteer earns and redeems a reward
- ❌ Create `RewardCriteria` entity — defines the rules for earning a reward (e.g., "attend 5 events", "collect 100 lbs")
- ❌ Admin UI: manage reward partners, create/edit rewards, set eligibility criteria
- ❌ Database migrations with proper indexes and audit fields

### Phase 2 — Partner Sourcing & Recruitment (Admin Only)

- ❌ Partner prospect list — admin tool to search for and track potential reward partners
- ❌ Outreach templates — email/letter templates for approaching businesses about contributing rewards
- ❌ Partner onboarding workflow — agreement terms, reward commitment, contact info, logo/branding
- ❌ AI-assisted partner discovery — suggest local businesses near active cleanup areas that might be good reward partners (coffee shops, restaurants, outdoor retailers, etc.)
- ❌ Partner dashboard — show reward partners the impact of their contribution (volunteers reached, redemptions, brand impressions)

### Phase 3 — Reward Eligibility & Earning Rules

- ❌ Define reward criteria types:
  - Event-based: attend N events (total or within time window)
  - Impact-based: collect N bags or N lbs of trash
  - Streak-based: attend events N consecutive weeks/months
  - Milestone-based: first event, 10th event, 50th event, etc.
  - Community-based: participate in N events within a specific community
  - Campaign-based: participate in a specific campaign or challenge
- ❌ Criteria engine — evaluate volunteer activity against reward criteria, flag when earned
- ❌ Tiered rewards — bronze/silver/gold levels with increasing reward value for deeper engagement
- ❌ Integration with Project 20 gamification data (achievements, streaks, leaderboard position)

### Phase 4 — Fraud & Abuse Prevention

- ❌ Event verification — rewards only count for events with verified Event Summaries submitted by the event lead
- ❌ Attendance validation — cross-reference with event attendance records; require event lead confirmation
- ❌ Rate limiting — cap the number of rewards a single volunteer can claim per time period
- ❌ Anomaly detection — flag unusual patterns (e.g., one-person events with inflated metrics, same user creating and attending their own events exclusively)
- ❌ Cooldown periods — minimum time between claiming the same reward
- ❌ Admin review queue — suspicious claims flagged for manual review before fulfillment
- ❌ Audit log — complete history of reward earning and redemption for accountability

### Phase 5 — Reward Distribution

- ❌ Digital reward codes — generate unique codes that volunteers redeem with the partner business
- ❌ QR code generation — scannable codes for in-store redemption
- ❌ Email delivery — send reward codes/vouchers via email with partner branding
- ❌ In-app reward wallet — volunteers see their earned rewards, codes, and expiration dates
- ❌ Redemption tracking — mark rewards as redeemed (partner confirms or volunteer self-reports)
- ❌ Expiration management — auto-expire unclaimed rewards, notify volunteers before expiration
- ❌ Inventory management — track available reward quantity, auto-disable when depleted, notify admin

### Phase 6 — Volunteer Awareness & Engagement

- ❌ Rewards page — browse available rewards with eligibility requirements and progress indicators
- ❌ Progress tracking — "You're 2 events away from earning a free coffee at [Partner]!"
- ❌ Push notifications / email — notify when a reward is earned, when close to earning, when expiring
- ❌ Post-event nudge — after event completion, show progress toward next reward
- ❌ Rewards showcase on profile — display earned rewards alongside badges/achievements
- ❌ Social sharing — "I earned [reward] by cleaning up my neighborhood with @TrashMob!"
- ❌ Home page/dashboard integration — highlight available rewards to drive engagement

### Phase 7 — Reporting & Analytics

- ❌ Program metrics dashboard — rewards distributed, redemption rate, partner ROI
- ❌ Volunteer impact report — does the rewards program improve retention and event attendance?
- ❌ Partner performance report — which partners' rewards drive the most engagement?
- ❌ Cost analysis — program cost (admin time, partner management) vs. volunteer retention value
- ❌ Geographic analysis — which communities have the best reward partner coverage?

---

## Out-of-Scope

- ❌ Cash rewards or direct monetary compensation — TrashMob is a volunteer organization
- ❌ Cryptocurrency or NFT-based rewards
- ❌ Complex points/currency system — keep it simple with direct reward earning, not virtual currency
- ❌ Marketplace where volunteers choose from a catalog — rewards are partner-specific offerings
- ❌ Payment processing for reward purchases — rewards are donated by partners, not purchased
- ❌ Mobile app reward features — web only initially; mobile is a future enhancement
- ❌ Automated partner billing or invoicing

---

## Success Metrics

### Quantitative
- **Partner enrollment** — 10+ reward partners within first 6 months of launch
- **Reward redemption rate** — >60% of earned rewards redeemed
- **Volunteer retention impact** — measurable improvement in 90-day volunteer retention for reward-eligible vs. non-eligible volunteers
- **Event attendance** — increase in repeat event attendance after reward program launch
- **Fraud rate** — <1% of reward claims flagged as suspicious

### Qualitative
- Volunteers find rewards motivating but not the primary reason they participate
- Partners see value and renew their reward commitments
- Program is sustainable without significant admin overhead
- Rewards complement (not replace) intrinsic motivation from gamification

---

## Dependencies

### Blockers
- **Project 20 (Gamification)** — Reward criteria should integrate with existing achievement and streak tracking
- **Project 51 (Contact Management)** — Partner sourcing tools can leverage the contact management infrastructure

### Enables
- **Future: Corporate partnership expansion** — Reward program demonstrates concrete partner ROI
- **Future: Sponsorship tiers** — Partners contributing rewards could be part of a formalized sponsorship program
- **Project 36 (Marketing Materials)** — Reward program is a strong marketing message for volunteer recruitment

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Perverse incentives** — volunteers focus on gaming metrics rather than genuine cleanup | Medium | High | Require event lead verification of attendance and Event Summary; weight fraud detection toward verified group events rather than solo events |
| **Partner fatigue** — businesses lose interest if redemption volume is too low | Medium | Medium | Start with partners who have genuine community interest; provide regular impact reports; keep partner commitment small and renewable |
| **Intrinsic motivation crowding** — extrinsic rewards reduce intrinsic motivation | Low | High | Position rewards as "thank you" not "payment"; keep reward value modest; emphasize recognition and community over material value; research shows small token rewards don't crowd out intrinsic motivation |
| **Admin overhead** — managing partners and rewards consumes too much staff time | Medium | Medium | Automate as much as possible (eligibility checks, code generation, notifications); self-service partner dashboard; start small with few partners |
| **Equity concerns** — rewards only available in certain communities with partners | High | Medium | Offer universal digital rewards (TrashMob merchandise, donation-in-your-name) alongside local partner rewards; prioritize partner recruitment in underserved areas |
| **Legal/tax complexity** — reward value may have tax implications | Low | Medium | Keep individual reward values under IRS de minimis threshold ($75); consult tax advisor before launch; document fair market value of all rewards |
| **Fraud at scale** — coordinated abuse by bad actors | Low | High | Rate limiting, anomaly detection, admin review queue, cooldown periods; can pause program if abuse detected |

---

## Implementation Plan

### Data Model Changes

**New Entities:**

**`RewardPartner`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `Name` | string (256) | Yes | Business name |
| `ContactName` | string (200) | No | Primary contact person |
| `ContactEmail` | string (256) | No | |
| `ContactPhone` | string (20) | No | |
| `LogoUrl` | string (500) | No | Partner logo for display |
| `Website` | string (500) | No | |
| `Description` | string (1000) | No | About the partner |
| `PartnerId` | Guid? (FK) | No | Link to existing Partner entity if applicable |
| `ContactId` | Guid? (FK) | No | Link to Contact (Project 51) if applicable |
| `AgreementStartDate` | DateTimeOffset? | No | When partnership began |
| `AgreementEndDate` | DateTimeOffset? | No | When partnership expires |
| `IsActive` | bool | Yes | Currently contributing rewards |
| `Notes` | string (2000) | No | |
| Audit fields | | | |

**`Reward`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `RewardPartnerId` | Guid (FK) | Yes | Contributing partner |
| `Title` | string (200) | Yes | e.g., "Free Medium Coffee" |
| `Description` | string (1000) | No | Details and terms |
| `RewardType` | enum | Yes | DigitalCode, QRCode, Merchandise, DonationInYourName |
| `Value` | decimal? | No | Estimated fair market value |
| `TotalQuantity` | int? | No | Total available (null = unlimited) |
| `RemainingQuantity` | int? | No | Remaining available |
| `ExpirationDate` | DateTimeOffset? | No | When reward offer expires |
| `CooldownDays` | int | No | Days before same volunteer can earn again |
| `MaxClaimsPerVolunteer` | int? | No | Lifetime cap per volunteer (null = unlimited) |
| `ImageUrl` | string (500) | No | Reward image for display |
| `IsActive` | bool | Yes | Currently available |
| `CommunityId` | Guid? (FK) | No | Restrict to specific community (null = global) |
| Audit fields | | | |

**`RewardCriteria`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `RewardId` | Guid (FK) | Yes | Parent reward |
| `CriteriaType` | enum | Yes | EventCount, BagCount, WeightCollected, StreakWeeks, Milestone, CampaignParticipation |
| `Threshold` | int | Yes | Required value to earn (e.g., 5 events, 100 lbs) |
| `TimeWindowDays` | int? | No | Rolling window (null = all time) |
| `CommunityId` | Guid? (FK) | No | Restrict criteria to specific community |
| Audit fields | | | |

**`RewardClaim`:**

| Field | Type | Required | Notes |
|-------|------|----------|-------|
| `Id` | Guid (PK) | Yes | KeyedModel base |
| `RewardId` | Guid (FK) | Yes | Reward earned |
| `UserId` | Guid (FK) | Yes | Volunteer who earned it |
| `EarnedDate` | DateTimeOffset | Yes | When criteria were met |
| `ClaimStatus` | enum | Yes | Earned, Claimed, Redeemed, Expired, Revoked |
| `RedemptionCode` | string (50) | No | Unique code for partner redemption |
| `RedeemedDate` | DateTimeOffset? | No | When volunteer used the reward |
| `ExpiresDate` | DateTimeOffset? | No | When this specific claim expires |
| `FlaggedForReview` | bool | No | Fraud flag |
| `ReviewNotes` | string (1000) | No | Admin notes on flagged claims |
| Audit fields | | | |

### API Changes

**Admin endpoints (Site Admin only):**

**RewardPartnersController:**
- `GET /api/rewardpartners` — List reward partners
- `GET /api/rewardpartners/{id}` — Get partner with rewards and stats
- `POST /api/rewardpartners` — Create reward partner
- `PUT /api/rewardpartners/{id}` — Update reward partner
- `DELETE /api/rewardpartners/{id}` — Deactivate reward partner

**RewardsController (Admin):**
- `GET /api/rewards` — List all rewards with filters (partner, status, community)
- `POST /api/rewards` — Create reward with criteria
- `PUT /api/rewards/{id}` — Update reward
- `DELETE /api/rewards/{id}` — Deactivate reward

**RewardClaimsController (Admin):**
- `GET /api/rewardclaims` — List all claims with filters (status, flagged)
- `PUT /api/rewardclaims/{id}/approve` — Approve flagged claim
- `PUT /api/rewardclaims/{id}/revoke` — Revoke fraudulent claim

**Volunteer-facing endpoints (authenticated users):**
- `GET /api/rewards/available` — List rewards the user is eligible for or progressing toward
- `GET /api/rewards/my-claims` — List user's earned/claimed rewards
- `POST /api/rewards/{id}/claim` — Claim an earned reward (generates redemption code)

### Web UX Changes

**Admin Pages:**
- `/admin/rewards` — Reward program dashboard (active rewards, claims, partner summary)
- `/admin/rewards/partners` — Manage reward partners
- `/admin/rewards/partners/{id}` — Partner detail with reward offerings and redemption stats
- `/admin/rewards/catalog` — Manage reward offerings and criteria
- `/admin/rewards/claims` — Review claims, handle flagged items

**Volunteer Pages:**
- `/rewards` — Browse available rewards with progress indicators
- Profile section — earned rewards and redemption codes

### Mobile App Changes

Future phase — not in initial scope.

---

## Implementation Phases

### Phase 1: Data Model & Admin Foundation
1. Create all entities in `TrashMob.Models/`
2. Add EF Core configurations and migration
3. Create repositories and managers
4. Register in `ServiceBuilder.cs`
5. Create admin controllers (RewardPartners, Rewards, RewardClaims)
6. Build admin reward management UI
7. Unit tests for managers

### Phase 2: Partner Sourcing & Recruitment
1. Build partner prospect tracking (leveraging Project 51 contacts)
2. Create outreach email templates
3. Build partner onboarding form and workflow
4. Implement AI-assisted local business discovery
5. Build partner impact dashboard

### Phase 3: Reward Eligibility Engine
1. Implement criteria evaluation engine
2. Integrate with gamification data (events, bags, weight, streaks)
3. Build tiered reward levels
4. Create background job to evaluate eligibility periodically
5. Unit tests for criteria engine with edge cases

### Phase 4: Fraud Prevention
1. Implement Event Summary verification requirement
2. Build attendance validation checks
3. Add rate limiting and cooldown enforcement
4. Create anomaly detection rules
5. Build admin review queue for flagged claims
6. Add comprehensive audit logging

### Phase 5: Reward Distribution
1. Implement digital code generation (unique, non-guessable)
2. Add QR code generation
3. Build email delivery with partner branding
4. Create in-app reward wallet
5. Implement redemption tracking
6. Add expiration management and notifications
7. Build inventory tracking and depletion alerts

### Phase 6: Volunteer Awareness
1. Build rewards browse page with progress indicators
2. Implement push/email notifications (earned, near-earning, expiring)
3. Add post-event reward progress nudge
4. Add rewards section to volunteer profile
5. Build social sharing for earned rewards
6. Integrate with home page / dashboard

### Phase 7: Reporting & Analytics
1. Build program metrics dashboard
2. Create retention impact analysis
3. Build partner performance reports
4. Add cost analysis tools
5. Geographic coverage analysis

**Note:** Phases are sequential but not time-bound. This is a future project to be prioritized as the platform grows.

---

## Open Questions

1. **Should rewards be visible to all users or only in communities with active reward partners?**
   **Decision:** TBD
   **Status:** Showing rewards globally drives awareness but may frustrate users in areas without partners. Consider a "coming to your area" approach.

2. **What is the minimum viable reward value that motivates without creating perverse incentives?**
   **Decision:** TBD
   **Status:** Research suggests small, symbolic rewards (free coffee, $5 discount) are effective. Start with low-value rewards and adjust based on data.

3. **Should reward partners pay a fee or is this purely a donated/sponsored arrangement?**
   **Decision:** TBD
   **Status:** Initially free for partners to encourage participation. Evaluate fee structure later if program demonstrates clear marketing value for partners.

4. **How should universal/digital rewards (TrashMob merchandise) be funded?**
   **Decision:** TBD
   **Status:** Could use donation funds (Project 51), grant funding, or Spreadshop merchandise at-cost. Needs budget planning.

---

## Design Considerations

### Intrinsic vs. Extrinsic Motivation

Research on volunteer motivation suggests:
- **Small token rewards** complement intrinsic motivation without crowding it out
- **Large rewards** risk turning volunteering into transactional behavior
- **Recognition-framed rewards** ("thank you for your service") work better than payment-framed ("you earned this")
- **Surprise rewards** (unexpected recognition) boost satisfaction more than expected entitlements

**Recommendation:** Frame the program as "community thank you" not "volunteer compensation." Keep reward values modest. Emphasize the partner's gratitude and community connection over material value.

### Tax Implications

- IRS de minimis fringe benefit threshold: rewards under $75 per occasion are generally not taxable
- If reward value exceeds thresholds, may need to issue 1099 forms
- Consult tax advisor before launch
- Document fair market value of all rewards

---

## Related Documents

- **[Project 20 - Gamification](./Project_20_Gamification.md)** — Leaderboards, badges, achievements that reward criteria can build on
- **[Project 51 - Contact Management](./Project_51_Contact_Management.md)** — Partner sourcing can leverage contact management infrastructure
- **[Project 36 - Marketing Materials](./Project_36_Marketing_Materials.md)** — Reward program as a marketing/recruitment tool
- **[Project 45 - Community Showcase](./Project_45_Community_Showcase.md)** — Community-specific rewards complement community pages

---

**Last Updated:** February 28, 2026
**Owner:** Product & Engineering Team
**Status:** Not Started — Future project for consideration as platform grows
**Next Review:** When volunteer base and partner network reach sufficient scale
