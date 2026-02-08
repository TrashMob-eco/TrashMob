namespace TrashMobMobile;

using TrashMobMobile.Pages;

public partial class MainTabsPage : Shell
{
    public MainTabsPage()
    {
        InitializeComponent();

        Navigating += OnNavigating;
    }

    private async void OnNavigating(object? sender, ShellNavigatingEventArgs e)
    {
        if (e.Target?.Location?.OriginalString?.Contains("QuickActionPlaceholderPage") == true)
        {
            e.Cancel();

            var action = await DisplayActionSheet("Quick Actions", "Cancel", null, "Create Event", "Report Litter");

            if (action == "Create Event")
            {
                await GoToAsync(nameof(CreateEventPage));
            }
            else if (action == "Report Litter")
            {
                await GoToAsync(nameof(CreateLitterReportPage));
            }
        }
    }
}
