namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Maps;

[QueryProperty(nameof(PickupLocationId), nameof(PickupLocationId))]
public partial class ViewPickupLocationPage : ContentPage
{
    private readonly ViewPickupLocationViewModel viewModel;

    public ViewPickupLocationPage(ViewPickupLocationViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    public string PickupLocationId { get; set; }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(new Guid(PickupLocationId));

        if (viewModel?.PickupLocationViewModel?.Address?.Location != null)
        {
            var mapSpan = new MapSpan(viewModel.PickupLocationViewModel.Address.Location, 0.05, 0.05);
            pickupLocationsMap.MoveToRegion(mapSpan);
        }
    }
}