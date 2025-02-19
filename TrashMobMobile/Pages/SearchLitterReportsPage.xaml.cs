namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class SearchLitterReportsPage : ContentPage
{
    private readonly SearchLitterReportsViewModel viewModel;

    public SearchLitterReportsPage(SearchLitterReportsViewModel viewModel)
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
            litterImagesMap.InitialMapSpanAndroid = mapSpan;
            litterImagesMap.MoveToRegion(mapSpan);
        }
    }

    private async void OnReportStatusRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked)
        {
            viewModel.ReportStatus = (string)radioButton.Content;
            await viewModel.Init();
        }
    }

    private async void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
    {
        var p = (Pin)sender;

        var litterReportId = p.AutomationId;
        await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={litterReportId}");
    }
}