namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Extensions;
using TrashMobMobile.Controls;

public partial class QuickActionPlaceholderPage : ContentPage
{
    public QuickActionPlaceholderPage()
    {
        InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // Fallback: if the Navigating interception didn't work, show styled popup here
        var popup = new QuickActionPopup();
        var result = await this.ShowPopupAsync<string>(popup);
        var action = result?.Result;

        if (action == QuickActionPopup.CreateEvent)
        {
            await Shell.Current.GoToAsync($"../{nameof(CreateEventPage)}");
        }
        else if (action == QuickActionPopup.ReportLitter)
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
