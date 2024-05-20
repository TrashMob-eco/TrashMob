namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Maps;

[QueryProperty(nameof(LitterReportId), nameof(LitterReportId))]
public partial class EditLitterReportPage : ContentPage
{
    private readonly EditLitterReportViewModel _viewModel;
    public string LitterReportId { get; set; }

    public EditLitterReportPage(EditLitterReportViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Navigation = Navigation;
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

    private async void TakePhoto_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // save the file into local storage
                _viewModel.LocalFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(_viewModel.LocalFilePath);

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