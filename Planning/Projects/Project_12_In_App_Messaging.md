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

**New Entity: EventMessage**
```csharp
// New file: TrashMob.Models/EventMessage.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a message sent by an event lead to attendees.
    /// </summary>
    public class EventMessage : KeyedModel
    {
        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the sender's user identifier.
        /// </summary>
        public Guid SenderUserId { get; set; }

        /// <summary>
        /// Gets or sets the message subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the message body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the message type (Custom, LocationChange, TimeChange, Reminder, Cancellation).
        /// </summary>
        public string MessageType { get; set; } = "Custom";

        /// <summary>
        /// Gets or sets when the message was sent.
        /// </summary>
        public DateTimeOffset SentDate { get; set; }

        /// <summary>
        /// Gets or sets the number of recipients.
        /// </summary>
        public int RecipientCount { get; set; }

        // Navigation properties
        public virtual Event Event { get; set; }
        public virtual User SenderUser { get; set; }
        public virtual ICollection<EventMessageRecipient> Recipients { get; set; }
        public virtual ICollection<MessageReport> Reports { get; set; }
    }
}
```

**New Entity: EventMessageRecipient**
```csharp
// New file: TrashMob.Models/EventMessageRecipient.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Tracks message delivery and read status for each recipient.
    /// </summary>
    public class EventMessageRecipient : KeyedModel
    {
        /// <summary>
        /// Gets or sets the message identifier.
        /// </summary>
        public Guid EventMessageId { get; set; }

        /// <summary>
        /// Gets or sets the recipient's user identifier.
        /// </summary>
        public Guid RecipientUserId { get; set; }

        /// <summary>
        /// Gets or sets when the message was delivered.
        /// </summary>
        public DateTimeOffset? DeliveredDate { get; set; }

        /// <summary>
        /// Gets or sets when the message was read.
        /// </summary>
        public DateTimeOffset? ReadDate { get; set; }

        // Navigation properties
        public virtual EventMessage EventMessage { get; set; }
        public virtual User RecipientUser { get; set; }
    }
}
```

**New Entity: MessageTemplate (lookup table)**
```csharp
// New file: TrashMob.Models/MessageTemplate.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a canned message template for common event communications.
    /// </summary>
    public class MessageTemplate : LookupModel
    {
        /// <summary>
        /// Gets or sets the template subject.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the template body.
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the message type this template is for.
        /// </summary>
        public string MessageType { get; set; }

        /// <summary>
        /// Gets or sets whether this template is active.
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}
```

**New Entity: MessageReport**
```csharp
// New file: TrashMob.Models/MessageReport.cs
namespace TrashMob.Models
{
    /// <summary>
    /// Represents a user report of an abusive or inappropriate message.
    /// </summary>
    public class MessageReport : KeyedModel
    {
        /// <summary>
        /// Gets or sets the reported message identifier.
        /// </summary>
        public Guid EventMessageId { get; set; }

        /// <summary>
        /// Gets or sets the reporting user's identifier.
        /// </summary>
        public Guid ReporterUserId { get; set; }

        /// <summary>
        /// Gets or sets the reason for the report.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets when the report was submitted.
        /// </summary>
        public DateTimeOffset ReportDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the admin who reviewed the report.
        /// </summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the report was reviewed.
        /// </summary>
        public DateTimeOffset? ReviewedDate { get; set; }

        /// <summary>
        /// Gets or sets the outcome (Dismissed, Warning, MessageRemoved, UserSuspended).
        /// </summary>
        public string ReviewOutcome { get; set; }

        // Navigation properties
        public virtual EventMessage EventMessage { get; set; }
        public virtual User ReporterUser { get; set; }
        public virtual User ReviewedByUser { get; set; }
    }
}
```

**DbContext Configuration (in MobDbContext.cs):**
```csharp
modelBuilder.Entity<EventMessage>(entity =>
{
    entity.Property(e => e.Subject).HasMaxLength(200);
    entity.Property(e => e.MessageType).HasMaxLength(50);

    entity.HasOne(e => e.Event)
        .WithMany()
        .HasForeignKey(e => e.EventId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.SenderUser)
        .WithMany()
        .HasForeignKey(e => e.SenderUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.EventId);
    entity.HasIndex(e => e.SenderUserId);
});

modelBuilder.Entity<EventMessageRecipient>(entity =>
{
    entity.HasOne(e => e.EventMessage)
        .WithMany(m => m.Recipients)
        .HasForeignKey(e => e.EventMessageId)
        .OnDelete(DeleteBehavior.Cascade);

    entity.HasOne(e => e.RecipientUser)
        .WithMany()
        .HasForeignKey(e => e.RecipientUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.RecipientUserId);
});

modelBuilder.Entity<MessageTemplate>(entity =>
{
    entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
    entity.Property(e => e.Subject).HasMaxLength(200).IsRequired();
    entity.Property(e => e.MessageType).HasMaxLength(50).IsRequired();
});

modelBuilder.Entity<MessageReport>(entity =>
{
    entity.Property(e => e.Reason).HasMaxLength(500).IsRequired();
    entity.Property(e => e.ReviewOutcome).HasMaxLength(50);

    entity.HasOne(e => e.EventMessage)
        .WithMany(m => m.Reports)
        .HasForeignKey(e => e.EventMessageId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.ReporterUser)
        .WithMany()
        .HasForeignKey(e => e.ReporterUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasOne(e => e.ReviewedByUser)
        .WithMany()
        .HasForeignKey(e => e.ReviewedByUserId)
        .OnDelete(DeleteBehavior.NoAction);

    entity.HasIndex(e => e.ReviewedDate)
        .HasFilter("[ReviewedDate] IS NULL");
});
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
