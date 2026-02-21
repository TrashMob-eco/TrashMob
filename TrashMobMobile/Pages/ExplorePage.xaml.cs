namespace TrashMobMobile.Pages;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class ExplorePage : ContentPage
{
    private readonly ExploreViewModel viewModel;

    public ExplorePage(ExploreViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = this.viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init();

        if (viewModel.HasUserLocation && viewModel.UserMapSpan != null)
        {
            exploreMap.InitialMapSpanAndroid = viewModel.UserMapSpan;
            exploreMap.MoveToRegion(viewModel.UserMapSpan);
            UpdateRadiusCircle();
        }
    }

    private void UpdateRadiusCircle()
    {
        if (!viewModel.HasUserLocation || viewModel.UserLocation.Latitude == null || viewModel.UserLocation.Longitude == null)
            return;

        exploreMap.MapElements.Clear();

        var center = new Location(viewModel.UserLocation.Latitude.Value, viewModel.UserLocation.Longitude.Value);
        var radius = viewModel.GetTravelRadius();

        var circle = new Circle
        {
            Center = center,
            Radius = radius,
            StrokeColor = Color.FromArgb("#80005B4C"),
            StrokeWidth = 2,
            FillColor = Color.FromArgb("#20005B4C"),
        };

        exploreMap.MapElements.Add(circle);
    }

    private async void Pin_InfoWindowClicked(object? sender, PinClickedEventArgs e)
    {
        if (sender is not Pin p)
        {
            return;
        }

        var automationId = p.AutomationId;
        if (string.IsNullOrEmpty(automationId))
        {
            return;
        }

        var parts = automationId.Split(':');
        if (parts.Length < 2)
        {
            return;
        }

        var type = parts[0];
        var id = parts[1];

        if (type == "Event")
        {
            await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={id}");
        }
        else if (type == "LitterImage")
        {
            await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={id}");
        }
    }
}
