namespace TrashMob.Shared.Engine
{
    public enum NotificationTypeEnum    
    {
        EventSummaryAttendee = 1,
        EventSummaryHostReminder = 2,
        UpcomingEventAttendingThisWeek = 3,
        UpcomingEventAttendingSoon = 4,
        UpcomingEventHostingThisWeek = 5,
        UpcomingEventHostingSoon = 6,
        UpcomingEventsInYourAreaThisWeek = 7,
        UpcomingEventsInYourAreaSoon = 8,
        Generic = 9,
        WelcomeToTrashMob = 10,
        EventPartnerRequest = 11,
        EventPartnerResponse = 12,
        PartnerRequestAccepted = 13,
        PartnerRequestDeclined = 14,
        EventCancelledNotice = 15,
        ContactRequestReceived = 16,
        EventUpdatedNotice = 17,
        EventSummaryHostWeekReminder = 18,
        UserProfileUpdateLocation = 19,
        InviteGovernmentPartner = 20,
        InviteBusinessPartner = 21
    }
}
