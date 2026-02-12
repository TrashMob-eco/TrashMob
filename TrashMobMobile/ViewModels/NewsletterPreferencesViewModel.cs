namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Services;

public partial class NewsletterPreferencesViewModel(
    INewsletterPreferenceManager newsletterPreferenceManager,
    INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly INewsletterPreferenceManager newsletterPreferenceManager = newsletterPreferenceManager;

    [ObservableProperty]
    private bool arePreferencesFound;

    [ObservableProperty]
    private bool areNoPreferencesFound = true;

    public ObservableCollection<NewsletterPreferenceItemViewModel> Preferences { get; } = [];

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            var preferences = await newsletterPreferenceManager.GetMyPreferencesAsync();

            Preferences.Clear();

            foreach (var pref in preferences)
            {
                Preferences.Add(new NewsletterPreferenceItemViewModel
                {
                    CategoryId = pref.CategoryId,
                    CategoryName = pref.CategoryName,
                    CategoryDescription = pref.CategoryDescription,
                    IsSubscribed = pref.IsSubscribed,
                });
            }

            ArePreferencesFound = Preferences.Count > 0;
            AreNoPreferencesFound = !ArePreferencesFound;
        }, "Failed to load newsletter preferences. Please try again.");
    }

    [RelayCommand]
    private async Task TogglePreference(NewsletterPreferenceItemViewModel? item)
    {
        if (item == null)
        {
            return;
        }

        var newValue = !item.IsSubscribed;

        await ExecuteAsync(async () =>
        {
            await newsletterPreferenceManager.UpdatePreferenceAsync(item.CategoryId, newValue);
            item.IsSubscribed = newValue;
        }, "Failed to update preference. Please try again.");
    }

    [RelayCommand]
    private async Task UnsubscribeAll()
    {
        var confirm = await Shell.Current.DisplayAlertAsync(
            "Unsubscribe from All",
            "Are you sure you want to unsubscribe from all newsletters?",
            "Unsubscribe All",
            "Cancel");

        if (!confirm)
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            await newsletterPreferenceManager.UnsubscribeAllAsync();

            foreach (var pref in Preferences)
            {
                pref.IsSubscribed = false;
            }

            await NotificationService.Notify("Unsubscribed from all newsletters.");
        }, "Failed to unsubscribe. Please try again.");
    }
}
