﻿namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Extensions;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class EditEventViewModel(IMobEventManager mobEventManager,
    IEventTypeRestService eventTypeRestService,
    IMapRestService mapRestService,
    INotificationService notificationService,
    IUserManager userManager,
    IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService,
    ILitterReportManager litterReportManager,
    IEventLitterReportManager eventLitterReportManager)
    : BaseViewModel(notificationService)
{
    private const int NewLitterReportStatus = 1;
    private readonly IEventTypeRestService eventTypeRestService = eventTypeRestService;
    private readonly IMapRestService mapRestService = mapRestService;
    private readonly IUserManager userManager = userManager;
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IEventLitterReportManager eventLitterReportManager = eventLitterReportManager;
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private EventPartnerLocationViewModel selectedEventPartnerLocation;
    
    private List<LitterReport> RawLitterReports { get; set; } = [];

    [ObservableProperty]
    private EventViewModel eventViewModel;

    [ObservableProperty]
    private string selectedEventType;

    [ObservableProperty]
    private AddressViewModel userLocation;

    private Event MobEvent { get; set; }

    [ObservableProperty] 
    private bool arePartnersAvailable;

    [ObservableProperty] 
    private bool areNoPartnersAvailable;

    [ObservableProperty] 
    private bool areLitterReportsAvailable;

    [ObservableProperty] 
    private bool areNoLitterReportsAvailable;

    [ObservableProperty]
    private bool isLitterReportMapSelected;

    [ObservableProperty]
    private bool isLitterReportListSelected;

    public ObservableCollection<EventPartnerLocationViewModel> AvailablePartners { get; set; } = new();

    public EventPartnerLocationViewModel SelectedEventPartnerLocation
    {
        get => selectedEventPartnerLocation;
        set
        {
            if (selectedEventPartnerLocation != value)
            {
                selectedEventPartnerLocation = value;
                OnPropertyChanged(nameof(selectedEventPartnerLocation));

                if (selectedEventPartnerLocation != null)
                {
                    PerformNavigation(selectedEventPartnerLocation);
                }
            }
        }
    }

    // This is only for the map point
    public ObservableCollection<EventViewModel> Events { get; set; } = new();

    private List<EventType> EventTypes { get; set; } = new();

    public ObservableCollection<string> ETypes { get; set; } = new();

    public ObservableCollection<EventLitterReportViewModel> EventLitterReports { get; set; } = [];

    public ObservableCollection<LitterImageViewModel> LitterImages { get; set; } = [];

    public Action UpdateMapLocation { get; set; }

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        try
        {
            UserLocation = userManager.CurrentUser.GetAddress();
            EventTypes = (await eventTypeRestService.GetEventTypesAsync()).ToList();

            MobEvent = await mobEventManager.GetEventAsync(eventId);
            
            foreach (var eventType in EventTypes)
            {
                ETypes.Add(eventType.Name);
            }

            SelectedEventType = EventTypes.First(et => et.Id == MobEvent.EventTypeId).Name;

            EventViewModel = MobEvent.ToEventViewModel(userManager.CurrentUser.Id);
            
            await LoadPartners();
            await LoadLitterReports();

            Events.Add(EventViewModel);
            
            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error has occurred while loading the event. Please wait and try again in a moment.");
        }
    }

    public async Task UpdateLitterAssignment(Guid litterReportId)
    {
        var eventLitterReport = EventLitterReports.FirstOrDefault(l => l.Id == litterReportId);

        if (eventLitterReport != null && eventLitterReport.CanRemoveFromEvent) 
        {
            await eventLitterReport.RemoveLitterReportFromEvent();
        }
        else
        {
            if (eventLitterReport != null)
            {
                await eventLitterReport.AddLitterReportToEvent();
            }
            else
            {
                var litterReport = RawLitterReports.FirstOrDefault(l => l.Id == litterReportId);
                if (litterReport != null)
                {
                    var eventLitterReportViewModel = litterReport.ToEventLitterReportViewModel(NotificationService, eventLitterReportManager, EventViewModel.Id);
                    await eventLitterReportViewModel.AddLitterReportToEvent();
                }
            }
        }

        await LoadLitterReports();
    }

    [RelayCommand]
    private async Task SaveEvent()
    {
        IsBusy = true;

        try
        {
            if (!await Validate())
            {
                IsBusy = false;
                return;
            }

            if (!string.IsNullOrEmpty(SelectedEventType))
            {
                var eventType = EventTypes.FirstOrDefault(e => e.Name == SelectedEventType);
                if (eventType != null)
                {
                    EventViewModel.EventTypeId = eventType.Id;
                }
            }

            // We need to copy back the property values that could have changed back to the event to be updated
            // (there are other values that cannot be updated) via the form that must be preserved on edit.
            MobEvent.City = EventViewModel.Address.City;
            MobEvent.Country = EventViewModel.Address.Country;
            MobEvent.Description = EventViewModel.Description;
            MobEvent.DurationHours = EventViewModel.DurationHours;
            MobEvent.DurationMinutes = EventViewModel.DurationMinutes;
            MobEvent.EventDate = EventViewModel.EventDate;
            MobEvent.EventTypeId = EventViewModel.EventTypeId;
            MobEvent.IsEventPublic = EventViewModel.IsEventPublic;
            MobEvent.Latitude = EventViewModel.Address.Latitude;
            MobEvent.Longitude = EventViewModel.Address.Longitude;
            MobEvent.MaxNumberOfParticipants = EventViewModel.MaxNumberOfParticipants;
            MobEvent.Name = EventViewModel.Name;
            MobEvent.PostalCode = EventViewModel.Address.PostalCode;
            MobEvent.Region = EventViewModel.Address.Region;
            MobEvent.StreetAddress = EventViewModel.Address.StreetAddress;

            MobEvent = await mobEventManager.UpdateEventAsync(MobEvent);

            EventViewModel = MobEvent.ToEventViewModel(userManager.CurrentUser.Id);
            Events.Clear();
            Events.Add(EventViewModel);

            IsBusy = false;

            await NotificationService.Notify("Event has been saved.");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error has occurred while saving the event. Please wait and try again in a moment.");
        }
    }

    public async Task ChangeLocation(Microsoft.Maui.Devices.Sensors.Location location)
    {
        IsBusy = true;

        var addr = await mapRestService.GetAddressAsync(location.Latitude, location.Longitude);

        EventViewModel.Address.City = addr.City;
        EventViewModel.Address.Country = addr.Country;
        EventViewModel.Address.Latitude = location.Latitude;
        EventViewModel.Address.Longitude = location.Longitude;
        EventViewModel.Address.Location = location;
        EventViewModel.Address.PostalCode = addr.PostalCode;
        EventViewModel.Address.Region = addr.Region;
        EventViewModel.Address.StreetAddress = addr.StreetAddress;

        Events.Clear();
        Events.Add(EventViewModel);

        IsBusy = false;
    }

    private async Task<bool> Validate()
    {
        if (EventViewModel.IsEventPublic && EventViewModel.EventDate < DateTimeOffset.Now)
        {
            await NotificationService.NotifyError("Event Dates for new public events must be in the future.");
            return false;
        }

        return true;
    }

    private async Task LoadPartners()
    {
        ArePartnersAvailable = false;
        AreNoPartnersAvailable = true;

        var eventPartnerLocations =
            await eventPartnerLocationServiceRestService.GetEventPartnerLocationsAsync(EventViewModel.Id);

        AvailablePartners.Clear();

        foreach (var eventPartnerLocation in eventPartnerLocations)
        {
            var eventPartnerLocationViewModel = new EventPartnerLocationViewModel
            {
                PartnerLocationId = eventPartnerLocation.PartnerLocationId,
                PartnerLocationName = eventPartnerLocation.PartnerLocationName,
                PartnerLocationNotes = eventPartnerLocation.PartnerLocationNotes,
                PartnerServicesEngaged = eventPartnerLocation.PartnerServicesEngaged,
                PartnerId = eventPartnerLocation.PartnerId,
            };

            AvailablePartners.Add(eventPartnerLocationViewModel);
        }

        ArePartnersAvailable = AvailablePartners.Any();
        AreNoPartnersAvailable = !ArePartnersAvailable;
    }

    private async Task LoadLitterReports()
    {
        AreLitterReportsAvailable = false;
        AreNoLitterReportsAvailable = true;
        IsLitterReportMapSelected = true;
        IsLitterReportListSelected = false;

        // Get new litter reports
        var filter = new TrashMob.Models.Poco.LitterReportFilter()
        {
            City = EventViewModel.Address.City,
            Country = EventViewModel.Address.Country,
            LitterReportStatusId = NewLitterReportStatus,
            IncludeLitterImages = true,
        };

        RawLitterReports = await litterReportManager.GetLitterReportsAsync(filter, ImageSizeEnum.Thumb, true);

        var assignedLitterReports = await eventLitterReportManager.GetEventLitterReportsAsync(EventViewModel.Id, ImageSizeEnum.Thumb, true);

        // Add the Litter Reports assigned to this event to the list of litter reports
        foreach (var assignedLitterReport in assignedLitterReports)
        {
            RawLitterReports.Add(assignedLitterReport.LitterReport.ToLitterReport());
        }

        UpdateLitterReportViewModels(assignedLitterReports);

        AreLitterReportsAvailable = EventLitterReports.Any();
        AreNoLitterReportsAvailable = !AreLitterReportsAvailable;
    }

    private void UpdateLitterReportViewModels(IEnumerable<TrashMob.Models.Poco.FullEventLitterReport> assignedLitterReports)
    {
        EventLitterReports.Clear();
        LitterImages.Clear();

        foreach (var litterReport in RawLitterReports.OrderByDescending(l => l.CreatedDate))
        {
            var vm = litterReport.ToEventLitterReportViewModel(NotificationService, eventLitterReportManager, EventViewModel.Id);

            if (assignedLitterReports.Any(l => l.LitterReportId == litterReport.Id))
            {
                vm.CanAddToEvent = false;
                vm.CanRemoveFromEvent = true;
                vm.Status = "Assigned to this event";
            }

            foreach (var litterImage in litterReport.LitterImages)
            {
                var litterImageViewModel = litterImage.ToLitterImageViewModel(litterReport.LitterReportStatusId, NotificationService);

                if (litterImageViewModel != null)
                {
                    litterImageViewModel.Address.DisplayName = litterReport.Name;
                    litterImageViewModel.Address.ParentId = litterReport.Id;
                    LitterImages.Add(litterImageViewModel);
                }
            }

            EventLitterReports.Add(vm);
        }
    }

    private async void PerformNavigation(EventPartnerLocationViewModel eventPartnerLocationViewModel)
    {
        await Shell.Current.GoToAsync(
            $"{nameof(EditEventPartnerLocationServicesPage)}?EventId={EventViewModel.Id}&PartnerLocationId={eventPartnerLocationViewModel.PartnerLocationId}");
    }

    [RelayCommand]
    private Task MapSelected()
    {
        IsLitterReportMapSelected = true;
        IsLitterReportListSelected = false;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task ListSelected()
    {
        IsLitterReportMapSelected = false;
        IsLitterReportListSelected = true;
        return Task.CompletedTask;
    }
}