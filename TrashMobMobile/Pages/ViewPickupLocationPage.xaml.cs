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
        this.viewModel.Notify = Notify;
        this.viewModel.NotifyError = NotifyError;
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

    private async Task Notify(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(message, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);
    }

    private async Task NotifyError(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Colors.Red,
            TextColor = Colors.White,
            CornerRadius = new CornerRadius(10),
            Font = Microsoft.Maui.Font.SystemFontOfSize(14),
        };

        var text = message;
        var duration = TimeSpan.FromSeconds(3);

        var snackbar = Snackbar.Make(text, duration: duration, visualOptions: snackbarOptions);

        await snackbar.Show(cancellationTokenSource.Token);
    }
}