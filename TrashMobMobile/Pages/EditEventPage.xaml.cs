namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

[QueryProperty(nameof(EventId), nameof(EventId))]
public partial class EditEventPage : ContentPage
{
    private readonly EditEventViewModel _viewModel;

    public EditEventPage(EditEventViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Notify = Notify;
        _viewModel.NotifyError = NotifyError;
        _viewModel.Navigation = Navigation;
        BindingContext = _viewModel;
    }

    public string EventId { get; set; }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.Init(new Guid(EventId));

        if (_viewModel?.EventViewModel?.Address?.Location != null)
        {
            var mapSpan = new MapSpan(_viewModel.EventViewModel.Address.Location, 0.05, 0.05);
            eventLocationMap.MoveToRegion(mapSpan);
        }
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        await _viewModel.ChangeLocation(e.Location);
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