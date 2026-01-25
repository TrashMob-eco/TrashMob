# Complete Project Extraction Script
# This script contains all project specifications ready for file creation

Write-Host "Extracting all remaining project files..."
Write-Host ""

$projects = @{
    9 = @{
        Title = "Teams"
        Status = "Ready for Review"
        Priority = "High"
        Risk = "Large"
        Size = "Very Large"
        Content = @"
Enable users to form teams, manage membership, own/join events, and showcase impact publicly.

## Objectives
- Public/private teams with join workflows
- Multiple team leads
- Team metrics, album, and map presence
- Team discovery and search

## Data Model
- Team, TeamMembers, TeamEvents, TeamPhotos tables
- EventAttendees.TeamId for team participation

## Implementation
Teams page with map, create/edit/manage flows, dashboard integration
"@
    }
    10 = @{
        Title = "Community_Pages"
        Status = "Planning"
        Priority = "High"
        Risk = "Very High"
        Size = "Very Large"
        Content = @"
Branded public pages for partner communities with metrics, photos, contact info, SSO, and adopt-a programs.

## Dependencies
- Auth migration (SSO)
- Waivers V3
- Teams
- CMS

## Data Model
- Partner: HomePageStart/EndDate
- CommunityProgramTypes & CommunityPrograms tables
"@
    }
    11 = @{
        Title = "Adopt_A_Location"
        Status = "Planning"
        Priority = "Medium"
        Risk = "High"
        Size = "Very Large"
        Content = @"
Model adoptable areas with availability and safety rules; manage team applications; reporting & reminders.

## Data Model
- CommunityPrograms (polygons, frequency)
- TeamPrograms (membership & dates)
- TeamProgramEvents (event linkage)
"@
    }
    12 = @{
        Title = "In_App_Messaging"
        Status = "Not Started"
        Priority = "Low"
        Risk = "High"
        Size = "Medium"
        Content = @"
Notify attendees about logistics with strong auditability and abuse prevention.

## Objectives
- Lead ? attendee broadcast
- Audit logs & moderation
- Rate limiting & canned messages

## Open Questions
- Scope for communities/teams broadcast?
- Abuse prevention policy?
"@
    }
    13 = @{
        Title = "Bulk_Email_Invites"
        Status = "Not Started"
        Priority = "High"
        Risk = "Low"
        Size = "Medium"
        Content = @"
Enable admins/communities/users to invite at scale with batching and audit trails.

## Objectives
- Paste lists; batch >100 sends
- History of batches & statuses
- User-level small batch invites
- SendGrid cost controls
"@
    }
    14 = @{
        Title = "Social_Media"
        Status = "Not Started"
        Priority = "Medium"
        Risk = "Low"
        Size = "Small"
        Content = @"
Modernize social integrations across key touchpoints.

## Objectives
- Audit current integrations
- Add current platforms (TikTok, Instagram, etc.)
- Define sharing by context with design input
"@
    }
    15 = @{
        Title = "Route_Tracing"
        Status = "Planning"
        Priority = "Medium"
        Risk = "High"
        Size = "Large"
        Content = @"
Record and share anonymized routes; enable filters, editing, and association of metrics.

## Objectives
- Record & trim routes
- Anonymized overlays & filters
- Associate bags/weight/notes
- Privacy controls & sharing options
"@
    }
    16 = @{
        Title = "Content_Management"
        Status = "In Progress (PR #2364)"
        Priority = "Medium"
        Risk = "Low"
        Size = "Medium"
        Content = @"
Allow non-developers to update home/partners with preview, scheduling, and versioning.

## Status
Strapi CMS integration in progress (PR #2364)

## Complete
- Infrastructure (Bicep, container, workflow)
- Backend (CmsController, proxy)
- Frontend (cms service, home page sections, admin page)
- Strapi project (v5, content types, Dockerfile)
- Security (internal-only ingress, auth proxy)

## Future
Enables Community Pages, Teams branding, Partner customization, News/announcements
"@
    }
    17 = @{
        Title = "MCP_Server"
        Status = "Not Started"
        Priority = "Low"
        Risk = "Moderate"
        Size = "Medium"
        Content = @"
Provide safe, privacy-aware AI access to events/metrics via MCP for natural language queries.

## Objectives
- MCP server exposing scoped metrics/events
- Privacy constraints & anonymization
"@
    }
    18 = @{
        Title = "Before_After_Photos"
        Status = "Planning"
        Priority = "Low"
        Risk = "Moderate"
        Size = "Medium"
        Content = @"
Empower leads to document impact visually with admin moderation and TOS enforcement.

## Objectives
- Upload/manage photos; mark before/after
- Admin moderation queue & removal workflow
- Notification to uploader on removal
"@
    }
    19 = @{
        Title = "Newsletter"
        Status = "Not Started"
        Priority = "Medium"
        Risk = "Low"
        Size = "Medium"
        Content = @"
Communicate monthly updates with categories/opt-outs, batching/scheduling, and templates.

## Objectives
- Template library
- SendGrid categories & opt-out respect
- Test sends & scheduling
- Batched processing
"@
    }
    20 = @{
        Title = "Gamification"
        Status = "Not Started"
        Priority = "Medium"
        Risk = "High"
        Size = "Medium"
        Content = @"
Drive engagement with leaderboards across roles and time ranges.

## Objectives
- Leaderboards by user/team/community
- Time windows (Today ? All time)
- Anti-gaming guardrails
"@
    }
    21 = @{
        Title = "Event_Co_Leads"
        Status = "Planning"
        Priority = "Medium"
        Risk = "High"
        Size = "Medium"
        Content = @"
Support multiple admins per event and update security/queries accordingly.

## Data Model
- EventAttendee: IsEventLead flag

## Implementation
- APIs to set/clear IsEventLead
- Update all admin checks
- Edit Event: attendee list with lead toggles
- Mobile parity
"@
    }
    22 = @{
        Title = "Attendee_Metrics"
        Status = "Not Started"
        Priority = "Medium"
        Risk = "Medium"
        Size = "Medium"
        Content = @"
Let attendees enter personal stats and give leads tools to reconcile.

## Objectives
- Attendee entries for bags/weight/time
- Reconciliation workflow
- Event leaderboards
- Bag drop location notes & alerts
"@
    }
    23 = @{
        Title = "Parental_Consent"
        Status = "Planning"
        Priority = "High"
        Risk = "High"
        Size = "Large"
        Content = @"
Support parent-managed dependents or direct minor registration with age verification and protections.

## Vendor
Privo.com selected for age verification and parental consent.

## Services
- Age verification for 13+
- Verifiable Parental Consent (VPC) collection
- COPPA compliance documentation
- Parent notification and consent workflows
- Consent artifact retention

## Integration Points
1. Sign-up flow: Privo age gate
2. Parental consent: Redirect to Privo
3. Consent verification: API callback
4. Ongoing compliance: Periodic checks
"@
    }
    25 = @{
        Title = "Automated_Testing"
        Status = "Not Started"
        Priority = "Medium"
        Risk = "Low"
        Size = "Medium"
        Content = @"
Reduce regression risk with executable test suites for web and mobile.

## Scope
Web (Playwright):
- User registration/sign-in flows
- Event create/edit/cancel
- Event registration & waiver signing
- Litter report management
- Partner flows
- Admin access controls

Mobile (Appium/MAUI Testing):
- Authentication
- Event discovery/details
- Event registration
- Litter report with camera
- Dashboard/stats
- Map interactions

## Success Metrics
- Coverage ? 80% of critical flows
- CI runs on PRs
- Execution < 10 minutes
- Flaky rate < 5%
- Manual testing reduced 50%
"@
    }
}

Write-Host "Projects ready for extraction: $($projects.Count)"
Write-Host ""
Write-Host "To create files, use AI command for each:"
Write-Host ""

foreach ($num in $projects.Keys | Sort-Object) {
    $p = $projects[$num]
    Write-Host "  'Create Project_$($num.ToString('00'))_$($p.Title).md'"
}

Write-Host ""
Write-Host "Or say: 'Create all remaining project files using the content above'"
