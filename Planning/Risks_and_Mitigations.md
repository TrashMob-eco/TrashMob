# TrashMob.eco � 2026 Risks & Mitigations

**Version:** 1.0  
**Date:** January 24, 2026  
**Owner:** Director of Product & Engineering

---

## Purpose

This document consolidates all risks across the 2026 product and engineering portfolio, providing a centralized view of threats to project success and organizational mitigation strategies.

---

## Risk Assessment Framework

### Likelihood Scale
- **Low:** < 20% chance of occurring
- **Medium:** 20-50% chance
- **High:** 50-80% chance
- **Very High:** > 80% chance

### Impact Scale
- **Low:** Minor inconvenience, no project delay
- **Medium:** Delays < 2 weeks, limited scope reduction
- **High:** Delays 2-4 weeks, significant scope adjustment
- **Critical:** Delays > 4 weeks, project cancellation risk

### Risk Priority Matrix
Priority = Likelihood � Impact

| Likelihood ? Impact ? | Low | Medium | High | Critical |
|----------------------|-----|--------|------|----------|
| **Very High** | ?? Medium | ?? High | ?? Critical | ?? Critical |
| **High** | ?? Medium | ?? High | ?? Critical | ?? Critical |
| **Medium** | ?? Low | ?? Medium | ?? High | ?? Critical |
| **Low** | ?? Low | ?? Low | ?? Medium | ?? High |

---

## Strategic Risks

### SR-1: Legal & Compliance (COPPA, Waivers)

**Category:** Regulatory  
**Likelihood:** Medium  
**Impact:** Critical  
**Priority:** ?? Critical

**Description:**
Failure to comply with COPPA for minors (13+) or improper waiver enforcement could result in legal liability, fines, or forced shutdown of features.

**Affected Projects:**
- Project 1 (Auth Revamp)
- Project 8 (Waivers V3)
- Project 23 (Parental Consent)

**Mitigations:**
1. **Legal Gates:** Require legal sign-off before any build begins
2. **Expert Consultation:** Engage Privo.com for age verification and COPPA compliance
3. **Document Retention:** Implement robust consent artifact storage per legal requirements
4. **Regular Audits:** Quarterly compliance reviews
5. **Insurance:** Maintain appropriate liability insurance coverage

**Owner:** Legal Team + Product Lead  
**Status:** Active monitoring

---

### SR-2: Auth Migration Complexity

**Category:** Technical  
**Likelihood:** High  
**Impact:** High  
**Priority:** ?? Critical

**Description:**
Migrating from Azure B2C to Entra External ID (or refactoring within B2C) is complex and could cause authentication failures, data loss, or extended downtime.

**Affected Projects:**
- Project 1 (Auth Revamp)
- Project 10 (Community Pages - depends on SSO)
- Project 23 (Parental Consent)

**Mitigations:**
1. **Rollback Plans:** Maintain hot rollback capability for at least 30 days
2. **Load Testing:** Simulate 10× current traffic before production deployment
3. **Shadow Deployment:** Run new auth system in parallel without routing traffic
4. **Expert Assistance:** Contract Microsoft SME or experienced consultant
5. **User Communication:** In-app notifications, help center updates, support team training

**Owner:** Security Engineer + Engineering Lead  
**Status:** Planning phase (Q1 2026)

---

### SR-3: Volunteer Capacity & Turnover

**Category:** Organizational  
**Likelihood:** Very High  
**Impact:** High  
**Priority:** ?? Critical

**Description:**
Volunteer developers have limited availability and may leave mid-project due to personal commitments, leading to knowledge loss and project delays.

**Affected Projects:** All

**Mitigations:**
1. **Modular Work:** Chunk projects into 3-6 month deliverables that can be handed off
2. **Documentation:** Require comprehensive documentation for all projects
3. **Pair Programming:** Encourage knowledge sharing through pairing
4. **Handover Checklists:** Standardized transition process when volunteers leave
5. **Maintainer Rotation:** Distribute critical system knowledge across multiple people
6. **Recruiting Pipeline:** Continuous recruiting to backfill attrition
7. **Alumni Network:** Keep in touch with past contributors who may return

**Owner:** Product Lead + Community Manager  
**Status:** Ongoing process

---

### SR-4: Mobile Quality Debt

**Category:** Technical  
**Likelihood:** High  
**Impact:** High  
**Priority:** ?? Critical

**Description:**
Current mobile app has stability issues, outdated toolchain, and insufficient error handling, leading to poor app store ratings and user churn.

**Affected Projects:**
- Project 4 (Mobile Robustness) - **Primary mitigation**
- All mobile features in other projects

**Mitigations:**
1. **Q1 Stabilization:** Prioritize Project 4 before adding new mobile features
2. **.NET/MAUI Upgrades:** Upgrade to .NET 10 and latest MAUI stable version
3. **Instrumentation:** Add Sentry.io for crash reporting and performance monitoring
4. **Testing:** Increase unit test coverage to ? 60%
5. **Device Matrix:** Test on physical devices covering iOS 15+ and Android 8.0+
6. **Staged Rollouts:** Use TestFlight and Google Play Beta before full release

**Owner:** Mobile Product Lead + MAUI Developers  
**Status:** Active mitigation (Q1 2026)

---

### SR-5: Cost Spikes (Azure, Maps, Email)

**Category:** Financial  
**Likelihood:** Medium  
**Impact:** High  
**Priority:** ?? High

**Description:**
Rapid growth or inefficient usage could cause unexpected spikes in Azure, Google Maps API, or SendGrid costs, straining the nonprofit budget.

**Affected Projects:** All (infrastructure)

**Mitigations:**
1. **Cost Reviews:** Monthly review of Azure, Maps, and SendGrid usage
2. **Caching:** Implement aggressive caching for Maps API calls
3. **Quotas:** Set up billing alerts and spending caps
4. **Batching:** Batch email sends to minimize SendGrid costs
5. **Docker Migration:** Move to Docker/ACA to reduce Azure App Service costs
6. **Query Optimization:** Review and optimize expensive database queries
7. **CDN Usage:** Leverage CDN for static assets to reduce bandwidth costs

**Owner:** Engineering Lead + Finance  
**Status:** Monthly monitoring

---

## Project-Specific Risks

### PR-1: Two API Versions Increase Complexity

**Category:** Technical  
**Likelihood:** High  
**Impact:** Medium  
**Priority:** ?? High

**Affected Project:** Project 24 (API v2 Modernization)

**Mitigations:**
- Shared business logic layer for v1 and v2
- Automated deprecation warnings in v1 responses
- Feature flags to control rollout pace
- Comprehensive documentation for migration

**Owner:** Engineering Team  
**Timeline:** Q2-Q4 2026

---

### PR-2: Client Generation Edge Cases

**Category:** Technical  
**Likelihood:** Medium  
**Impact:** Medium  
**Priority:** ?? Medium

**Affected Project:** Project 24 (API v2 Modernization)

**Mitigations:**
- Manual client overrides where auto-generation fails
- Comprehensive testing of generated clients
- Fallback patterns for unsupported scenarios
- Monitor and iterate on generation templates

**Owner:** Engineering Team  
**Timeline:** Q2-Q4 2026

---

### PR-3: Team Learning Curves for New Patterns

**Category:** Organizational  
**Likelihood:** High  
**Impact:** Low  
**Priority:** ?? Medium

**Affected Projects:** Multiple (API v2, CMS, etc.)

**Mitigations:**
- Comprehensive documentation with examples
- Pair programming sessions
- Code reviews with feedback
- Weekly office hours for questions
- Create video tutorials for complex topics

**Owner:** Tech Leads  
**Timeline:** Ongoing

---

### PR-4: MAUI Upgrade Breaks Existing Features

**Category:** Technical  
**Likelihood:** Medium  
**Impact:** High  
**Priority:** ?? High

**Affected Project:** Project 4 (Mobile Robustness)

**Mitigations:**
- Thorough regression testing on device matrix
- Maintain .NET 9 fallback branch
- Staged rollout with monitoring
- Beta testing program with diverse devices

**Owner:** Mobile Team  
**Timeline:** Q1 2026

---

### PR-5: Privo.com Integration Issues

**Category:** Technical  
**Likelihood:** Low  
**Impact:** High  
**Priority:** ?? Medium

**Affected Projects:** Project 1 (Auth), Project 23 (Minors)

**Mitigations:**
- Early integration testing in Q1
- Fallback to manual verification if Privo unavailable
- Clear error messages for users
- Support escalation path with Privo

**Owner:** Security Engineer  
**Timeline:** Q1-Q2 2026

---

### PR-6: App Store Approval Delays

**Category:** External  
**Likelihood:** Medium  
**Impact:** Medium  
**Priority:** ?? Medium

**Affected Projects:** Project 1 (Auth), Project 4 (Mobile)

**Mitigations:**
- Submit updates early (allow 2-week buffer)
- Follow Apple and Google guidelines strictly
- Have rollback plan for rejected updates
- Maintain communication with app store reviewers

**Owner:** Mobile Team  
**Timeline:** Ongoing

---

### PR-7: CMS Adoption Resistance

**Category:** Organizational  
**Likelihood:** Medium  
**Impact:** Low  
**Priority:** ?? Low

**Affected Project:** Project 16 (Content Management)

**Mitigations:**
- Training sessions for content editors
- User-friendly Strapi interface
- Clear documentation and video tutorials
- Gradual rollout starting with simple content
- Provide support during transition

**Owner:** Product Team  
**Timeline:** Q2 2026

---

## Operational Risks

### OR-1: Data Loss During Migration

**Category:** Technical  
**Likelihood:** Low  
**Impact:** Critical  
**Priority:** ?? High

**Affected Projects:** Project 1 (Auth), Project 5 (Pipelines)

**Mitigations:**
- Comprehensive backups before any migration
- Dry-run migrations in staging environment
- Validation scripts to verify data integrity
- Point-in-time restore capability
- Rollback plan within 24 hours

**Owner:** Engineering Lead  
**Status:** Standard procedure

---

### OR-2: Security Vulnerabilities

**Category:** Security  
**Likelihood:** Medium  
**Impact:** Critical  
**Priority:** ?? Critical

**Affected Projects:** All

**Mitigations:**
- Regular dependency updates (monthly)
- Automated vulnerability scanning (Dependabot)
- Security audits before major releases
- Penetration testing (annual)
- Bug bounty program (future consideration)
- Incident response plan

**Owner:** Security Engineer  
**Status:** Ongoing

---

### OR-3: Insufficient Testing Coverage

**Category:** Quality  
**Likelihood:** High  
**Impact:** Medium  
**Priority:** ?? High

**Affected Projects:** All

**Mitigations:**
- Project 25 (Automated Testing) to improve coverage
- Require tests for all new features
- Code coverage targets (? 60% for critical paths)
- Manual test scenarios documented
- Beta testing programs

**Owner:** Engineering Team  
**Timeline:** Ongoing + Project 25 (TBD)

---

## Monitoring & Review

### Monthly Risk Reviews
- **When:** First Monday of each month
- **Participants:** Product Lead, Engineering Lead, Security Engineer
- **Agenda:**
  1. Review existing risk statuses
  2. Identify new risks from current projects
  3. Update mitigation effectiveness
  4. Escalate critical risks to board if needed

### Quarterly Risk Assessment
- **When:** End of each quarter
- **Participants:** Full leadership team + board representative
- **Agenda:**
  1. Comprehensive risk landscape review
  2. Adjust priorities based on strategic changes
  3. Budget allocation for risk mitigation
  4. Lessons learned from materialized risks

### Incident Response
- **Critical Risk Materialization:** Immediate escalation to leadership
- **Post-Mortem:** Within 48 hours of resolution
- **Action Items:** Tracked in GitHub Issues
- **Documentation:** Update this document and project-specific risk sections

---

## Risk Dashboard (Updated Monthly)

| Risk ID | Risk | Priority | Status | Last Review |
|---------|------|----------|--------|-------------|
| SR-1 | Legal & Compliance | ?? Critical | Active | 2026-01-24 |
| SR-2 | Auth Migration | ?? Critical | Planning | 2026-01-24 |
| SR-3 | Volunteer Turnover | ?? Critical | Ongoing | 2026-01-24 |
| SR-4 | Mobile Quality | ?? Critical | Mitigating | 2026-01-24 |
| SR-5 | Cost Spikes | ?? High | Monitoring | 2026-01-24 |
| PR-1 | Two API Versions | ?? High | Future | 2026-01-24 |
| PR-2 | Client Generation | ?? Medium | Future | 2026-01-24 |
| PR-3 | Learning Curves | ?? Medium | Ongoing | 2026-01-24 |
| PR-4 | MAUI Upgrade | ?? High | Q1 2026 | 2026-01-24 |
| PR-5 | Privo Integration | ?? Medium | Q1-Q2 | 2026-01-24 |
| PR-6 | App Store Delays | ?? Medium | Ongoing | 2026-01-24 |
| PR-7 | CMS Adoption | ?? Low | Q2 2026 | 2026-01-24 |
| OR-1 | Data Loss | ?? High | Standard | 2026-01-24 |
| OR-2 | Security Vulns | ?? Critical | Ongoing | 2026-01-24 |
| OR-3 | Test Coverage | ?? High | Improving | 2026-01-24 |

---

## Related Documents

- **[Executive Summary](../Executive_Summary.md)** - Strategic overview with risk summary
- **[Project Files](../Projects/)** - Individual project risk sections
- **[Incident Response Plan]** - (To be created)
- **[Security Audit Reports]** - (Annual, restricted access)

---

**Last Updated:** January 24, 2026  
**Next Review:** February 5, 2026  
**Owner:** Director of Product & Engineering
