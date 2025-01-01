namespace TrashMobMobile.Pages;

using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

[QueryProperty(nameof(EventId), nameof(EventId))]
public partial class EditEventPage : ContentPage
{
    private readonly EditEventViewModel viewModel;

    public EditEventPage(EditEventViewModel viewModel)
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
        await viewModel.Init(new Guid(EventId));
        viewModel.UpdateMapLocation = UpdateMapLocation;
        UpdateMapLocation();
    }

    private void UpdateMapLocation()
    {
        if (viewModel?.EventViewModel?.Address?.Location != null)
        {
            var mapSpan = new MapSpan(viewModel.EventViewModel.Address.Location, 0.05, 0.05);
            eventLocationMap.InitialMapSpanAndroid = mapSpan;
            eventLocationMap.MoveToRegion(mapSpan);
            litterImagesMap.InitialMapSpanAndroid = mapSpan;
            litterImagesMap.MoveToRegion(mapSpan);
        }
    }

    private async void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
    {
        var p = (Pin)sender;

        var litterReportId = p.AutomationId;
        await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={litterReportId}");
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        await viewModel.ChangeLocation(e.Location);
    }
}