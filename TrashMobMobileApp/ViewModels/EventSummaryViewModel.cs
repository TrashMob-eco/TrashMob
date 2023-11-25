namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class EventSummaryViewModel : ObservableObject
{
    public EventSummaryViewModel()
    {
    }

    [ObservableProperty]
    Guid eventId;

    [ObservableProperty]
    int numberOfBuckets;

    [ObservableProperty]
    int numberOfBags;

    [ObservableProperty]
    int durationInMinutes;

    [ObservableProperty]
    int actualNumberOfAttendees;

    [ObservableProperty]
    string notes;

    [ObservableProperty]
    bool isBusy = false;

    [ObservableProperty]
    bool isError = false;
}
