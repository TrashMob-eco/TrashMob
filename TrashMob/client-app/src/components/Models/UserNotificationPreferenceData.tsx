import { Guid } from "guid-typescript";

class UserNotificationPreferenceData {
    id: string = Guid.createEmpty().toString();
    userId: string = Guid.createEmpty().toString();
    userNotificationTypeId: number = 0;
    isOptedOut: boolean = false;
    userFriendlyName: string = "";
}

export default UserNotificationPreferenceData;

export enum UserNotificationTypes {
    EventSummaryAttendee = 1,
    EventSummaryHostReminder = 2,
    UpcomingEventAttendingThisWeek = 3,
    UpcomingEventAttendingToday = 4,
    UpcomingEventHostingThisWeek = 5,
    UpcomingEventHostingToday = 6,
    UpcomingEventsInYourAreaThisWeek = 7,
    UpcomingEventsInYourAreaToday = 8,
}

export const UserNotificationPreferenceDefaults: UserNotificationPreferenceData[] = [
    {
        id: Guid.createEmpty().toString(),
        userId: Guid.createEmpty.toString(),
        userNotificationTypeId: UserNotificationTypes.EventSummaryAttendee,
        isOptedOut: false,
        userFriendlyName: "Opt out of Post Event Summary"
    },
    {
        id: Guid.createEmpty().toString(),
        userId: Guid.createEmpty.toString(),
        userNotificationTypeId: UserNotificationTypes.EventSummaryHostReminder,
        isOptedOut: false,
        userFriendlyName: "Opt out of Event Summary Reminder for events you have lead"
    },
    {
        id: Guid.createEmpty().toString(),
        userId: Guid.createEmpty.toString(),
        userNotificationTypeId: UserNotificationTypes.UpcomingEventAttendingThisWeek,
        isOptedOut: false,
        userFriendlyName: "Opt out of notifications for events upcoming this week you are attending"
    },
    {
        id: Guid.createEmpty().toString(),
        userId: Guid.createEmpty.toString(),
        userNotificationTypeId: UserNotificationTypes.UpcomingEventAttendingToday,
        isOptedOut: false,
        userFriendlyName: "Opt out of notifications for events happening today you are attending"
    },
    {
        id: Guid.createEmpty().toString(),
        userId: Guid.createEmpty.toString(),
        userNotificationTypeId: UserNotificationTypes.UpcomingEventHostingThisWeek,
        isOptedOut: false,
        userFriendlyName: "Opt out of notifications for events upcoming this week you are leading"
    },
    {
        id: Guid.createEmpty().toString(),
        userId: Guid.createEmpty.toString(),
        userNotificationTypeId: UserNotificationTypes.UpcomingEventHostingToday,
        isOptedOut: false,
        userFriendlyName: "Opt out of notifications for events happening today you are leading"
    },
    {
        id: Guid.createEmpty().toString(),
        userId: Guid.createEmpty.toString(),
        userNotificationTypeId: UserNotificationTypes.UpcomingEventsInYourAreaThisWeek,
        isOptedOut: false,
        userFriendlyName: "Opt out of notification for new events upcoming in your area this week"
    },
    {
        id: Guid.createEmpty().toString(),
        userId: Guid.createEmpty.toString(),
        userNotificationTypeId: UserNotificationTypes.UpcomingEventsInYourAreaToday,
        isOptedOut: false,
        userFriendlyName: "Opt out of notification for new events happening in your area today"
    },
]