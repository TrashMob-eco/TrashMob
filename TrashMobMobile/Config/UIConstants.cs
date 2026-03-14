namespace TrashMobMobile.Config;

/// <summary>
/// UI-facing string constants for filters, visibility options, and status labels.
/// These are displayed in pickers, radio buttons, and status labels throughout the app.
/// </summary>
public static class UIConstants
{
    // Event filter types (used in ExploreViewModel, SearchEventsViewModel)
    public const string EventFilterUpcoming = "Upcoming";
    public const string EventFilterCompleted = "Completed";

    // Litter report status filters (used in ExploreViewModel, SearchLitterReportsViewModel)
    public const string LitterFilterNew = "New";
    public const string LitterFilterAssigned = "Assigned";
    public const string LitterFilterCleaned = "Cleaned";

    // Event visibility options (used in CreateEventViewModel, EditEventViewModel, EventViewModel, EventExtensions)
    public const string VisibilityPublic = "Public";
    public const string VisibilityTeamOnly = "Team Only";
    public const string VisibilityPrivate = "Private";
}
