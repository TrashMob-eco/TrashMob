# Project 24 - API v2 Modernization

| Attribute | Value |
|-----------|-------|
| **Status** | In Progress (Phases 1, 2a–2i complete — 72+ v2 controllers, 100+ DTOs, 72+ test suites; Phases 3, 4 remaining) |
| **Priority** | High |
| **Risk** | Medium |
| **Size** | Very Large |
| **Dependencies** | None (v1 endpoints remain untouched throughout) |

---

## Business Rationale

Create a modern, scalable, and developer-friendly API layer (v2) that improves reliability, debuggability, and reduces manual code maintenance. Current v1 APIs lack pagination, consistent error handling, and return raw entities. V2 endpoints provide lean DTOs, server-side pagination/filtering, and standardized error responses.

---

## Prior Work & Current State

Before starting, it's important to recognize what already exists. These items were completed through other projects (primarily Project 6 and ongoing infrastructure work) and do **not** need to be repeated:

| Already Done | Where | Notes |
|---|---|---|
| Response compression (Brotli + Gzip) | `Program.cs` | `CompressionLevel.Fastest`, HTTPS enabled |
| XML documentation on all public APIs | All `.csproj` files | `<GenerateDocumentationFile>true</GenerateDocumentationFile>` |
| Swagger/OpenAPI v1 configuration | `Program.cs` | Single doc, Bearer auth, XML comments included |
| OpenTelemetry (traces, metrics, logs) | `Program.cs` | Already configured for Application Insights |
| 7 authorization policies | `Program.cs` | ValidUser, UserOwnsEntity, UserIsAdmin, etc. |
| ServiceResult pattern defined | `TrashMob.Shared/Poco/ServiceResult.cs` | Exists but not used in controllers yet |
| 59 Poco/DTO files | `TrashMob.Models/Poco/` | Stats, DisplayEvent, filters, requests, etc. |
| Entity-to-DTO extension methods | `PocoExtensions.cs`, `LitterReportExtensions.cs` | `ToDisplayEvent()`, `ToDisplayUser()`, `ToFullLitterReport()` |
| Primary constructors on all classes | Project 6 Phase 2.5 | Controllers, managers, repositories |
| Structured logging | Project 6 Phase 2.5 | Message templates, not string interpolation |
| Health checks (SQL Server) | `Program.cs` | Basic database health check configured |
| Rate limiting thresholds decided | Project 6 | Public: 100/min, Auth: 300/min, Admin: 600/min |

### Current DTO Coverage Gaps

Controllers currently return **raw entities** inconsistently:

| Controller | Returns Entities Directly | Returns DTOs |
|---|---|---|
| EventsController | `Event` (Get, Add, Update, Delete) | `DisplayEvent` (Active, Completed, NotCanceled) |
| UsersController | `User` (all endpoints) | `UserImpactStats` (impact only) |
| CommunitiesController | `Partner` (list, get) | `CommunityDashboard`, `CommunityPublicStats` |
| TeamsController | `Team` (all endpoints) | None |
| StatsController | None | `Stats` (all endpoints) |
| LitterReportsController | `LitterReport` (CRUD) | `FullLitterReport` (display) |

This means entities with navigation properties, audit fields, and internal IDs are crossing the wire to clients.

### PR #2490 (Closed Without Merge)

A community contributor (Francisco Loureiro) submitted PR #2490 "Add swagger v2" which attempted API versioning and an optimized stats endpoint. It was closed because:
- Used deprecated `Microsoft.AspNetCore.Mvc.Versioning` package (should use `Asp.Versioning.Mvc`)
- SQL logic bugs in raw query (`PickedWeight` vs `PickedWeightUnitId`)
- Generated C# code pasted into a `.ts` file (wrong code generator)
- Large formatting-only noise in MobDbContext.cs
- Debug logger left in MobDbContext

The goals were valid and are fully covered by this project.

---

## Objectives

### Core Improvements
- Implement pagination on all collection endpoints (offset-based default)
- Standardized error responses (RFC 9457 Problem Details)
- Comprehensive OpenAPI 3.1 documentation with v1 + v2 docs
- Correlation IDs for distributed tracing
- Server-side filtering to reduce network traffic (especially mobile)

### Modernization Strategies
- API versioning via URL path (`/api/v2/events`)
- ETags for conditional requests and caching
- Rate limiting with token buckets (thresholds already decided)
- Consistent DTO layer — no entities cross the wire in v2
- Standard query parameter filtering per endpoint
- Sorting query parameters (`?sort=-date,name`)

---

## Scope

### Phase 1 - Foundation ✅

Infrastructure that all v2 endpoints will use. No v1 endpoints are modified.

- [x] **API versioning** - `Asp.Versioning.Mvc` + `Asp.Versioning.Mvc.ApiExplorer` installed; default v1; `UrlSegmentApiVersionReader`; v2 Swagger doc alongside v1 (`Program.cs`)
- [x] **Problem Details middleware** - `GlobalExceptionHandlerMiddleware` returns RFC 9457 responses with correlation IDs; maps exception types to HTTP status codes (ArgumentException→400, KeyNotFoundException→404, etc.)
- [x] **Correlation ID middleware** - `CorrelationIdMiddleware` generates/propagates `X-Correlation-ID` header; integrates with OpenTelemetry tags and structured logging scopes
- [x] **Pagination framework** - `QueryParameters` (page, pageSize, sort; max 100, default 25) and `PagedResponse<T>` (items + `PaginationMetadata`); `QueryableExtensions.ToPagedAsync()` in `TrashMob.Shared/Extensions/`
- [x] **Server-side filtering framework** - Per-endpoint `QueryParameters` subclasses (`EventQueryParameters`, `LitterReportQueryParameters`, `TeamQueryParameters`, `UserQueryParameters`, `PartnerQueryParameters`); manager methods return `IQueryable<T>` for composable filtering
- [x] **V2 DTO layer** - 64 DTOs in `TrashMob.Models/Poco/V2/`; 21 mapping extension files in `TrashMob.Models/Extensions/V2/`; bidirectional `ToV2Dto()`/`ToEntity()` pattern; read/write DTO separation (`UserDto` vs `UserWriteDto`)
- [x] **OpenAPI 3.1 dual-doc** - v1 + v2 side by side in Swagger UI (`/swagger/v1/swagger.json`, `/swagger/v2/swagger.json`)

> **Note:** Auto-generated client libraries (NSwag/Kiota) have been removed from scope. The mobile app already calls v2 endpoints directly via manual service classes, and the web app will migrate to v2 endpoints the same way. The overhead of maintaining a client generation pipeline is not justified for a single-consumer API.

### Phase 2a - Core Endpoints ✅ (complete — 30 controllers, 64 DTOs, 30+ test suites)

New v2 controllers alongside existing v1 controllers. V1 remains untouched. All controllers use `[ApiVersion("2.0")]`, primary constructors, `[EnableCors]`, and DTO-only request/response patterns. Tests in `TrashMob.Shared.Tests/Controllers/V2/`.

**Core Resources:**
- [x] **Events v2** - `EventsV2Controller`: paginated list with `EventQueryParameters` (status, type, city, region, country, date range); user events; location queries; POST-based filtered pagination
- [x] **Users v2** - `UsersV2Controller`: paginated list with `UserQueryParameters`; `UserDto` (PII-safe) / `UserWriteDto` (PII included); server-managed field preservation on PUT; impact stats; photo upload
- [x] **Partners v2** - `PartnersV2Controller`: paginated active partners with `PartnerQueryParameters`
- [x] **Communities v2** - `CommunitiesV2Controller`: community listing and detail
- [x] **Teams v2** - `TeamsV2Controller`: paginated with `TeamQueryParameters`; my teams; name availability check; team lead auth
- [x] **LitterReports v2** - `LitterReportsV2Controller`: paginated with `LitterReportQueryParameters`; user reports; location queries; image upload/retrieval
- [x] **Stats v2** - `StatsV2Controller`: platform and user statistics

**Nested Resources:**
- [x] **EventAttendees v2** - `EventAttendeesV2Controller`: paginated attendees per event; count; register/unregister; lead promotion/demotion; waiver status check
- [x] **EventSummary v2** - `EventSummaryV2Controller`: post-event metrics CRUD
- [x] **EventAttendeeMetrics v2** - `EventAttendeeMetricsV2Controller`: individual attendee metrics (bags, weight, duration)
- [x] **EventPhotos v2** - `EventPhotosV2Controller`: event photo listing, upload, deletion
- [x] **EventRoutes v2** - `EventRoutesV2Controller`: GPS route data per event
- [x] **EventAttendeeRoutes v2** - `EventAttendeeRoutesV2Controller`: individual attendee GPS routes
- [x] **EventLitterReports v2** - `EventLitterReportsV2Controller`: litter reports linked to events
- [x] **EventPartnerLocationServices v2** - `EventPartnerLocationServicesV2Controller`: partner services at events
- [x] **EventDependents v2** - `EventDependentsV2Controller`: minor dependents at events
- [x] **TeamMembers v2** - `TeamMembersV2Controller`: member listing, join, remove, promote/demote leads
- [x] **TeamEvents v2** - `TeamEventsV2Controller`: events associated with teams

**Dependents & Minors:**
- [x] **Dependents v2** - `DependentsV2Controller`: CRUD for parent-managed dependents (owner-only auth)
- [x] **DependentInvitations v2** - `DependentInvitationsV2Controller`: invitation system for minor account linking
- [x] **DependentWaivers v2** - `DependentWaiversV2Controller`: waiver management for dependents

**Platform Features:**
- [x] **Waivers v2** - `WaiversV2Controller`: required/signed waivers; accept/reject; minor-specific; event waivers; all versions
- [x] **Leaderboards v2** - `LeaderboardsV2Controller`: rankings by type (Events, Bags, Weight, Hours); user and team rankings
- [x] **Achievements v2** - `AchievementsV2Controller`: user achievements and achievement definitions
- [x] **Maps v2** - `MapsV2Controller`: event and litter report map data
- [x] **Lookups v2** - `LookupsV2Controller`: event types, service types, partner service statuses
- [x] **PickupLocations v2** - `PickupLocationsV2Controller`: cleanup location management
- [x] **NewsletterPreferences v2** - `NewsletterPreferencesV2Controller`: category subscriptions
- [x] **ContactRequest v2** - `ContactRequestV2Controller`: contact form submissions
- [x] **AppVersion v2** - `AppVersionV2Controller`: mobile app version checking

### Phase 2b - Remaining Lookup Endpoints ✅ COMPLETE

All lookup types consolidated into `LookupsV2Controller`. The controller now covers all 10 lookup types with cached endpoints (24h ResponseCache):

- [x] **Event statuses** - `EventStatusesController` → added to `LookupsV2Controller`
- [x] **Partner types** - `PartnerTypesController` → added to `LookupsV2Controller`
- [x] **Partner statuses** - `PartnerStatusesController` → added to `LookupsV2Controller`
- [x] **Partner request statuses** - `PartnerRequestStatusesController` → added to `LookupsV2Controller`
- [x] **Weight units** - `WeightUnitsController` → added to `LookupsV2Controller`
- [x] **Invitation statuses** - `InvitationStatusesController` → added to `LookupsV2Controller`
- [x] **Social media account types** - `SocialMediaAccountTypesController` → added to `LookupsV2Controller`

### Phase 2c - User & Route Endpoints ✅ COMPLETE

- [x] **User routes** - Already covered by `EventAttendeeRoutesV2Controller` (`GET by-user/{userId}`)
- [x] **Route metadata** - Already covered by `EventAttendeeRoutesV2Controller` (`PUT {routeId}`, `PUT {routeId}/trim-time`, `PUT {routeId}/restore-time`)
- [x] **User feedback** - `UserFeedbackV2Controller`: submit (anonymous allowed), admin CRUD with status management, DTO-only responses
- [x] **User invites** - `EmailInvitesV2Controller`: batches, quota, rate-limited send (10/batch, 50/month), DTO-only responses
- [x] **Image upload** - `ImageV2Controller`: event image upload/delete with event lead authorization

### Phase 2d - Partner Management (Web Admin) ✅ COMPLETE

All partner management endpoints migrated to v2 with DTO-only responses. 9 new controllers, 9 DTOs, consolidated mapping file.

- [x] **Partner locations** - `PartnerLocationsV2Controller`: CRUD + nearby search with PartnerLocationDto
- [x] **Partner location contacts** - `PartnerLocationContactsV2Controller`: CRUD with PartnerLocationContactDto
- [x] **Partner location services** - `PartnerLocationServicesV2Controller`: CRUD (composite key) with PartnerLocationServiceDto
- [x] **Partner location event services** - Already covered by `EventPartnerLocationServicesV2Controller` (Phase 2a)
- [x] **Partner contacts** - `PartnerContactsV2Controller`: CRUD with PartnerContactDto
- [x] **Partner admins** - `PartnerAdminsV2Controller`: admin listing, my partners, add admin with PartnerAdminDto
- [x] **Partner admin invitations** - `PartnerAdminInvitationsV2Controller`: invite/accept/decline/resend with PartnerAdminInvitationDto
- [x] **Partner requests** - `PartnerRequestsV2Controller`: submit/approve/deny with PartnerRequestDto
- [x] **Partner documents** - `PartnerDocumentsV2Controller`: CRUD + upload/download + admin view with PartnerDocumentDto (combines PartnerDocumentAdminController)
- [x] **Partner social media** - `PartnerSocialMediaAccountsV2Controller`: CRUD with PartnerSocialMediaAccountDto

### Phase 2e - Community & Adoption Management (Web Admin)

Community administration and area adoption features — 10 v1 controllers, all community admin/site admin features.

- [x] **Adoptable areas** - `AdoptableAreasV2Controller`: 11 endpoints, `AdoptableAreaDto` DTO ✅
- [x] **Area generation** - `AreaGenerationV2Controller`: 5 endpoints, `AreaGenerationBatchDto` DTO ✅
- [x] **Staged areas** - `StagedAreasV2Controller`: 7 endpoints, `StagedAdoptableAreaDto` DTO ✅
- [x] **Community adoptions** - `CommunityAdoptionsV2Controller`: 7 endpoints, `TeamAdoptionDto` DTO ✅
- [x] **Adoption events** - `AdoptionEventsV2Controller`: 3 endpoints, `TeamAdoptionEventDto` DTO ✅
- [x] **Community sponsored adoptions** - `CommunitySponsoredAdoptionsV2Controller`: 5 endpoints, `SponsoredAdoptionDto` DTO ✅
- [x] **Community sponsors** - `CommunitySponsorsV2Controller`: 6 endpoints, `SponsorDto` DTO ✅
- [x] **Community invites** - `CommunityInvitesV2Controller`: 3 endpoints ✅
- [x] **Community professional companies** - `CommunityProfessionalCompaniesV2Controller`: 7 endpoints, `ProfessionalCompanyDto` + `ProfessionalCompanyUserDto` DTOs ✅
- [x] **Community prospects** - `CommunityProspectsV2Controller`: 18 endpoints, `CommunityProspectDto` + `ProspectActivityDto` DTOs ✅

**Phase 2e totals:** 10 controllers, 72 endpoints, 11 DTOs, 1 mapping file, 38 tests

### Phase 2f - Team Administration & Portals (Web Admin)

Team admin, sponsor portals, and professional company portals — 7 v1 controllers.

- [x] **Team admin** - `TeamAdminV2Controller`: 3 endpoints (get all, delete, reactivate) ✅
- [x] **Team adoptions** - `TeamAdoptionsV2Controller`: 3 endpoints (list, submit application, active) ✅
- [x] **Team invites** - `TeamInvitesV2Controller`: 3 endpoints (batches, batch detail, create batch) ✅
- [x] **Sponsor portal** - `SponsorPortalV2Controller`: 3 endpoints (my sponsors, cleanup logs, CSV export) ✅
- [x] **Sponsor reports** - `SponsorReportsV2Controller`: 2 endpoints (adoptions, adoption reports) ✅
- [x] **Professional cleanup logs** - `ProfessionalCleanupLogsV2Controller`: 3 endpoints (logs, log cleanup, assignments) ✅
- [x] **Professional company portal** - `ProfessionalCompanyPortalV2Controller`: 1 endpoint (my companies) ✅

**Phase 2f totals:** 7 controllers, 18 endpoints, 2 DTOs, 1 mapping file, 30 tests

### Phase 2g - Admin & Moderation (Site Admin)

Site-wide administration and content moderation — 5 v1 controllers. All require site admin privileges. Web-only.

- [x] **Admin** - `AdminV2Controller`: 2 endpoints (update partner request, email templates) ✅
- [x] **Waiver admin** - `WaiverAdminV2Controller` + `CommunityWaiverAdminV2Controller`: 9 endpoints (CRUD + community assignments) ✅
- [x] **Waiver compliance** - `WaiverComplianceV2Controller`: 5 endpoints (summary, filtered list, expiring, CSV export, details) ✅
- [x] **Photo moderation** - `PhotoModerationV2Controller` + `PhotoFlagV2Controller`: 7 endpoints (pending/flagged/moderated queues, approve/reject/dismiss, user flagging) ✅
- [x] **Message requests** - `MessageRequestV2Controller`: 1 endpoint (send broadcast) ✅

**Phase 2g totals:** 7 controllers, 24 endpoints, 7 DTOs, 1 mapping file, 29 tests

### Phase 2h - CRM & Fundraising (Site Admin)

Fundraising and donor relationship management — 10 v1 controllers. All site admin only. Web-only.

- [x] **Contacts** - `ContactsV2Controller`: 7 endpoints (CRUD + search/filter + tag management) ✅
- [x] **Contact notes** - `ContactNotesV2Controller`: 4 endpoints (CRUD by contact) ✅
- [x] **Contact tags** - `ContactTagsV2Controller`: 4 endpoints (CRUD) ✅
- [x] **Donations** - `DonationsV2Controller`: 8 endpoints (CRUD + by contact + thank-you/receipt emails) ✅
- [x] **Pledges** - `PledgesV2Controller`: 6 endpoints (CRUD + by contact) ✅
- [x] **Grants** - `GrantsV2Controller`: 6 endpoints (CRUD + status filter + AI discovery) ✅
- [x] **Grant tasks** - `GrantTasksV2Controller`: 4 endpoints (CRUD by grant) ✅
- [x] **Fundraising appeals** - `FundraisingAppealsV2Controller`: 2 endpoints (single + bulk appeal) ✅
- [x] **Fundraising analytics** - `FundraisingAnalyticsV2Controller`: 7 endpoints (scores, dashboard, pipeline, LYBUNT, CSV exports) ✅
- [x] **CMS proxy** - `CmsV2Controller`: 8 endpoints (Strapi proxy + RSS feed + admin URL) ✅

**Phase 2h totals:** 10 controllers, 56 endpoints, 9 DTOs, 1 mapping file, 35 tests

### Phase 2i - Infrastructure & Webhook Endpoints

Infrastructure, auth, config, and webhook endpoints. These may not all need v2 equivalents — evaluate case by case.

- [x] **Newsletter webhooks** - `NewsletterWebhooksV2Controller`: SendGrid webhook receiver (tracking) ✅
- [x] **Newsletters** - `NewslettersAdminV2Controller`: 9 endpoints (CRUD + schedule/send/test/templates) ✅
- [x] **Privo webhooks** - `PrivoWebhooksV2Controller`: Privo consent webhook receiver ✅
- [x] **Authentication** - `AuthenticationV2Controller`: 5 endpoints (validate/signup/update/delete user) ✅
- [x] **Secrets** - `SecretsV2Controller`: retrieves config secrets by name ✅
- [x] **Config** - `ConfigV2Controller`: client-side config (App Insights, Entra settings) ✅
- [x] **Email invites** - `EmailInvitesV2Controller`: already existed (user-facing with rate limiting) ✅
- [x] **Job opportunities** - `JobOpportunitiesV2Controller`: 4 endpoints (GET public, CRUD admin) ✅
- [x] **Route simulation** - `RouteSimulationV2Controller`: dev/QA fake GPS route generator (non-prod only) ✅

**Phase 2i totals:** 8 controllers (1 pre-existing), 22 endpoints, 7 DTOs, 2 mapping files, 27 tests

### Phase 3 - Client Migration

Migrate web and mobile clients to use v2 endpoints. V1 endpoints remain running throughout.

**MAUI mobile app:** Already fully migrated to v2 (all 24 service files use `/api/v2.0/` via centralized `Settings.ApiBaseUrl`). No work needed.

**React web app:** All 48 service files currently call v1 endpoints (`/api/...`). The base URL is set in `client-app/src/config/services.config.ts` (`VITE_API_URL || '/api'`). Migration requires three types of changes per service file:

1. **URL path updates** — v2 routes may differ (nested resources, consolidated lookups, renamed segments)
2. **TypeScript model updates** — v2 DTOs have different shapes than v1 entities (field renames, omissions, flattened nav properties, type changes)
3. **Component updates** — any page referencing changed fields must be updated to match new model shapes

**Key v2 response shape changes across the board:**
- **PII separation**: `UserDto` (GET) excludes Email, DateOfBirth, ObjectId, IsSiteAdmin; `UserWriteDto` (POST/PUT) includes writable PII
- **No navigation properties**: v2 DTOs are flat — no nested `EventStatus`, `EventType`, `User`, `Team` objects
- **Flattened user info**: `TeamMemberDto`, `EventAttendeeDto` include `UserName`, `GivenName`, `ProfilePhotoUrl` as top-level fields (from User nav)
- **Boolean conversions**: `EventVisibilityId` (numeric) → `IsEventPublic` (boolean)
- **Image handling**: `LitterReportDto` includes `Images` array of `LitterImageDto` (field rename: `AzureBlobURL` → `ImageUrl`)
- **Paginated responses**: Collection endpoints return `PagedResponse<T>` with `{ items, pagination }` wrapper instead of raw arrays
- **Date type changes**: Some `DateTimeOffset?` → `DateTimeOffset` (with `MinValue` default) or `DateOnly`

#### Phase 3a - Core User Experience (highest traffic)

Service files for the main user-facing flows. Must be correct before flipping the base URL.

- [ ] `users.ts` — 9 endpoints → `UsersV2Controller` routes
  - **Model changes:** `UserData` → split into `UserDto` (reads, no Email/DateOfBirth/IsSiteAdmin) + `UserWriteDto` (writes, includes PII). Frontend `UserData` class needs refactoring or adapter layer. Pages displaying email/admin status must use auth context or separate endpoint.
- [ ] `events.ts` — 23 endpoints → `EventsV2Controller`, `EventAttendeesV2Controller`, `EventSummaryV2Controller`, `LookupsV2Controller` routes
  - **Model changes:** `EventData.eventVisibilityId` (int) → `EventDto.isEventPublic` (bool). `EventData.teamId` removed from DTO. `EventAttendeeData` gains `userName`, `givenName`, `profilePhotoUrl` (flattened from User nav). `EventSummaryData` loses navigation properties. Collection endpoints return `PagedResponse<EventDto>` instead of arrays.
- [ ] `event-routes.ts` — 9 endpoints → `EventRoutesV2Controller`, `EventAttendeeRoutesV2Controller` routes
  - **Model changes:** Route DTOs are structurally similar but use `DateTimeOffset` (not nullable). Nested under `/events/{eventId}/routes` and `/events/{eventId}/attendees/{userId}/routes`.
- [ ] `event-attendee-metrics.ts` — 11 endpoints → `EventAttendeeMetricsV2Controller` routes
  - **Model changes:** Metrics DTOs structurally similar. Nested under `/events/{eventId}/attendees/{userId}/metrics`.
- [ ] `event-photos.ts` — photo upload/delete → `EventPhotosV2Controller` routes
  - **Model changes:** Photo DTOs gain moderation status fields. Nested under `/events/{eventId}/photos`.
- [ ] `event-litter-reports.ts` — event-linked litter reports → `EventLitterReportsV2Controller` routes
  - **Model changes:** Uses `LitterReportDto` with `Images` array (not `LitterImages`). Nested under `/events/{eventId}/litterreports`.
- [ ] `dependents.ts` — 9 endpoints → `DependentsV2Controller`, `EventDependentsV2Controller`, `DependentWaiversV2Controller` routes
  - **Model changes:** `DependentDto.DateOfBirth` is `DateOnly` (not `DateTimeOffset`). Waiver DTOs are flat.
- [ ] `dependent-invitations.ts` — 5 endpoints → `DependentInvitationsV2Controller` routes
  - **Model changes:** Invitation DTOs are structurally similar with status enum changes.

**Phase 3a totals:** 8 service files, ~66 endpoints. **High model impact** — `UserData` and `EventData` are the most widely referenced frontend models.

#### Phase 3b - Teams & Community (public-facing)

- [ ] `teams.ts` — 29 endpoints → `TeamsV2Controller`, `TeamMembersV2Controller`, `TeamEventsV2Controller` routes
  - **Model changes:** `TeamData` loses all nav collections (Members, JoinRequests, Photos, Adoptions). `TeamMemberData` gains flattened `UserName`, `GivenName`, `ProfilePhotoUrl`. Member/event endpoints nested under `/teams/{teamId}/members` and `/teams/{teamId}/events`.
- [ ] `communities.ts` — 15 endpoints → `CommunitiesV2Controller` routes
  - **Model changes:** Community DTOs are flat — no nested Partner/Contact objects. Dashboard and stats have dedicated DTO shapes.
- [ ] `community-photos.ts` — community photo management → `CommunitiesV2Controller` photo routes
  - **Model changes:** Photo DTOs gain moderation fields, nested under community routes.
- [ ] `community-prospects.ts` — community prospect management → `CommunityProspectsV2Controller` routes
  - **Model changes:** New `CommunityProspectDto` + `ProspectActivityDto` — these are new DTOs not present in v1.
- [ ] `stats.ts` — 2 endpoints → `StatsV2Controller` routes
  - **Model changes:** Minimal — stats DTOs are structurally similar to v1 `Stats` model.
- [ ] `leaderboards.ts` — 5 endpoints → `LeaderboardsV2Controller` routes
  - **Model changes:** Leaderboard DTOs may include rank position fields not in v1.
- [ ] `achievements.ts` — 5 endpoints → `AchievementsV2Controller` routes
  - **Model changes:** Achievement DTOs structurally similar.

**Phase 3b totals:** 7 service files, ~56 endpoints. **Medium model impact** — `TeamData` nav property removal affects team detail pages.

#### Phase 3c - Litter Reports, Waivers & Locations

- [ ] `litter-report.ts` — 10 endpoints → `LitterReportsV2Controller` routes
  - **Model changes:** `LitterReportData` → `LitterReportDto`: `LitterImages` → `Images` (field rename), image objects use `ImageUrl` instead of `AzureBlobURL`. Only non-cancelled images included. JSON property names use camelCase via `[JsonPropertyName]`.
- [ ] `locations.ts` — 26 endpoints → `PickupLocationsV2Controller`, `PartnerLocationsV2Controller`, `PartnerLocationServicesV2Controller`, `EventPartnerLocationServicesV2Controller` routes
  - **Model changes:** Location DTOs are flat — no nested Partner/Service navigation. Partner location endpoints split across multiple v2 controllers.
- [ ] `waivers.ts` — waiver lookup → `WaiversV2Controller` routes
  - **Model changes:** Waiver DTOs are flat with version tracking fields.
- [ ] `user-waivers.ts` — 9 endpoints → `WaiversV2Controller` user waiver routes
  - **Model changes:** User waiver DTOs include waiver metadata inline (no separate lookup needed).
- [ ] `maps.ts` — address geocoding → `MapsV2Controller` routes
  - **Model changes:** Minimal — geocoding response is structurally similar.

**Phase 3c totals:** 5 service files, ~45 endpoints. **Medium model impact** — litter report image field rename affects image display components.

#### Phase 3d - Partners & Invitations

- [ ] `partners.ts` — 14 endpoints → `PartnersV2Controller`, `PartnerRequestsV2Controller` routes
  - **Model changes:** Partner DTOs are flat — no nested PartnerType, PartnerStatus nav objects. Status/type resolved to name strings. Paginated responses.
- [ ] `invitations.ts` — 9 endpoints → `PartnerAdminInvitationsV2Controller` routes
  - **Model changes:** Invitation DTOs include flattened inviter/invitee info.
- [ ] `admin.ts` — 4 endpoints → `AdminV2Controller`, `PartnerAdminsV2Controller` routes
  - **Model changes:** Admin DTOs are flat with user info flattened.
- [ ] `email-invites.ts` — 13 endpoints → `EmailInvitesV2Controller`, `CommunityInvitesV2Controller`, `TeamInvitesV2Controller` routes
  - **Model changes:** Invite DTOs include rate limit metadata (quota remaining, batch limits).
- [ ] `social-media.ts` — partner social media → `PartnerSocialMediaAccountsV2Controller` routes
  - **Model changes:** Minimal structural changes.
- [ ] `documents.ts` — partner documents → `PartnerDocumentsV2Controller` routes
  - **Model changes:** Document DTOs combine admin and partner views into single DTO with role-based field visibility.

**Phase 3d totals:** 6 service files, ~40 endpoints. **Low-medium model impact** — mostly admin pages with limited component reuse.

#### Phase 3e - Adoptions & Sponsorships

- [ ] `adoptable-areas.ts` — 24 endpoints → `AdoptableAreasV2Controller`, `AreaGenerationV2Controller`, `StagedAreasV2Controller` routes
  - **Model changes:** `AdoptableAreaDto`, `AreaGenerationBatchDto`, `StagedAdoptableAreaDto` — new v2 DTOs. GeoJSON geometry fields may differ from v1 entity.
- [ ] `team-adoptions.ts` — 9 endpoints → `TeamAdoptionsV2Controller`, `CommunityAdoptionsV2Controller` routes
  - **Model changes:** `TeamAdoptionDto` — flat, no nested Team/Area nav properties.
- [ ] `sponsored-adoptions.ts` — 6 endpoints → `CommunitySponsoredAdoptionsV2Controller`, `SponsorReportsV2Controller` routes
  - **Model changes:** `SponsoredAdoptionDto` — flat, sponsor info inline.
- [ ] `sponsors.ts` — 6 endpoints → `CommunitySponsorsV2Controller` routes
  - **Model changes:** `SponsorDto` — flat, no nested Company nav.
- [ ] `professional-companies.ts` — 7 endpoints → `CommunityProfessionalCompaniesV2Controller` routes
  - **Model changes:** `ProfessionalCompanyDto` + `ProfessionalCompanyUserDto` — new DTOs.
- [ ] `professional-company-portal.ts` — professional portal → `ProfessionalCompanyPortalV2Controller`, `ProfessionalCleanupLogsV2Controller` routes
  - **Model changes:** Cleanup log DTOs are new v2 shapes.
- [ ] `sponsor-portal.ts` — sponsor portal → `SponsorPortalV2Controller` routes
  - **Model changes:** CSV export endpoint returns file download (same as v1).

**Phase 3e totals:** 7 service files, ~52 endpoints. **Medium model impact** — adoption/sponsor DTOs are mostly new, limited existing frontend model overlap.

#### Phase 3f - CRM, Fundraising & Admin

- [ ] `contacts.ts` — 16 endpoints → `ContactsV2Controller`, `ContactNotesV2Controller`, `ContactTagsV2Controller`, `DonationsV2Controller`, `FundraisingAppealsV2Controller`, `FundraisingAnalyticsV2Controller` routes
  - **Model changes:** CRM DTOs are largely new (added in Phase 2h). Frontend models may already align if CRM UI was built against v2-era DTOs.
- [ ] `grants.ts` — 10 endpoints → `GrantsV2Controller`, `GrantTasksV2Controller` routes
  - **Model changes:** Grant DTOs include status filter support. Task DTOs nested under `/grants/{grantId}/tasks`.
- [ ] `waiver-admin.ts` — 14 endpoints → `WaiverAdminV2Controller`, `CommunityWaiverAdminV2Controller`, `WaiverComplianceV2Controller` routes
  - **Model changes:** Compliance DTOs are new (summary, filtered list, expiring, CSV export). Admin waiver DTOs are flat.
- [ ] `photo-moderation.ts` — 7 endpoints → `PhotoModerationV2Controller`, `PhotoFlagV2Controller` routes
  - **Model changes:** Moderation queue DTOs include photo metadata + flag reason inline.
- [ ] `newsletters.ts` — newsletter admin → `NewslettersAdminV2Controller` routes
  - **Model changes:** `NewsletterDto` includes flattened `CategoryName` and 7 stat counters (TotalRecipients, Delivered, Opens, Clicks, Bounces, SpamReports, Unsubscribes). Separate `CreateNewsletterDto`/`UpdateNewsletterDto`/`ScheduleNewsletterDto` for writes.
- [ ] `cms.ts` — 7 endpoints → `CmsV2Controller` routes
  - **Model changes:** CMS proxy responses pass through from Strapi — minimal DTO changes.
- [ ] `opportunities.ts` — 5 endpoints → `JobOpportunitiesV2Controller` routes
  - **Model changes:** `JobOpportunityDto` — flat, structurally similar to v1 but with explicit `IsActive` field.
- [ ] `job-opportunities.ts` — (if separate from opportunities.ts) → `JobOpportunitiesV2Controller` routes

**Phase 3f totals:** 7–8 service files, ~59 endpoints. **Low model impact** — mostly admin-only pages with newer DTOs.

#### Phase 3g - Infrastructure & Base URL Switch

- [ ] `config.ts` — client config → `ConfigV2Controller` routes
  - **Model changes:** Config response shape unchanged (App Insights key + Entra settings).
- [ ] `services.ts` — service type lookup → `LookupsV2Controller` routes
  - **Model changes:** Lookup responses now from consolidated `LookupsV2Controller` with 24h cache headers.
- [ ] `contact.ts` — contact request → `ContactRequestV2Controller` routes
  - **Model changes:** Contact request DTO structurally similar.
- [ ] `feedback.ts` — user feedback → `UserFeedbackV2Controller` routes
  - **Model changes:** Feedback DTOs include status management fields for admin.
- [ ] `message.ts` — message request → `MessageRequestV2Controller` routes
  - **Model changes:** Broadcast message DTO structurally similar.
- [ ] `index.ts` — **Switch `BASE_URL` from `/api` to `/api/v2.0`** (final step after all service files updated)
- [ ] Update `vite.config.ts` proxy to forward `/api/v2.0` requests
- [ ] **Smoke test all pages** after base URL switch

**Phase 3g totals:** 5 service files + base URL switch + smoke test. **Low model impact** — infrastructure endpoints with minimal shape changes.

#### Phase 3h - Verification & Cleanup

- [ ] **Verify v1 traffic drops to zero** via Application Insights
- [x] **MAUI mobile app** — already fully on v2, no work needed ✅
- [ ] Run full `npm run lint` and `npm run build` after all migrations
- [ ] Remove any dead v1-only code paths in service files
- [ ] **Remove unused TypeScript model fields** that were only needed for v1 entity shapes (e.g., navigation property types)
- [ ] **Update TypeScript model classes** in `components/Models/` to match v2 DTO shapes as canonical types
- [ ] **Verify paginated response handling** — all list pages must unwrap `PagedResponse<T>.items` instead of consuming raw arrays

### Phase 4 - Advanced Features

Only after v2 is stable and clients are migrated.

- [ ] ETags and conditional requests (304 Not Modified)
- [ ] Rate limiting middleware (apply decided thresholds)
- [ ] Bulk operations (batch create/update for events, litter reports)
- [ ] Webhook infrastructure for event notifications

---

## Filtering Strategy

### Standard Framework (Build Once)

A reusable filtering/sorting/pagination pipeline that applies to any `IQueryable<T>`:

```csharp
// Shared infrastructure - works for any entity
public class QueryParameters
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 25;  // Max 100
    public string? Sort { get; set; }         // e.g., "-date,name"
}

public static class QueryableExtensions
{
    public static async Task<PagedResponse<TDto>> ToPagedAsync<TEntity, TDto>(
        this IQueryable<TEntity> query,
        QueryParameters parameters,
        Func<TEntity, TDto> mapper,
        CancellationToken cancellationToken) { ... }
}
```

### Per-Endpoint Filters (Case by Case)

Each endpoint defines which fields are filterable, because the useful filters differ:

```csharp
// Events: filter by status, location, date range, type
public class EventFilterV2 : QueryParameters
{
    public int? EventStatusId { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public DateTimeOffset? FromDate { get; set; }
    public DateTimeOffset? ToDate { get; set; }
    public int? EventTypeId { get; set; }
}

// LitterReports: filter by status, location radius, reporter
public class LitterReportFilterV2 : QueryParameters
{
    public int? StatusId { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public double? RadiusMiles { get; set; }
}
```

### Mobile-Specific Benefits

The biggest network savings for mobile come from:

1. **Pagination** - Currently fetching all events/litter reports in one call. A 25-item page vs. 500+ items is a 95% reduction.
2. **Status filtering server-side** - Mobile currently fetches all events then filters client-side. Server-side `?eventStatusId=1` avoids transferring cancelled/completed events.
3. **DTOs without navigation properties** - Entity `Event` includes lazy-loaded `CreatedByUser`, `EventType`, `EventStatus`, attendees, etc. A flat `EventDto` with just the needed fields is dramatically smaller.
4. **ETags (Phase 4)** - Mobile repeatedly fetches the same lists. `304 Not Modified` eliminates redundant transfers entirely.

---

## Out-of-Scope

- v1 endpoint removal (maintain both until all clients migrated)
- GraphQL API (evaluate separately if demand exists)
- API Gateway (defer until microservices architecture needed)
- gRPC endpoints (not needed for current use cases)
- External/public developer API (internal use only)
- Field-level sparse fieldsets (`?fields=name,date`) — adds complexity with diminishing returns when DTOs are already lean
- OData query syntax — too complex; simple typed filter classes are clearer and safer

---

## Rollout Plan (Risk-Minimized)

The key principle: **v1 endpoints are never modified or removed.** All v2 work is purely additive until Phase 3 client migration, which is incremental (one page/screen at a time).

### Step 1 - Infrastructure ✅

1. ✅ Installed `Asp.Versioning.Mvc` + `Asp.Versioning.Mvc.ApiExplorer` NuGet packages
2. ✅ Configured API versioning with default v1 and `UrlSegmentApiVersionReader`
3. ✅ Added `[ApiVersion("1.0")]` to `BaseController`
4. ✅ Added `GlobalExceptionHandlerMiddleware` (Problem Details with correlation IDs)
5. ✅ Added `CorrelationIdMiddleware` (generates/propagates `X-Correlation-ID`, OpenTelemetry integration)
6. ✅ Created `PagedResponse<T>`, `QueryParameters`, `QueryableExtensions.ToPagedAsync()` in `TrashMob.Shared`
7. ✅ Created v2 DTO layer (64 DTOs in `TrashMob.Models/Poco/V2/`, 21 mapping files in `TrashMob.Models/Extensions/V2/`)
8. ✅ Added v2 Swagger doc alongside v1 in Swagger UI
**Validation:** 727+ backend tests passing. Swagger UI shows both v1 and v2 docs. All existing v1 API calls from web and mobile work unchanged.

### Step 2 - Core v2 Endpoints ✅ (Complete — 30 controllers)

All core and nested resource controllers implemented with pagination, filtering, DTOs, auth, and test coverage. See Phase 2a above for full list.

**Validation:** 30+ v2 controller test suites. Mobile app (MAUI) migrated to v2 endpoints. Response sizes reduced via lean DTOs and server-side pagination.

### Step 3 - Remaining v2 Endpoints (Phases 2b–2h)

Categorized by priority:

1. **High priority (mobile-relevant):** Phase 2b (lookup consolidation) + Phase 2c (user routes, feedback, invites)
2. **Medium priority (web admin):** Phase 2d (partner management) + Phase 2e (community/adoption) + Phase 2f (team admin/portals)
3. **Low priority (site admin):** Phase 2g (admin/moderation) + Phase 2h (CRM/fundraising)
4. **Evaluate individually:** Phase 2i (infrastructure/webhooks — many may not need v2 equivalents)

**Risk:** Low. Same proven v2 controller pattern. V1 remains untouched. Many of these are web-only admin features that may not justify v2 migration if the web app continues using v1.

### Step 4 - Web Migration (Phase 3, Incremental)

1. Migrate React pages one at a time to use generated v2 TypeScript client
2. Replace 49 manual service files as each page migrates
3. Each migration is a separate PR — easy to review and revert
4. Track v1 vs v2 traffic in Application Insights

**Risk:** Medium. This is where web users start seeing v2 responses. Mitigated by incremental approach (one page per PR) and the fact that v1 endpoints remain as fallback.

### Step 5 - Mobile Migration (Phase 3, Incremental)

1. Migrate MAUI screens one at a time to use generated .NET client
2. Replace 95 manual RestService files as each screen migrates
3. Test on both Android and iOS
4. Monitor Sentry for any new crash patterns

**Note:** MAUI app already uses many v2 endpoints directly. This step covers migrating the remaining v1 calls.

**Risk:** Medium. App store release cycle adds latency. Mitigated by keeping v1 endpoints alive.

### Step 6 - Advanced Features (Phase 4)

1. Implement ETags (biggest mobile benefit — eliminates redundant transfers)
2. Apply rate limiting middleware (thresholds: 100/300/600 per min)
3. Bulk operations where needed (e.g., batch event creation for community programs)
4. Webhook infrastructure (event created/updated notifications)

**Risk:** Low-Medium. Additive features on v2 endpoints. ETags are purely additive. Rate limiting needs careful threshold tuning.

### Step 7 - Stabilization & v1 Deprecation

1. Monitor v1 traffic — should be near zero
2. Add deprecation headers to v1 endpoints (`Sunset` header)
3. Remove v1 endpoints only after confirming zero traffic for 30+ days
4. Clean up old service files from web and mobile

**Risk:** Low by this point. Data-driven decision based on traffic monitoring.

---

## Success Metrics

### Quantitative
- API response times <= 200ms (p95)
- Error resolution time decreased by 50% (via correlation IDs + Problem Details)
- Mobile network traffic reduced by 80%+ on list endpoints (pagination + DTOs)
- Zero breaking changes to v1 endpoints during entire transition

### Qualitative
- Developer onboarding time reduced (better docs + consistent patterns)
- Improved API consumer satisfaction
- Faster feature development (consistent v2 endpoint patterns)

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|-----------|--------|------------|
| Maintaining two API versions increases complexity | Medium | Medium | Shared business logic (managers); v2 controllers are thin wrappers; time-bound v1 deprecation |
| Client generation doesn't handle edge cases | Low | Medium | Manual client overrides where needed; comprehensive testing; fallback to v1 |
| Breaking changes during development | Low | High | V1 never modified; v2 is additive; incremental migration with per-PR rollback |
| Team learning curve for new patterns | Medium | Low | Clear examples in pilot endpoint; CLAUDE.md updated with v2 patterns |
| Mobile app store release latency | High | Medium | Keep v1 alive until app store updates propagate (minimum 2 weeks) |

---

## Dependencies

### Blockers
None — v2 work is purely additive and can proceed in parallel with other projects.

### Enables
- MCP server can reuse v2 DTOs for tool schemas
- Future external API access built on v2 foundation
- Mobile app performance improvements (pagination, smaller payloads)

---

## Implementation Examples

### Pagination Models (C#)

```csharp
public class PagedResponse<T>
{
    public IEnumerable<T> Items { get; set; }
    public PaginationMetadata Pagination { get; set; }
}

public class PaginationMetadata
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public bool HasNext { get; set; }
    public bool HasPrevious { get; set; }
}
```

### V2 DTO Pattern (Manual Mapping)

```csharp
// TrashMob.Models/Poco/V2/EventDto.cs
public class EventDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTimeOffset EventDate { get; set; }
    public string City { get; set; }
    public string Region { get; set; }
    public string Country { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public int MaxNumberOfParticipants { get; set; }
    public bool IsEventPublic { get; set; }
    public string EventTypeName { get; set; }
    public string EventStatusName { get; set; }
    public string CreatedByUserName { get; set; }
    public int AttendeeCount { get; set; }
}

// TrashMob.Models/Extensions/V2/EventMappingsV2.cs
public static class EventMappingsV2
{
    public static EventDto ToV2Dto(this Event entity, string userName, int attendeeCount = 0)
    {
        return new EventDto
        {
            Id = entity.Id,
            Name = entity.Name,
            // ... flat mapping, no navigation properties
        };
    }
}
```

### Error Models (RFC 9457 Problem Details)

```csharp
public class ProblemDetailsExtension : ProblemDetails
{
    public string? TraceId { get; set; }
    public string? CorrelationId { get; set; }
    public DateTime Timestamp { get; set; }
    public IDictionary<string, string[]>? ValidationErrors { get; set; }
}
```

### V2 Controller Pattern

```csharp
[ApiVersion("2.0")]
[Route("api/v{version:apiVersion}/events")]
[Produces("application/json")]
public class EventsV2Controller(
    IEventManager eventManager,
    ILogger<EventsV2Controller> logger) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResponse<EventDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetEvents(
        [FromQuery] EventFilterV2 filter,
        CancellationToken cancellationToken)
    {
        var result = await eventManager.GetPagedAsync(filter, cancellationToken);
        return Ok(result);
    }
}
```

### Correlation ID Middleware

```csharp
public class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers["X-Correlation-ID"].FirstOrDefault()
                         ?? Guid.NewGuid().ToString();

        context.Items["CorrelationId"] = correlationId;
        context.Response.Headers["X-Correlation-ID"] = correlationId;

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["CorrelationId"] = correlationId
        }))
        {
            await next(context);
        }
    }
}
```

---

## Decisions

1. **~~Client generation tools?~~**
   **Decision:** Removed from scope. Manual service classes are sufficient for a single-consumer API. Both web and mobile call v2 endpoints directly.

2. **Query filtering syntax?**
   **Decision:** Typed filter classes per endpoint (not OData). Simpler, type-safe, no parser complexity.

3. **Pagination default?**
   **Decision:** Offset-based with 25 items/page default, 100 max. Cursor-based not needed for current scale.

4. **DTO mapping approach?**
   **Decision:** Manual mapping via extension methods (no AutoMapper). Consistent with existing `PocoExtensions.cs` pattern.

5. **Should DTOs be reused for MCP server?**
   **Decision:** Yes. V2 DTOs designed for reuse as MCP tool schemas.

6. **Sparse fieldsets (`?fields=`)?**
   **Decision:** Out of scope. Lean DTOs eliminate the need. Adds serialization complexity for marginal benefit.

7. **Where to put V2 DTOs?**
   **Decision:** `TrashMob.Models/Poco/V2/` — keeps them near existing Pocos, follows established conventions.

---

## Monitoring & Observability

### Key Metrics to Track
- API response times (p50, p95, p99) by endpoint and version
- Error rates by type (4xx vs 5xx) and version
- V1 vs V2 traffic split (migration progress indicator)
- Response payload sizes (v1 entity vs v2 DTO)
- Mobile-specific: network bytes per session
- Cache hit rates (ETags, Phase 4)
- Rate limit violations (Phase 4)

### Dashboards
- Application Insights: API performance by version
- Sentry: Mobile crash patterns during migration
- Custom: Migration progress (% of traffic on v2)

---

## Related Documents

- **[Project 6 - Backend Standards](./Project_06_Backend_Standards.md)** - Foundation work (code modernization, auth, tests)
- **[Project 5 - Deployment](./Project_05_Deployment_Pipelines.md)** - CI/CD pipeline
- **PR #2490** (closed) - Community contribution that motivated timeline review

---

**Last Updated:** March 14, 2026
**Owner:** Engineering Team
**Status:** In Progress — Phase 1 complete; Phase 2a complete (30 v2 controllers); Phases 2b–2i, 3, 4 remaining
**Next Review:** Phase 2b–2c (lookups + user/route endpoints) prioritization

---

## Changelog

- **2026-03-14:** Phase 3 sub-phases updated with v2 DTO response shape changes per sub-phase — documents field renames, PII separation, navigation property removal, paginated response wrappers, boolean conversions, and TypeScript model impact for each service file group. Added cleanup tasks to Phase 3h for unused model fields and paginated response handling.
- **2026-03-13:** Major status update — audited actual codebase against project plan. Phase 1 foundation complete. Phase 2 restructured into sub-phases (2a–2i) based on actual state: 2a (30 core v2 controllers, 64 DTOs, 21 mapping files, 30+ test suites) fully complete; 2b–2i categorize ~60 remaining v1 controllers by type (lookups, user/route, partner admin, community admin, team/sponsor portals, site admin, CRM/fundraising, infrastructure/webhooks). Removed client generation pipeline (NSwag/Kiota) from scope — manual service classes sufficient for single-consumer API. Updated status from "Not Started" to "In Progress".
- **2026-03-08:** Major revision — fixed misleading status markers (were showing checkmarks for unstarted items); documented prior work from Project 6; added "Current State" inventory; incorporated learnings from PR #2490; added server-side filtering strategy section; restructured rollout plan with per-step risk analysis and validation criteria; moved response compression to "already done"; moved sparse fieldsets and OData to out-of-scope; added per-endpoint typed filter approach
- **2026-01-31:** Added v2 DTO layer requirement with manual mapping (MCP server reuse)
- **2026-01-31:** Removed week-based schedule from rollout plan (agile approach)
- **2026-01-31:** Resolved open questions; confirmed scope items; added out-of-scope items
