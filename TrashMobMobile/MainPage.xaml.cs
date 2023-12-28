namespace TrashMobMobile
{
    using CommunityToolkit.Maui.Alerts;
    using CommunityToolkit.Maui.Core;
    using Microsoft.Maui.Maps;

    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _viewModel.Navigation = Navigation;
            _viewModel.Notify = Notify;
            BindingContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            await _viewModel.Init();

            if (_viewModel?.UserLocation?.Location != null)
            {
                var mapSpan = new MapSpan(new Location(_viewModel.UserLocation.Location.Latitude, _viewModel.UserLocation.Location.Longitude), 0.05, 0.05);
                upcomingEventsMap.MoveToRegion(mapSpan);
            }
        }

        private async Task Notify(string message)
        {
            CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

            ToastDuration duration = ToastDuration.Short;
            double fontSize = 14;

            var toast = Toast.Make(message, duration, fontSize);
            await toast.Show(cancellationTokenSource.Token);
        }
    }
}
