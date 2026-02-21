namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class EventAttendeeViewModel : ObservableObject
{
    [ObservableProperty]
    private Guid eventId;

    [ObservableProperty]
    private Guid attendeeId;

    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private string memberSince = string.Empty;

    [ObservableProperty]
    private string role = string.Empty;

    [ObservableProperty]
    private bool isEventCreator;

    [ObservableProperty]
    private bool isEventLead;

    [ObservableProperty]
    private bool canPromote;

    [ObservableProperty]
    private bool canDemote;
}