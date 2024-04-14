﻿namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class CreateLitterReportViewModel : BaseViewModel
{
    private readonly IMapRestService mapRestService;
    private readonly ILitterReportRestService litterReportRestService;
    private const int NewLitterReportStatus = 1;

    [ObservableProperty]
    LitterReportViewModel litterReportViewModel;
    
    public ObservableCollection<LitterImageViewModel> LitterImageViewModels { get; init; } = [];
    
    public LitterImageViewModel? SelectedLitterImageViewModel { get; set; }

    public string DefaultLitterReportName { get; } = "New Litter Report";

    public CreateLitterReportViewModel(ILitterReportRestService litterReportRestService, IMapRestService mapRestService)
    {
        SaveLitterReportCommand = new Command(async () => await SaveLitterReport());
        this.litterReportRestService = litterReportRestService;
        this.mapRestService = mapRestService;
        LitterReportViewModel = new LitterReportViewModel
        {
            Name = DefaultLitterReportName,
            LitterReportStatusId = NewLitterReportStatus
        };
    }

    public ICommand SaveLitterReportCommand { get; set; }

    public string LocalFilePath { get; set; } = string.Empty;

    public async Task AddImageToCollection()
    {
        Location? location = await GetCurrentLocation();

        if (location != null)
        {
            if (SelectedLitterImageViewModel == null)
            {
                SelectedLitterImageViewModel = new LitterImageViewModel();
            }

            var address = await mapRestService.GetAddressAsync(location.Latitude, location.Longitude);
            SelectedLitterImageViewModel.Address.Longitude = location.Longitude;
            SelectedLitterImageViewModel.Address.Latitude = location.Latitude;
            SelectedLitterImageViewModel.Address.City = address.City;
            SelectedLitterImageViewModel.Address.Country = address.Country;
            SelectedLitterImageViewModel.Address.County = address.County;
            SelectedLitterImageViewModel.Address.PostalCode = address.PostalCode;
            SelectedLitterImageViewModel.Address.Region = address.Region;
            SelectedLitterImageViewModel.Address.StreetAddress = address.StreetAddress;
            SelectedLitterImageViewModel.Address.Location = new Location(SelectedLitterImageViewModel.Address.Latitude.Value, SelectedLitterImageViewModel.Address.Longitude.Value);
            LitterImageViewModels.Add(SelectedLitterImageViewModel);
            SelectedLitterImageViewModel = null;
        }
        else
        {
            await NotifyError("Could not get location for image");
        }
    }

    public static async Task<Location?> GetCurrentLocation()
    {
        try
        {
            GeolocationRequest request = new GeolocationRequest(GeolocationAccuracy.Best, TimeSpan.FromSeconds(10));

            var cancelTokenSource = new CancellationTokenSource();

            return await Geolocation.Default.GetLocationAsync(request, cancelTokenSource.Token);
        }
        //catch (FeatureNotSupportedException fnsEx)
        //{
        //    // Handle not supported on device exception
        //}
        //catch (FeatureNotEnabledException fneEx)
        //{
        //    // Handle not enabled on device exception
        //}
        //catch (PermissionException pEx)
        //{
        //    // Handle permission exception
        //}
        catch
        {
            // Unable to get location
        }

        return null;
    }

    private async Task SaveLitterReport()
    {
        IsBusy = true;

        if (!await Validate())
        {
            IsBusy = false;
            return;
        }

        var litterReport = LitterReportViewModel.ToLitterReport();

        foreach (var litterImageViewModel in LitterImageViewModels)
        {
            var litterImage = new LitterImage
            {
                City = litterImageViewModel.Address.City,
                Country = litterImageViewModel.Address.Country,
                LitterReportId = litterImageViewModel.LitterReportId,
                Latitude = litterImageViewModel.Address.Latitude,
                Longitude = litterImageViewModel.Address.Longitude,
                PostalCode = litterImageViewModel.Address.PostalCode,
                Region = litterImageViewModel.Address.Region,
                StreetAddress = litterImageViewModel.Address.StreetAddress,

                // Use the Azure Blob Url as local file on create
                AzureBlobURL = litterImageViewModel.FilePath
            };

            litterReport.LitterImages.Add(litterImage);
        }

        var updatedLitterReport = await litterReportRestService.AddLitterReportAsync(litterReport);

        IsBusy = false;

        await Notify("Litter Report has been saved.");
    }

    private async Task<bool> Validate()
    {
        return true;
    }
}