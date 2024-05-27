namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

public partial class CreateLitterReportPage : ContentPage
{
    private readonly CreateLitterReportViewModel _viewModel;

    public CreateLitterReportPage(CreateLitterReportViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Notify = Notify;
        _viewModel.NotifyError = NotifyError;
        _viewModel.Navigation = Navigation;
        BindingContext = _viewModel;
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

    private async void TakePhoto_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // save the file into local storage
                _viewModel.LocalFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using var sourceStream = await photo.OpenReadAsync();
                using var localFileStream = File.OpenWrite(_viewModel.LocalFilePath);

                await sourceStream.CopyToAsync(localFileStream);
                await _viewModel.AddImageToCollection();
                _viewModel.ValidateReport();
            }
        }
    }

    private void DeleteLitterImage_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var litterImageViewModel = button?.BindingContext as LitterImageViewModel;

        if (litterImageViewModel != null)
        {
            _viewModel.LitterImageViewModels.Remove(litterImageViewModel);
        }

        _viewModel.ValidateReport();
    }
}