namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class AddressViewModel : ObservableObject
{
    private string city = string.Empty;

    [ObservableProperty]
    private string country;

    [ObservableProperty]
    private string county;

    [ObservableProperty]
    private string displayAddress;

    [ObservableProperty]
    private double? latitude;

    [ObservableProperty]
    private Location location;

    [ObservableProperty]
    private double? longitude;

    [ObservableProperty]
    private string postalCode;

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
}