namespace TrashMobMobile.Pages;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

[QueryProperty(nameof(EventId), nameof(EventId))]
public partial class ViewEventSummaryPage : ContentPage
{
    private readonly ViewEventSummaryViewModel viewModel;

    public ViewEventSummaryPage(ViewEventSummaryViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    public string EventId { get; set; }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(new Guid(EventId), UpdateRoutes);

        if (viewModel?.EventViewModel?.Address?.Location != null)
        {
            var mapSpan = new MapSpan(viewModel.EventViewModel.Address.Location, 0.01, 0.01);
            pickupLocationsMap.InitialMapSpanAndroid = mapSpan;
            pickupLocationsMap.MoveToRegion(mapSpan);
        }
    }

    private async void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
    {
        var p = (Pin)sender;

        var pickupLocationId = p.AutomationId;
        await Shell.Current.GoToAsync($"{nameof(ViewPickupLocationPage)}?LitterReportId={pickupLocationId}");
    }

    private void UpdateRoutes()
    {
        pickupLocationsMap.MapElements.Clear();

        foreach (var route in viewModel.EventAttendeeRoutes)
        {
            var polyline = new Polyline
            {
                StrokeColor = Color.FromArgb("c7d762"),
                StrokeWidth = 5,
            };

            foreach (var location in route.Locations)
            {
                polyline.Geopath.Add(new Location(location.Latitude, location.Longitude));
            }

            pickupLocationsMap.MapElements.Add(polyline);
        }
    }
}