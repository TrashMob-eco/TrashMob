namespace TrashMobMobile
{
    using CommunityToolkit.Maui.Alerts;
    using CommunityToolkit.Maui.Core;
    using Microsoft.Maui.Controls.Maps;
    using Microsoft.Maui.Maps;

    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            this.viewModel = viewModel;
            this.viewModel.Navigation = Navigation;
            this.viewModel.Notify = Notify;
            BindingContext = this.viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            await viewModel.Init();

            if (viewModel?.UserLocation?.Location != null)
            {
                var mapSpan =
                    new MapSpan(
                        new Location(viewModel.UserLocation.Location.Latitude,
                            viewModel.UserLocation.Location.Longitude), 0.05, 0.05);
                upcomingEventsMap.MoveToRegion(mapSpan);
            }
        }

        private async Task Notify(string message)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var duration = ToastDuration.Short;
            double fontSize = 14;

            var toast = Toast.Make(message, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }

        private async void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
        {
            var p = (Pin)sender;

            var eventId = p.AutomationId;
            await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventId}");
        }
    }
}