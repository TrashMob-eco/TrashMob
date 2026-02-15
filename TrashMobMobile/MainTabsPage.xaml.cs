namespace TrashMobMobile;

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

            var action = await DisplayActionSheetAsync("Quick Actions", "Cancel", null, "Create Event", "Report Litter");

            if (action == "Create Event")
            {
                if (!await waiverManager.HasUserSignedTrashMobWaiverAsync())
                {
                    await GoToAsync(nameof(WaiverPage));
                    return;
                }

                await GoToAsync(nameof(CreateEventPage));
            }
            else if (action == "Report Litter")
            {
                await GoToAsync(nameof(CreateLitterReportPage));
            }
        }
    }
}
