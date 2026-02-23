# Data Processing Inventory (GDPR Article 30)

**Organization:** TrashMob.eco
**Legal Entity Type:** Non-profit 501(c)(3) corporation
**Jurisdiction:** Washington State, USA
**Role:** Data Controller
**Contact:** info@trashmob.eco
**Document Version:** 1.0
**Effective Date:** February 23, 2026
**Last Reviewed:** February 23, 2026

---

## Table of Contents

1. [Purpose](#1-purpose)
2. [Personal Data Categories](#2-personal-data-categories)
3. [Third-Party Processors](#3-third-party-processors)
4. [User Rights Implementation](#4-user-rights-implementation)
5. [Data Protection Measures](#5-data-protection-measures)
6. [Deletion Behavior Summary](#6-deletion-behavior-summary)
7. [International Transfers](#7-international-transfers)
8. [Retention Schedule Summary](#8-retention-schedule-summary)
9. [Change Log](#9-change-log)

---

## 1. Purpose

This document constitutes the Record of Processing Activities maintained by TrashMob.eco in accordance with Article 30 of the General Data Protection Regulation (EU) 2016/679. TrashMob.eco is a non-profit environmental cleanup coordination platform that connects volunteers with community litter cleanup events.

Although TrashMob.eco is a U.S.-based organization, this inventory is maintained as a matter of best practice and to support transparency obligations where the platform may process data of individuals located in the European Economic Area or other jurisdictions with comparable data protection requirements.

---

## 2. Personal Data Categories

### 2.1 Identity Data

| Data Element | Source | Lawful Basis | Retention | Access Scope |
|---|---|---|---|---|
| Given name, Surname | User registration | Legitimate interest (event coordination) | While account active; deleted on account deletion | User, Event Leads (for events attended), Site Admins |
| Email address | Registration / Identity Provider | Legitimate interest (communication) | Deleted on account deletion | User, Site Admins |
| Username | User-chosen at registration | Legitimate interest (public display) | Deleted on account deletion | Public |
| Date of birth | Registration | Legal obligation (age verification, COPPA compliance) | Deleted on account deletion | System only |
| Profile photo | User upload or Identity Provider | Consent | Blob storage deleted on account deletion | Public |
| ObjectId / NameIdentifier | Identity Provider (Entra ID) | Contract (authentication) | Deleted on account deletion | System only |

### 2.2 Location Data

| Data Element | Source | Lawful Basis | Retention | Access Scope |
|---|---|---|---|---|
| City, Region, Country, PostalCode | User-provided | Legitimate interest (local event matching) | Deleted on account deletion | User, Site Admins |
| Latitude, Longitude (profile) | User-provided / geocoded | Legitimate interest (proximity-based notifications) | Deleted on account deletion | System only |
| GPS route data (EventAttendeeRoute.UserPath) | Mobile app, captured during events | Consent (user initiates tracking) | Anonymized on account deletion; geometry preserved for aggregate heatmaps | System only during active use; anonymized aggregate post-deletion |

### 2.3 Activity Data

| Data Element | Source | Lawful Basis | Retention | Access Scope |
|---|---|---|---|---|
| Event attendance records | User registration for events | Contract performance | Deleted on account deletion | User, Event Leads, Site Admins |
| Cleanup metrics (bags, weight, duration, attendee count) | User / Event Lead submission | Legitimate interest (impact measurement) | Anonymized on account deletion; preserved for aggregate reporting | Event participants, Site Admins; anonymized aggregates are public |
| Litter reports and photos | User submission | Legitimate interest (community reporting) | Audit fields anonymized on account deletion; report content preserved | Public (report), User and Site Admins (audit fields) |
| Event photos | User upload | Consent | Upload reference anonymized on account deletion; photo file preserved | Public |

### 2.4 Legal Records

| Data Element | Source | Lawful Basis | Retention | Access Scope |
|---|---|---|---|---|
| Waiver signatures (TypedLegalName, IP address, UserAgent) | E-signature or paper upload | Legal obligation (liability protection) | Anonymized on account deletion; waiver record retained for 7 years from event date | User (own waivers), Site Admins |
| Guardian information (minor waivers) | Guardian input | Legal obligation (COPPA compliance, parental consent) | Same as waiver retention: anonymized on deletion, retained 7 years from event date | Guardian, Site Admins |

### 2.5 Communication Data

| Data Element | Source | Lawful Basis | Retention | Access Scope |
|---|---|---|---|---|
| Newsletter preferences | User settings | Consent | Deleted on account deletion | User, Site Admins |
| Notification history | System-generated | Legitimate interest (service delivery) | Deleted on account deletion | User, Site Admins |
| User feedback | User submission | Legitimate interest (product improvement) | UserId nullified on account deletion; feedback text preserved | Site Admins |

### 2.6 Social and Organizational Data

| Data Element | Source | Lawful Basis | Retention | Access Scope |
|---|---|---|---|---|
| Team memberships | User action | Contract performance | Deleted on account deletion; team lead role transferred first | Team members, Site Admins |
| Partner admin roles | Assigned by partner organization | Legitimate interest (partner management) | Deleted on account deletion | Partner organization members, Site Admins |
| Achievements and badges | System-calculated | Legitimate interest (engagement) | Deleted on account deletion | Public (if user opts in), User, Site Admins |

### 2.7 Technical and Diagnostic Data

| Data Element | Source | Lawful Basis | Retention | Access Scope |
|---|---|---|---|---|
| Crash logs | Mobile app via Sentry.io | Legitimate interest (application stability) | 90-day retention | Engineering team |
| Application telemetry | Web app via Azure Application Insights | Legitimate interest (performance monitoring) | 90-day retention | Engineering team |
| Server logs | Web server infrastructure | Legitimate interest (security, incident response) | 90-day Azure default retention | Engineering team |

---

## 3. Third-Party Processors

All third-party processors have been evaluated for adequate data protection practices. Data Processing Agreements (DPAs) are in place with each processor.

| Processor | Data Processed | Processing Purpose | DPA Status | Privacy Information |
|---|---|---|---|---|
| Microsoft Azure (SQL, Blob Storage, Maps, Application Insights) | All stored personal data, telemetry | Hosting, data storage, geocoding, application telemetry | Microsoft DPA in effect | https://privacy.microsoft.com |
| SendGrid (Twilio) | Email addresses | Transactional and notification email delivery | Twilio DPA in effect | https://www.twilio.com/legal/privacy |
| Sentry.io | Crash logs, device information | Mobile application error tracking and diagnostics | Sentry DPA in effect | https://sentry.io/privacy/ |
| Microsoft Entra ID | Authentication tokens, user identifiers | User authentication (OAuth 2.0 / OIDC) | Microsoft DPA in effect | https://privacy.microsoft.com |
| Google Maps Platform (Android) | Location coordinates | Map display and rendering on mobile | Google DPA in effect | https://policies.google.com/privacy |

---

## 4. User Rights Implementation

TrashMob.eco provides the following mechanisms for users to exercise their data protection rights:

| Right | Implementation | Mechanism |
|---|---|---|
| Right to Access | Download My Data | `GET /api/users/{id}/export` (JSON export of all personal data) |
| Right to Rectification | Edit Profile | `PUT /api/users/{id}` (user self-service via profile settings) |
| Right to Erasure | Delete My Data | `DELETE /api/users/{id}` via `UserDeletionService` (multi-phase deletion process) |
| Right to Data Portability | Download My Data | `GET /api/users/{id}/export` (machine-readable JSON format) |
| Right to Object | Unsubscribe / Opt-out | Unsubscribe from emails via user settings; leaderboard opt-out available |
| Right to Restrict Processing | Manual request | Contact info@trashmob.eco for manual processing restriction |

**Rate Limiting:** Data export requests are rate-limited to one request per 24-hour period per user to prevent abuse.

**Response Time:** All automated rights requests are fulfilled immediately. Manual restriction requests are processed within 30 days.

---

## 5. Data Protection Measures

### 5.1 Technical Measures

| Measure | Implementation |
|---|---|
| Encryption at rest | Azure SQL Transparent Data Encryption (TDE); Azure Blob Storage encryption |
| Encryption in transit | TLS 1.2+ enforced on all connections |
| Authentication | OAuth 2.0 / OpenID Connect via Microsoft Entra ID |
| Authorization | Role-Based Access Control (RBAC) at application and infrastructure levels |
| Secrets management | Azure Key Vault with RBAC authorization model |
| Photo moderation | Automated content moderation pipeline for user-generated images |
| API rate limiting | Rate limiting on sensitive endpoints including data export (1 request per 24 hours) |
| Deletion integrity | Transaction-wrapped multi-phase deletion via `UserDeletionService` |
| Backup encryption | Azure SQL automated backups encrypted at rest |

### 5.2 Organizational Measures

| Measure | Description |
|---|---|
| Data minimization | Minimal PII collection; only data necessary for platform operation is requested |
| No data monetization | Personal data is never sold, licensed, or shared for advertising purposes |
| No ad tracking | No third-party advertising trackers or analytics beyond operational telemetry |
| Open-source transparency | Codebase is publicly available for inspection on GitHub |
| Minor privacy protections | Privacy protections for users aged 13-17 are enabled by default (COPPA compliance) |
| Access control | Role-based access limits data visibility to authorized personnel only |

---

## 6. Deletion Behavior Summary

When a user requests account deletion, the `UserDeletionService` executes the following phases in order within a database transaction:

| Phase | Action | Affected Data |
|---|---|---|
| **Phase A** | Full delete | Event attendee records, notifications, IFTTT triggers, professional company user records, partner admin roles |
| **Phase B** | Anonymize (UserId replaced with `Guid.Empty`) | Route data, cleanup metrics, photo moderation flags, moderation logs, email invite batches |
| **Phase C** | Anonymize photo references | Event photo upload/moderation references, partner photo references, team photo references |
| **Phase D** | Nullify UserId | Feedback records, email invitation records, team adoption records, litter image records |
| **Phase E** | Anonymize waiver link | User association removed from waiver records; waiver content retained per legal obligation (7-year retention) |
| **Phase F** | Anonymize audit fields | `CreatedByUserId` and `LastUpdatedByUserId` fields across 40+ entity tables |
| **Phase G** | Transfer team leadership | Team lead role transferred to longest-tenured active team member |
| **Phase H** | Delete user record | Profile photo blob deleted from storage; user database row permanently removed |

**Post-Deletion State:** After Phase H completes, no personal data remains that can identify the deleted user. Anonymized records (routes, metrics, reports) are retained for aggregate community impact reporting with no link to the original user.

---

## 7. International Transfers

TrashMob.eco infrastructure is hosted in the **Microsoft Azure West US 2 region** (Washington State, USA). Personal data is primarily stored and processed within the United States.

| Transfer Scenario | Destination | Safeguard |
|---|---|---|
| Azure services | USA (West US 2) | Microsoft DPA with Standard Contractual Clauses |
| SendGrid email delivery | USA | Twilio DPA with Standard Contractual Clauses |
| Sentry.io error tracking | USA | Sentry DPA with Standard Contractual Clauses |
| Google Maps API | USA | Google DPA with Standard Contractual Clauses |

All processors maintain Standard Contractual Clauses (SCCs) as approved by the European Commission for transfers of personal data to third countries.

---

## 8. Retention Schedule Summary

| Data Category | Active Retention | Post-Deletion Treatment |
|---|---|---|
| Identity data (name, email, DOB, photo, auth IDs) | While account is active | Permanently deleted |
| Location data (city, region, coordinates) | While account is active | Permanently deleted |
| GPS route data | While account is active | Anonymized; geometry preserved for heatmaps |
| Event attendance | While account is active | Permanently deleted |
| Cleanup metrics | While account is active | Anonymized; aggregates preserved |
| Litter reports and photos | While account is active | Audit fields anonymized; content preserved |
| Event photos | While account is active | Upload references anonymized; photos preserved |
| Waiver signatures | While account is active + 7 years from event date | Anonymized; record retained for legal obligation |
| Guardian information | While account is active + 7 years from event date | Anonymized; record retained for legal obligation |
| Communication preferences | While account is active | Permanently deleted |
| Notification history | While account is active | Permanently deleted |
| User feedback | While account is active | UserId nullified; text preserved |
| Team memberships | While account is active | Permanently deleted (lead transferred first) |
| Partner admin roles | While account is active | Permanently deleted |
| Achievements | While account is active | Permanently deleted |
| Crash logs (Sentry) | 90 days | Automatically purged |
| Application telemetry (App Insights) | 90 days | Automatically purged |
| Server logs | 90 days | Automatically purged |

---

## 9. Change Log

| Date | Version | Author | Description |
|---|---|---|---|
| 2026-02-23 | 1.0 | TrashMob.eco | Initial creation of GDPR Article 30 Data Processing Inventory |
