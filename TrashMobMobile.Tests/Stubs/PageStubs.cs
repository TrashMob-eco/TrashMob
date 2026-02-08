// Stub classes so linked ViewModel source files compile in the test project.

namespace TrashMobMobile;

using TrashMob.Models;

// App stub - provides the static CurrentUser property used by some ViewModels
internal partial class App : Application
{
    public static User? CurrentUser { get; set; }
}

// Page stubs for Shell navigation targets (nameof() references)
internal class ViewEventPage { }
internal class WelcomePage { }
internal class MyDashboardPage { }
internal class CreateLitterReportPage { }
internal class SearchLitterReportsPage { }
internal class CreateEventPage { }
internal class SetUserLocationPreferencePage { }
internal class SearchEventsPage { }
internal class CancelEventPage { }
internal class EditEventPartnerLocationServicesPage { }
internal class WaiverPage { }
internal class ViewLitterReportPage { }
internal class EditEventPage { }
internal class ViewEventSummaryPage { }
internal class EditLitterReportPage { }
internal class EditEventSummaryPage { }
internal class CreatePickupLocationPage { }
internal class EditPickupLocationPage { }
internal class ViewPickupLocationPage { }
internal class MainTabsPage { }
internal class ContactUsPage { }
internal class LogoutPage { }
internal class AchievementsPage { }
internal class BrowseTeamsPage { }
internal class LeaderboardsPage { }
internal class ViewTeamPage { }
