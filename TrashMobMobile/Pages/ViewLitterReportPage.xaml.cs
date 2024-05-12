namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Maps;

[QueryProperty(nameof(LitterReportId), nameof(LitterReportId))]
public partial class ViewLitterReportPage : ContentPage
{
    private readonly ViewLitterReportViewModel _viewModel;
    public string LitterReportId { get; set; }

    public ViewLitterReportPage(ViewLitterReportViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Notify = Notify;
        BindingContext = _viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.Init(new Guid(LitterReportId));

        if (_viewModel?.LitterImageViewModels?.FirstOrDefault()?.Address?.Location != null)
        {
            var mapSpan = new MapSpan(_viewModel?.LitterImageViewModels?.FirstOrDefault()?.Address?.Location, 0.05, 0.05);
            litterReportLocationMap.MoveToRegion(mapSpan);
        }
    }

    private async Task Notify(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(message, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);
    }
}