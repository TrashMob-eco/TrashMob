namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Maps;

[QueryProperty(nameof(LitterReportId), nameof(LitterReportId))]
public partial class CreateLitterImagePage : ContentPage
{
    private readonly CreateLitterImageViewModel _viewModel;
    
    public CreateLitterImagePage(CreateLitterImageViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Notify = Notify;
        _viewModel.NotifyError = NotifyError;
        _viewModel.Navigation = Navigation;
        BindingContext = _viewModel;
    }

    public string LitterReportId { get; set; }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.Init(new Guid(LitterReportId));

        // Default the map zoom to the location of the event
        if (_viewModel?.LitterImageViewModel?.Address != null)
        {
            var mapSpan = new MapSpan(_viewModel.LitterImageViewModel.Address.Location, 0.05, 0.05);
            litterImageLocationsMap.MoveToRegion(mapSpan);
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

    private async Task NotifyError(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        var snackbarOptions = new SnackbarOptions
        {
            BackgroundColor = Colors.Red,
            TextColor = Colors.White,
            CornerRadius = new CornerRadius(10),
            Font = Microsoft.Maui.Font.SystemFontOfSize(14),            
        };

        string text = message;
        TimeSpan duration = TimeSpan.FromSeconds(3);

        var snackbar = Snackbar.Make(text, duration: duration, visualOptions: snackbarOptions);

        await snackbar.Show(cancellationTokenSource.Token);
    }

    private async void TakePhoto_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // save the file into local storage
                _viewModel.LocalFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(_viewModel.LocalFilePath);

                await sourceStream.CopyToAsync(localFileStream);

                litterPhoto.Source = _viewModel.LocalFilePath;
                litterPhoto.IsVisible = true;
            }
        }

        await _viewModel.UpdateLocation();
    }
}