# Generate All Remaining Project Files
# This script creates all 15 remaining project files from the 2026 plan

Write-Host "=========================================="
Write-Host "TrashMob Project File Generator"
Write-Host "=========================================="
Write-Host ""
Write-Host "Generating 15 remaining project files..."
Write-Host ""

# Function to create project file
function New-ProjectFile {
    param(
        [int]$Number,
        [string]$Title,
        [string]$Status,
        [string]$Priority,
        [string]$Risk,
        [string]$Size,
        [string]$Dependencies,
        [string]$Rationale,
        [string]$Objectives,
        [string]$Scope,
        [string]$OutOfScope,
        [string]$SuccessMetrics,
        [string]$DependencyDetails,
        [string]$Risks,
        [string]$Implementation,
        [string]$Phases,
        [string]$OpenQuestions
    )
    
    $fileName = "Planning/Projects/Project_{0:D2}_{1}.md" -f $Number, ($Title -replace ' ', '_')
    
    $content = @"
# Project $Number — $Title

| Attribute | Value |
|-----------|-------|
| **Status** | $Status |
| **Priority** | $Priority |
| **Risk** | $Risk |
| **Size** | $Size |
| **Dependencies** | $Dependencies |

---

## Business Rationale

$Rationale

---

## Objectives

$Objectives

---

## Scope

$Scope

---

## Out-of-Scope

$OutOfScope

---

## Success Metrics

$SuccessMetrics

---

## Dependencies

$DependencyDetails

---

## Risks & Mitigations

$Risks

---

## Implementation Plan

$Implementation

---

## Implementation Phases

$Phases

---

## Open Questions

$OpenQuestions

---

## Related Documents

- **[Project files in Planning/Projects/](./)**
- **[PRD](../../TrashMob/TrashMob.prd)** - User stories and requirements

---

**Last Updated:** January 24, 2026  
**Owner:** Product & Engineering Team  
**Status:** $Status  
**Next Review:** When volunteer picks up work
"@
    
    $content | Out-File -FilePath $fileName -Encoding UTF8
    Write-Host "  ? Created: $fileName"
}

# Project 10 - Community Pages
New-ProjectFile -Number 10 -Title "Community Pages" -Status "Planning" -Priority "High" -Risk "Very High" -Size "Very Large" `
    -Dependencies "Projects 1 (Auth/SSO), 8 (Waivers), 9 (Teams), 16 (CMS)" `
    -Rationale @"
Branded public pages for partner communities with metrics, photos, contact info, SSO, and adopt-a programs. Communities need dedicated pages to showcase their impact, manage members, and promote programs.
"@ `
    -Objectives @"
### Primary Goals
- **Community discovery map** showing all partner communities
- **Public home pages** with branding, description, and impact metrics
- **Admin management** with SSO for community administrators
- **Metrics dashboard** showing community-wide statistics
- **Event listings** filtered by community
- **Opt-in to adopt-a programs** and other community initiatives

### Secondary Goals
- Custom domains for communities
- Community-specific content management
- Member directory (opt-in)
- Community announcements/news
"@ `
    -Scope @"
### Phase 1 - Community Pages Infrastructure
- ? Database schema for community profiles
- ? Public community pages with branding
- ? Community discovery map
- ? Basic metrics display

### Phase 2 - Admin Features
- ? SSO integration for community admins
- ? Admin dashboard for community management
- ? Community-specific waivers
- ? Member management

### Phase 3 - Programs Integration
- ? Adopt-a-location program opt-in
- ? Custom program types
- ? Program management UI
- ? Reporting for programs

### Phase 4 - Content Management
- ? CMS integration for community content
- ? Photo galleries
- ? News/announcements
- ? Custom branding options
"@ `
    -OutOfScope @"
- ? Custom domains (Phase 2)
- ? E-commerce/donations (separate project)
- ? Community-specific mobile apps
- ? Social network features (forums, chat)
"@ `
    -SuccessMetrics @"
### Quantitative
- **Communities with pages:** 50+ within 12 months
- **Page views:** 10,000+ per month across all communities
- **Admin adoption:** ? 80% of communities use admin features
- **Program participation:** ? 30% of communities opt into programs

### Qualitative
- Positive feedback from community partners
- Increased community engagement
- Successful program launches
"@ `
    -DependencyDetails @"
### Blockers
- **Project 1 (Auth):** SSO required for admin access
- **Project 8 (Waivers):** Community-specific waivers
- **Project 9 (Teams):** Teams belong to communities
- **Project 16 (CMS):** Content management for pages

### Enables
- **Project 11 (Adopt-A-Location):** Communities manage programs
- Subscription revenue from communities
"@ `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Complex permissions model** | High | High | Clear role definitions; extensive testing |
| **SSO integration issues** | Medium | High | Phased rollout; fallback auth |
| **Communities want customization** | High | Medium | Templated approach; CMS for flexibility |
| **Data privacy concerns** | Low | High | Clear policies; opt-in features |
"@ `
    -Implementation @"
### Data Model Changes
\`\`\`sql
-- Enhanced Partner table
ALTER TABLE Partners
ADD HomePageStart DATETIMEOFFSET NULL,
    HomePageEnd DATETIMEOFFSET NULL,
    LogoUrl NVARCHAR(500) NULL,
    BannerImageUrl NVARCHAR(500) NULL,
    PrimaryColor NVARCHAR(7) NULL,
    SecondaryColor NVARCHAR(7) NULL,
    CustomDomain NVARCHAR(200) NULL;

-- Community programs
CREATE TABLE CommunityProgramTypes (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(MAX) NULL
);

CREATE TABLE CommunityPrograms (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    PartnerId UNIQUEIDENTIFIER NOT NULL,
    ProgramTypeId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    FOREIGN KEY (PartnerId) REFERENCES Partners(Id),
    FOREIGN KEY (ProgramTypeId) REFERENCES CommunityProgramTypes(Id)
);
\`\`\`

### API Changes
\`\`\`csharp
[HttpGet("api/communities")]
public async Task<ActionResult<IEnumerable<CommunityDto>>> GetCommunities()

[HttpGet("api/communities/{id}")]
public async Task<ActionResult<CommunityDto>> GetCommunity(Guid id)

[HttpGet("api/communities/{id}/metrics")]
public async Task<ActionResult<CommunityMetricsDto>> GetCommunityMetrics(Guid id)

[Authorize(Roles = "CommunityAdmin")]
[HttpPut("api/communities/{id}")]
public async Task<ActionResult<CommunityDto>> UpdateCommunity(Guid id, UpdateCommunityRequest request)
\`\`\`
"@ `
    -Phases @"
### Phase 1: Public Pages
- Create community page templates
- Display basic information and metrics
- Community discovery map

### Phase 2: Admin Features
- SSO integration
- Admin dashboard
- Community management tools

### Phase 3: Programs
- Program infrastructure
- Adopt-a-location integration
- Program reporting

### Phase 4: Polish
- CMS integration
- Custom branding
- Advanced features

**Note:** Community partners can start using pages after Phase 1; admin features added incrementally.
"@ `
    -OpenQuestions @"
1. **What SSO providers should we support?**  
   **Recommendation:** Start with Microsoft Entra ID; add others as needed  
   **Owner:** Product Lead  
   **Due:** Before Phase 2

2. **Should communities pay for pages?**  
   **Recommendation:** Free basic page; paid for programs and premium features  
   **Owner:** Business team  
   **Due:** Before launch

3. **How much customization to allow?**  
   **Recommendation:** Templates with color/logo customization; CMS for content  
   **Owner:** UX team  
   **Due:** Before Phase 1
"@

# Project 11 - Adopt-A-Location
New-ProjectFile -Number 11 -Title "Adopt A Location" -Status "Planning" -Priority "Medium" -Risk "High" -Size "Very Large" `
    -Dependencies "Projects 9 (Teams), 10 (Community Pages)" `
    -Rationale @"
Model adoptable areas with availability and safety rules; manage team applications; reporting & reminders. Communities want programs where teams adopt specific locations for ongoing maintenance.
"@ `
    -Objectives @"
### Primary Goals
- **Create/manage adoptable areas** with geographic boundaries
- **Team applications and approvals** for adoption
- **Public visibility rules** for adopted locations
- **Delinquency reports and reminders** for inactive adoptions
- **Program reporting** for community administrators

### Secondary Goals
- Adoption certificates
- Recognition/badging
- Multi-team collaborations on locations
"@ `
    -Scope @"
### Phase 1 - Location Management
- ? Define adoptable areas with polygons
- ? Set frequency requirements (monthly, quarterly, etc.)
- ? Safety rules and guidelines
- ? Availability status

### Phase 2 - Team Applications
- ? Teams can apply to adopt locations
- ? Community admin approval workflow
- ? Adoption period and renewal
- ? Team assignment to locations

### Phase 3 - Compliance & Reporting
- ? Track adoption compliance
- ? Delinquency detection
- ? Reminder notifications
- ? Admin reports and dashboards

### Phase 4 - Public Features
- ? Map showing adopted locations
- ? Team recognition on location pages
- ? Adoption certificates (printable)
"@ `
    -OutOfScope @"
- ? Payment/deposits for adoptions
- ? Automated equipment provision
- ? Integration with municipal 311 systems
- ? Physical signage management
"@ `
    -SuccessMetrics @"
### Quantitative
- **Adopted locations:** 200+ within 12 months
- **Active adoptions:** ? 80% compliance rate
- **Renewal rate:** ? 70% of adoptions renew
- **Communities with programs:** 20+ communities

### Qualitative
- Successful pilot with 3-5 communities
- Positive feedback from teams and communities
- Measurable impact on adopted locations
"@ `
    -DependencyDetails @"
### Blockers
- **Project 9 (Teams):** Teams must exist to adopt
- **Project 10 (Community Pages):** Communities manage programs

### Enables
- Recurring volunteer engagement
- Community subscription revenue
"@ `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Complex geographic data** | Medium | High | Use proven mapping libraries; simplify MVP polygons |
| **Low adoption compliance** | High | Medium | Clear expectations; friendly reminders; gamification |
| **Community admin burden** | Medium | Medium | Automated reminders; simple approval flows |
| **Legal liability** | Low | High | Clear ToS; insurance requirements; safety guidelines |
"@ `
    -Implementation @"
### Data Model
\`\`\`sql
CREATE TABLE CommunityPrograms (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    PartnerId UNIQUEIDENTIFIER NOT NULL,
    Name NVARCHAR(200) NOT NULL,
    Polygon GEOGRAPHY NULL, -- Boundary polygon
    Frequency INT NOT NULL, -- Days between required events
    IsActive BIT NOT NULL
);

CREATE TABLE TeamPrograms (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    TeamId UNIQUEIDENTIFIER NOT NULL,
    ProgramId UNIQUEIDENTIFIER NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NULL,
    Status NVARCHAR(20) NOT NULL -- Active, Delinquent, Expired
);

CREATE TABLE TeamProgramEvents (
    TeamProgramId UNIQUEIDENTIFIER NOT NULL,
    EventId UNIQUEIDENTIFIER NOT NULL,
    PRIMARY KEY (TeamProgramId, EventId)
);
\`\`\`
"@ `
    -Phases @"
### Phase 1: Infrastructure
- Database schema
- API endpoints
- Admin UI for location creation

### Phase 2: Applications
- Team application flow
- Approval workflow
- Assignment logic

### Phase 3: Monitoring
- Compliance tracking
- Delinquency detection
- Automated reminders

### Phase 4: Public Features
- Map visualization
- Team recognition
- Certificates

**Note:** Pilot with 2-3 communities before full rollout.
"@ `
    -OpenQuestions @"
1. **How to handle team delinquency?**  
   **Recommendation:** 3 warnings, then revoke adoption; allow reapplication  
   **Owner:** Product Lead  
   **Due:** Before Phase 3

2. **Should teams pay for adoptions?**  
   **Recommendation:** Free; communities may require deposits  
   **Owner:** Business team  
   **Due:** Before launch
"@

# Continue with remaining projects...
Write-Host ""
Write-Host "Generating remaining 13 projects..."

# Project 12 - In-App Messaging
New-ProjectFile -Number 12 -Title "In App Messaging" -Status "Not Started" -Priority "Low" -Risk "High" -Size "Medium" `
    -Dependencies "None" `
    -Rationale "Notify attendees about logistics with strong auditability and abuse prevention. Event leads need to communicate last-minute changes, but email opens are low." `
    -Objectives @"
### Primary Goals
- **Lead ? attendee broadcast** for event updates
- **Audit logs** for all messages
- **Rate limiting** to prevent spam
- **Canned messages** for common scenarios
- **Moderation** and abuse reporting

### Secondary Goals
- Community/team broadcasts
- Message templates
- Scheduled messages
"@ `
    -Scope @"
### Phase 1 - Basic Messaging
- ? Event leads can message attendees
- ? Simple text messages
- ? Audit trail
- ? Rate limiting (max 3 messages per event per day)

### Phase 2 - Safety Features
- ? User-level opt-out
- ? Report abuse function
- ? Admin review queue
- ? Auto-ban for policy violations

### Phase 3 - Enhanced Features
- ? Message templates
- ? Scheduled sends
- ? Delivery status tracking
"@ `
    -OutOfScope "? 1-on-1 messaging\n? Group chats\n? File attachments\n? Real-time chat" `
    -SuccessMetrics "**Message sent rate:** ? 30% of events\n**Opt-out rate:** < 5%\n**Abuse reports:** < 1% of messages\n**Delivery rate:** ? 95%" `
    -DependencyDetails "### Blockers\nNone - independent feature\n\n### Enables\nImproved event communication" `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Abuse/spam** | High | High | Rate limits; moderation; auto-ban |
| **Privacy concerns** | Medium | High | Opt-out; clear ToS; audit logs |
| **Low adoption** | Medium | Low | Make easy to use; templates |
"@ `
    -Implementation "SMS via Twilio or push notifications via Firebase. Canned messages for 'Weather delay', 'Location change', 'Cancelled', etc." `
    -Phases "Phase 1: Basic lead?attendee\nPhase 2: Safety and moderation\nPhase 3: Templates and scheduling" `
    -OpenQuestions "1. SMS or push notifications?\n2. Scope for community/team broadcasts?\n3. Abuse prevention policy?"

# Project 13 - Bulk Email Invites
New-ProjectFile -Number 13 -Title "Bulk Email Invites" -Status "Not Started" -Priority "High" -Risk "Low" -Size "Medium" `
    -Dependencies "None" `
    -Rationale "Enable admins/communities/users to invite at scale with batching and audit trails while controlling email costs." `
    -Objectives @"
### Primary Goals
- **Paste email lists** for bulk invites
- **Batch processing** for sends >100
- **History tracking** of batches and statuses
- **User-level invites** (small batches, friends/family)
- **SendGrid cost controls**

### Secondary Goals
- CSV import
- Duplicate detection
- Bounce handling
"@ `
    -Scope @"
### Phase 1 - Admin Bulk Invites
- ? Admin UI for bulk invites
- ? Paste comma-separated emails
- ? Batch processing (100 at a time)
- ? Status tracking (sent, bounced, failed)

### Phase 2 - User Invites
- ? Users can invite up to 10 people per event
- ? Simple paste/import UI
- ? Personal message option

### Phase 3 - Advanced Features
- ? CSV import
- ? Duplicate detection across batches
- ? Bounce tracking
- ? SendGrid category tagging
"@ `
    -OutOfScope "? Integration with contact lists\n? Email verification service\n? Advanced segmentation" `
    -SuccessMetrics "**Batch success rate:** ? 95%\n**Bounce rate:** < 5%\n**SendGrid cost:** Stay within budget\n**User adoption:** ? 40% of events use invites" `
    -DependencyDetails "### Blockers\nNone\n\n### Enables\nEasier event promotion and growth" `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Spam complaints** | Medium | High | Clear opt-out; comply with CAN-SPAM |
| **Cost overruns** | Low | Medium | Batch limits; admin approval for large sends |
| **Deliverability issues** | Medium | Medium | Warm up IPs; monitor bounces |
"@ `
    -Implementation "SendGrid batch API with queuing. Store batches in database. Background job processes sends." `
    -Phases "Phase 1: Admin bulk sends\nPhase 2: User small batches\nPhase 3: CSV and advanced" `
    -OpenQuestions "1. Batch size limits?\n2. Approval workflow for large sends?\n3. Personal invites count towards SendGrid quota?"

# Continue with Projects 14-25...
Write-Host "  ? Projects 10-13 complete"

# Project 14 - Social Media
New-ProjectFile -Number 14 -Title "Social Media Integration" -Status "Not Started" -Priority "Medium" -Risk "Low" -Size "Small" `
    -Dependencies "None" `
    -Rationale "Modernize social integrations across key touchpoints with thoughtful UX." `
    -Objectives "- Audit current integrations\n- Add current platforms (TikTok, Instagram, etc.)\n- Define sharing by context with design/social input" `
    -Scope "### Phase 1\n- ? Audit existing social features\n- ? Update sharing buttons\n- ? Add modern platforms\n- ? OpenGraph meta tags" `
    -OutOfScope "? Social login (covered in Auth)\n? Social media management tools\n? Paid advertising integration" `
    -SuccessMetrics "**Share rate:** Increase by 25%\n**Referral traffic:** Increase by 30%\n**Social mentions:** Track and grow" `
    -DependencyDetails "None" `
    -Risks "Low risk - straightforward implementation" `
    -Implementation "Update share buttons, add og:tags, test on all platforms" `
    -Phases "Single phase: Audit, update, test, deploy" `
    -OpenQuestions "Which platforms to prioritize?"

# Project 15 - Route Tracing
New-ProjectFile -Number 15 -Title "Route Tracing" -Status "Planning" -Priority "Medium" -Risk "High" -Size "Large" `
    -Dependencies "None" `
    -Rationale "Record and share anonymized routes; enable filters, editing, and association of metrics and notes." `
    -Objectives "- Record & trim routes\n- Anonymized overlays & filters\n- Associate bags/weight/notes\n- Privacy controls & sharing options" `
    -Scope "### Phase 1\n- ? GPS tracking during events\n- ? Route recording\n- ? Basic visualization\n\n### Phase 2\n- ? Privacy anonymization\n- ? Decay/fading over time\n- ? Associate metrics with routes" `
    -OutOfScope "? Real-time tracking\n? Live location sharing\n? Historical heatmaps (Phase 2)" `
    -SuccessMetrics "**Routes recorded:** ? 40% of events\n**Privacy compliance:** 100%\n**User satisfaction:** High" `
    -DependencyDetails "None - can start immediately" `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Privacy concerns** | High | Critical | Anonymization; decay; opt-in; clear policy |
| **Battery drain** | Medium | Medium | Optimize GPS polling; user control |
| **Data storage costs** | Low | Medium | Compress routes; expire old data |
"@ `
    -Implementation "Store GPS coordinates with timestamps. Anonymize by rounding/offsetting. Apply decay algorithm over time." `
    -Phases "Phase 1: Basic recording\nPhase 2: Anonymization and decay\nPhase 3: Advanced visualization" `
    -OpenQuestions "1. Opt-in or opt-out?\n2. Decay timeline?\n3. Share publicly or team-only?"

# Project 16 - CMS (Already in progress via PR #2364)
New-ProjectFile -Number 16 -Title "Content Management" -Status "In Progress (PR #2364)" -Priority "Medium" -Risk "Low" -Size "Medium" `
    -Dependencies "None" `
    -Rationale "Allow non-developers to update home/partners with preview, scheduling, and versioning/rollback." `
    -Objectives "- CMS tool & workflow using Strapi headless CMS\n- Preview & scheduled publish\n- Versioning/rollback" `
    -Scope @"
### Completed (PR #2364)
- ? Infrastructure (Bicep, container, workflow)
- ? Backend (CmsController, HttpClient proxy)
- ? Frontend (cms service, React Query hooks, home page sections)
- ? Strapi project (v5, content types, Dockerfile)
- ? Security (internal-only ingress, auth proxy)
- ? Admin page for content management

### Strapi Content Types Created
- Hero Section (title, subtitle, image, CTA)
- What Is TrashMob (title, description, items)
- Getting Started (title, steps)

### Deployment
- Strapi on Azure Container Apps (internal only)
- Strapi database on Azure SQL (Basic tier)
- GitHub Actions workflow for deployment
- Health probes configured
"@ `
    -OutOfScope "? Multi-language support\n? Workflow approvals\n? Advanced permissions" `
    -SuccessMetrics "**Content updates:** Non-devs can update pages\n**Update frequency:** Increase from quarterly to monthly\n**Errors:** < 1% of updates need rollback" `
    -DependencyDetails "### Enables\n- Project 2 (Home Page improvements)\n- Project 10 (Community Pages)\n- Dynamic content across site" `
    -Risks "Low - PR already in progress with working implementation" `
    -Implementation "Strapi v5 headless CMS with PostgreSQL. All access proxied through ASP.NET Core for security." `
    -Phases "### Remaining Work\n- Test and merge PR #2364\n- Add more content types as needed\n- Create editor documentation" `
    -OpenQuestions "1. Who gets editor access?\n2. Approval workflow needed?\n3. Additional content types?"

# Project 17 - MCP Server
New-ProjectFile -Number 17 -Title "MCP Server" -Status "Not Started" -Priority "Low" -Risk "Moderate" -Size "Medium" `
    -Dependencies "None" `
    -Rationale "Provide safe, privacy-aware AI access to events/metrics via MCP for natural language queries." `
    -Objectives "- MCP server exposing scoped metrics/events\n- Privacy constraints & anonymization\n- Natural language queries for impact data" `
    -Scope "### Phase 1\n- ? Basic MCP server\n- ? Read-only event data\n- ? Public metrics only\n\n### Phase 2\n- ? User-scoped data\n- ? Authentication\n- ? Advanced queries" `
    -OutOfScope "? Write operations via AI\n? Personal data exposure\n? Automated actions" `
    -SuccessMetrics "**Query success rate:** ? 90%\n**Privacy violations:** 0\n**User adoption:** 50+ queries/month" `
    -DependencyDetails "None - experimental feature" `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Privacy leak** | Low | Critical | Strict data scoping; audit logs; review |
| **Low adoption** | High | Low | Good documentation; example queries |
| **API abuse** | Medium | Medium | Rate limiting; authentication |
"@ `
    -Implementation "MCP server in .NET exposing TrashMob API data. Structured prompts for common queries." `
    -Phases "Phase 1: Read-only public data\nPhase 2: Authenticated queries\nPhase 3: Advanced features" `
    -OpenQuestions "1. Which AI assistants to support?\n2. Query rate limits?\n3. Authentication method?"

# Project 18 - Before/After Photos
New-ProjectFile -Number 18 -Title "Before After Photos" -Status "Planning" -Priority "Low" -Risk "Moderate" -Size "Medium" `
    -Dependencies "None" `
    -Rationale "Empower leads to document impact visually with admin moderation and TOS enforcement." `
    -Objectives "- Upload/manage photos; mark before/after\n- Admin moderation queue & removal workflow\n- Notification to uploader on removal" `
    -Scope "### Phase 1\n- ? Photo upload for event leads\n- ? Before/after tagging\n- ? Display in event summary\n\n### Phase 2\n- ? Admin moderation queue\n- ? Report/flag functionality\n- ? Removal workflow with notifications" `
    -OutOfScope "? Automated moderation (AI)\n? Photo editing tools\n? Social features (likes, comments)" `
    -SuccessMetrics "**Events with photos:** ? 50%\n**Moderation response time:** < 24 hours\n**Policy violations:** < 2%" `
    -DependencyDetails "None" `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Inappropriate content** | Medium | High | Moderation queue; AI pre-screening; quick response |
| **Storage costs** | Low | Medium | Image compression; size limits |
| **Privacy violations** | Low | High | Clear guidelines; face blurring option |
"@ `
    -Implementation "Azure Blob Storage for photos. Image optimization. Admin review interface." `
    -Phases "Phase 1: Basic upload\nPhase 2: Moderation\nPhase 3: Advanced features" `
    -OpenQuestions "1. Max photo size?\n2. How many photos per event?\n3. Automatic face blurring?"

# Project 19 - Newsletter
New-ProjectFile -Number 19 -Title "Newsletter" -Status "Not Started" -Priority "Medium" -Risk "Low" -Size "Medium" `
    -Dependencies "None" `
    -Rationale "Communicate monthly updates with categories/opt-outs, batching/scheduling, and templates (sitewide, team, community)." `
    -Objectives "- Template library\n- SendGrid categories & opt-out respect\n- Test sends & scheduling\n- Batched processing" `
    -Scope "### Phase 1\n- ? Newsletter templates\n- ? Admin compose interface\n- ? SendGrid integration\n\n### Phase 2\n- ? Scheduling\n- ? Test sends\n- ? Analytics tracking\n\n### Phase 3\n- ? Team newsletters\n- ? Community newsletters\n- ? Personalization" `
    -OutOfScope "? Advanced segmentation\n? A/B testing\n? Automated content generation" `
    -SuccessMetrics "**Open rate:** ? 25%\n**Click rate:** ? 5%\n**Unsubscribe rate:** < 2%\n**Monthly sends:** Consistent" `
    -DependencyDetails "None" `
    -Risks "Low risk - standard email marketing" `
    -Implementation "SendGrid templates and categories. Admin UI for composition. Batched sends." `
    -Phases "Phase 1: Basic newsletter\nPhase 2: Scheduling and analytics\nPhase 3: Team/community newsletters" `
    -OpenQuestions "1. Frequency?\n2. Required vs optional categories?\n3. Who can send?"

# Project 20 - Gamification
New-ProjectFile -Number 20 -Title "Gamification" -Status "Not Started" -Priority "Medium" -Risk "High" -Size "Medium" `
    -Dependencies "Projects 7 (Weights), 9 (Teams), 22 (Attendee Metrics)" `
    -Rationale "Drive engagement with leaderboards across roles and time ranges while preventing fraud." `
    -Objectives "- Leaderboards by user/team/community\n- Time windows (Today ? All time)\n- Anti-gaming guardrails\n- Badges and achievements" `
    -Scope "### Phase 1\n- ? User leaderboards (bags, weight, events)\n- ? Time windows (week, month, year, all-time)\n- ? Basic anti-gaming (rate limits, validation)\n\n### Phase 2\n- ? Team leaderboards\n- ? Community leaderboards\n- ? Badges/achievements\n\n### Phase 3\n- ? Custom challenges\n- ? Seasonal competitions\n- ? Recognition system" `
    -OutOfScope "? Prizes/rewards\n? Real-time rankings\n? Social competition features" `
    -SuccessMetrics "**Engagement increase:** +20% repeat participation\n**Gaming attempts:** < 1%\n**User satisfaction:** Positive feedback" `
    -DependencyDetails "### Depends On\n- Project 7 (Weights)\n- Project 9 (Teams)\n- Project 22 (Attendee Metrics)" `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Gaming the system** | High | High | Anti-gaming rules; admin review; caps |
| **Negative competition** | Medium | Medium | Friendly framing; celebrate all contributions |
| **Data accuracy** | Medium | High | Validation rules; admin override |
"@ `
    -Implementation "Cached leaderboard tables. Nightly recalculation. Anti-gaming thresholds." `
    -Phases "Phase 1: Basic leaderboards\nPhase 2: Teams and communities\nPhase 3: Achievements and challenges" `
    -OpenQuestions "1. How to prevent gaming?\n2. Include or exclude outliers?\n3. Public or opt-in leaderboards?"

# Project 21 - Event Co-Leads
New-ProjectFile -Number 21 -Title "Event Co Leads" -Status "Planning" -Priority "Medium" -Risk "High" -Size "Medium" `
    -Dependencies "None" `
    -Rationale "Support multiple admins per event and update security/queries accordingly." `
    -Objectives "- Invite & manage co-leads\n- Attendee list management UI\n- Notifications parity for co-leads" `
    -Scope @"
### Phase 1 - Data Model
- ? Add IsEventLead flag to EventAttendees
- ? Update queries to check IsEventLead
- ? Migration for existing events

### Phase 2 - API Changes
- ? APIs to set/clear IsEventLead
- ? Update all admin checks to query EventAttendee
- ? Permission validation

### Phase 3 - Web UX
- ? Edit Event: attendee list with lead toggles
- ? Create Event: auto-add creator as lead
- ? Co-lead invitation flow

### Phase 4 - Mobile Parity
- ? Same features as web
- ? Co-lead management
"@ `
    -OutOfScope "? Hierarchical roles\n? Fine-grained permissions\n? Co-lead approval workflow" `
    -SuccessMetrics "**Events with co-leads:** ? 20%\n**Permission bugs:** 0\n**User satisfaction:** Positive" `
    -DependencyDetails "None" `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Permission bugs** | Medium | High | Extensive testing; code review |
| **Complex UX** | Medium | Medium | Clear UI; good documentation |
| **Migration issues** | Low | High | Test migration; backup data |
"@ `
    -Implementation @"
### Data Model
\`\`\`sql
ALTER TABLE EventAttendees
ADD IsEventLead BIT NOT NULL DEFAULT 0;

UPDATE EventAttendees
SET IsEventLead = 1
WHERE UserId IN (SELECT CreatedByUserId FROM Events WHERE Id = EventAttendees.EventId);
\`\`\`

### Security Check Pattern
\`\`\`csharp
public async Task<bool> IsEventLeadAsync(Guid eventId, Guid userId)
{
    return await _context.EventAttendees
        .AnyAsync(ea => ea.EventId == eventId && ea.UserId == userId && ea.IsEventLead);
}
\`\`\`
"@ `
    -Phases "Phase 1: Data model\nPhase 2: API updates\nPhase 3: Web UI\nPhase 4: Mobile UI" `
    -OpenQuestions "1. Limit on number of co-leads?\n2. Can co-leads add more co-leads?\n3. What happens if original creator leaves?"

# Project 22 - Attendee Metrics
New-ProjectFile -Number 22 -Title "Attendee Metrics" -Status "Not Started" -Priority "Medium" -Risk "Medium" -Size "Medium" `
    -Dependencies "Project 7 (Event Weights foundation)" `
    -Rationale "Let attendees enter personal stats and give leads tools to reconcile without double counting." `
    -Objectives "- Attendee entries for bags/weight/time\n- Reconciliation workflow\n- Event leaderboards\n- Bag drop location notes & alerts" `
    -Scope @"
### Phase 1 - Attendee Entry
- ? Attendees can enter bags collected
- ? Attendees can enter weight collected
- ? Attendees can enter time spent
- ? Notes field for details

### Phase 2 - Event Lead Reconciliation
- ? View all attendee entries
- ? Calculate totals vs event summary
- ? Flag discrepancies
- ? Override capability

### Phase 3 - Leaderboards
- ? Event-level rankings
- ? Top contributors display
- ? Opt-in visibility

### Phase 4 - Drop Locations
- ? Mark bag drop locations on map
- ? Associate bags with locations
- ? Alerts for full drop points
"@ `
    -OutOfScope "? Real-time tracking\n? Automated counting\n? IoT integration" `
    -SuccessMetrics "**Attendee entry rate:** ? 40%\n**Data accuracy:** ? 95%\n**Lead satisfaction:** Positive" `
    -DependencyDetails "### Depends On\n- Project 7 (Event Weights) - provides EventSummaryAttendee table\n\n### Enables\n- Project 20 (Gamification) - detailed metrics for leaderboards" `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Data inconsistency** | Medium | Medium | Validation rules; lead review; caps |
| **Over-reporting** | Medium | Low | Reasonable ranges; spot checks |
| **Low adoption** | High | Low | Make easy; optional; show value |
"@ `
    -Implementation "Extend EventSummaryAttendee table (from Project 7). UI for attendee entry. Lead reconciliation view." `
    -Phases "Phase 1: Basic entry\nPhase 2: Reconciliation\nPhase 3: Leaderboards\nPhase 4: Drop locations" `
    -OpenQuestions "1. Make entry required or optional?\n2. How to handle outliers?\n3. Public or private leaderboards?"

# Project 23 - Parental Consent
New-ProjectFile -Number 23 -Title "Parental Consent" -Status "Planning" -Priority "High" -Risk "High" -Size "Large" `
    -Dependencies "Project 1 (Auth Revamp - minors support)" `
    -Rationale "Support parent-managed dependents or direct minor registration with age verification and protections." `
    -Objectives @"
### Primary Goals
- **Parent-managed dependents** workflow
- **Direct minor registration** with age verification
- **Enhanced protections** (no DMs, adult presence required)
- **Age verification** via Privo.com
- **Parental consent** collection and storage
- **COPPA compliance** documentation

### Secondary Goals
- Transition to adult account at age 18
- Guardian dashboard
- Bulk consent for siblings
"@ `
    -Scope @"
### Phase 1 - Privo.com Integration
- ? Age gate during registration
- ? Redirect to Privo for verification
- ? API callback for consent status
- ? Store consent artifacts

### Phase 2 - Parent-Managed Flow
- ? Parents create accounts for dependents
- ? Link parent-child accounts
- ? Parent approval for event registration
- ? Guardian dashboard

### Phase 3 - Direct Minor Flow
- ? Minor creates own account
- ? Triggers parental consent via Privo
- ? Restricted features until consent
- ? Parent notification

### Phase 4 - Protections
- ? Minors can't create events
- ? Require adult at events
- ? No messaging access
- ? Privacy restrictions
"@ `
    -OutOfScope "? Background checks\n? Minor-specific events\n? School group management" `
    -SuccessMetrics "**Minors registered:** 500+ within 12 months\n**COPPA compliance:** 100%\n**Legal incidents:** 0\n**Parent satisfaction:** Positive" `
    -DependencyDetails @"
### Blockers
- **Project 1 (Auth):** Entra External ID with minors support
- **Privo.com contract:** Age verification vendor
- **Legal review:** COPPA compliance sign-off

### Enables
- Safe minor participation
- Broader volunteer base
"@ `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **COPPA violation** | Low | Critical | Privo.com integration; legal review; strict compliance |
| **Privo.com dependency** | Low | High | Fallback to manual verification; clear error handling |
| **Complex workflows** | Medium | Medium | Clear UX; documentation; support |
| **Age misrepresentation** | Medium | High | Privo verification; audit logs |
"@ `
    -Implementation @"
### Privo.com Integration

**Services:**
- Age verification for users claiming 13+
- Verifiable Parental Consent (VPC) collection
- COPPA compliance documentation
- Parent notification workflows
- Consent artifact retention per legal requirements

**Integration Points:**
1. **Sign-Up Flow:** Age gate redirects to Privo
2. **Consent Collection:** Privo handles parent verification
3. **Callback:** Privo confirms consent status
4. **Ongoing:** Periodic consent checks

**Data Model:**
\`\`\`sql
ALTER TABLE Users
ADD IsMinor BIT NOT NULL DEFAULT 0,
    ParentUserId UNIQUEIDENTIFIER NULL,
    ParentConsentDate DATETIMEOFFSET NULL,
    PrivoConsentId NVARCHAR(200) NULL,
    FOREIGN KEY (ParentUserId) REFERENCES Users(Id);
\`\`\`
"@ `
    -Phases @"
### Phase 1: Privo Integration
- Contract with Privo.com
- Technical integration
- Testing and validation

### Phase 2: Parent-Managed
- Parent creates dependent accounts
- Parent approval workflows
- Guardian dashboard

### Phase 3: Direct Minor
- Minor self-registration
- Privo consent flow
- Feature restrictions

### Phase 4: Protections
- Event restrictions
- Privacy controls
- Age transition (18th birthday)

**Note:** Legal sign-off required before Phase 1.
"@ `
    -OpenQuestions @"
1. **Exact Privo.com integration timeline?**  
   **Owner:** Engineering + Privo team  
   **Due:** Before Phase 1

2. **Cost structure with Privo?**  
   **Owner:** Finance team  
   **Due:** Before contract

3. **Fallback if Privo unavailable?**  
   **Owner:** Engineering team  
   **Due:** Before Phase 1

4. **Can guardians sign for multiple minors?**  
   **Recommendation:** Yes, with clear UI  
   **Owner:** Product team  
   **Due:** Before Phase 2

5. **Age transition process at 18?**  
   **Recommendation:** Automated check; require re-consent  
   **Owner:** Product + Legal  
   **Due:** Before Phase 4
"@

# Project 25 - Automated Testing
New-ProjectFile -Number 25 -Title "Automated Testing" -Status "Not Started" -Priority "Medium" -Risk "Low" -Size "Medium" `
    -Dependencies "Project 5 (CI/CD Pipelines)" `
    -Rationale "Reduce regression risk, enable confident releases, and replace manual test scenarios with executable test suites for web and mobile." `
    -Objectives @"
### Primary Goals
- **Playwright tests** for web UI with CI integration
- **Mobile UI tests** (Appium or MAUI test framework)
- **Convert manual scenarios** (TestScenarios.md) to automated
- **GitHub Actions integration** with test reports
- **Parallel execution** for faster feedback

### Secondary Goals
- Visual regression testing
- Performance testing
- Accessibility testing
"@ `
    -Scope @"
### Web (Playwright)
- ? User registration and sign-in flows
- ? Event creation, editing, and cancellation
- ? Event registration and waiver signing
- ? Litter report creation and management
- ? Partner request and management flows
- ? Site administration access controls
- ? Contact form submission

### Mobile (Appium/.NET MAUI Testing)
- ? Authentication flows
- ? Event discovery and details
- ? Event registration
- ? Litter report creation with camera
- ? Dashboard and stats viewing
- ? Map interactions
"@ `
    -OutOfScope "? Visual regression testing (future phase)\n? Performance/load testing (separate project)\n? API-only tests (covered by unit tests)" `
    -SuccessMetrics @"
### Quantitative
- **Test coverage:** ? 80% of critical user flows
- **CI execution time:** < 10 minutes
- **Flaky test rate:** < 5%
- **Manual testing time:** Reduced by 50%
- **Bug detection:** Catch regressions before production

### Qualitative
- Developer confidence in deployments
- Faster release cycles
- Better documentation of expected behavior
"@ `
    -DependencyDetails "### Depends On\n- Project 5 (CI/CD Pipelines): Stable CI environment\n\n### Enables\n- Faster development cycles\n- Confident refactoring\n- Better regression prevention" `
    -Risks @"
| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Flaky tests** | High | Medium | Stable selectors; retry logic; good waits |
| **Slow execution** | Medium | Medium | Parallel execution; optimize tests |
| **Maintenance burden** | Medium | Medium | Page object pattern; clear documentation |
| **Mobile CI complexity** | High | High | Start with Android; emulator in CI |
"@ `
    -Implementation @"
### Playwright Setup
\`\`\`typescript
// TrashMob/client-app/e2e/tests/auth.spec.ts
import { test, expect } from '@playwright/test';

test('user can sign in', async ({ page }) => {
  await page.goto('/');
  await page.click('text=Sign In');
  await page.fill('[name="email"]', 'test@example.com');
  await page.fill('[name="password"]', 'password');
  await page.click('button[type="submit"]');
  await expect(page.locator('text=Dashboard')).toBeVisible();
});
\`\`\`

### Mobile Testing
- Evaluate Appium vs .NET MAUI Test Framework
- Set up Android emulator in GitHub Actions
- Page object pattern for maintainability

### CI Integration
\`\`\`yaml
# .github/workflows/e2e-tests.yml
name: E2E Tests

on: [pull_request]

jobs:
  playwright:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-node@v4
      - name: Install dependencies
        run: npm ci
      - name: Install Playwright
        run: npx playwright install --with-deps
      - name: Run tests
        run: npx playwright test
      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: playwright-report
          path: playwright-report/
\`\`\`
"@ `
    -Phases @"
### Phase 1: Web Tests (Playwright)
- Set up Playwright in project
- Implement smoke tests for critical paths
- Configure CI integration
- Generate HTML reports

### Phase 2: Core Coverage
- Convert TestScenarios.md to automated tests
- Expand coverage to 80% of critical flows
- Optimize for speed

### Phase 3: Mobile Tests
- Evaluate and choose framework
- Set up mobile test project
- Implement core mobile journeys
- Configure emulator in CI

### Phase 4: Maintenance
- Delete TestScenarios.md
- Document test patterns
- Set up test data seeding
- Enable required checks on PRs

**Note:** Start with web tests (easier setup) before tackling mobile.
"@ `
    -OpenQuestions @"
1. **Appium or .NET MAUI Test Framework?**  
   **Recommendation:** Evaluate both; start with MAUI if adequate  
   **Owner:** Mobile team  
   **Due:** Before Phase 3

2. **Test data strategy?**  
   **Recommendation:** Seed data in test database; reset between runs  
   **Owner:** Engineering team  
   **Due:** Before Phase 1

3. **Flaky test policy?**  
   **Recommendation:** Auto-disable after 3 failures; must fix before re-enable  
   **Owner:** Engineering team  
   **Due:** Before Phase 2

4. **Required checks on PRs?**  
   **Recommendation:** Make required after stabilization (< 5% flaky rate)  
   **Owner:** Engineering Lead  
   **Due:** Before Phase 4
"@

Write-Host ""
Write-Host "=========================================="
Write-Host "? ALL 15 REMAINING PROJECT FILES CREATED!"
Write-Host "=========================================="
Write-Host ""
Write-Host "Projects 10-25 have been generated in Planning/Projects/"
Write-Host ""
Write-Host "Summary:"
Write-Host "  Total projects: 25"
Write-Host "  Completed: 25 (100%)"
Write-Host ""
Write-Host "Next steps:"
Write-Host "  1. Review all project files for accuracy"
Write-Host "  2. Update TODO_Project_Extraction.md"
Write-Host "  3. Update generate_projects.ps1"
Write-Host "  4. Commit all changes"
Write-Host ""
Write-Host "?? Project extraction complete!"
