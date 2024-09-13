namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Maps;

[QueryProperty(nameof(EventId), nameof(EventId))]
public partial class CreatePickupLocationPage : ContentPage
{
    private readonly CreatePickupLocationViewModel viewModel;

    public CreatePickupLocationPage(CreatePickupLocationViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    public string EventId { get; set; }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(new Guid(EventId));

        // Default the map zoom to the location of the event
        if (viewModel?.EventViewModel?.Address != null)
        {
            var mapSpan = new MapSpan(viewModel.EventViewModel.Address.Location, 0.05, 0.05);
            pickupLocationMap.MoveToRegion(mapSpan);
        }
    }

    private async void TakePhoto_Clicked(object sender, EventArgs e)
    {
        if (MediaPicker.Default.IsCaptureSupported)
        {
            var photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // save the file into local storage
                viewModel.LocalFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                using var sourceStream = await photo.OpenReadAsync();
                using var localFileStream = File.OpenWrite(viewModel.LocalFilePath);

                await sourceStream.CopyToAsync(localFileStream);

                pickupPhoto.Source = viewModel.LocalFilePath;
                pickupPhoto.IsVisible = true;
            }
        }

        await viewModel.UpdateLocation();
    }
}