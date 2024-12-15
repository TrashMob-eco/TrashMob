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
        Routing.RegisterRoute(nameof(EditEventPartnerLocationServicesPage),
            typeof(EditEventPartnerLocationServicesPage));
        Routing.RegisterRoute(nameof(EditEventSummaryPage), typeof(EditEventSummaryPage));
        Routing.RegisterRoute(nameof(EditLitterReportPage), typeof(EditLitterReportPage));
        Routing.RegisterRoute(nameof(EditPickupLocationPage), typeof(EditPickupLocationPage));
        Routing.RegisterRoute(nameof(LogoutPage), typeof(LogoutPage));
        Routing.RegisterRoute(nameof(ManageEventPartnersPage), typeof(ManageEventPartnersPage));
        Routing.RegisterRoute(nameof(MyDashboardPage), typeof(MyDashboardPage));
        Routing.RegisterRoute(nameof(SearchEventsPage), typeof(SearchEventsPage));
        Routing.RegisterRoute(nameof(SearchLitterReportsPage), typeof(SearchLitterReportsPage));
        Routing.RegisterRoute(nameof(SetUserLocationPreferencePage), typeof(SetUserLocationPreferencePage));
        Routing.RegisterRoute(nameof(ViewEventPage), typeof(ViewEventPage));
        Routing.RegisterRoute(nameof(ViewEventSummaryPage), typeof(ViewEventSummaryPage));
        Routing.RegisterRoute(nameof(ViewLitterReportPage), typeof(ViewLitterReportPage));
        Routing.RegisterRoute(nameof(ViewPickupLocationPage), typeof(ViewPickupLocationPage));
        Routing.RegisterRoute(nameof(WaiverPage), typeof(WaiverPage));
        Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
    }

    public async void OnMyDashboardClicked(object sender, EventArgs e)
    {
        Current.FlyoutIsPresented = false;
        await Current.GoToAsync($"{nameof(MyDashboardPage)}");
    }

    public async void OnSetMyLocationClicked(object sender, EventArgs e)
    {
        Current.FlyoutIsPresented = false;
        await Current.GoToAsync($"{nameof(SetUserLocationPreferencePage)}");
    }

    public async void OnPrivacyPolicyClicked(object sender, EventArgs e)
    {
        Current.FlyoutIsPresented = false;
        var uri = new Uri("https://www.trashmob.eco/privacypolicy");
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
    }

    public async void OnTermsOfUseClicked(object sender, EventArgs e)
    {
        Current.FlyoutIsPresented = false;
        var uri = new Uri("https://www.trashmob.eco/termsofservice");
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
    }

    public async void OnSignWaiverClicked(object sender, EventArgs e)
    {
        Current.FlyoutIsPresented = false;
        await Current.GoToAsync($"{nameof(WaiverPage)}");
    }

    public async void OnContactUsClicked(object sender, EventArgs e)
    {
        Current.FlyoutIsPresented = false;
        await Current.GoToAsync(nameof(ContactUsPage));
    }

    public async void OnLogoutClicked(object sender, EventArgs e)
    {
        Current.FlyoutIsPresented = false;
        await Current.GoToAsync(nameof(LogoutPage));
    }
}