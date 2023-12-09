namespace TrashMobMobile
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
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

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            await _viewModel.Init();
        }
    }
}
