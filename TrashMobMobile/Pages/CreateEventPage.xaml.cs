namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class CreateEventPage : ContentPage
{
    private readonly CreateEventViewModel _viewModel;

    public CreateEventPage(CreateEventViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Notify = Notify;
        _viewModel.NotifyError = NotifyError;
        _viewModel.Navigation = Navigation;
        BindingContext = _viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.Init();

        if (_viewModel?.UserLocation?.Location != null)
        {
            var mapSpan =
                new MapSpan(
                    new Location(_viewModel.UserLocation.Location.Latitude, _viewModel.UserLocation.Location.Longitude),
                    0.05, 0.05);
            eventLocationMap.MoveToRegion(mapSpan);
        }
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        await _viewModel.ChangeLocation(e.Location);
    }

    // TODO: These can be moved to a service/abstraction
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