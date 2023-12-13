namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
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
        };
    }
}
