namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Maps;

[QueryProperty(nameof(LitterReportId), nameof(LitterReportId))]
public partial class ViewLitterReportPage : ContentPage
{
    private readonly ViewLitterReportViewModel viewModel;

    public ViewLitterReportPage(ViewLitterReportViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    public string LitterReportId { get; set; } = string.Empty;

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(new Guid(LitterReportId));

        var location = viewModel?.LitterImageViewModels?.FirstOrDefault()?.Address?.Location;

        if (location != null)
        {
            var mapSpan = new MapSpan(location, 0.05, 0.05);
            litterReportLocationMap.InitialMapSpanAndroid = mapSpan;
            litterReportLocationMap.MoveToRegion(mapSpan);
        }
    }
}