# TrashMob.eco � 2026 Executive Summary

**Version:** 1.0  
**Date:** January 24, 2026  
**Owner:** Director of Product & Engineering

---

## Purpose

Enable sustained growth of TrashMob.eco by strengthening onboarding & safety (minors and waivers), launching community & team features that cities will subscribe to, and raising product quality across mobile, web, and infrastructure.

---

## Strategic Objectives

| # | Objective | Description |
|---|-----------|-------------|
| 1 | **Trust & Safety** | Enable minors participation safely and legally; modernize auth/waivers |
| 2 | **City Value** | Deliver Community Pages and Adopt-A-Location as subscription-ready modules |
| 3 | **Volunteer Network Effects** | Launch Teams, make participation visible (metrics, photos, routes), and drive engagement (light gamification) |
| 4 | **Quality & Velocity** | Stabilize mobile, upgrade stacks, streamline CI/CD, and instrument end-to-end telemetry |
| 5 | **Monetization Foundations** | Introduce tasteful sponsorship on home page and simple manual billing |

---

## Key 2026 Feature Themes

### Onboarding & Safety
- Minors (13+) with parental consent (Privo.com)
- SSO for partner communities
- Flexible waivers (TrashMob + community-specific)
- Smoother auth (B2C ? Entra External ID migration)

### Community & Teams
- Branded community pages
- Adopt-a-location programs
- Team creation and management
- Leaderboards and gamification
- Content management system (Strapi)

### Impact Metrics
- Event weights tracking
- Attendee-level metrics
- Map route tracing with decay
- Before/after photo documentation

### Platform Improvements
- Complete litter reporting on web
- API v2 with pagination and auto-generated clients
- Advertising/sponsorship surfacing
- Newsletter support
- In-app messaging (with guardrails)
- Improved social media integration
- IFTTT completion

### Technical & Operations
- Mobile stabilization (MAUI)
- .NET 10 upgrades across all projects
- Docker hosting and containerization
- Enhanced CI/CD pipelines
- Automated UI/E2E testing
- Cost optimization and accessibility reviews
- Better error handling and observability (Sentry.io)

---

## 2026 Roadmap Overview

**Note:** TrashMob operates with an agile, volunteer-driven approach. Projects are organized by priority rather than fixed timelines, and volunteers pick up work as they have availability.

### Critical Priority (Infrastructure & Blockers)
- **Project 4:** Mobile Robustness (launch readiness + error handling)
- **Project 5:** Pipelines & Infrastructure (CI/CD, containerization)
- **Project 1:** Auth Revamp (Entra External ID migration + minors support)

### High Priority (Core Features)
- **Project 1:** Auth Revamp implementation
- **Project 3:** Litter Reporting on Web (feature complete)
- **Project 7:** Event Weights
- **Project 8:** Waivers V3 (with legal gating)
- **Project 9:** Teams (MVP)
- **Project 10:** Community Pages (MVP)
- **Project 13:** Bulk Email Invites
- **Project 16:** Page Content Management (CMS)
- **Project 23:** Parental Consent (Privo.com)
- **Project 24:** API v2 Modernization

### Medium Priority (Enhancements)
- **Project 2:** Home Page Improvements
- **Project 6:** Backend Standards
- **Project 11:** Adopt-A-Location
- **Project 14:** Social Media Integration
- **Project 15:** Route Tracing
- **Project 19:** Newsletter Support
- **Project 20:** Gamification (leaderboards)
- **Project 21:** Event Co-Leads
- **Project 22:** Attendee-Level Metrics
- **Project 25:** Automated Testing

### Low Priority (Nice-to-Have)
- **Project 12:** In-App Messaging (gated)
- **Project 17:** MCP Server (AI)
- **Project 18:** Before/After Photos

> **Note:** Specific completion dates will emerge based on volunteer availability and project dependencies.

---

## Success Metrics

### North Star Metric
**Total Bags of Litter Collected** � ultimate measure of environmental impact

### Key Performance Indicators (KPIs)

#### Engagement Metrics
- **Monthly Active Users (MAU):** Target 50,000 by end of 2026
- **Event Registrations per Month:** Target 10,000
- **New User Signups:** Target 5,000/month
- **Retention Rate:** Target 60% (return within 30 days)

#### Impact Metrics
- **Events Created:** Target 500/month
- **Events Completed:** Target 90% completion rate
- **Avg Attendees per Event:** Target 10
- **Total Bags Collected:** Target 100,000 bags in 2026
- **Total Weight Collected:** Target 50,000 lbs in 2026
- **Total Hours Volunteered:** Target 200,000 hours in 2026

#### Quality Metrics
- **Events with Summaries:** Target 95%
- **Waiver Compliance:** Target 100%
- **Partner Satisfaction:** Target 4.5/5
- **User Satisfaction:** Target 4.3/5

#### Technical Metrics
- **API Uptime:** Target 99.9%
- **P95 API Latency:** Target ? 300ms (v1), ? 200ms (v2)
- **Mobile Crash-Free Rate:** Target ? 99.5%
- **Error Rate:** Target ? 0.5%
- **Lead Time for Change:** ? 30%
- **Deployment Frequency:** ? 2�
- **Change Failure Rate:** ? 10%

---

## Revenue Approach

**Primary Revenue Streams (2026):**
- Grants and donations
- Paid community subscriptions (Community Pages, Adopt-A-Location programs)
- Sponsorship and tasteful advertising

**Billing Infrastructure:**
- Manual operations via QuickBooks for 2026
- Automated billing deferred to 2027

---

## Resourcing Plan

### Target Team Structure

| Role | Count |
|------|-------|
| Mobile App Product Lead | 1 |
| Web Site Product Lead | 1 |
| UX Designers | 2�3 |
| .NET MAUI Mobile Devs | 2�3 |
| Web Devs (React/TypeScript) | 2�3 |

**Note:** Build/deployment and security engineering tasks are handled via AI-assisted development tooling.

### Volunteer Dynamics
- Chunk work into **3�6 month deliverables**
- Ongoing recruiting to backfill attrition
- Handover documentation for continuity
- Modular project structure to enable parallel work

---

## Major Risks & Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| **Legal/Compliance** (COPPA, waivers) | Very High | Legal sign-off gates before build; Privo.com partnership |
| **Auth Migration Complexity** | High | Canary rollouts, rollback playbooks, extensive testing |
| **Volunteer Capacity/Turnover** | High | Modular backlog, handover checklists, maintainer rotation |
| **Mobile Quality Debt** | Medium | Q1 stabilization priority, .NET/MAUI upgrades, instrumentation |
| **Cost Spikes** (Azure/Maps/Email) | Medium | Cost reviews, caching strategies, quotas, batching |
| **Two API Versions** | Medium | Shared business logic, automated deprecation, 12-month transition |
| **Team Learning Curves** | Low | Documentation, examples, pair programming sessions |

---

## Technology Stack

### Backend
- **.NET 10** (ASP.NET Core Web API)
- **Entity Framework Core** for data access
- **Azure SQL Database** for persistence
- **Azure App Service** ? transitioning to **Docker/Azure Container Apps**
- **Azure B2C** ? migrating to **Entra External ID** (Q2 2026)
- **SendGrid** for email notifications
- **Google Maps API** for location services
- **Strapi** for content management

### Frontend
- **React 18** (TypeScript) for web application
- **Vite** as build tool
- **TanStack Query** for server state
- **Tailwind CSS** + **Radix UI** for styling/components
- **.NET MAUI** for cross-platform mobile apps (iOS/Android)

### Infrastructure & DevOps
- **GitHub Actions** for CI/CD
- **Docker** containerization
- **Sentry.io** for error tracking and APM
- **Azure Monitor** for observability
- **Azure Blob Storage** for file storage
- **Azure Key Vault** for secrets management

---

## Cross-Cutting Initiatives

### Accessibility
- Commit to **WCAG 2.2 AA** compliance
- Full audit and remediation plan in 2026
- Screen reader support, keyboard navigation, color contrast

### Privacy & Compliance
- COPPA compliance for minors (13+)
- Parental consent workflows (Privo.com)
- PII protection and data retention policies
- Waiver legal requirements

### Observability & Reliability
- Front-to-back error handling
- Business event logging
- SLOs, alerting, and dashboards
- Correlation IDs for distributed tracing

### Cost Optimization
- Docker-based hosting to reduce Azure costs
- Database migration optimization (init containers)
- Maps API caching and quota management
- Email batching and category management

---

## Dependencies Between Projects

**Critical Path:**
1. **Project 1 (Auth Revamp)** ? Unblocks Project 10 (Community Pages), Project 23 (Minors)
2. **Project 4 (Mobile Robustness)** ? Prerequisite for mobile features in Projects 9, 15, 18, 22
3. **Project 5 (CI/CD)** ? Enables all other projects with reliable deployments
4. **Project 24 (API v2)** ? Accelerates Projects 3, 9, 10, 11 via auto-generated clients
5. **Project 16 (CMS)** ? Enables dynamic content for Projects 2, 10

---

## Project Prioritization Matrix

| Priority | Projects |
|----------|----------|
| **Critical** | 1 (Auth), 4 (Mobile), 5 (CI/CD) |
| **High** | 3 (Litter Web), 7 (Weights), 8 (Waivers), 9 (Teams), 10 (Communities), 13 (Bulk Email), 23 (Minors), 24 (API v2) |
| **Medium** | 2 (Home Page), 6 (Backend Standards), 11 (Adopt-A), 14 (Social), 15 (Routes), 16 (CMS), 19 (Newsletter), 20 (Gamification), 21 (Co-Leads), 22 (Attendee Metrics), 25 (Testing) |
| **Low** | 12 (Messaging), 17 (MCP), 18 (Photos) |

---

## Open Strategic Questions

1. **Entra External ID vs Azure B2C:** Final decision pending feature gap analysis and cost comparison (Q1)
2. **GraphQL adoption:** Evaluate after API v2 stabilizes (Q4)
3. **API Gateway/BFF pattern:** Defer to 2027 unless microservices needed
4. **Mobile app store launch readiness:** Dependent on Project 4 completion
5. **Monetization billing automation:** Manual for 2026, automated system in 2027

---

## Communication & Governance

### Quarterly Planning
- Review roadmap and adjust priorities
- Finalize success metrics targets
- Resource allocation and recruitment
- Risk assessment updates

### Monthly Reviews
- Project status updates
- Blocker escalation
- Volunteer onboarding/offboarding
- Metrics review

### Weekly Standups
- Active project check-ins
- Cross-team dependencies
- Quick decision-making

---

## Document Structure

This executive summary is part of a larger planning documentation set:

- **[Navigation & Project Index](./README.md)** - Quick access to all project docs
- **[Roadmap & Resourcing](./Roadmap_and_Resourcing.md)** - Detailed quarterly plan and team structure
- **[Operations & Standards](./Operations_and_Standards.md)** - Metrics, privacy, accessibility guidelines
- **[Risks & Mitigations](./Risks_and_Mitigations.md)** - Comprehensive risk management
- **[Individual Project Files](./Projects/)** - Detailed specs for each of 25 projects

---

**For Questions or Updates:**  
Contact the Director of Product & Engineering or submit issues via GitHub.

---

**Last Updated:** January 24, 2026  
**Next Review:** End of Q1 2026 (March 31, 2026)
