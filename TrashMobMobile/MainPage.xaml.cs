namespace TrashMobMobile
{
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
                mainMap.InitialMapSpanAndroid = mapSpan;
                mainMap.MoveToRegion(mapSpan);
            }
        }

        private async void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
        {
            var p = (Pin)sender;
            var automationId = p.AutomationId;
            var addressParts = automationId.Split(':');
            var addressType = addressParts[0];
            var parentId = addressParts[1];

            if (addressType == AddressType.Event.ToString())
            {
                await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={parentId}");
            }
            else if (addressType == AddressType.LitterImage.ToString())
            {
                await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={parentId}");
            }
        }
    }
}