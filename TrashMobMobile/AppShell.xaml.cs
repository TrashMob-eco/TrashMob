namespace TrashMobMobile;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(CreatePickupLocationPage), typeof(CreatePickupLocationPage));
        Routing.RegisterRoute(nameof(CancelEventPage), typeof(CancelEventPage));
        Routing.RegisterRoute(nameof(ContactUsPage), typeof(ContactUsPage));
        Routing.RegisterRoute(nameof(CreateEventPage), typeof(CreateEventPage));
        Routing.RegisterRoute(nameof(CreateLitterReportPage), typeof(CreateLitterReportPage));
        Routing.RegisterRoute(nameof(EditEventPage), typeof(EditEventPage));
        Routing.RegisterRoute(nameof(EditEventPartnerLocationServicesPage), typeof(EditEventPartnerLocationServicesPage));
        Routing.RegisterRoute(nameof(EditEventSummaryPage), typeof(EditEventSummaryPage));
        Routing.RegisterRoute(nameof(EditPickupLocationPage), typeof(EditPickupLocationPage));
        Routing.RegisterRoute(nameof(ManageEventPartnersPage), typeof(ManageEventPartnersPage));
        Routing.RegisterRoute(nameof(MyDashboardPage), typeof(MyDashboardPage));
        Routing.RegisterRoute(nameof(SearchEventsPage), typeof(SearchEventsPage));
        Routing.RegisterRoute(nameof(SearchLitterReportsPage), typeof(SearchLitterReportsPage));
        Routing.RegisterRoute(nameof(SetUserLocationPreferencePage), typeof(SetUserLocationPreferencePage));
        Routing.RegisterRoute(nameof(ViewEventPage), typeof(ViewEventPage));
        Routing.RegisterRoute(nameof(ViewEventSummaryPage), typeof(ViewEventSummaryPage));
        Routing.RegisterRoute(nameof(ViewLitterReportPage), typeof(ViewLitterReportPage));
        Routing.RegisterRoute(nameof(WaiverPage), typeof(WaiverPage));
        Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
    }
}
