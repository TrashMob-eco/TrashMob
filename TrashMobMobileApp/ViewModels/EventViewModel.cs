namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using TrashMob.Models;

public partial class EventViewModel : ObservableObject
{
    private string name;

    private string description;

    private DateTimeOffset eventDate;

    private int durationHours;

    private int durationMinutes;

    private int eventTypeId;

    private int eventStatusId;

    private string streetAddress;

    private string city;

    private string region;

    private string country;

    private string postalCode;

    private double? latitude;

    private double? longitude;

    private int maxNumberOfParticipants;

    private bool isEventPublic;

    private string cancellationReason;

    public EventViewModel()
    {
    }

    public string Name
    {
        get => name;

        set
        {
            name = value;
            OnPropertyChanged();
        }
    }

    public string Description
    {
        get => description;

        set
        {
            description = value;
            OnPropertyChanged();
        }
    }

    public DateTimeOffset EventDate
    {
        get => eventDate;

        set
        {
            eventDate = value;
            OnPropertyChanged();
        }
    }

    public int DurationHours
    {
        get => durationHours;

        set
        {
            durationHours = value;
            OnPropertyChanged();
        }
    }

    public int DurationMinutes
    {
        get => durationMinutes;

        set
        {
            durationMinutes = value;
            OnPropertyChanged();
        }
    }

    public int EventTypeId
    {
        get => eventTypeId;

        set
        {
            eventTypeId = value;
            OnPropertyChanged();
        }
    }

    public int EventStatusId
    {
        get => eventStatusId;

        set
        {
            eventStatusId = value;
            OnPropertyChanged();
        }
    }

    public string StreetAddress
    {
        get => streetAddress;

        set
        {
            streetAddress = value;
            OnPropertyChanged();
        }
    }

    public string City
    {
        get => city;

        set
        {
            city = value;
            OnPropertyChanged();
        }
    }

    public string Region
    {
        get => region;

        set
        {
            region = value;
            OnPropertyChanged();
        }
    }

    public string Country
    {
        get => country;

        set
        {
            country = value;
            OnPropertyChanged();
        }
    }

    public string PostalCode
    {
        get => postalCode;

        set
        {
            postalCode = value;
            OnPropertyChanged();
        }
    }

    public double? Latitude
    {
        get => latitude;

        set
        {
            latitude = value;
            OnPropertyChanged();
        }
    }

    public double? Longitude
    {
        get => longitude;

        set
        {
            longitude = value;
            OnPropertyChanged();
        }
    }

    public int MaxNumberOfParticipants
    {
        get => maxNumberOfParticipants;

        set
        {
            maxNumberOfParticipants = value;
            OnPropertyChanged();
        }
    }

    public bool IsEventPublic
    {
        get => isEventPublic;

        set
        {
            isEventPublic = value;
            OnPropertyChanged();
        }
    }

    public string CancellationReason
    {
        get => cancellationReason;

        set
        {
            cancellationReason = value;
            OnPropertyChanged();
        }
    }
}
