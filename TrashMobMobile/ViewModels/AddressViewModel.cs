namespace TrashMobMobile.ViewModels;

#nullable enable

using CommunityToolkit.Mvvm.ComponentModel;

public partial class AddressViewModel : ObservableObject
{
    public AddressViewModel()
    {
    }

    [ObservableProperty]
    string streetAddress;

    [ObservableProperty]
    string city;

    [ObservableProperty]
    string county;

    [ObservableProperty]
    string region;

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
}
