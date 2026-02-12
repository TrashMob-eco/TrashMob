namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class AddressViewModel : ObservableObject
{
    private string city = string.Empty;

    [ObservableProperty]
    private Guid parentId;

    [ObservableProperty]
    private string displayName = string.Empty;

    [ObservableProperty]
    private string iconFile = string.Empty;

    [ObservableProperty]
    private AddressType addressType;

    [ObservableProperty]
    private string country = string.Empty;

    [ObservableProperty]
    private string county = string.Empty;

    [ObservableProperty]
    private string displayAddress = string.Empty;

    [ObservableProperty]
    private double? latitude;

    [ObservableProperty]
    private Location location = null!;

    [ObservableProperty]
    private double? longitude;

    [ObservableProperty]
    private string postalCode = string.Empty;

    private string region = string.Empty;

    private string streetAddress = string.Empty;

    public string StreetAddress
    {
        get => streetAddress;
        set
        {
            streetAddress = value;
            OnPropertyChanged();
            UpdateDisplayAddress();
        }
    }

    public string City
    {
        get => city;
        set
        {
            city = value;
            OnPropertyChanged();
            UpdateDisplayAddress();
        }
    }

    public string Region
    {
        get => region;
        set
        {
            region = value;
            OnPropertyChanged();
            UpdateDisplayAddress();
        }
    }

    private void UpdateDisplayAddress()
    {
        DisplayAddress = $"{StreetAddress}, {City}, {Region}";
    }

    public string AutomationId
    {
        get
        {
            return $"{AddressType}:{ParentId}";
        }
    }
}