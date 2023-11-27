namespace TrashMobMobileApp;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(ContactUsPage), typeof(ContactUsPage));
        Routing.RegisterRoute(nameof(CreateEventPage), typeof(CreateEventPage));
        Routing.RegisterRoute(nameof(EditEventPage), typeof(EditEventPage));
        Routing.RegisterRoute(nameof(SearchEventsPage), typeof(SearchEventsPage));
        Routing.RegisterRoute(nameof(SearchLitterReportsPage), typeof(SearchLitterReportsPage));
        Routing.RegisterRoute(nameof(SetUserLocationPreferencePage), typeof(SetUserLocationPreferencePage));
        Routing.RegisterRoute(nameof(SubmitLitterReportPage), typeof(SubmitLitterReportPage));
        Routing.RegisterRoute(nameof(ViewEventPage), typeof(ViewEventPage));
        Routing.RegisterRoute(nameof(ViewEventSummaryPage), typeof(ViewEventSummaryPage));
        Routing.RegisterRoute(nameof(ViewLitterReportPage), typeof(ViewLitterReportPage));
        Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
    }
}
