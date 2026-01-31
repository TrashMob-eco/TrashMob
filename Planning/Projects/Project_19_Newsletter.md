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
- ✅ Team newsletters (team leads can send directly to team members)
- ✅ Community newsletters (community admins can send directly)
- ✅ Newsletter preview before sending

**Newsletter Audience Rules:**
- **Sitewide:** All subscribed users
- **Team:** Members of the team
- **Community:** Users whose profile location (city + region/state) matches the community's location

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
None (core newsletter functionality)

### Soft Dependencies
- **Project 23 (Parental Consent):** For first name personalization - User model needs `FirstName`/`LastName` fields (can use username fallback until then)

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

**New Entity: NewsletterCategory (lookup table)**
```csharp
// New file: TrashMob.Models/NewsletterCategory.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a newsletter category that users can subscribe/unsubscribe to.
    /// </summary>
    public class NewsletterCategory : LookupModel
    {
        /// <summary>
        /// Gets or sets the category description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets whether new users are auto-subscribed.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// Gets or sets whether this category is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual ICollection<UserNewsletterPreference> UserPreferences { get; set; }
        public virtual ICollection<Newsletter> Newsletters { get; set; }
    }
}
```

**New Entity: UserNewsletterPreference**
```csharp
// New file: TrashMob.Models/UserNewsletterPreference.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a user's subscription preference for a newsletter category.
    /// </summary>
    public class UserNewsletterPreference
    {
        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the category identifier.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets whether the user is subscribed.
        /// </summary>
        public bool IsSubscribed { get; set; } = true;

        /// <summary>
        /// Gets or sets when the user subscribed.
        /// </summary>
        public DateTimeOffset? SubscribedDate { get; set; }

        /// <summary>
        /// Gets or sets when the user unsubscribed.
        /// </summary>
        public DateTimeOffset? UnsubscribedDate { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual NewsletterCategory Category { get; set; }
    }
}
```

**New Entity: Newsletter**
```csharp
// New file: TrashMob.Models/Newsletter.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a newsletter to be sent to subscribers.
    /// </summary>
    public class Newsletter : KeyedModel
    {
        /// <summary>
        /// Gets or sets the newsletter category.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the preview text (inbox preview).
        /// </summary>
        public string PreviewText { get; set; }

        /// <summary>
        /// Gets or sets the HTML content.
        /// </summary>
        public string HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets the plain text content.
        /// </summary>
        public string TextContent { get; set; }

        #region Targeting

        /// <summary>
        /// Gets or sets the target type (All, Community, Team).
        /// </summary>
        public string TargetType { get; set; } = "All";

        /// <summary>
        /// Gets or sets the target ID (community or team).
        /// </summary>
        public Guid? TargetId { get; set; }

        #endregion

        #region Status

        /// <summary>
        /// Gets or sets the status (Draft, Scheduled, Sending, Sent).
        /// </summary>
        public string Status { get; set; } = "Draft";

        /// <summary>
        /// Gets or sets the scheduled send date.
        /// </summary>
        public DateTimeOffset? ScheduledDate { get; set; }

        /// <summary>
        /// Gets or sets when the newsletter was sent.
        /// </summary>
        public DateTimeOffset? SentDate { get; set; }

        #endregion

        #region Statistics

        /// <summary>
        /// Gets or sets the total recipient count.
        /// </summary>
        public int RecipientCount { get; set; }

        /// <summary>
        /// Gets or sets the count of successfully sent emails.
        /// </summary>
        public int SentCount { get; set; }

        /// <summary>
        /// Gets or sets the count of opened emails.
        /// </summary>
        public int OpenCount { get; set; }

        /// <summary>
        /// Gets or sets the count of clicked emails.
        /// </summary>
        public int ClickCount { get; set; }

        #endregion

        // Navigation properties
        public virtual NewsletterCategory Category { get; set; }
    }
}
```

**New Entity: NewsletterTemplate (lookup table)**
```csharp
// New file: TrashMob.Models/NewsletterTemplate.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a reusable newsletter template.
    /// </summary>
    public class NewsletterTemplate : LookupModel
    {
        /// <summary>
        /// Gets or sets the template description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the HTML content template.
        /// </summary>
        public string HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets the plain text content template.
        /// </summary>
        public string TextContent { get; set; }

        /// <summary>
        /// Gets or sets whether this template is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<NewsletterCategory>(entity =>
{
    entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
    entity.Property(e => e.Description).HasMaxLength(500);
});

modelBuilder.Entity<UserNewsletterPreference>(entity =>
{
    entity.HasKey(e => new { e.UserId, e.CategoryId });

    entity.HasOne(e => e.User)
        .WithMany()
        .HasForeignKey(e => e.UserId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.Category)
        .WithMany(c => c.UserPreferences)
        .HasForeignKey(e => e.CategoryId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasIndex(e => e.UserId);
});

modelBuilder.Entity<Newsletter>(entity =>
{
    entity.Property(e => e.Subject).HasMaxLength(200).IsRequired();
    entity.Property(e => e.PreviewText).HasMaxLength(500);
    entity.Property(e => e.TargetType).HasMaxLength(50);
    entity.Property(e => e.Status).HasMaxLength(50);

    entity.HasOne(e => e.Category)
        .WithMany(c => c.Newsletters)
        .HasForeignKey(e => e.CategoryId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.Status);
    entity.HasIndex(e => e.ScheduledDate)
        .HasFilter("[Status] = 'Scheduled'");
});

modelBuilder.Entity<NewsletterTemplate>(entity =>
{
    entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
    entity.Property(e => e.Description).HasMaxLength(500);
});
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

1. ~~**Newsletter frequency?**~~
   **Decision:** Monthly sitewide newsletter only
   **Status:** ✅ Resolved

2. ~~**Who can send team/community newsletters?**~~
   **Decision:** Team leads and community admins can send directly without approval
   **Status:** ✅ Resolved

3. ~~**Newsletter content guidelines?**~~
   **Decision:** Create editorial guidelines document; no formal review process
   **Status:** ✅ Resolved

4. ~~**Personalization scope?**~~
   **Decision:** Basic tokens (city, username) for v1; first name requires User model changes (see note below)
   **Status:** ✅ Resolved

5. ~~**Name personalization dependency?**~~
   **Decision:** Currently User model only has `UserName` (display) and `City` - no first/last name fields. To use "Hi {FirstName}" personalization:
   - Add optional `FirstName` and `LastName` fields to User model
   - Update registration flow (web + mobile) with optional name fields
   - Can pre-populate from identity provider claims
   - Update privacy policy to disclose name collection
   - For minors: Already covered by Privo.com consent (Project 23)
   - Personalization fallback: `FirstName ?? UserName ?? "there"`
   - **Recommendation:** Implement name fields as part of Project 23 (Parental Consent) since minors already require name display ("first name + last initial")
   **Status:** ✅ Resolved

---

## Related Documents

- **[Project 13 - Bulk Email](./Project_13_Bulk_Email_Invites.md)** - Shared email infrastructure
- **[Project 9 - Teams](./Project_09_Teams.md)** - Team newsletters
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community newsletters

---

**Last Updated:** January 31, 2026
**Owner:** Marketing + Engineering
**Status:** Not Started
**Next Review:** When prioritized
