namespace TrashMobMobile.Pages;

public partial class MorePage : ContentPage
{
    public MorePage()
    {
        InitializeComponent();
        VersionLabel.Text = $"Version {AppInfo.Current.VersionString} ({AppInfo.Current.BuildString})";
    }

    private async void OnPrivacyPolicyClicked(object sender, EventArgs e)
    {
        var uri = new Uri("https://www.trashmob.eco/privacypolicy");
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
    }

    private async void OnTermsOfUseClicked(object sender, EventArgs e)
    {
        var uri = new Uri("https://www.trashmob.eco/termsofservice");
        await Browser.Default.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
    }

    private async void OnSignWaiverClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(WaiverPage));
    }

    private async void OnContactUsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ContactUsPage));
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(LogoutPage));
    }
}
