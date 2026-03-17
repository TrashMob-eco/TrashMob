#nullable disable

namespace TrashMob.Models.Poco.V2;

using System;
using System.Collections.Generic;

/// <summary>Request body for linking an event to an adoption.</summary>
public class LinkEventRequest
{
    /// <summary>Gets or sets optional notes about why this event is linked.</summary>
    public string Notes { get; set; }
}

/// <summary>Request body for rejecting a community adoption.</summary>
public class RejectAdoptionRequest
{
    /// <summary>Gets or sets the reason for rejection.</summary>
    public string RejectionReason { get; set; }
}

/// <summary>Response returned when an event attendee registration is blocked by a missing waiver.</summary>
public class WaiverRequiredResponse
{
    /// <summary>Gets or sets the error message.</summary>
    public string Message { get; set; }

    /// <summary>Gets or sets the count of required waivers that need to be signed.</summary>
    public int RequiredWaiverCount { get; set; }

    /// <summary>Gets or sets the IDs of the waiver versions that need to be signed.</summary>
    public List<Guid> RequiredWaiverIds { get; set; }
}

/// <summary>SendGrid webhook event payload.</summary>
public class SendGridEvent
{
    /// <summary>Gets or sets the event type (delivered, open, click, bounce, dropped, unsubscribe, etc.).</summary>
    public string Event { get; set; }

    /// <summary>Gets or sets the email address.</summary>
    public string Email { get; set; }

    /// <summary>Gets or sets the Unix timestamp.</summary>
    public long Timestamp { get; set; }

    /// <summary>Gets or sets the SendGrid message ID.</summary>
    public string SgMessageId { get; set; }

    /// <summary>Gets or sets the newsletter ID from custom arguments.</summary>
    public string NewsletterId { get; set; }

    /// <summary>Gets the newsletter ID as a GUID if valid.</summary>
    public Guid? NewsletterIdValue
    {
        get
        {
            if (Guid.TryParse(NewsletterId, out var id))
            {
                return id;
            }
            return null;
        }
    }

    /// <summary>Gets or sets the bounce reason if applicable.</summary>
    public string Reason { get; set; }

    /// <summary>Gets or sets the clicked URL if applicable.</summary>
    public string Url { get; set; }
}

