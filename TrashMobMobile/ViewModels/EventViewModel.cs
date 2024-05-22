namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;

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

    private DateTimeOffset _eventDate;
    public DateTimeOffset EventDate
    {
        get
        {
            return _eventDate;
        }

        set
        {
            if ( _eventDate != value)
            {
                _eventDate = value;
                OnPropertyChanged(nameof(EventDate));
                OnPropertyChanged(nameof(DisplayDate));
                OnPropertyChanged(nameof(DisplayTime));
                OnPropertyChanged(nameof(EventTime));
                OnPropertyChanged(nameof(EventDateOnly));
            }
        }
    }

    [ObservableProperty]
    int durationHours;

    [ObservableProperty]
    int durationMinutes;

    [ObservableProperty]
    int eventTypeId;

    [ObservableProperty]
    int eventStatusId;

    [ObservableProperty]
    AddressViewModel address;

    [ObservableProperty]
    int maxNumberOfParticipants;

    [ObservableProperty]
    bool isEventPublic;

    [ObservableProperty]
    string cancellationReason;

    [ObservableProperty]
    bool canCancelEvent;

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

    private bool isUserAttending;

    public bool IsUserAttending
    {
        get { return isUserAttending; }
        set
        {
            if ( isUserAttending != value )
            {
                isUserAttending = value;
                OnPropertyChanged(nameof(IsUserAttending));
                OnPropertyChanged(nameof(UserRoleForEvent));
            }
        }
    }

    public string DisplayDate
    {
        get
        {
            return EventDate.GetFormattedLocalDate();
        }
    }

    public string DisplayTime
    {
        get
        {
            return EventDate.GetFormattedLocalTime();
        }
    }

    public TimeSpan EventTime
    {
        get
        {
            return EventDate.TimeOfDay;
        }
        set
        {
            var fullDateTime = EventDateOnly.Add(value);
            EventDate = fullDateTime;
        }
    }

    public DateTime EventDateOnly
    {
        get
        {
            return EventDate.Date;
        }
        set
        {
            var fullDateTime = value.Add(EventTime);
            EventDate = fullDateTime;
        }
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

    [ObservableProperty]
    string userRoleForEvent;

    public string ErrorMessage { get; set; }

    public Event ToEvent()
    {
        return new Event()
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
