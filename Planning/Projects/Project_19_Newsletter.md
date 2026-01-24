# Project 19 — Newsletter Support

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

Communicate monthly updates with categories/opt-outs, batching/scheduling, and templates for sitewide, team, and community newsletters. Regular communication keeps volunteers engaged and informed.

---

## Objectives

### Primary Goals
- **Template library** for consistent branding
- **SendGrid categories** and opt-out respect
- **Test sends** before full distribution
- **Scheduling** and batched processing
- **Multiple newsletter types** (sitewide, team, community)

### Secondary Goals
- Newsletter analytics (open rates, clicks)
- A/B testing subject lines
- Personalization tokens
- Archive of past newsletters

---

## Scope

### Phase 1 - Sitewide Newsletter
- ✅ Newsletter template creation
- ✅ User opt-in/opt-out management
- ✅ Admin compose interface
- ✅ Test send functionality
- ✅ Scheduled sending

### Phase 2 - Categories & Batching
- ✅ Newsletter categories (monthly digest, event updates, community news)
- ✅ Category-based opt-outs
- ✅ Batch processing for large lists
- ✅ SendGrid integration with tracking

### Phase 3 - Team/Community
- ❓ Team newsletters (team leads)
- ❓ Community newsletters (community admins)
- ❓ Newsletter preview/approval

---

## Out-of-Scope

- ❌ Marketing automation sequences
- ❌ Third-party newsletter platform integration
- ❌ Paid newsletter subscriptions
- ❌ Newsletter commenting/feedback
- ❌ SMS newsletters

---

## Success Metrics

### Quantitative
- **Open rate:** ≥ 25%
- **Click rate:** ≥ 5%
- **Unsubscribe rate:** < 1% per send
- **Delivery rate:** ≥ 98%

### Qualitative
- Volunteers value newsletter content
- Admins find composition easy
- No spam complaints

---

## Dependencies

### Blockers
None

### Enables
- Volunteer retention
- Event promotion
- Community engagement

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Low engagement** | Medium | Medium | Quality content; proper frequency; A/B testing |
| **SendGrid costs** | Medium | Medium | Batch limits; monitoring; optimize list hygiene |
| **Deliverability issues** | Low | High | SPF/DKIM setup; warm-up; reputation monitoring |
| **CAN-SPAM compliance** | Low | High | Proper unsubscribe; physical address; clear sender |

---

## Implementation Plan

### Data Model Changes

```sql
-- Newsletter categories
CREATE TABLE NewsletterCategories (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    IsDefault BIT NOT NULL DEFAULT 0, -- Auto-subscribe new users
    IsActive BIT NOT NULL DEFAULT 1
);

-- User newsletter preferences
CREATE TABLE UserNewsletterPreferences (
    UserId UNIQUEIDENTIFIER NOT NULL,
    CategoryId INT NOT NULL,
    IsSubscribed BIT NOT NULL DEFAULT 1,
    SubscribedDate DATETIMEOFFSET NULL,
    UnsubscribedDate DATETIMEOFFSET NULL,
    PRIMARY KEY (UserId, CategoryId),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (CategoryId) REFERENCES NewsletterCategories(Id)
);

-- Newsletters
CREATE TABLE Newsletters (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    CategoryId INT NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    PreviewText NVARCHAR(500) NULL,
    HtmlContent NVARCHAR(MAX) NOT NULL,
    TextContent NVARCHAR(MAX) NOT NULL,
    -- Targeting
    TargetType NVARCHAR(50) NOT NULL DEFAULT 'All', -- All, Community, Team
    TargetId UNIQUEIDENTIFIER NULL, -- Community or Team ID
    -- Status
    Status NVARCHAR(50) NOT NULL DEFAULT 'Draft', -- Draft, Scheduled, Sending, Sent
    ScheduledDate DATETIMEOFFSET NULL,
    SentDate DATETIMEOFFSET NULL,
    -- Stats
    RecipientCount INT NOT NULL DEFAULT 0,
    SentCount INT NOT NULL DEFAULT 0,
    OpenCount INT NOT NULL DEFAULT 0,
    ClickCount INT NOT NULL DEFAULT 0,
    -- Audit
    CreatedByUserId UNIQUEIDENTIFIER NOT NULL,
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    FOREIGN KEY (CategoryId) REFERENCES NewsletterCategories(Id),
    FOREIGN KEY (CreatedByUserId) REFERENCES Users(Id)
);

-- Newsletter templates
CREATE TABLE NewsletterTemplates (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(500) NULL,
    HtmlContent NVARCHAR(MAX) NOT NULL,
    TextContent NVARCHAR(MAX) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

CREATE INDEX IX_UserNewsletterPreferences_UserId ON UserNewsletterPreferences(UserId);
CREATE INDEX IX_Newsletters_Status ON Newsletters(Status);
CREATE INDEX IX_Newsletters_ScheduledDate ON Newsletters(ScheduledDate) WHERE Status = 'Scheduled';
```

### API Changes

```csharp
// Newsletter categories
[HttpGet("api/newsletters/categories")]
public async Task<ActionResult<IEnumerable<NewsletterCategoryDto>>> GetCategories()
{
    // Get available newsletter categories
}

// User preferences
[Authorize]
[HttpGet("api/users/me/newsletter-preferences")]
public async Task<ActionResult<IEnumerable<UserPreferenceDto>>> GetMyPreferences()
{
    // Get user's subscription status per category
}

[Authorize]
[HttpPut("api/users/me/newsletter-preferences")]
public async Task<ActionResult> UpdatePreferences([FromBody] UpdatePreferencesRequest request)
{
    // Update subscription preferences
}

// Unsubscribe (no auth required - from email link)
[HttpPost("api/newsletters/unsubscribe")]
public async Task<ActionResult> Unsubscribe([FromBody] UnsubscribeRequest request)
{
    // Validate token, unsubscribe user
}

// Admin - Newsletter management
[Authorize(Roles = "SiteAdmin")]
[HttpGet("api/admin/newsletters")]
public async Task<ActionResult<IEnumerable<NewsletterDto>>> GetNewsletters()
{
    // Get newsletters with stats
}

[Authorize(Roles = "SiteAdmin")]
[HttpPost("api/admin/newsletters")]
public async Task<ActionResult<NewsletterDto>> CreateNewsletter([FromBody] CreateNewsletterRequest request)
{
    // Create draft newsletter
}

[Authorize(Roles = "SiteAdmin")]
[HttpPost("api/admin/newsletters/{id}/test")]
public async Task<ActionResult> SendTestNewsletter(Guid id, [FromBody] TestSendRequest request)
{
    // Send to test email addresses
}

[Authorize(Roles = "SiteAdmin")]
[HttpPost("api/admin/newsletters/{id}/schedule")]
public async Task<ActionResult> ScheduleNewsletter(Guid id, [FromBody] ScheduleRequest request)
{
    // Schedule for future send
}

[Authorize(Roles = "SiteAdmin")]
[HttpPost("api/admin/newsletters/{id}/send")]
public async Task<ActionResult> SendNewsletter(Guid id)
{
    // Send immediately (queue for batched processing)
}

// SendGrid webhook for tracking
[HttpPost("api/webhooks/sendgrid/newsletter")]
public async Task<ActionResult> ProcessNewsletterWebhook([FromBody] SendGridEvent[] events)
{
    // Update open/click counts
}
```

### Web UX Changes

**User Settings:**
- Newsletter preferences section
- Category toggles
- Unsubscribe all option

**Admin Newsletter Management:**
- Newsletter list with status and stats
- Create/edit newsletter
- Template selection
- Rich text editor for content
- Preview mode
- Test send button
- Schedule picker
- Send confirmation

**Unsubscribe Page:**
- Confirm unsubscribe
- Manage preferences option
- Resubscribe option

---

## Implementation Phases

### Phase 1: Core Infrastructure
- Database schema
- User preferences API
- Unsubscribe handling
- Basic admin UI

### Phase 2: Composition & Sending
- Newsletter editor
- Template system
- Test send
- Batch sending job
- SendGrid integration

### Phase 3: Tracking & Analytics
- Open/click tracking
- Webhook processing
- Newsletter analytics dashboard

### Phase 4: Team/Community
- Scoped newsletters
- Additional admin roles
- Approval workflow

---

## Email Templates

**Standard Newsletter Layout:**
```html
<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width">
</head>
<body style="font-family: Arial, sans-serif;">
    <!-- Header with logo -->
    <div style="background: #00a651; padding: 20px; text-align: center;">
        <img src="{{logo_url}}" alt="TrashMob.eco" height="50">
    </div>

    <!-- Content area -->
    <div style="padding: 20px; max-width: 600px; margin: 0 auto;">
        {{content}}
    </div>

    <!-- Footer -->
    <div style="background: #f5f5f5; padding: 20px; text-align: center; font-size: 12px;">
        <p>TrashMob.eco | PO Box 12345, City, ST 12345</p>
        <p><a href="{{unsubscribe_url}}">Unsubscribe</a> | <a href="{{preferences_url}}">Manage Preferences</a></p>
    </div>
</body>
</html>
```

---

## Open Questions

1. **Newsletter frequency?**
   **Recommendation:** Monthly sitewide; weekly digest optional
   **Owner:** Marketing + Product
   **Due:** Before Phase 1

2. **Who can send team/community newsletters?**
   **Recommendation:** Team leads, community admins with approval
   **Owner:** Product Lead
   **Due:** Before Phase 3

3. **Newsletter content guidelines?**
   **Recommendation:** Create editorial guidelines; review process for first sends
   **Owner:** Marketing
   **Due:** Before Phase 2

4. **Personalization scope?**
   **Recommendation:** Basic tokens (name, city) for v1; advanced later
   **Owner:** Engineering
   **Due:** Before Phase 2

---

## Related Documents

- **[Project 13 - Bulk Email](./Project_13_Bulk_Email_Invites.md)** - Shared email infrastructure
- **[Project 9 - Teams](./Project_09_Teams.md)** - Team newsletters
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community newsletters

---

**Last Updated:** January 24, 2026
**Owner:** Marketing + Engineering
**Status:** Not Started
**Next Review:** When prioritized
