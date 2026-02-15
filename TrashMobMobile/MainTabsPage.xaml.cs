namespace TrashMobMobile;

using CommunityToolkit.Maui.Extensions;
using TrashMobMobile.Controls;
using TrashMobMobile.Pages;
using TrashMobMobile.Services;

public partial class MainTabsPage : Shell
{
    private readonly IWaiverManager waiverManager;

    public MainTabsPage(IWaiverManager waiverManager)
    {
        InitializeComponent();
        this.waiverManager = waiverManager;

        Navigating += OnNavigating;
    }

    private async void OnNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        if (e.Target?.Location?.OriginalString?.Contains("QuickActionPlaceholderPage") == true)
        {
            e.Cancel();

            try
            {
                var popup = new QuickActionPopup();
                var result = await this.ShowPopupAsync<string>(popup);

                var action = result?.Result;

                if (action == QuickActionPopup.CreateEvent)
                {
                    if (!await waiverManager.HasUserSignedAllRequiredWaiversAsync())
                    {
                        await GoToAsync(nameof(WaiverListPage));
                        return;
                    }

                    await GoToAsync(nameof(CreateEventPage));
                }
                else if (action == QuickActionPopup.ReportLitter)
                {
                    await GoToAsync(nameof(CreateLitterReportPage));
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
                await DisplayAlertAsync("Error", "An error occurred. Please try again.", "OK");
            }
        }
    }
}
