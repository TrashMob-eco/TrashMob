namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Maps;

public partial class SearchLitterReportsPage : ContentPage
{
    private readonly SearchLitterReportsViewModel _viewModel;

    public SearchLitterReportsPage(SearchLitterReportsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Notify = Notify;
        _viewModel.Navigation = Navigation;
        BindingContext = _viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.Init();

        if (_viewModel?.UserLocation?.Location != null)
        {
            var mapSpan = new MapSpan(new Location(_viewModel.UserLocation.Location.Latitude, _viewModel.UserLocation.Location.Longitude), 0.05, 0.05);
            litterImagesMap.MoveToRegion(mapSpan);
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

    private async void OnReportStatusRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked)
        {
            _viewModel.ReportStatus = (string)radioButton.Content;
            await _viewModel.Init();
        }
    }
}