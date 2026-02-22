namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Extensions;
using TrashMobMobile.Controls;
using TrashMobMobile.Services;

public partial class WelcomePage : ContentPage
{
    private readonly WelcomeViewModel viewModel;
    private readonly IAppVersionCheckService appVersionCheckService;

    public WelcomePage(WelcomeViewModel viewModel, IAppVersionCheckService appVersionCheckService)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.appVersionCheckService = appVersionCheckService;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await CheckAppVersionAsync();
        await viewModel.Init();
    }

    private async Task CheckAppVersionAsync()
    {
        try
        {
            var result = await appVersionCheckService.CheckVersionAsync();

            if (result == VersionCheckResult.HardBlock)
            {
                while (true)
                {
                    var popup = new UpdateRequiredPopup(isHardBlock: true);
                    var popupResult = await this.ShowPopupAsync<string>(popup);

                    await OpenStoreAsync();
                }
            }
            else if (result == VersionCheckResult.SoftNudge)
            {
                var popup = new UpdateRequiredPopup(isHardBlock: false);
                var popupResult = await this.ShowPopupAsync<string>(popup);

                if (popupResult?.Result == UpdateRequiredPopup.UpdateNow)
                {
                    await OpenStoreAsync();
                }
            }
        }
        catch (Exception)
        {
            // If anything goes wrong with the version check, allow app to continue
        }
    }

    private static async Task OpenStoreAsync()
    {
        string storeUrl;

        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            storeUrl = "https://play.google.com/store/apps/details?id=eco.trashmob.trashmobmobileapp";
        }
        else if (DeviceInfo.Platform == DevicePlatform.iOS)
        {
            storeUrl = "https://apps.apple.com/us/app/trashmob/id1599996743";
        }
        else
        {
            storeUrl = "https://www.trashmob.eco";
        }

        await Launcher.OpenAsync(new Uri(storeUrl));
    }
}
