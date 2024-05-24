namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class EventSummaryViewModel : ObservableObject
{
    [ObservableProperty]
    private int actualNumberOfAttendees;

    [ObservableProperty]
    private int durationInMinutes;

    [ObservableProperty]
    private Guid eventId;

    [ObservableProperty]
    private string notes;

    [ObservableProperty]
    private int numberOfBags;

    [ObservableProperty]
    private int numberOfBuckets;
}