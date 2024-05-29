namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;

public partial class EventViewModel : ObservableObject
{
    private DateTimeOffset eventDate;

    [ObservableProperty]
    private AddressViewModel address;

    [ObservableProperty]
    private bool canCancelEvent;

    [ObservableProperty]
    private string cancellationReason;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private int durationHours;

    [ObservableProperty]
    private int durationMinutes;

    [ObservableProperty]
    private int eventStatusId;

    [ObservableProperty]
    private int eventTypeId;

    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private bool isEventPublic;

    private bool isUserAttending;

    [ObservableProperty]
    private int maxNumberOfParticipants;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string userRoleForEvent;

    public DateTimeOffset EventDate
    {
        get => eventDate;

        set
        {
            if (eventDate != value)
            {
                eventDate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(DisplayDate));
                OnPropertyChanged(nameof(DisplayTime));
                OnPropertyChanged(nameof(EventTime));
                OnPropertyChanged(nameof(EventDateOnly));
            }
        }
    }

    public bool IsUserAttending
    {
        get => isUserAttending;
        set
        {
            if (isUserAttending != value)
            {
                isUserAttending = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(UserRoleForEvent));
            }
        }
    }

    public string DisplayDate => EventDate.GetFormattedLocalDate();

    public string DisplayTime => EventDate.GetFormattedLocalTime();

    public TimeSpan EventTime
    {
        get => EventDate.TimeOfDay;
        set
        {
            var fullDateTime = EventDateOnly.Add(value);
            EventDate = fullDateTime;
        }
    }

    public DateTime EventDateOnly
    {
        get => EventDate.Date;
        set
        {
            var fullDateTime = value.Add(EventTime);
            EventDate = fullDateTime;
        }
    }

    public string ErrorMessage { get; set; }

    public string GetUserRole(Event mobEvent)
    {
        if (mobEvent.IsEventLead())
        {
            return "Lead";
        }

        if (IsUserAttending)
        {
            return "Attendee";
        }

        return string.Empty;
    }

    public bool IsValid()
    {
        if (EventDate == DateTimeOffset.MinValue)
        {
            ErrorMessage = "Event Date and Time must be specified.";
            return false;
        }

        return true;
    }

    public Event ToEvent()
    {
        return new Event
        {
            Id = Id,
            EventDate = EventDate,
            Name = Name,
            Description = Description,
            CancellationReason = CancellationReason,
            City = Address.City,
            Country = Address.Country,
            DurationHours = DurationHours,
            DurationMinutes = DurationMinutes,
            EventTypeId = EventTypeId,
            IsEventPublic = IsEventPublic,
            Latitude = Address.Latitude,
            Longitude = Address.Longitude,
            MaxNumberOfParticipants = MaxNumberOfParticipants,
            PostalCode = Address.PostalCode,
            Region = Address.Region,
            StreetAddress = Address.StreetAddress,
            EventStatusId = EventStatusId,
        };
    }

    [RelayCommand]
    private async Task CancelEvent()
    {
        await Shell.Current.GoToAsync($"{nameof(CancelEventPage)}?EventId={Id}");
    }
}