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

### Phase 3 - User Invites
- ✅ Users can invite friends (limited batch)
- ✅ Referral tracking
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

```sql
-- Email invite batches
CREATE TABLE EmailInviteBatches (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    SenderUserId UNIQUEIDENTIFIER NOT NULL,
    BatchType NVARCHAR(50) NOT NULL, -- Admin, Community, User
    CommunityId UNIQUEIDENTIFIER NULL, -- If community invite
    TemplateId INT NULL,
    TotalCount INT NOT NULL DEFAULT 0,
    SentCount INT NOT NULL DEFAULT 0,
    DeliveredCount INT NOT NULL DEFAULT 0,
    OpenedCount INT NOT NULL DEFAULT 0,
    ClickedCount INT NOT NULL DEFAULT 0,
    BouncedCount INT NOT NULL DEFAULT 0,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Processing, Complete, Failed
    CreatedDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    CompletedDate DATETIMEOFFSET NULL,
    FOREIGN KEY (SenderUserId) REFERENCES Users(Id),
    FOREIGN KEY (CommunityId) REFERENCES Partners(Id)
);

-- Individual invite records
CREATE TABLE EmailInvites (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    BatchId UNIQUEIDENTIFIER NOT NULL,
    Email NVARCHAR(256) NOT NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending, Sent, Delivered, Opened, Clicked, Bounced, Unsubscribed
    SendGridMessageId NVARCHAR(100) NULL,
    SentDate DATETIMEOFFSET NULL,
    DeliveredDate DATETIMEOFFSET NULL,
    OpenedDate DATETIMEOFFSET NULL,
    ClickedDate DATETIMEOFFSET NULL,
    SignedUpUserId UNIQUEIDENTIFIER NULL, -- If invite converted to signup
    SignedUpDate DATETIMEOFFSET NULL,
    FOREIGN KEY (BatchId) REFERENCES EmailInviteBatches(Id) ON DELETE CASCADE,
    FOREIGN KEY (SignedUpUserId) REFERENCES Users(Id)
);

-- Invite templates
CREATE TABLE EmailInviteTemplates (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    HtmlBody NVARCHAR(MAX) NOT NULL,
    TextBody NVARCHAR(MAX) NOT NULL,
    TemplateType NVARCHAR(50) NOT NULL, -- Default, Community, Event
    IsActive BIT NOT NULL DEFAULT 1
);

-- Daily send limits tracking
CREATE TABLE EmailSendLimits (
    Date DATE PRIMARY KEY,
    AdminSent INT NOT NULL DEFAULT 0,
    CommunitySent INT NOT NULL DEFAULT 0,
    UserSent INT NOT NULL DEFAULT 0,
    TotalSent INT NOT NULL DEFAULT 0
);

CREATE INDEX IX_EmailInviteBatches_SenderUserId ON EmailInviteBatches(SenderUserId);
CREATE INDEX IX_EmailInviteBatches_Status ON EmailInviteBatches(Status);
CREATE INDEX IX_EmailInvites_BatchId ON EmailInvites(BatchId);
CREATE INDEX IX_EmailInvites_Email ON EmailInvites(Email);
CREATE INDEX IX_EmailInvites_Status ON EmailInvites(Status);
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

**User Dashboard:**
- "Invite Friends" section
- Simple email input (max 10)
- Invite history with conversion tracking

### Mobile App Changes

- Share/invite friends feature
- Simple email input
- Referral link generation

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

### Phase 3: Community & User
- Community admin invites
- User invite feature
- Referral tracking

**Note:** Phase 1 is high priority; enables rapid volunteer acquisition.

---

## Open Questions

1. **Daily send limits?**
   **Recommendation:** 1000/day admin, 500/day per community, 50/month per user
   **Owner:** Product + Finance
   **Due:** Before Phase 1

2. **SendGrid plan tier?**
   **Recommendation:** Pro plan with webhooks; monitor costs
   **Owner:** Engineering + Finance
   **Due:** Before Phase 1

3. **Email template approval process?**
   **Recommendation:** Admin templates pre-approved; community templates reviewed
   **Owner:** Product Lead
   **Due:** Before Phase 2

4. **Referral incentives?**
   **Recommendation:** Track only for v1; incentives TBD
   **Owner:** Product Lead
   **Due:** After Phase 3

---

## Related Documents

- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Community invites
- **[Project 19 - Newsletter](./Project_19_Newsletter.md)** - Email infrastructure shared

---

**Last Updated:** January 24, 2026
**Owner:** Engineering Team
**Status:** Not Started
**Next Review:** When prioritized
