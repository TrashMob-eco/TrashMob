namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

[QueryProperty(nameof(EventId), nameof(EventId))]
public partial class ViewEventSummaryPage : ContentPage
{
    private readonly ViewEventSummaryViewModel _viewModel;

    public ViewEventSummaryPage(ViewEventSummaryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Navigation = Navigation;
        _viewModel.Notify = Notify;
        BindingContext = _viewModel;
    }

    public string EventId { get; set; }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.Init(new Guid(EventId));

        if (_viewModel?.EventViewModel?.Address?.Location != null)
        {
            var mapSpan = new MapSpan(_viewModel.EventViewModel.Address.Location, 0.01, 0.01);
            pickupLocationsMap.MoveToRegion(mapSpan);
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

        var pickupLocationId = p.AutomationId;
        await Shell.Current.GoToAsync($"{nameof(ViewPickupLocationPage)}?LitterReportId={pickupLocationId}");
    }
}