namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

public partial class CreateLitterReportPage : ContentPage
{
    private readonly CreateLitterReportViewModel viewModel;

    public CreateLitterReportPage(CreateLitterReportViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    private async void TakePhoto_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                await DisplayAlert("Photo Ok", "The photo was successful.", "Ok");

                // save the file into local storage
                viewModel.LocalFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using var sourceStream = await photo.OpenReadAsync();
                using var localFileStream = File.OpenWrite(viewModel.LocalFilePath);

                await sourceStream.CopyToAsync(localFileStream);
                await DisplayAlert("Photo Ok", "Adding photo to collection.", "Ok");
                await viewModel.AddImageToCollection();
                viewModel.ValidateReport();
            }
            else
            {
                await DisplayAlert("Photo Error", "The photo did not work.", "Ok");
            }
        }
    }

    private void DeleteLitterImage_Clicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var litterImageViewModel = button?.BindingContext as LitterImageViewModel;

        if (litterImageViewModel != null)
        {
            viewModel.LitterImageViewModels.Remove(litterImageViewModel);
        }

        viewModel.ValidateReport();
    }
}