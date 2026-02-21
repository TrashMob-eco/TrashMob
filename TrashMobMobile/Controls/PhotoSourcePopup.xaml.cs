namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Extensions;

public partial class PhotoSourcePopup : ContentView
{
    public const string TakePhoto = "TakePhoto";
    public const string ChooseGallery = "ChooseGallery";

    public string? SelectedAction { get; private set; }

    public PhotoSourcePopup()
    {
        InitializeComponent();
    }

    private async void OnTakePhotoClicked(object? sender, EventArgs e)
    {
        SelectedAction = TakePhoto;
        await Shell.Current.ClosePopupAsync(SelectedAction);
    }

    private async void OnChooseGalleryClicked(object? sender, EventArgs e)
    {
        SelectedAction = ChooseGallery;
        await Shell.Current.ClosePopupAsync(SelectedAction);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }
}
