namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class SearchEventsPage : ContentPage
{
    private readonly SearchEventsViewModel viewModel;

    public SearchEventsPage(SearchEventsViewModel viewModel)
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
                    new Location(viewModel.UserLocation.Location.Latitude, viewModel.UserLocation.Location.Longitude),
                    0.05, 0.05);
            eventsMap.InitialMapSpanAndroid = mapSpan;
            eventsMap.MoveToRegion(mapSpan);
        }
    }

    private async void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
    {
        var p = (Pin)sender;

        var eventId = p.AutomationId;
        await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventId}");
    }
}