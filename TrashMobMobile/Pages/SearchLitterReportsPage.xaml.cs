namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

public partial class SearchLitterReportsPage : ContentPage
{
    private readonly SearchLitterReportsViewModel _viewModel;

    public SearchLitterReportsPage(SearchLitterReportsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Notify = Notify;
        BindingContext = _viewModel;
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