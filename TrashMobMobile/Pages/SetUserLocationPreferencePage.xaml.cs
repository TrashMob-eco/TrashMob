namespace TrashMobMobile.Pages;

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
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        await viewModel.Init();

        if (viewModel?.Address?.Latitude != null && viewModel?.Address?.Longitude != null)
        {
            var mapSpan =
                new MapSpan(new Location(viewModel.Address.Latitude.Value, viewModel.Address.Longitude.Value), 0.01,
                    0.01);
            userLocationMap.MoveToRegion(mapSpan);
        }
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        await viewModel.ChangeLocation(e.Location);
    }
}