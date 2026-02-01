# Project 13 — Bulk Email Invites

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | High |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | None |

---

## Business Rationale

Enable admins, communities, and users to invite potential volunteers at scale with batching and audit trails while controlling email costs. Currently, invitations must be sent one at a time, limiting outreach capabilities.

---

## Objectives

### Primary Goals
- **Paste lists** of email addresses for bulk invites
- **Batch processing** for >100 sends to manage SendGrid costs
- **History tracking** of batches and statuses
- **User-level small batch invites** (up to 10)

### Secondary Goals
- Import from CSV files
- Email template customization
- Bounce/unsubscribe handling
- Invite analytics

---

## Scope

### Phase 1 - Admin Bulk Invites
- ✅ Site admins can paste email lists
- ✅ Batch processing with progress tracking
- ✅ Invite history and status dashboard
- ✅ SendGrid integration with cost monitoring

### Phase 2 - Community Invites
- ✅ Community admins can invite within community
- ✅ Custom invite templates per community
- ✅ Tracking attribution to community

### Phase 3 - Team Invites
- ✅ Team leads can invite potential members
- ✅ Custom invite templates per team
- ✅ **Auto-add to team** when invited user accepts and signs up
- ✅ Tracking attribution to team

### Phase 4 - User Invites
- ✅ Users can invite friends (limited batch)
- ✅ Invite success notifications

---

## Out-of-Scope

- ❌ Cold email marketing campaigns
- ❌ Automated email list purchasing
- ❌ Complex email workflows/drip campaigns
- ❌ A/B testing of invites
- ❌ SMS invitations

---

## Success Metrics

### Quantitative
- **Successful delivery rate:** ≥ 95%
- **Invite acceptance rate:** ≥ 10%
- **SendGrid cost per invite:** Track and optimize
- **Bounce rate:** < 5%

### Qualitative
- Admins find bulk invite process easy
- Communities successfully onboard volunteers
- No spam complaints or blacklisting

---

## Dependencies

### Blockers
None - independent feature

### Enables
- Faster community onboarding
- User-driven growth
- Event promotion at scale

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **SendGrid cost spikes** | Medium | Medium | Batch limits; daily caps; cost monitoring alerts |
| **Spam complaints** | Low | High | Clear opt-out; valid sender; quality email lists |
| **Email deliverability** | Medium | Medium | Proper SPF/DKIM; warm-up sending; reputation monitoring |
| **Abuse by users** | Low | Medium | Rate limits; review for high-volume users |

---

## Implementation Plan

### Data Model Changes

**New Entity: EmailInviteBatch**
```csharp
// New file: TrashMob.Models/EmailInviteBatch.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a batch of email invitations sent by an admin, community, or user.
    /// </summary>
    public class EmailInviteBatch : KeyedModel
    {
        /// <summary>
        /// Gets or sets the sender's user identifier.
        /// </summary>
        public Guid SenderUserId { get; set; }

        /// <summary>
        /// Gets or sets the batch type (Admin, Community, Team, User).
        /// </summary>
        public string BatchType { get; set; }

        /// <summary>
        /// Gets or sets the community identifier (if community invite).
        /// </summary>
        public Guid? CommunityId { get; set; }

        /// <summary>
        /// Gets or sets the team identifier (if team invite).
        /// </summary>
        public Guid? TeamId { get; set; }

        /// <summary>
        /// Gets or sets the template identifier used.
        /// </summary>
        public int? TemplateId { get; set; }

        #region Statistics

        /// <summary>
        /// Gets or sets the total number of invites in this batch.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the number of invites sent.
        /// </summary>
        public int SentCount { get; set; }

        /// <summary>
        /// Gets or sets the number of invites delivered.
        /// </summary>
        public int DeliveredCount { get; set; }

        /// <summary>
        /// Gets or sets the number of invites opened.
        /// </summary>
        public int OpenedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of invites clicked.
        /// </summary>
        public int ClickedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of invites bounced.
        /// </summary>
        public int BouncedCount { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the batch status (Pending, Processing, Complete, Failed).
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets when processing completed.
        /// </summary>
        public DateTimeOffset? CompletedDate { get; set; }

        // Navigation properties
        public virtual User SenderUser { get; set; }
        public virtual Partner Community { get; set; }
        public virtual Team Team { get; set; }
        public virtual EmailInviteTemplate Template { get; set; }
        public virtual ICollection<EmailInvite> Invites { get; set; }
    }
}
```

**New Entity: EmailInvite**
```csharp
// New file: TrashMob.Models/EmailInvite.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents an individual email invitation with tracking.
    /// </summary>
    public class EmailInvite : KeyedModel
    {
        /// <summary>
        /// Gets or sets the batch identifier.
        /// </summary>
        public Guid BatchId { get; set; }

        /// <summary>
        /// Gets or sets the recipient email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the invite status.
        /// </summary>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets the SendGrid message ID for tracking.
        /// </summary>
        public string SendGridMessageId { get; set; }

        #region Tracking Dates

        /// <summary>
        /// Gets or sets when the invite was sent.
        /// </summary>
        public DateTimeOffset? SentDate { get; set; }

        /// <summary>
        /// Gets or sets when the invite was delivered.
        /// </summary>
        public DateTimeOffset? DeliveredDate { get; set; }

        /// <summary>
        /// Gets or sets when the invite was opened.
        /// </summary>
        public DateTimeOffset? OpenedDate { get; set; }

        /// <summary>
        /// Gets or sets when the invite link was clicked.
        /// </summary>
        public DateTimeOffset? ClickedDate { get; set; }

        #endregion

        #region Conversion Tracking

        /// <summary>
        /// Gets or sets the user ID if the invite converted to a signup.
        /// </summary>
        public Guid? SignedUpUserId { get; set; }

        /// <summary>
        /// Gets or sets when the invited person signed up.
        /// </summary>
        public DateTimeOffset? SignedUpDate { get; set; }

        /// <summary>
        /// Gets or sets whether the user was auto-added to the team.
        /// </summary>
        public bool AddedToTeam { get; set; }

        #endregion

        // Navigation properties
        public virtual EmailInviteBatch Batch { get; set; }
        public virtual User SignedUpUser { get; set; }
    }
}
```

**New Entity: EmailInviteTemplate (lookup table)**
```csharp
// New file: TrashMob.Models/EmailInviteTemplate.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents an email template for invitations.
    /// </summary>
    public class EmailInviteTemplate : LookupModel
    {
        /// <summary>
        /// Gets or sets the email subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the HTML body content.
        /// </summary>
        public string HtmlBody { get; set; }

        /// <summary>
        /// Gets or sets the plain text body content.
        /// </summary>
        public string TextBody { get; set; }

        /// <summary>
        /// Gets or sets the template type (Default, Community, Event).
        /// </summary>
        public string TemplateType { get; set; }

        /// <summary>
        /// Gets or sets whether this template is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
```

**New Entity: EmailSendLimit**
```csharp
// New file: TrashMob.Models/EmailSendLimit.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Tracks daily email send limits for rate limiting.
    /// </summary>
    public class EmailSendLimit
    {
        /// <summary>
        /// Gets or sets the date (primary key).
        /// </summary>
        public DateOnly Date { get; set; }

        /// <summary>
        /// Gets or sets admin emails sent today.
        /// </summary>
        public int AdminSent { get; set; }

        /// <summary>
        /// Gets or sets community emails sent today.
        /// </summary>
        public int CommunitySent { get; set; }

        /// <summary>
        /// Gets or sets team emails sent today.
        /// </summary>
        public int TeamSent { get; set; }

        /// <summary>
        /// Gets or sets user emails sent today.
        /// </summary>
        public int UserSent { get; set; }

        /// <summary>
        /// Gets or sets total emails sent today.
        /// </summary>
        public int TotalSent { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<EmailInviteBatch>(entity =>
{
    entity.Property(e => e.BatchType).HasMaxLength(50).IsRequired();
    entity.Property(e => e.Status).HasMaxLength(50);

    entity.HasOne(e => e.SenderUser)
        .WithMany()
        .HasForeignKey(e => e.SenderUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.Community)
        .WithMany()
        .HasForeignKey(e => e.CommunityId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.Team)
        .WithMany()
        .HasForeignKey(e => e.TeamId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.Template)
        .WithMany()
        .HasForeignKey(e => e.TemplateId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.SenderUserId);
    entity.HasIndex(e => e.Status);
});

modelBuilder.Entity<EmailInvite>(entity =>
{
    entity.Property(e => e.Email).HasMaxLength(256).IsRequired();
    entity.Property(e => e.Status).HasMaxLength(50);
    entity.Property(e => e.SendGridMessageId).HasMaxLength(100);

    entity.HasOne(e => e.Batch)
        .WithMany(b => b.Invites)
        .HasForeignKey(e => e.BatchId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.SignedUpUser)
        .WithMany()
        .HasForeignKey(e => e.SignedUpUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.BatchId);
    entity.HasIndex(e => e.Email);
    entity.HasIndex(e => e.Status);
});

modelBuilder.Entity<EmailInviteTemplate>(entity =>
{
    entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
    entity.Property(e => e.Subject).HasMaxLength(200).IsRequired();
    entity.Property(e => e.TemplateType).HasMaxLength(50).IsRequired();
});

modelBuilder.Entity<EmailSendLimit>(entity =>
{
    entity.HasKey(e => e.Date);
});
```

### API Changes

```csharp
// Create bulk invite batch
[Authorize(Roles = "SiteAdmin")]
[HttpPost("api/admin/invites/batch")]
public async Task<ActionResult<InviteBatchDto>> CreateAdminInviteBatch(
    [FromBody] CreateBatchRequest request)
{
    // Validate email list
    // Check daily limits
    // Create batch and queue for processing
}

// Community batch invites
[Authorize(Policy = "CommunityAdmin")]
[HttpPost("api/communities/{communityId}/invites/batch")]
public async Task<ActionResult<InviteBatchDto>> CreateCommunityInviteBatch(
    Guid communityId, [FromBody] CreateBatchRequest request)
{
    // Similar to admin but scoped to community
}

// Team batch invites
[Authorize(Policy = "TeamLead")]
[HttpPost("api/teams/{teamId}/invites/batch")]
public async Task<ActionResult<InviteBatchDto>> CreateTeamInviteBatch(
    Guid teamId, [FromBody] CreateBatchRequest request)
{
    // Team lead can invite potential members
    // Invites are tagged with TeamId for auto-add on signup
}

// User invites (limited)
[Authorize]
[HttpPost("api/invites")]
public async Task<ActionResult<InviteBatchDto>> CreateUserInviteBatch(
    [FromBody] CreateUserBatchRequest request)
{
    // Max 10 emails per batch
    // Max 50 per month per user
}

// Get batch status
[HttpGet("api/invites/batches/{batchId}")]
public async Task<ActionResult<InviteBatchDetailDto>> GetBatchStatus(Guid batchId)
{
    // Return batch with invite statuses
}

// Get invite history
[Authorize]
[HttpGet("api/invites/history")]
public async Task<ActionResult<IEnumerable<InviteBatchDto>>> GetInviteHistory()
{
    // Return user's invite batches
}

// SendGrid webhook for status updates
[HttpPost("api/webhooks/sendgrid")]
public async Task<ActionResult> ProcessSendGridWebhook([FromBody] SendGridEvent[] events)
{
    // Update invite statuses from SendGrid events
}
```

### Team Invite Auto-Add Flow

When a team lead sends invites, the following flow ensures invited users are automatically added to the team:

1. **Invite Sent:** Email contains a unique invite link with encrypted token (e.g., `/signup?invite={token}`)
2. **Token Contains:** InviteId, TeamId, Email (encrypted/signed to prevent tampering)
3. **User Clicks Link:** Directed to signup page with invite context
4. **User Signs Up:**
   - System validates token and email match
   - Creates user account
   - Automatically adds user to team as member
   - Updates EmailInvite record (SignedUpUserId, SignedUpDate, AddedToTeam = true)
5. **Notifications:**
   - New user receives welcome + team welcome message
   - Team lead receives notification of successful signup

**Edge Cases:**
- **Existing user clicks invite:** Prompt to login; if email matches, add to team
- **Different email used:** Invite not auto-fulfilled; user can manually join team
- **Token expired:** Show friendly message; allow manual signup

### Web UX Changes

**Admin Dashboard:**
- Bulk invite form with email paste area
- CSV import option
- Template selection
- Progress indicator during processing
- Batch history with statistics

**Community Admin:**
- Invite form scoped to community
- Community-specific templates
- Attribution tracking

**Team Lead:**
- "Invite Members" section on team management page
- Email paste area (up to 50 emails per batch)
- Team-specific invite templates
- Invite history with signup/join tracking
- Clear indication that invitees will be auto-added to team on signup

**User Dashboard:**
- "Invite Friends" section
- Simple email input (max 10)
- Invite history with conversion tracking

### Mobile App Changes

**All Users:**
- Share/invite friends feature
- Simple email input (max 10)
- Referral link generation

**Team Leads:**
- "Invite Members" on team page
- Email input or contact picker
- Invite history with status tracking

---

## Implementation Phases

### Phase 1: Admin Bulk
- Database schema
- Batch processing job
- Admin UI
- SendGrid integration

### Phase 2: Webhooks & Tracking
- SendGrid webhook handler
- Status updates
- Analytics dashboard
- Cost monitoring

### Phase 3: Team Invites
- Team lead invite UI
- Auto-add to team on signup
- Team attribution tracking

### Phase 4: Community & User
- Community admin invites
- User invite feature

**Note:** Phase 1 is high priority; enables rapid volunteer acquisition.

---

## Open Questions

1. ~~**Daily send limits?**~~
   **Decision:** 1000/day admin, 500/day per community, 100/day per team, 50/month per user
   **Status:** ✅ Resolved

2. ~~**SendGrid plan tier?**~~
   **Decision:** Pro plan with webhooks; monitor costs
   **Status:** ✅ Resolved

3. ~~**Email template approval process?**~~
   **Decision:** Self-service for all templates with post-hoc moderation if reported; reduces friction while maintaining abuse prevention through reporting mechanism
   **Status:** ✅ Resolved

4. ~~**Referral incentives?**~~
   **Decision:** Skip referral tracking for v1; add in future version if needed
   **Status:** ✅ Resolved

---

## GitHub Issues

The following GitHub issues are tracked as part of this project:

- **[#2235](https://github.com/trashmob/TrashMob/issues/2235)** - Project 13: Bulk Email Invites (tracking issue)

---

## Related Documents

- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community invites
- **[Project 19 - Newsletter](./Project_19_Newsletter.md)** - Email infrastructure shared

---

**Last Updated:** January 31, 2026
**Owner:** Engineering Team
**Status:** Not Started
**Next Review:** When prioritized
