namespace TrashMobMobile
{
    using Microsoft.Maui.Maps;

    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            _viewModel.Navigation = Navigation;
            BindingContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigatedToEventArgs args)
        {
            base.OnNavigatedTo(args);
            await _viewModel.Init();
            var mapSpan = new MapSpan(new Location(_viewModel.UserLocation.Location.Latitude, _viewModel.UserLocation.Location.Longitude), 0.01, 0.01);
            upcomingEventsMap.MoveToRegion(mapSpan);
        }
    }
}
