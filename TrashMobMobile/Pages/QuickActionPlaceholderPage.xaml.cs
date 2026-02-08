namespace TrashMobMobile.Pages;

public partial class QuickActionPlaceholderPage : ContentPage
{
    public QuickActionPlaceholderPage()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // Fallback: if the Navigating interception didn't work, show action sheet here
        var action = await DisplayActionSheet("Quick Actions", "Cancel", null, "Create Event", "Report Litter");

        if (action == "Create Event")
        {
            await Shell.Current.GoToAsync($"../{nameof(CreateEventPage)}");
        }
        else if (action == "Report Litter")
        {
            await Shell.Current.GoToAsync($"../{nameof(CreateLitterReportPage)}");
        }
        else
        {
            // Go back to previous tab
            await Shell.Current.GoToAsync("..");
        }
    }
}
