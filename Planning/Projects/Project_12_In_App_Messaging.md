# Project 12 — In-App Messaging

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Low |
| **Risk** | High |
| **Size** | Medium |
| **Dependencies** | None (optional enhancement) |

---

## Business Rationale

Notify attendees about event logistics with strong auditability and abuse prevention. Currently, event leads have no direct way to communicate with registered attendees through the platform.

---

## Objectives

### Primary Goals
- **Lead → attendee broadcast** for event updates
- **Audit logs** for all messages sent
- **Moderation tools** for abuse prevention
- **Rate limiting** to prevent spam

### Secondary Goals
- Canned message templates
- Team/community announcements
- Read receipts
- Message scheduling

---

## Scope

### Phase 1 - Event Messages
- ✅ Event leads can send messages to all attendees
- ✅ Message stored in database with audit trail
- ✅ Attendees receive notification (push + in-app)
- ✅ Message history viewable by lead

### Phase 2 - Safeguards
- ✅ Rate limiting (max messages per day)
- ✅ Canned messages for common updates
- ✅ Report message function
- ✅ Admin moderation queue

### Phase 3 - Extended Messaging (Future)
- ❓ Team announcements
- ❓ Community broadcasts
- ❓ Direct messaging (with restrictions)

---

## Out-of-Scope

- ❌ Real-time chat
- ❌ Group conversations
- ❌ Media/image sharing in messages
- ❌ Message threading
- ❌ External messaging (SMS, email via this system)

---

## Success Metrics

### Quantitative
- **Message delivery rate:** ≥ 99%
- **Abuse reports:** < 0.1% of messages
- **Lead usage:** ≥ 50% of events with 5+ attendees use messaging

### Qualitative
- Positive feedback from event leads
- Reduced no-shows due to better communication
- No significant abuse incidents

---

## Dependencies

### Blockers
None - can be built independently

### Enables
- Better event coordination
- Higher attendee satisfaction
- Reduced email reliance

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Abuse/spam** | Medium | High | Rate limiting; canned messages; report function; moderation |
| **Privacy concerns** | Medium | High | Clear ToS; no direct DMs; audit trail; opt-out |
| **Push notification fatigue** | Medium | Medium | Frequency limits; batching; user preferences |
| **Legal/compliance** | Low | High | Audit trail; retention policy; legal review |

---

## Implementation Plan

### Data Model Changes

```sql
-- Event messages
CREATE TABLE EventMessages (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventId UNIQUEIDENTIFIER NOT NULL,
    SenderUserId UNIQUEIDENTIFIER NOT NULL,
    Subject NVARCHAR(200) NULL,
    Body NVARCHAR(MAX) NOT NULL,
    MessageType NVARCHAR(50) NOT NULL DEFAULT 'Custom', -- Custom, LocationChange, TimeChange, Reminder, Cancellation
    SentDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    RecipientCount INT NOT NULL DEFAULT 0,
    FOREIGN KEY (EventId) REFERENCES Events(Id),
    FOREIGN KEY (SenderUserId) REFERENCES Users(Id)
);

-- Message delivery tracking
CREATE TABLE EventMessageRecipients (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventMessageId UNIQUEIDENTIFIER NOT NULL,
    RecipientUserId UNIQUEIDENTIFIER NOT NULL,
    DeliveredDate DATETIMEOFFSET NULL,
    ReadDate DATETIMEOFFSET NULL,
    FOREIGN KEY (EventMessageId) REFERENCES EventMessages(Id) ON DELETE CASCADE,
    FOREIGN KEY (RecipientUserId) REFERENCES Users(Id)
);

-- Canned message templates
CREATE TABLE MessageTemplates (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Subject NVARCHAR(200) NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    MessageType NVARCHAR(50) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1
);

-- Message reports (abuse)
CREATE TABLE MessageReports (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    EventMessageId UNIQUEIDENTIFIER NOT NULL,
    ReporterUserId UNIQUEIDENTIFIER NOT NULL,
    Reason NVARCHAR(500) NOT NULL,
    ReportDate DATETIMEOFFSET NOT NULL DEFAULT SYSDATETIMEOFFSET(),
    ReviewedByUserId UNIQUEIDENTIFIER NULL,
    ReviewedDate DATETIMEOFFSET NULL,
    ReviewOutcome NVARCHAR(50) NULL, -- Dismissed, Warning, MessageRemoved, UserSuspended
    FOREIGN KEY (EventMessageId) REFERENCES EventMessages(Id),
    FOREIGN KEY (ReporterUserId) REFERENCES Users(Id),
    FOREIGN KEY (ReviewedByUserId) REFERENCES Users(Id)
);

CREATE INDEX IX_EventMessages_EventId ON EventMessages(EventId);
CREATE INDEX IX_EventMessages_SenderUserId ON EventMessages(SenderUserId);
CREATE INDEX IX_EventMessageRecipients_RecipientUserId ON EventMessageRecipients(RecipientUserId);
CREATE INDEX IX_MessageReports_ReviewedDate ON MessageReports(ReviewedDate) WHERE ReviewedDate IS NULL;
```

### API Changes

```csharp
// Send message to event attendees
[Authorize]
[HttpPost("api/events/{eventId}/messages")]
public async Task<ActionResult<EventMessageDto>> SendEventMessage(
    Guid eventId, [FromBody] SendMessageRequest request)
{
    // Validate user is event lead
    // Check rate limits
    // Send to all attendees
    // Trigger push notifications
}

[HttpGet("api/events/{eventId}/messages")]
public async Task<ActionResult<IEnumerable<EventMessageDto>>> GetEventMessages(Guid eventId)
{
    // Get messages for event (leads see all, attendees see received)
}

// Templates
[HttpGet("api/messages/templates")]
public async Task<ActionResult<IEnumerable<MessageTemplateDto>>> GetMessageTemplates()
{
    // Get canned message templates
}

// Reporting
[Authorize]
[HttpPost("api/messages/{messageId}/report")]
public async Task<ActionResult> ReportMessage(Guid messageId, [FromBody] ReportRequest request)
{
    // Submit abuse report
}

// Admin moderation
[Authorize(Roles = "SiteAdmin")]
[HttpGet("api/admin/messages/reports")]
public async Task<ActionResult<IEnumerable<MessageReportDto>>> GetPendingReports()
{
    // Get unreviewed reports
}

[Authorize(Roles = "SiteAdmin")]
[HttpPut("api/admin/messages/reports/{reportId}")]
public async Task<ActionResult> ReviewReport(Guid reportId, [FromBody] ReviewRequest request)
{
    // Process report
}
```

### Web UX Changes

**Event Lead View:**
- "Message Attendees" button on event page
- Message composer with template dropdown
- Message history with delivery stats
- Rate limit indicator

**Attendee View:**
- Inbox/notification center showing event messages
- Mark as read
- Report message option

**Admin View:**
- Moderation queue for reported messages
- User messaging history
- Rate limit overrides

### Mobile App Changes

- Push notification for new messages
- Message inbox
- View message details
- Report function

---

## Implementation Phases

### Phase 1: Core Messaging
- Database schema
- API endpoints
- Basic send/receive
- Push notifications

### Phase 2: Safeguards
- Rate limiting
- Canned templates
- Report function
- Admin moderation

### Phase 3: Enhancements
- Read receipts
- Scheduled messages
- Analytics

**Note:** This is a lower-priority feature; implement only when higher-priority projects complete.

---

## Open Questions

1. **Rate limit: how many messages per event per day?**
   **Recommendation:** 3 custom messages per day; system messages unlimited
   **Owner:** Product Lead
   **Due:** Before Phase 1

2. **Scope for communities/teams broadcast?**
   **Recommendation:** Phase 3 only; focus on events first
   **Owner:** Product Lead
   **Due:** After Phase 2

3. **Abuse prevention policy and enforcement?**
   **Recommendation:** Clear ToS; 3-strike policy; legal review
   **Owner:** Legal + Product
   **Due:** Before Phase 1

4. **Message retention period?**
   **Recommendation:** 1 year; archive older messages
   **Owner:** Legal + Engineering
   **Due:** Before Phase 1

---

## Related Documents

- **[Project 9 - Teams](./Project_09_Teams.md)** - Future team messaging
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Future community broadcasts
- **[Project 13 - Bulk Email](./Project_13_Bulk_Email_Invites.md)** - Email alternative

---

**Last Updated:** January 24, 2026
**Owner:** Product Lead
**Status:** Not Started
**Next Review:** When prioritized
