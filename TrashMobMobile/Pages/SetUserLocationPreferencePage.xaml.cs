namespace TrashMobMobile.Pages;

using System.ComponentModel;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class SetUserLocationPreferencePage : ContentPage
{
    private readonly UserLocationPreferenceViewModel viewModel;

    public SetUserLocationPreferencePage(UserLocationPreferenceViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;

        viewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await viewModel.Init();

        if (viewModel?.Address?.Latitude != null && viewModel?.Address?.Longitude != null)
        {
            var center = new Location(viewModel.Address.Latitude.Value, viewModel.Address.Longitude.Value);
            var mapSpan = MapSpan.FromCenterAndRadius(center, GetTravelRadius());
            userLocationMap.InitialMapSpanAndroid = mapSpan;
            userLocationMap.MoveToRegion(mapSpan);
            UpdateRadiusCircle();
        }
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        await viewModel.ChangeLocation(e.Location);
        UpdateRadiusCircle();
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName is nameof(viewModel.TravelDistance) or nameof(viewModel.Units))
        {
            UpdateRadiusCircle();
        }
    }

    private void UpdateRadiusCircle()
    {
        if (viewModel?.Address?.Latitude == null || viewModel?.Address?.Longitude == null)
            return;

        userLocationMap.MapElements.Clear();

        var center = new Location(viewModel.Address.Latitude.Value, viewModel.Address.Longitude.Value);
        var radius = GetTravelRadius();

        var circle = new Circle
        {
            Center = center,
            Radius = radius,
            StrokeColor = Color.FromArgb("#80005B4C"),
            StrokeWidth = 2,
            FillColor = Color.FromArgb("#20005B4C"),
        };

        userLocationMap.MapElements.Add(circle);

        // Zoom map to fit the circle
        var mapSpan = MapSpan.FromCenterAndRadius(center, radius);
        userLocationMap.MoveToRegion(mapSpan);
    }

    private Distance GetTravelRadius()
    {
        var distance = viewModel.TravelDistance > 0 ? viewModel.TravelDistance : 1;
        return viewModel.Units == "Miles"
            ? Distance.FromMiles(distance)
            : Distance.FromKilometers(distance);
    }
}