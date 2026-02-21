namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Services;

/// <summary>
/// Base class for ViewModels that provide country/region/city filtering and map/list toggling.
/// </summary>
public abstract partial class LocationFilterViewModel(INotificationService notificationService)
    : BaseViewModel(notificationService)
{
    protected IEnumerable<TrashMob.Models.Poco.Location> Locations { get; set; } = [];

    private string? selectedCountry;
    private string? selectedRegion;
    private string? selectedCity;

    [ObservableProperty]
    private bool isMapSelected;

    [ObservableProperty]
    private bool isListSelected;

    public ObservableCollection<string> CountryCollection { get; } = [];

    public ObservableCollection<string> RegionCollection { get; } = [];

    public ObservableCollection<string> CityCollection { get; } = [];

    public string? SelectedCountry
    {
        get => selectedCountry;
        set
        {
            selectedCountry = value;
            OnPropertyChanged();
            OnCountrySelected();
        }
    }

    public string? SelectedRegion
    {
        get => selectedRegion;
        set
        {
            selectedRegion = value;
            OnPropertyChanged();
            OnRegionSelected();
        }
    }

    public string? SelectedCity
    {
        get => selectedCity;
        set
        {
            selectedCity = value;
            OnPropertyChanged();
            OnCitySelected();
        }
    }

    [RelayCommand]
    private Task MapSelected()
    {
        IsMapSelected = true;
        IsListSelected = false;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task ListSelected()
    {
        IsMapSelected = false;
        IsListSelected = true;
        return Task.CompletedTask;
    }

    protected void PopulateCountries()
    {
        CountryCollection.Clear();
        RegionCollection.Clear();
        CityCollection.Clear();

        if (Locations == null || !Locations.Any())
        {
            return;
        }

        foreach (var country in Locations.Select(l => l.Country).Distinct())
        {
            if (!string.IsNullOrEmpty(country))
            {
                CountryCollection.Add(country);
            }
        }
    }

    protected virtual void OnCountrySelected()
    {
        selectedRegion = null;
        OnPropertyChanged(nameof(SelectedRegion));
        selectedCity = null;
        OnPropertyChanged(nameof(SelectedCity));

        RegionCollection.Clear();

        if (Locations != null && !string.IsNullOrEmpty(selectedCountry))
        {
            foreach (var region in Locations.Where(l => l.Country == selectedCountry).Select(l => l.Region).Distinct())
            {
                if (!string.IsNullOrEmpty(region))
                {
                    RegionCollection.Add(region);
                }
            }
        }

        ApplyFilters();
    }

    protected virtual void OnRegionSelected()
    {
        selectedCity = null;
        OnPropertyChanged(nameof(SelectedCity));

        CityCollection.Clear();

        if (Locations != null && !string.IsNullOrEmpty(selectedCountry) && !string.IsNullOrEmpty(selectedRegion))
        {
            foreach (var city in Locations.Where(l => l.Country == selectedCountry && l.Region == selectedRegion).Select(l => l.City).Distinct())
            {
                if (!string.IsNullOrEmpty(city))
                {
                    CityCollection.Add(city);
                }
            }
        }

        ApplyFilters();
    }

    protected virtual void OnCitySelected()
    {
        ApplyFilters();
    }

    protected void ClearLocationSelections()
    {
        SelectedCountry = null;
        SelectedRegion = null;
        SelectedCity = null;
    }

    /// <summary>
    /// Called when any location filter changes. Override to apply the filters to your data.
    /// </summary>
    protected abstract void ApplyFilters();
}
