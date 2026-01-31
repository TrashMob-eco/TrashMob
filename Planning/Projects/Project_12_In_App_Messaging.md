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

## Minor User Protections

TrashMob allows users aged 13+ with special protections for minors (13-17). This feature must comply with privacy requirements for minors.

### Message Receipt
- Minors **can receive** messages from event leads for events they are registered for
- **No unsolicited messages** - only from leads of events the minor has explicitly joined
- No direct messaging between users (prevents grooming/inappropriate contact)

### Parental Visibility
- If minor has a **linked parent/guardian account**, parent can view all messages sent to the minor
- Parent can **disable messaging** for their minor's account
- Parent receives notification when minor reports a message

### Enhanced Moderation
- Reports involving minor recipients are **prioritized** in moderation queue
- Faster escalation timeline for abuse reports from/about minors
- Audit log includes recipient age category for compliance review

### Notification Controls
- Minors (or their parents) can **opt out** of push notifications
- Default notification settings for minors: **event logistics only** (no promotional)
- Parents can manage notification preferences for linked minor accounts

### Data Handling
- Same 1-year retention policy applies
- Parents can request **data deletion** for minor's messages
- Messages to minors included in any COPPA/privacy data export requests

### App Store Compliance
- **Apple:** Enhanced moderation required for user-generated content involving minors
- **Google:** Compliance with Families Policy for messaging features
- **Both:** Clear privacy policy disclosure about minor data handling

**Note:** Legal review required before launch to ensure compliance with COPPA, state privacy laws, and international regulations (GDPR for EU users under 16).

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
    entity.Property(e => e.Body).HasMaxLength(1000);
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

**Event Lead View (Phase 1-2) - Web & Mobile:**
- "Message Attendees" button on event page
- Message composer with template dropdown
- Character count showing remaining characters (Subject: 200, Body: 1,000)
- Message history with delivery stats
- Rate limit indicator

**Team Lead View (Phase 3) - Web & Mobile:**
- "Message Team" button on team page
- Message composer with template dropdown
- Character count showing remaining characters
- Message history with delivery stats

**Community Lead View (Phase 4/Future) - Web & Mobile:**
- "Message Community" button on community admin page
- Message composer with template dropdown
- Character count showing remaining characters
- Message history with delivery stats

**Attendee View (Web):**
- Message inbox accessible from **profile icon** in navigation
- **Unread indicator** (badge/dot) on profile icon when unread messages exist
- Inbox/notification center showing event messages
- Message history viewable on website
- Mark as read
- Report message option
- Notification preferences in user settings

**Admin View:**
- Moderation queue for reported messages
- User messaging history
- Rate limit overrides

### Mobile App Changes

**All Users:**
- Push notification for new messages (works when app is closed)
- Message inbox accessible from **profile/menu** with **unread indicator**
- Message inbox with full history
- View message details
- Report function
- Notification preferences in app settings

**Event/Team/Community Leads:**
- "Message Attendees/Members" button on event/team detail page
- Message composer with template dropdown
- Character count showing remaining characters
- Message history with delivery stats
- Same rate limits as web

### Push Notification Behavior

- **Delivery:** Push notifications appear on device lock screen/notification tray even when app is closed
- **Opt-in:** Users must grant push notification permission on first message-enabled action
- **Content preview:** Show subject line in notification; tap to open full message
- **Badge count:** Show unread message count on app icon

### User Notification Preferences

Users can configure notification settings (in both web and mobile):

| Setting | Options | Default |
|---------|---------|---------|
| **Event messages** | All, Important only, None | All |
| **Mute specific events** | Per-event toggle | Unmuted |
| **Push notifications** | Enabled/Disabled | Enabled (if permitted) |
| **Email digest** | Daily, Weekly, None | None |

**Important only** includes: cancellations, time/location changes, safety alerts

### App Store Compliance Requirements

| Requirement | Apple App Store | Google Play Store | Our Approach |
|-------------|-----------------|-------------------|--------------|
| **Push notification consent** | Required | Required | Opt-in prompt before first notification |
| **Unsubscribe mechanism** | Required | Required | Per-event mute + global disable |
| **No spam/unsolicited** | Required | Required | Rate limits; only for registered events |
| **Privacy policy disclosure** | Required | Required | Update privacy policy before launch |
| **Data retention disclosure** | Required | Recommended | 1 year retention documented |
| **Minor protections** | Enhanced moderation | Families Policy | See Minor User Protections section |
| **Content moderation** | Required for UGC | Required for UGC | Report function + admin queue |

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

1. ~~**Rate limit: how many messages per event per day?**~~
   **Decision:** 5 custom messages per event per day; system messages (cancellation, time/location changes) unlimited
   **Status:** ✅ Resolved

2. ~~**Scope for communities/teams broadcast?**~~
   **Decision:** Priority order: (1) Events first (Phase 1-2), (2) Team announcements (Phase 3), (3) Community broadcasts (Phase 4/future)
   **Status:** ✅ Resolved

3. ~~**Abuse prevention policy and enforcement?**~~
   **Decision:** Clear ToS update required; 3-strike policy (warning → temp suspension → permanent ban); legal review before launch
   **Status:** ✅ Resolved

4. ~~**Message retention period?**~~
   **Decision:** 1 year; archive older messages
   **Status:** ✅ Resolved

5. ~~**Message length limits?**~~
   **Decision:** Subject: 200 characters; Body: 1,000 characters
   **Status:** ✅ Resolved

6. ~~**Minor user privacy protections?**~~
   **Decision:** See dedicated "Minor User Protections" section; includes parental visibility, enhanced moderation, notification controls, and app store compliance requirements
   **Status:** ✅ Resolved

---

## Related Documents

- **[Project 9 - Teams](./Project_09_Teams.md)** - Future team messaging
- **[Project 10 - Community Pages](./Project_10_Community_Pages.md)** - Future community broadcasts
- **[Project 13 - Bulk Email](./Project_13_Bulk_Email_Invites.md)** - Email alternative

---

**Last Updated:** January 31, 2026
**Owner:** Product Lead
**Status:** Not Started
**Next Review:** When prioritized
