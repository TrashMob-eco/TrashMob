namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Extensions;

public partial class PrivacyPopup : ContentView
{
    public const string Private = "Private";
    public const string EventOnly = "EventOnly";
    public const string Public = "Public";

    public string? SelectedPrivacy { get; private set; }

    public PrivacyPopup()
    {
        InitializeComponent();
    }

    private async void OnPrivateClicked(object? sender, EventArgs e)
    {
        SelectedPrivacy = Private;
        await Shell.Current.ClosePopupAsync(SelectedPrivacy);
    }

    private async void OnEventOnlyClicked(object? sender, EventArgs e)
    {
        SelectedPrivacy = EventOnly;
        await Shell.Current.ClosePopupAsync(SelectedPrivacy);
    }

    private async void OnPublicClicked(object? sender, EventArgs e)
    {
        SelectedPrivacy = Public;
        await Shell.Current.ClosePopupAsync(SelectedPrivacy);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }
}
