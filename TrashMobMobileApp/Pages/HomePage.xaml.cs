namespace TrashMobMobileApp.Pages;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _viewModel;

    public HomePage(HomeViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    private void ContactUs_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new ContactUsPage(new ContactUsViewModel()));
    }

    private void MyDashboard_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new MyDashboardPage(new MyDashboardViewModel()));
    }

    private void SearchEvents_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SearchEventsPage(new SearchEventsViewModel()));
    }

    private void CreateEvent_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new CreateEventPage(new CreateEventViewModel()));
    }

    private void SubmitLitterReport_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SubmitLitterReportPage(new SubmitLitterReportViewModel()));
    }

    private void SearchLitterReports_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SearchLitterReportsPage(new SearchLitterReportsViewModel()));
    }

    private void SetMyLocationPreference_Clicked(object sender, EventArgs e)
    {
        Navigation.PushAsync(new SetUserLocationPreferencePage(new UserLocationPreferenceViewModel()));
    }

    private void Logout_Clicked(object sender, EventArgs e)
    {
    }
}