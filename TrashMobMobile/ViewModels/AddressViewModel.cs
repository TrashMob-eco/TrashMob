namespace TrashMobMobile.ViewModels;

#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;

public partial class AddressViewModel : ObservableObject
{
    public AddressViewModel()
    {
    }

    private string streetAddress = string.Empty;
    private string city = string.Empty;
    private string region = string.Empty;

    public string StreetAddress
    {
        get => streetAddress;
        set
        {
            streetAddress = value;
            OnPropertyChanged(nameof(StreetAddress));
            UpdateDisplayAddress();
        }
    }

    public string City
    {
        get => city;
        set
        {
            city = value;
            OnPropertyChanged(nameof(City));
            UpdateDisplayAddress();
        }
    }

    [ObservableProperty]
    string county;

    public string Region
    {
        get => region;
        set
        {
            region = value;
            OnPropertyChanged(nameof(Region));
            UpdateDisplayAddress();
        }
    }

    [ObservableProperty]
    string country;

    [ObservableProperty]
    string postalCode;

    [ObservableProperty]
    double? latitude;

    [ObservableProperty]
    double? longitude;

    [ObservableProperty]
    Location location;

    [ObservableProperty]
    string displayAddress;

    private void UpdateDisplayAddress()
    {
        DisplayAddress = $"{StreetAddress}, {City}, {Region}";
    }
}
