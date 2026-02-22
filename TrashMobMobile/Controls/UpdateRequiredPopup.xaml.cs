namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Extensions;

public partial class UpdateRequiredPopup : ContentView
{
    public const string UpdateNow = "UpdateNow";

    public UpdateRequiredPopup(bool isHardBlock)
    {
        InitializeComponent();

        if (isHardBlock)
        {
            titleLabel.Text = "Update Required";
            messageLabel.Text = "A critical update is available. Please update the app to continue.";
            laterButton.IsVisible = false;
        }
        else
        {
            titleLabel.Text = "Update Available";
            messageLabel.Text = "A new version of TrashMob is available with improvements and new features.";
            laterButton.IsVisible = true;
        }
    }

    private async void OnUpdateNowClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync(UpdateNow);
    }

    private async void OnLaterClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }
}
