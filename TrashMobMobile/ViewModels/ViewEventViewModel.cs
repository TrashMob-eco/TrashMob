namespace TrashMobMobile.ViewModels;

using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class ViewEventViewModel(IMobEventManager mobEventManager,
    IEventTypeRestService eventTypeRestService,
    IWaiverManager waiverManager,
    IEventAttendeeRestService eventAttendeeRestService,
    IEventAttendeeRouteRestService eventAttendeeRouteRestService,
    INotificationService notificationService,
    IEventLitterReportManager eventLitterReportManager,
    IUserManager userManager,
    IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService,
    ILitterReportManager litterReportManager) : BaseViewModel(notificationService)
{
    private readonly IEventAttendeeRestService eventAttendeeRestService = eventAttendeeRestService;
    private readonly IEventLitterReportManager eventLitterReportManager = eventLitterReportManager;
    private readonly IUserManager userManager = userManager;
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IEventTypeRestService eventTypeRestService = eventTypeRestService;
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IWaiverManager waiverManager = waiverManager;
 
    [ObservableProperty]
    private string attendeeCount = string.Empty;

    [ObservableProperty]
    private string displayDuration = string.Empty;

    [ObservableProperty]
    private bool enableEditEvent;

    [ObservableProperty]
    private bool enableRegister;

    [ObservableProperty]
    private bool enableUnregister;

    [ObservableProperty]
    private bool enableStartTrackEventRoute = false;

    [ObservableProperty]
    private bool enableStopTrackEventRoute = false;

    [ObservableProperty]
    private bool enableViewEventSummary;

    [ObservableProperty]
    private bool enableViewEventDetails;

    [ObservableProperty]
    private bool enableViewEventPartners;

    [ObservableProperty]
    private bool enableViewEventLitterReports;

    [ObservableProperty]
    private bool enableViewEventAttendees;

    [ObservableProperty]
    private bool isDetailsVisible;

    [ObservableProperty]
    private bool isPartnersVisible;

    [ObservableProperty]
    private bool isLitterReportsVisible;

    [ObservableProperty]
    private bool isAttendeesVisible;

    [ObservableProperty]
    private EventViewModel eventViewModel = new();

    private Event mobEvent = new();

    [ObservableProperty]
    private string selectedEventType = string.Empty;

    [ObservableProperty]
    private string spotsLeft = string.Empty;

    [ObservableProperty]
    private string whatToExpect = string.Empty;

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

    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    public ObservableCollection<AddressViewModel> Addresses { get; set; } = [];

    public ObservableCollection<LitterReportViewModel> EventLitterReports { get; set; } = [];

    public ObservableCollection<LitterImageViewModel> LitterImages { get; set; } = [];

    public ObservableCollection<EventAttendeeViewModel> EventAttendees { get; set; } = [];

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        try
        {
            mobEvent = await mobEventManager.GetEventAsync(eventId);
            EventViewModel = mobEvent.ToEventViewModel(userManager.CurrentUser.Id);

            var eventTypes = (await eventTypeRestService.GetEventTypesAsync()).ToList();
            SelectedEventType = eventTypes.First(et => et.Id == mobEvent.EventTypeId).Name;
            DisplayDuration = mobEvent.GetFormattedDuration();

            Events.Clear();
            Events.Add(EventViewModel);

            Addresses.Add(EventViewModel.Address);

            EnableEditEvent = mobEvent.IsEventLead(userManager.CurrentUser.Id) && !mobEvent.IsCompleted();
            EnableViewEventSummary = mobEvent.IsCompleted();
            EnableViewEventDetails = true;
            EnableViewEventPartners = true;
            EnableViewEventLitterReports = true;
            EnableViewEventAttendees = true;
            IsDetailsVisible = true;
            IsPartnersVisible = false;
            IsLitterReportsVisible = false;
            IsAttendeesVisible = false;

            EnableStartTrackEventRoute = mobEvent.IsEventLead(userManager.CurrentUser.Id) && !mobEvent.IsCompleted();
            EnableStopTrackEventRoute = false;

            WhatToExpect =
                "What to Expect: \n\tCleanup supplies provided\n\tMeet fellow community members\n\tContribute to a cleaner environment.";

            await SetRegistrationOptions();
            await GetAttendeeCount();
            await LoadPartners();
            await LoadLitterReports();

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while loading the event. Please try again.");
        }
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

    [RelayCommand]
    private void ViewEventPartners()
    {
        IsDetailsVisible = false;
        IsPartnersVisible = true;
        IsLitterReportsVisible = false;
        IsAttendeesVisible = false;
    }

    [RelayCommand]
    private void ViewEventAttendees()
    {
        IsDetailsVisible = false;
        IsPartnersVisible = false;
        IsLitterReportsVisible = false;
        IsAttendeesVisible = true;
    }

    [RelayCommand]
    private void ViewLitterReports()
    {
        IsDetailsVisible = false;
        IsPartnersVisible = false;
        IsLitterReportsVisible = true;
        IsAttendeesVisible = false;
    }

    [RelayCommand]
    private void ViewEventDetails()
    {
        IsDetailsVisible = true;
        IsPartnersVisible = false;
        IsLitterReportsVisible = false;
        IsAttendeesVisible = false;
    }

    private async Task LoadPartners()
    {
        ArePartnersAvailable = false;
        AreNoPartnersAvailable = true;

        var eventPartnerLocations =
            await eventPartnerLocationServiceRestService.GetEventPartnerLocationsAsync(EventViewModel.Id);

        AvailablePartners.Clear();

        foreach (var eventPartnerLocation in eventPartnerLocations.Where(e => e.PartnerServicesEngaged != "None"))
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

        var assignedLitterReports = await eventLitterReportManager.GetEventLitterReportsAsync(EventViewModel.Id, ImageSizeEnum.Thumb);

        UpdateLitterReportViewModels(assignedLitterReports);

        AreLitterReportsAvailable = EventLitterReports.Any();
        AreNoLitterReportsAvailable = !AreLitterReportsAvailable;
    }

    private void UpdateLitterReportViewModels(IEnumerable<TrashMob.Models.Poco.FullEventLitterReport> assignedLitterReports)
    {
        EventLitterReports.Clear();
        LitterImages.Clear();

        foreach (var eventLitterReport in assignedLitterReports.OrderByDescending(l => l.LitterReport.CreatedDate))
        {
            var vm = eventLitterReport.LitterReport.ToEventLitterReportViewModel(NotificationService, eventLitterReportManager, EventViewModel.Id);
            vm.Status = "Assigned to this event";

            foreach (var litterImage in eventLitterReport.LitterReport.LitterImages)
            {
                var litterImageViewModel = litterImage.ToLitterImageViewModel(eventLitterReport.LitterReport.LitterReportStatusId, NotificationService);

                if (litterImageViewModel != null)
                {
                    litterImageViewModel.Address.DisplayName = eventLitterReport.LitterReport.Name;
                    litterImageViewModel.Address.ParentId = eventLitterReport.LitterReport.Id;
                    LitterImages.Add(litterImageViewModel);
                }
            }

            EventLitterReports.Add(vm);
        }
    }

    private async Task GetAttendeeCount()
    {
        var attendees = await eventAttendeeRestService.GetEventAttendeesAsync(mobEvent.Id);

        EventAttendees.Clear();

        foreach (var attendee in attendees)
        {
            var attendeeVm = attendee.ToEventAttendeeViewModel();
            attendeeVm.Role = mobEvent.IsEventLead(attendee.Id) ? "Lead" : "Attendee";
            EventAttendees.Add(attendeeVm);
        }

        if (attendees.Count() == 1)
        {
            AttendeeCount = $"{attendees.Count()} person is going!";
        }
        else
        {
            AttendeeCount = $"{attendees.Count()} people are going!";
        }

        if (mobEvent.MaxNumberOfParticipants > 0)
        {
            if (mobEvent.MaxNumberOfParticipants - attendees.Count() > 0)
            {
                SpotsLeft = $"{mobEvent.MaxNumberOfParticipants - attendees.Count()} spot left!";
            }
            else
            {
                SpotsLeft = "We're sorry. This event is currently full.";
            }
        }
        else
        {
            SpotsLeft = "";
        }
    }

    [RelayCommand]
    private async Task EditEvent()
    {
        await Shell.Current.GoToAsync($"{nameof(EditEventPage)}?EventId={EventViewModel.Id}");
    }
    
    private Microsoft.Maui.Devices.Sensors.Location? currentLocation;
    
    public ObservableCollection<Microsoft.Maui.Devices.Sensors.Location> Locations { get; } = [];

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task RealTimeLocationTracker(CancellationToken cancellationToken)
    {
        if (EnableStartTrackEventRoute)
        {
            EnableStopTrackEventRoute = true;
            EnableStartTrackEventRoute = false;
        }

        cancellationToken.Register(async () =>
        {
            await SaveRoute();
            Locations.Clear();
            EnableStopTrackEventRoute = false;
            EnableStartTrackEventRoute = true;
        });

        var progress = new Progress<Microsoft.Maui.Devices.Sensors.Location>(location =>
        {
            if (currentLocation is null)
            {
                currentLocation = location;
            }
            else
            {
                Locations.Remove(currentLocation);
                currentLocation = location;
            }

            Locations.Add(currentLocation);
        });

        await Geolocator.Default.StartListening(progress, cancellationToken);
    }

    private async Task SaveRoute()
    {
        try
        {
            // If there are no locations, then there is nothing to save.
            if (Locations.Count == 0)
            {
                return;
            }

            // If there is only one location, then add a second location to make a line.
            if (Locations.Count == 1)
            {
                Locations.Add(Locations[0]);
            }

            await eventAttendeeRouteRestService.AddEventAttendeeRouteAsync(new DisplayEventAttendeeRoute
            {
                EventId = mobEvent.Id,
                UserId = userManager.CurrentUser.Id,
                Locations = GetSortableLocations()
            });
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while saving your route.");
        }
    }

    private List<SortableLocation> GetSortableLocations()
    {
        var sortableLocations = new List<SortableLocation>();
        int order = 0;
        foreach (var location in Locations.OrderBy(l => l.Timestamp))
        {
            var sortableLocation = new SortableLocation
            {
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                SortOrder = order++
            };

            sortableLocations.Add(sortableLocation);
        }

        return sortableLocations;
    }

    [RelayCommand]
    private async Task ViewEventSummary()
    {
        await Shell.Current.GoToAsync($"{nameof(ViewEventSummaryPage)}?EventId={EventViewModel.Id}");
    }

    [RelayCommand]
    private async Task Register()
    {
        IsBusy = true;

        try
        {
            if (!await waiverManager.HasUserSignedTrashMobWaiverAsync())
            {
                await Shell.Current.GoToAsync($"{nameof(WaiverPage)}");
            }

            var eventAttendee = new EventAttendee
            {
                EventId = EventViewModel.Id,
                UserId = userManager.CurrentUser.Id,
            };

            await mobEventManager.AddEventAttendeeAsync(eventAttendee);

            await SetRegistrationOptions();
            await GetAttendeeCount();

            IsBusy = false;

            await NotificationService.Notify("You have been registered for this event.");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while registering you for this event. Please try again.");
        }
    }

    [RelayCommand]
    private async Task Unregister()
    {
        IsBusy = true;

        try
        {
            var eventAttendee = new EventAttendee
            {
                EventId = EventViewModel.Id,
                UserId = userManager.CurrentUser.Id,
            };

            await mobEventManager.RemoveEventAttendeeAsync(eventAttendee);

            await SetRegistrationOptions();
            await GetAttendeeCount();

            IsBusy = false;

            await NotificationService.Notify("You have been unregistered for this event.");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while unregistering you for this event. Please try again.");
        }
    }

    private async Task SetRegistrationOptions()
    {
        var isAttending = await mobEventManager.IsUserAttendingAsync(mobEvent.Id, userManager.CurrentUser.Id);

        EnableRegister = !mobEvent.IsEventLead(userManager.CurrentUser.Id) && !isAttending && mobEvent.AreNewRegistrationsAllowed();
        EnableUnregister = !mobEvent.IsEventLead(userManager.CurrentUser.Id) && isAttending && mobEvent.AreUnregistrationsAllowed();
    }
}