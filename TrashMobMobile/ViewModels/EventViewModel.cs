namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class EventViewModel : ObservableObject
{
    public EventViewModel()
    {
    }

    [ObservableProperty]
    Guid id;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string description;

    [ObservableProperty]
    DateTimeOffset eventDate;

    [ObservableProperty]
    int durationHours;

    [ObservableProperty]
    int durationMinutes;

    [ObservableProperty]
    int eventTypeId;

    [ObservableProperty]
    int eventStatusId;

    [ObservableProperty]
    AddressViewModel addressViewModel;

    [ObservableProperty]
    int maxNumberOfParticipants;

    [ObservableProperty]
    bool isEventPublic;

    [ObservableProperty]
    string cancellationReason;
}
