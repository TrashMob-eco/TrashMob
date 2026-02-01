# Project 34 — User Feedback Tool

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Low |
| **Risk** | Low |
| **Size** | Small |
| **Dependencies** | None |

---

## Business Rationale

Understanding user pain points and feature requests directly from users is critical for prioritizing development work. Currently, TrashMob has no structured way to collect in-app feedback. Users must find the GitHub issues page or contact the team directly, creating friction and missing valuable insights.

A lightweight feedback widget enables users to report issues, suggest features, and share positive experiences without leaving the app. This creates a continuous feedback loop that improves product quality and user satisfaction.

---

## Objectives

### Primary Goals
- **Easy feedback submission** from any page on the website
- **Categorized feedback** (bug, feature request, general comment)
- **Screenshot attachment** for bug reports
- **Admin dashboard** to review and triage feedback

### Secondary Goals (Nice-to-Have)
- Sentiment analysis on feedback
- Integration with GitHub Issues for developer workflow
- Feedback voting/upvoting for feature requests
- Mobile app feedback integration

---

## Scope

### Phase 1 - Basic Feedback Widget
- ✅ Add floating feedback button to website
- ✅ Feedback form with category, description, and optional email
- ✅ Screenshot capture functionality
- ✅ Store feedback in database
- ✅ Email notification to admin on new feedback

### Phase 2 - Admin Dashboard
- ✅ View all feedback submissions
- ✅ Filter by category, status, date
- ✅ Mark feedback as reviewed/resolved/deferred
- ✅ Add internal notes to feedback items
- ✅ Export feedback to CSV

### Phase 3 - Integration
- ✅ Create GitHub issue from feedback (optional)
- ✅ Link feedback to user account (if logged in)
- ✅ Aggregate feedback analytics

---

## Out-of-Scope

- ❌ Real-time chat support
- ❌ AI-powered response suggestions
- ❌ Third-party feedback tools (Intercom, Zendesk, etc.)
- ❌ Full ticketing/helpdesk system
- ❌ Mobile app integration (future phase)

---

## Success Metrics

### Quantitative
- **Feedback submissions:** ≥ 10 per month within 3 months of launch
- **Bug reports with screenshots:** ≥ 50% of bug category submissions
- **Admin response time:** < 48 hours for critical issues
- **Feedback-to-issue conversion:** ≥ 30% of actionable feedback becomes GitHub issues

### Qualitative
- Users feel heard and valued
- Development team has clearer priority insights
- Reduced friction for reporting issues

---

## Dependencies

### Blockers (Must be complete before this project starts)
None - can proceed independently

### Enablers for Other Projects (What this unlocks)
- **All projects:** User feedback informs prioritization
- **Project 29 (Feature Usage Metrics):** Qualitative data to complement quantitative metrics

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Spam submissions** | Medium | Low | Require email or login; add rate limiting; CAPTCHA for anonymous |
| **Low adoption** | Medium | Medium | Prominent placement; onboarding prompt; incentivize feedback |
| **Overwhelming volume** | Low | Medium | Categories and filters; automated tagging; defer to later if volume grows |
| **Privacy concerns** | Low | Medium | Clear privacy policy; optional email; no PII in screenshots |

---

## Implementation Plan

### Data Model Changes

**New Entity: UserFeedback**
```csharp
// New file: TrashMob.Models/UserFeedback.cs
namespace TrashMob.Models
{
    public class UserFeedback : KeyedModel
    {
        public Guid? UserId { get; set; }
        public string Category { get; set; } // Bug, FeatureRequest, General, Praise
        public string Description { get; set; }
        public string Email { get; set; } // Optional, for anonymous users
        public string ScreenshotUrl { get; set; }
        public string PageUrl { get; set; } // Where feedback was submitted from
        public string UserAgent { get; set; }
        public string Status { get; set; } // New, Reviewed, Resolved, Deferred
        public string InternalNotes { get; set; }
        public Guid? ReviewedByUserId { get; set; }
        public DateTimeOffset? ReviewedDate { get; set; }
        public string GitHubIssueUrl { get; set; } // If converted to issue

        // Navigation properties
        public virtual User User { get; set; }
        public virtual User ReviewedByUser { get; set; }
    }
}
```

### API Changes

```csharp
// Submit feedback (public, rate-limited)
[HttpPost("api/feedback")]
public async Task<ActionResult<FeedbackDto>> SubmitFeedback([FromBody] SubmitFeedbackRequest request)
{
    // Validate and store feedback
    // Send notification email
}

// Admin: Get all feedback
[Authorize(Policy = "SiteAdmin")]
[HttpGet("api/admin/feedback")]
public async Task<ActionResult<PagedResult<FeedbackAdminDto>>> GetFeedback([FromQuery] FeedbackFilter filter)
{
    // Return paginated feedback with filters
}

// Admin: Update feedback status
[Authorize(Policy = "SiteAdmin")]
[HttpPut("api/admin/feedback/{id}")]
public async Task<ActionResult> UpdateFeedback(Guid id, [FromBody] UpdateFeedbackRequest request)
{
    // Update status, notes
}

// Admin: Create GitHub issue from feedback
[Authorize(Policy = "SiteAdmin")]
[HttpPost("api/admin/feedback/{id}/create-issue")]
public async Task<ActionResult<string>> CreateGitHubIssue(Guid id)
{
    // Use GitHub API to create issue
    // Return issue URL
}
```

### Web UX Changes

**Feedback Widget:**
- Floating button in bottom-right corner (like Intercom style)
- Opens modal with category selection
- Text area for description
- Optional screenshot capture button
- Optional email field (pre-filled if logged in)
- Submit button with confirmation

**Admin Dashboard:**
- New page: `/siteadmin/feedback`
- Table with filters (category, status, date range)
- Detail view with full feedback and admin actions
- Quick actions: Mark Reviewed, Create Issue, Defer

---

## Implementation Phases

### Phase 1: Basic Widget
- Create database entity and migration
- Build feedback API endpoint
- Create feedback widget component
- Add to site layout
- Set up email notifications

### Phase 2: Admin Dashboard
- Build admin feedback list page
- Add filter and search functionality
- Create feedback detail view
- Add status management
- Add export functionality

### Phase 3: GitHub Integration
- Implement GitHub API integration
- Add "Create Issue" button
- Store issue URL with feedback
- Consider auto-categorization

**Note:** Phases are sequential but not time-bound. Volunteers pick up work as available.

---

## Technology Choices

### Screenshot Capture Options
1. **html2canvas** - Client-side screenshot of current page
2. **Browser native** - `navigator.mediaDevices.getDisplayMedia()` for user-selected capture
3. **File upload** - User uploads their own screenshot

**Recommendation:** html2canvas for automatic page capture + file upload option

### Feedback Widget Libraries
1. **Custom build** - Full control, no dependencies
2. **react-feedback-widget** - Lightweight option
3. **Headless UI modal** - Already in use for other modals

**Recommendation:** Custom build using existing Headless UI patterns for consistency

---

## Open Questions

1. **Should anonymous feedback be allowed?**
   **Recommendation:** Yes, with optional email for follow-up; helps capture feedback from non-registered users
   **Owner:** Product Lead
   **Due:** Before Phase 1

2. **Where should screenshots be stored?**
   **Recommendation:** Azure Blob Storage (same as other images)
   **Owner:** Engineering Team
   **Due:** Before Phase 1

3. **Should we integrate with existing Microsoft Clarity for session replay context?**
   **Recommendation:** Add Clarity session URL to feedback if available; helps debug issues
   **Owner:** Engineering Team
   **Due:** Phase 2

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#550](https://github.com/trashmob/TrashMob/issues/550)** - Add User Feedback tool to website (tracking issue)

---

## Related Documents

- **[Project 29 - Feature Usage Metrics](./Project_29_Feature_Usage_Metrics.md)** - Quantitative metrics to complement feedback
- **[CLAUDE.md](../../CLAUDE.md)** - Observability section

---

**Last Updated:** January 31, 2026
**Owner:** Product Lead + Engineering Team
**Status:** Not Started
**Next Review:** When prioritized

---

## Changelog

- **2026-01-31:** Created project from Issue #550
