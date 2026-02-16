namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Extensions;

public partial class ConfirmPopup : ContentView
{
    public const string Confirmed = "Confirmed";

    public ConfirmPopup(string title, string message, string confirmText = "Confirm")
    {
        InitializeComponent();
        titleLabel.Text = title;
        messageLabel.Text = message;
        confirmButton.Text = confirmText;
    }

    private async void OnConfirmClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync(Confirmed);
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }
}
