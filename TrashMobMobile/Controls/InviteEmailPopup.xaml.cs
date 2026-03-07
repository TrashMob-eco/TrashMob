namespace TrashMobMobile.Controls;

using CommunityToolkit.Maui.Extensions;

public partial class InviteEmailPopup : ContentView
{
    public InviteEmailPopup(string dependentFirstName)
    {
        InitializeComponent();
        titleLabel.Text = $"Invite {dependentFirstName} to Create Account";
    }

    private void OnEmailTextChanged(object? sender, TextChangedEventArgs e)
    {
        var text = e.NewTextValue?.Trim() ?? string.Empty;
        sendButton.IsEnabled = text.Contains('@') && text.Contains('.');
    }

    private async void OnSendClicked(object? sender, EventArgs e)
    {
        var email = emailEntry.Text?.Trim();
        if (!string.IsNullOrWhiteSpace(email))
        {
            await Shell.Current.ClosePopupAsync(email);
        }
    }

    private async void OnCancelClicked(object? sender, EventArgs e)
    {
        await Shell.Current.ClosePopupAsync();
    }
}
