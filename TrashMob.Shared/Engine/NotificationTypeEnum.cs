namespace TrashMob.Shared.Engine
{
    /// <summary>
    /// Defines the types of notifications that can be sent to users.
    /// </summary>
    public enum NotificationTypeEnum
    {
        /// <summary>
        /// Notification sent to attendees after an event is completed with summary information.
        /// </summary>
        EventSummaryAttendee = 1,

        /// <summary>
        /// Reminder notification sent to event hosts about upcoming event summary requirements.
        /// </summary>
        EventSummaryHostReminder = 2,

        /// <summary>
        /// Notification about events the user is attending this week.
        /// </summary>
        UpcomingEventAttendingThisWeek = 3,

        /// <summary>
        /// Notification about events the user is attending soon (within 24 hours).
        /// </summary>
        UpcomingEventAttendingSoon = 4,

        /// <summary>
        /// Notification about events the user is hosting this week.
        /// </summary>
        UpcomingEventHostingThisWeek = 5,

        /// <summary>
        /// Notification about events the user is hosting soon (within 24 hours).
        /// </summary>
        UpcomingEventHostingSoon = 6,

        /// <summary>
        /// Notification about upcoming events in the user's area this week.
        /// </summary>
        UpcomingEventsInYourAreaThisWeek = 7,

        /// <summary>
        /// Notification about upcoming events in the user's area soon.
        /// </summary>
        UpcomingEventsInYourAreaSoon = 8,

        /// <summary>
        /// Generic notification type for miscellaneous communications.
        /// </summary>
        Generic = 9,

        /// <summary>
        /// Welcome notification sent to new TrashMob users.
        /// </summary>
        WelcomeToTrashMob = 10,

        /// <summary>
        /// Notification sent to partners when an event requests their services.
        /// </summary>
        EventPartnerRequest = 11,

        /// <summary>
        /// Notification sent to event hosts when a partner responds to a service request.
        /// </summary>
        EventPartnerResponse = 12,

        /// <summary>
        /// Notification sent when a partner request is accepted.
        /// </summary>
        PartnerRequestAccepted = 13,

        /// <summary>
        /// Notification sent when a partner request is declined.
        /// </summary>
        PartnerRequestDeclined = 14,

        /// <summary>
        /// Notification sent to attendees when an event is cancelled.
        /// </summary>
        EventCancelledNotice = 15,

        /// <summary>
        /// Notification sent when a contact request is received.
        /// </summary>
        ContactRequestReceived = 16,

        /// <summary>
        /// Notification sent to attendees when event details are updated.
        /// </summary>
        EventUpdatedNotice = 17,

        /// <summary>
        /// Weekly reminder notification sent to event hosts about their upcoming events.
        /// </summary>
        EventSummaryHostWeekReminder = 18,

        /// <summary>
        /// Notification prompting users to update their profile location.
        /// </summary>
        UserProfileUpdateLocation = 19,

        /// <summary>
        /// Invitation sent to government entities to become partners.
        /// </summary>
        InviteGovernmentPartner = 20,

        /// <summary>
        /// Invitation sent to businesses to become partners.
        /// </summary>
        InviteBusinessPartner = 21,

        /// <summary>
        /// Invitation sent to new users to become partner administrators.
        /// </summary>
        InviteNewUserToBePartnerAdmin = 22,

        /// <summary>
        /// Invitation sent to existing users to become partner administrators.
        /// </summary>
        InviteExistingUserToBePartnerAdmin = 23,

        /// <summary>
        /// Notification sent to partners requesting pickup services for an event.
        /// </summary>
        EventPartnerPickupRequest = 24,

        /// <summary>
        /// Notification sent when a partner request is automatically approved.
        /// </summary>
        EventPartnerRequestAutoApproved = 25,

        /// <summary>
        /// Notification sent to the creator when their litter report is marked as cleaned.
        /// </summary>
        LitterReportCleaned = 26,

        /// <summary>
        /// Weekly digest notification about new litter reports in the user's area.
        /// </summary>
        WeeklyLitterReportDigest = 27,

        /// <summary>
        /// Notification sent when a user is promoted to event co-lead.
        /// </summary>
        EventCoLeadAdded = 28,

        /// <summary>
        /// Notification sent when a user is removed as event co-lead.
        /// </summary>
        EventCoLeadRemoved = 29,

        /// <summary>
        /// Notification sent to admins when user feedback is submitted.
        /// </summary>
        UserFeedbackReceived = 30,

        /// <summary>
        /// Notification sent to the photo uploader when their photo is removed by moderation.
        /// </summary>
        PhotoRemoved = 31,

        /// <summary>
        /// Notification sent to admins when a photo is flagged for review.
        /// </summary>
        PhotoFlagged = 32,

        /// <summary>
        /// Notification sent to community admins when a team submits an adoption application.
        /// </summary>
        AdoptionApplicationSubmitted = 33,

        /// <summary>
        /// Notification sent to team leads when their adoption application is approved.
        /// </summary>
        AdoptionApplicationApproved = 34,

        /// <summary>
        /// Notification sent to team leads when their adoption application is rejected.
        /// </summary>
        AdoptionApplicationRejected = 35,

        /// <summary>
        /// Reminder notification sent to users when their waiver is expiring soon.
        /// </summary>
        WaiverExpiringReminder = 36,

        /// <summary>
        /// Invitation sent to potential volunteers to join TrashMob.
        /// </summary>
        InviteToJoinTrashMob = 37,

        /// <summary>
        /// Invitation sent to potential volunteers to join a specific community.
        /// </summary>
        InviteToJoinCommunity = 38,

        /// <summary>
        /// Invitation sent to potential volunteers to join a specific team.
        /// </summary>
        InviteToJoinTeam = 39,

        /// <summary>
        /// Initial outreach email sent to a community prospect introducing TrashMob.
        /// </summary>
        ProspectOutreachInitial = 40,

        /// <summary>
        /// Follow-up outreach email sent to a community prospect.
        /// </summary>
        ProspectOutreachFollowUp = 41,

        /// <summary>
        /// Value-add outreach email sharing stats and impact data with a community prospect.
        /// </summary>
        ProspectOutreachValueAdd = 42,

        /// <summary>
        /// Final gentle follow-up outreach email sent to a community prospect.
        /// </summary>
        ProspectOutreachFinal = 43,

        /// <summary>
        /// Welcome email sent to a community partner after prospect-to-partner conversion.
        /// </summary>
        CommunityWelcome = 44,

        /// <summary>
        /// Notification sent to team members about upcoming team events this week.
        /// </summary>
        UpcomingTeamEventsThisWeek = 45,

        /// <summary>
        /// Notification sent to team members about upcoming team events soon (within 24 hours).
        /// </summary>
        UpcomingTeamEventsSoon = 46,
    }
}