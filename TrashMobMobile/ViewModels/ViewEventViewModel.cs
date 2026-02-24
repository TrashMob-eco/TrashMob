namespace TrashMobMobile.ViewModels;

using System;
using System.Collections.ObjectModel;
using System.Text.Json;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMobMobile.Controls;
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
    ILitterReportManager litterReportManager,
    IEventPhotoManager eventPhotoManager,
    IRouteTrackingSessionManager routeTrackingSessionManager,
    IEventAttendeeMetricsRestService eventAttendeeMetricsRestService) : BaseViewModel(notificationService)
{
    private readonly IEventAttendeeRestService eventAttendeeRestService = eventAttendeeRestService;
    private readonly IEventLitterReportManager eventLitterReportManager = eventLitterReportManager;
    private readonly IUserManager userManager = userManager;
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
    private readonly ILitterReportManager litterReportManager = litterReportManager;
    private readonly IEventTypeRestService eventTypeRestService = eventTypeRestService;
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IWaiverManager waiverManager = waiverManager;
    private readonly IEventPhotoManager eventPhotoManager = eventPhotoManager;
    private readonly IEventAttendeeMetricsRestService eventAttendeeMetricsRestService = eventAttendeeMetricsRestService;
 
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
    private bool canManageCoLeads;

    [ObservableProperty]
    private int coLeadCount;

    [ObservableProperty]
    private string coLeadCountDisplay = string.Empty;

    private const int MaxCoLeads = 5;

    [ObservableProperty]
    private bool isLitterReportMapSelected;

    [ObservableProperty]
    private bool isLitterReportListSelected;
    
    private Action UpdateRoutes = null!;

    [ObservableProperty]
    private DateTimeOffset routeStartTime;

    [ObservableProperty]
    private DateTimeOffset routeEndTime;

    [ObservableProperty]
    private bool arePhotosFound;

    [ObservableProperty]
    private bool areNoPhotosFound = true;

    [ObservableProperty]
    private bool canUploadPhoto;

    [ObservableProperty]
    private string photoCountDisplay = string.Empty;

    [ObservableProperty]
    private bool enableSimulateRoute;

    [ObservableProperty]
    private bool isRecordingRoute;

    [ObservableProperty]
    private bool areRoutesFound;

    [ObservableProperty]
    private bool areNoRoutesFound = true;

    [ObservableProperty]
    private string routeCountDisplay = "No routes";

    [ObservableProperty]
    private string totalDistanceDisplay = "0 m";

    [ObservableProperty]
    private string totalDurationDisplay = "0 min";

    [ObservableProperty]
    private string totalBagsDisplay = "0";

    [ObservableProperty]
    private bool showLogImpactButton;

    [ObservableProperty]
    private bool showMyContribution;

    [ObservableProperty]
    private int myBagsCollected;

    [ObservableProperty]
    private string myWeightDisplay = string.Empty;

    [ObservableProperty]
    private int myDurationMinutes;

    [ObservableProperty]
    private string myMetricsStatus = string.Empty;

    [ObservableProperty]
    private int pendingMetricsCount;

    [ObservableProperty]
    private bool showLeadMetricsActions;

    private EventAttendeeMetrics? existingMetrics;

    public List<string> PrivacyOptions { get; } = ["Private", "EventOnly", "Public"];

    public ObservableCollection<EventPhotoViewModel> EventPhotos { get; set; } = [];

    public ObservableCollection<EventPartnerLocationViewModel> AvailablePartners { get; set; } = new();

    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    public ObservableCollection<AddressViewModel> Addresses { get; set; } = [];

    public ObservableCollection<LitterReportViewModel> EventLitterReports { get; set; } = [];

    public ObservableCollection<LitterImageViewModel> LitterImages { get; set; } = [];

    public ObservableCollection<EventAttendeeViewModel> EventAttendees { get; set; } = [];

    public ObservableCollection<DisplayEventAttendeeRoute> EventAttendeeRoutes { get; set; } = [];

    public ObservableCollection<EventAttendeeRouteViewModel> EventAttendeeRouteViewModels { get; set; } = [];

    private bool partnersLoaded;
    private bool litterLoaded;
    private bool photosLoaded;
    private bool routesLoaded;

    public async Task Init(Guid eventId, Action updRoutes)
    {
        await ExecuteAsync(async () =>
        {
            UpdateRoutes = updRoutes;
            partnersLoaded = false;
            litterLoaded = false;
            photosLoaded = false;
            routesLoaded = false;

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

            EnableStartTrackEventRoute = !mobEvent.IsCompleted();
            EnableStopTrackEventRoute = false;

            // If this event is already being tracked, restore recording UI state
            if (routeTrackingSessionManager.IsTracking && routeTrackingSessionManager.ActiveEventId == eventId)
            {
                EnableStartTrackEventRoute = false;
                EnableStopTrackEventRoute = true;
                IsRecordingRoute = true;
            }

            WhatToExpect =
                "What to Expect:\n\u2022 Cleanup supplies provided\n\u2022 Meet fellow community members\n\u2022 Contribute to a cleaner environment";

            // Load only what the Details tab needs — registration status, attendee count, and metrics
            await Task.WhenAll(SetRegistrationOptions(), GetAttendeeCount());
            await LoadMyMetrics();
        }, "An error occurred while loading the event. Please try again.");
    }

    /// <summary>
    /// Called when a tab becomes visible. Loads data for that tab on first view.
    /// </summary>
    public async Task OnTabSelected(int tabIndex)
    {
        switch (tabIndex)
        {
            case 1 when !partnersLoaded:
                partnersLoaded = true;
                await LoadPartners();
                break;
            case 3 when !litterLoaded:
                litterLoaded = true;
                await LoadLitterReports();
                break;
            case 4 when !photosLoaded:
                photosLoaded = true;
                await LoadPhotos();
                break;
            case 5 when !routesLoaded:
                routesLoaded = true;
                var routes = await eventAttendeeRouteRestService.GetEventAttendeeRoutesForEventAsync(EventViewModel.Id);
                LoadRouteViewModels(routes);
                break;
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
        currentAttendeeCount = attendees.Count();
        var eventLeads = await eventAttendeeRestService.GetEventLeadsAsync(mobEvent.Id);
        var eventLeadIds = new HashSet<Guid>(eventLeads.Select(l => l.Id));

        CoLeadCount = eventLeadIds.Count;
        CoLeadCountDisplay = $"Co-leads: {CoLeadCount}/{MaxCoLeads}";
        CanManageCoLeads = mobEvent.IsEventLead(userManager.CurrentUser.Id) && !mobEvent.IsCompleted();

        EventAttendees.Clear();

        foreach (var attendee in attendees)
        {
            var attendeeVm = attendee.ToEventAttendeeViewModel();
            var isLead = eventLeadIds.Contains(attendee.Id);
            var isCreator = attendee.Id == mobEvent.CreatedByUserId;

            attendeeVm.EventId = mobEvent.Id;
            attendeeVm.IsEventLead = isLead;
            attendeeVm.IsEventCreator = isCreator;
            attendeeVm.Role = isLead ? (isCreator ? "Creator" : "Co-lead") : "Attendee";
            attendeeVm.CanPromote = CanManageCoLeads && !isLead && !isCreator && CoLeadCount < MaxCoLeads;
            attendeeVm.CanDemote = CanManageCoLeads && isLead && !isCreator && CoLeadCount > 1;

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
    
    public ObservableCollection<Microsoft.Maui.Devices.Sensors.Location> Locations { get; } = [];

    [RelayCommand(IncludeCancelCommand = true, AllowConcurrentExecutions = false)]
    private async Task RealTimeLocationTracker(CancellationToken cancellationToken)
    {
        // Prevent concurrent tracking across different events
        if (routeTrackingSessionManager.IsTracking && routeTrackingSessionManager.ActiveEventId != mobEvent.Id)
        {
            var popup = new ConfirmPopup(
                "Route In Progress",
                $"You're already tracking a route for \"{routeTrackingSessionManager.ActiveEventName}\". Only one route can be tracked at a time.",
                "OK");
            await Shell.Current.CurrentPage.ShowPopupAsync<string>(popup);

            EnableStopTrackEventRoute = false;
            EnableStartTrackEventRoute = true;
            IsRecordingRoute = false;
            return;
        }

        if (EnableStartTrackEventRoute)
        {
            RouteStartTime = DateTimeOffset.Now;
            RouteEndTime = DateTimeOffset.Now;
            EnableStopTrackEventRoute = true;
            EnableStartTrackEventRoute = false;
            IsRecordingRoute = true;
            routeTrackingSessionManager.TryStartSession(mobEvent.Id, mobEvent.Name);
        }

        if (DeviceInfo.DeviceType == DeviceType.Virtual)
        {
            // On emulators, wait for Stop to be pressed, then simulate
            var tcs = new TaskCompletionSource();
            cancellationToken.Register(() =>
            {
                IsRecordingRoute = false;
                RouteEndTime = DateTimeOffset.Now;
                routeTrackingSessionManager.EndSession();
                tcs.TrySetResult();
            });

            await tcs.Task;

            await SimulateRoute();
            EnableStopTrackEventRoute = false;
            EnableStartTrackEventRoute = true;
            return;
        }

        cancellationToken.Register(async () =>
        {
            IsRecordingRoute = false;
            RouteEndTime = DateTimeOffset.Now;
            routeTrackingSessionManager.EndSession();
            await SaveRoute();
            Locations.Clear();
            EnableStopTrackEventRoute = false;
            EnableStartTrackEventRoute = true;
        });

        var progress = new Progress<Microsoft.Maui.Devices.Sensors.Location>(location =>
        {
            location.Timestamp = DateTimeOffset.Now;
            Locations.Add(location);
        });

        await Geolocator.Default.StartListening(progress, cancellationToken);
    }

    private async Task SaveRoute()
    {
        await ExecuteAsync(async () =>
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
                Locations = GetSortableLocations(),
                StartTime = RouteStartTime,
                EndTime = RouteEndTime,
            });
        }, "An error occurred while saving your route.");
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
        await ExecuteAsync(async () =>
        {
            if (!await waiverManager.HasUserSignedAllRequiredWaiversAsync())
            {
                await Shell.Current.GoToAsync($"{nameof(WaiverListPage)}");
                return;
            }

            var eventAttendee = new EventAttendee
            {
                EventId = EventViewModel.Id,
                UserId = userManager.CurrentUser.Id,
            };

            await mobEventManager.AddEventAttendeeAsync(eventAttendee);

            await SetRegistrationOptions();
            await GetAttendeeCount();

            var registerPopup = new ConfirmPopup("Registered!", "You have been registered for this event. We'll see you there!", "OK");
            await Shell.Current.CurrentPage.ShowPopupAsync<string>(registerPopup);
        }, "An error occurred while registering you for this event. Please try again.");
    }

    [RelayCommand]
    private async Task Unregister()
    {
        await ExecuteAsync(async () =>
        {
            var eventAttendee = new EventAttendee
            {
                EventId = EventViewModel.Id,
                UserId = userManager.CurrentUser.Id,
            };

            await mobEventManager.RemoveEventAttendeeAsync(eventAttendee);

            await SetRegistrationOptions();
            await GetAttendeeCount();

            var unregisterPopup = new ConfirmPopup("Unregistered", "You have been unregistered from this event.", "OK");
            await Shell.Current.CurrentPage.ShowPopupAsync<string>(unregisterPopup);
        }, "An error occurred while unregistering you for this event. Please try again.");
    }

    private int currentAttendeeCount;

    private async Task SetRegistrationOptions()
    {
        var isAttending = await mobEventManager.IsUserAttendingAsync(mobEvent.Id, userManager.CurrentUser.Id);

        var isFull = mobEvent.MaxNumberOfParticipants > 0
                     && currentAttendeeCount >= mobEvent.MaxNumberOfParticipants;

        EnableRegister = !mobEvent.IsEventLead(userManager.CurrentUser.Id)
                        && !isAttending
                        && mobEvent.AreNewRegistrationsAllowed()
                        && !isFull;
        EnableUnregister = !mobEvent.IsEventLead(userManager.CurrentUser.Id)
                          && isAttending
                          && mobEvent.AreUnregistrationsAllowed();
    }

    [RelayCommand]
    private async Task PromoteToLead(EventAttendeeViewModel attendee)
    {
        if (!CanManageCoLeads || attendee.IsEventLead || CoLeadCount >= MaxCoLeads)
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            await eventAttendeeRestService.PromoteToLeadAsync(attendee.EventId, attendee.AttendeeId);
            await GetAttendeeCount();
            await NotificationService.Notify($"{attendee.UserName} has been promoted to co-lead.");
        }, "An error occurred while promoting the attendee. Please try again.");
    }

    [RelayCommand]
    private async Task DemoteFromLead(EventAttendeeViewModel attendee)
    {
        if (!CanManageCoLeads || !attendee.IsEventLead || attendee.IsEventCreator || CoLeadCount <= 1)
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            await eventAttendeeRestService.DemoteFromLeadAsync(attendee.EventId, attendee.AttendeeId);
            await GetAttendeeCount();
            await NotificationService.Notify($"{attendee.UserName} has been demoted from co-lead.");
        }, "An error occurred while demoting the co-lead. Please try again.");
    }

    private async Task LoadPhotos()
    {
        ArePhotosFound = false;
        AreNoPhotosFound = true;

        var photos = await eventPhotoManager.GetEventPhotosAsync(EventViewModel.Id);

        EventPhotos.Clear();

        var currentUserId = userManager.CurrentUser.Id;
        var isLead = mobEvent.IsEventLead(currentUserId);

        foreach (var photo in photos.OrderByDescending(p => p.UploadedDate))
        {
            EventPhotos.Add(new EventPhotoViewModel
            {
                Id = photo.Id,
                EventId = photo.EventId,
                ImageUrl = photo.ImageUrl ?? string.Empty,
                ThumbnailUrl = photo.ThumbnailUrl ?? string.Empty,
                PhotoType = photo.PhotoType,
                Caption = photo.Caption ?? string.Empty,
                UploadedDate = photo.UploadedDate,
                UploadedByUserId = photo.UploadedByUserId,
                CanDelete = photo.UploadedByUserId == currentUserId || isLead,
            });
        }

        ArePhotosFound = EventPhotos.Count > 0;
        AreNoPhotosFound = !ArePhotosFound;
        PhotoCountDisplay = EventPhotos.Count == 1 ? "1 photo" : $"{EventPhotos.Count} photos";

        var isAttending = await mobEventManager.IsUserAttendingAsync(mobEvent.Id, currentUserId);
        CanUploadPhoto = isLead || isAttending;
    }

    [RelayCommand]
    private async Task PickPhoto()
    {
        await ExecuteAsync(async () =>
        {
            var sourcePopup = new Controls.PhotoSourcePopup();
            var sourceResult = await Shell.Current.CurrentPage.ShowPopupAsync<string>(sourcePopup);
            var source = sourceResult?.Result;

            if (string.IsNullOrEmpty(source))
            {
                return;
            }

            FileResult? result;

            if (source == Controls.PhotoSourcePopup.TakePhoto)
            {
                result = await MediaPicker.Default.CapturePhotoAsync(new MediaPickerOptions
                {
                    Title = "Take a photo",
                });
            }
            else
            {
                var results = await MediaPicker.Default.PickPhotosAsync(new MediaPickerOptions
                {
                    Title = "Select a photo",
                });
                result = results?.FirstOrDefault();
            }

            if (result == null)
            {
                return;
            }

            // Copy to cache and compress before upload
            var cachedPath = Path.Combine(FileSystem.CacheDirectory, result.FileName);

            using (var sourceStream = await result.OpenReadAsync())
            using (var cacheStream = File.Create(cachedPath))
            {
                await sourceStream.CopyToAsync(cacheStream);
            }

            await ImageCompressor.CompressAsync(cachedPath);

            var typePopup = new Controls.PhotoTypePopup();
            var typeResult = await Shell.Current.CurrentPage.ShowPopupAsync<string>(typePopup);
            var photoType = typeResult?.Result;

            if (string.IsNullOrEmpty(photoType))
            {
                return;
            }

            var eventPhotoType = photoType switch
            {
                Controls.PhotoTypePopup.Before => EventPhotoType.Before,
                Controls.PhotoTypePopup.After => EventPhotoType.After,
                _ => EventPhotoType.During,
            };

            await eventPhotoManager.UploadPhotoAsync(
                EventViewModel.Id, cachedPath, eventPhotoType, string.Empty);

            await LoadPhotos();

            await NotificationService.Notify("Photo uploaded successfully.");
        }, "An error occurred while uploading the photo. Please try again.");
    }

    [RelayCommand]
    private async Task DeletePhoto(EventPhotoViewModel? photoVm)
    {
        if (photoVm == null)
        {
            return;
        }

        var popup = new Controls.ConfirmPopup("Delete Photo", "Are you sure you want to delete this photo?", "Delete");
        var result = await Shell.Current.CurrentPage.ShowPopupAsync<string>(popup);
        if (result?.Result != Controls.ConfirmPopup.Confirmed)
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            await eventPhotoManager.DeletePhotoAsync(photoVm.EventId, photoVm.Id);
            await LoadPhotos();
            await NotificationService.Notify("Photo deleted.");
        }, "An error occurred while deleting the photo. Please try again.");
    }

    [RelayCommand]
    private async Task SimulateRoute()
    {
        await ExecuteAsync(async () =>
        {
            var route = await eventAttendeeRouteRestService.SimulateRouteAsync(EventViewModel.Id);

            EventAttendeeRoutes.Add(route);
            EventAttendeeRouteViewModels.Add(
                EventAttendeeRouteViewModel.FromRoute(route, userManager.CurrentUser.Id));
            UpdateRouteStats();
            UpdateRoutes();

            await NotificationService.Notify("Route simulated successfully!");
        }, "An error occurred while simulating the route. Please try again.");
    }

    [RelayCommand]
    private async Task DeleteRoute(EventAttendeeRouteViewModel? routeVm)
    {
        if (routeVm == null)
        {
            return;
        }

        var popup = new Controls.ConfirmPopup("Delete Route", "Are you sure you want to delete this route?", "Delete");
        var result = await Shell.Current.CurrentPage.ShowPopupAsync<string>(popup);
        if (result?.Result != Controls.ConfirmPopup.Confirmed)
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            await eventAttendeeRouteRestService.DeleteEventAttendeeRouteAsync(routeVm.Id);

            var rawRoute = EventAttendeeRoutes.FirstOrDefault(r => r.Id == routeVm.Id);
            if (rawRoute != null)
            {
                EventAttendeeRoutes.Remove(rawRoute);
            }

            EventAttendeeRouteViewModels.Remove(routeVm);
            UpdateRouteStats();
            UpdateRoutes();

            await NotificationService.Notify("Route deleted.");
        }, "An error occurred while deleting the route. Please try again.");
    }

    [RelayCommand]
    private async Task ChangeRoutePrivacy(EventAttendeeRouteViewModel? routeVm)
    {
        if (routeVm == null)
        {
            return;
        }

        var popup = new Controls.PrivacyPopup();
        var popupResult = await Shell.Current.CurrentPage.ShowPopupAsync<string>(popup);
        var selectedPrivacy = popupResult?.Result;
        if (string.IsNullOrEmpty(selectedPrivacy))
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            var request = new UpdateRouteMetadataRequest
            {
                PrivacyLevel = selectedPrivacy,
            };

            var updated = await eventAttendeeRouteRestService.UpdateRouteMetadataAsync(routeVm.Id, request);

            routeVm.PrivacyLevel = updated.PrivacyLevel;

            var rawRoute = EventAttendeeRoutes.FirstOrDefault(r => r.Id == routeVm.Id);
            if (rawRoute != null)
            {
                rawRoute.PrivacyLevel = updated.PrivacyLevel;
            }

            await NotificationService.Notify("Privacy updated.");
        }, "An error occurred while updating privacy. Please try again.");
    }

    [RelayCommand]
    private async Task LogPickup(EventAttendeeRouteViewModel? routeVm)
    {
        if (routeVm == null || !routeVm.IsOwnRoute)
        {
            return;
        }

        var popup = new LogPickupPopup(
            routeVm.BagsCollected, routeVm.WeightCollected,
            routeVm.WeightUnitId, routeVm.Notes);
        var popupResult = await Shell.Current.CurrentPage.ShowPopupAsync<string>(popup);
        var resultJson = popupResult?.Result;

        if (string.IsNullOrEmpty(resultJson))
        {
            return;
        }

        var pickupResult = JsonConvert.DeserializeObject<LogPickupResult>(resultJson);
        if (pickupResult == null)
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            var request = new UpdateRouteMetadataRequest
            {
                PrivacyLevel = routeVm.PrivacyLevel,
                BagsCollected = pickupResult.BagsCollected,
                WeightCollected = pickupResult.WeightCollected,
                WeightUnitId = pickupResult.WeightUnitId,
                Notes = pickupResult.Notes,
            };

            var updated = await eventAttendeeRouteRestService.UpdateRouteMetadataAsync(routeVm.Id, request);

            // Update the view model
            var weightUnitLabel = updated.WeightUnitId == (int)WeightUnitEnum.Kilogram ? "kg" : "lbs";
            routeVm.BagsCollected = updated.BagsCollected;
            routeVm.WeightCollected = updated.WeightCollected;
            routeVm.WeightUnitId = updated.WeightUnitId;
            routeVm.Notes = updated.Notes;
            routeVm.BagsDisplay = updated.BagsCollected.HasValue ? $"{updated.BagsCollected} bags" : string.Empty;
            routeVm.WeightDisplay = updated.WeightCollected.HasValue ? $"{updated.WeightCollected:F1} {weightUnitLabel}" : string.Empty;

            // Update the raw route data too
            var rawRoute = EventAttendeeRoutes.FirstOrDefault(r => r.Id == routeVm.Id);
            if (rawRoute != null)
            {
                rawRoute.BagsCollected = updated.BagsCollected;
                rawRoute.WeightCollected = updated.WeightCollected;
                rawRoute.WeightUnitId = updated.WeightUnitId;
            }

            UpdateRouteStats();

            await NotificationService.Notify("Pickup logged!");
        }, "An error occurred while saving pickup data.");
    }

    [RelayCommand]
    private async Task TrimRouteTime(EventAttendeeRouteViewModel? routeVm)
    {
        if (routeVm == null || !routeVm.IsOwnRoute)
        {
            return;
        }

        if (routeVm.IsTimeTrimmed)
        {
            // Restore original route
            var confirmPopup = new ConfirmPopup("Restore Route", "Restore this route to its original end time?", "Restore");
            var confirmResult = await Shell.Current.CurrentPage.ShowPopupAsync<string>(confirmPopup);
            if (confirmResult?.Result != ConfirmPopup.Confirmed)
            {
                return;
            }

            await ExecuteAsync(async () =>
            {
                var updated = await eventAttendeeRouteRestService.RestoreRouteTimeAsync(routeVm.Id);
                RefreshRouteViewModel(routeVm, updated);
                await NotificationService.Notify("Route restored.");
            }, "An error occurred while restoring the route.");
        }
        else
        {
            // Trim end time
            var popup = new TrimRoutePopup(routeVm.StartTime, routeVm.EndTime);
            var popupResult = await Shell.Current.CurrentPage.ShowPopupAsync<string>(popup);
            var resultString = popupResult?.Result;

            if (string.IsNullOrEmpty(resultString) || !DateTimeOffset.TryParse(resultString, out var newEndTime))
            {
                return;
            }

            await ExecuteAsync(async () =>
            {
                var request = new TrimRouteTimeRequest { NewEndTime = newEndTime };
                var updated = await eventAttendeeRouteRestService.TrimRouteTimeAsync(routeVm.Id, request);
                RefreshRouteViewModel(routeVm, updated);
                await NotificationService.Notify("Route trimmed.");
            }, "An error occurred while trimming the route.");
        }
    }

    private void RefreshRouteViewModel(EventAttendeeRouteViewModel routeVm, DisplayEventAttendeeRoute updated)
    {
        var refreshed = EventAttendeeRouteViewModel.FromRoute(updated, userManager.CurrentUser.Id);
        routeVm.DistanceDisplay = refreshed.DistanceDisplay;
        routeVm.DurationDisplay = refreshed.DurationDisplay;
        routeVm.IsTimeTrimmed = refreshed.IsTimeTrimmed;
        routeVm.StartTime = refreshed.StartTime;
        routeVm.EndTime = refreshed.EndTime;
        routeVm.OriginalEndTime = refreshed.OriginalEndTime;
        routeVm.Locations = refreshed.Locations;

        // Update the raw route data for map re-rendering
        var rawRoute = EventAttendeeRoutes.FirstOrDefault(r => r.Id == routeVm.Id);
        if (rawRoute != null)
        {
            var index = EventAttendeeRoutes.IndexOf(rawRoute);
            EventAttendeeRoutes[index] = updated;
        }

        UpdateRouteStats();
    }

    private void LoadRouteViewModels(IEnumerable<DisplayEventAttendeeRoute> routes)
    {
        EventAttendeeRoutes.Clear();
        EventAttendeeRouteViewModels.Clear();

        var currentUserId = userManager.CurrentUser.Id;

        foreach (var route in routes)
        {
            EventAttendeeRoutes.Add(route);
            EventAttendeeRouteViewModels.Add(
                EventAttendeeRouteViewModel.FromRoute(route, currentUserId));
        }

        UpdateRouteStats();
        UpdateRoutes();
    }

    private void UpdateRouteStats()
    {
        var count = EventAttendeeRoutes.Count;
        AreRoutesFound = count > 0;
        AreNoRoutesFound = !AreRoutesFound;
        RouteCountDisplay = count == 1 ? "1 route" : $"{count} routes";

        var totalMeters = EventAttendeeRoutes.Sum(r => r.TotalDistanceMeters);
        TotalDistanceDisplay = totalMeters >= 1000
            ? $"{totalMeters / 1000.0:F1} km"
            : $"{totalMeters} m";

        var totalMinutes = EventAttendeeRoutes.Sum(r => r.DurationMinutes);
        TotalDurationDisplay = totalMinutes >= 60
            ? $"{totalMinutes / 60} hr {totalMinutes % 60} min"
            : $"{totalMinutes} min";

        var totalBags = EventAttendeeRoutes.Sum(r => r.BagsCollected ?? 0);
        TotalBagsDisplay = totalBags.ToString();
    }

    private async Task LoadMyMetrics()
    {
        if (!mobEvent.IsCompleted())
        {
            return;
        }

        var isAttending = await mobEventManager.IsUserAttendingAsync(mobEvent.Id, userManager.CurrentUser.Id);
        var isLead = mobEvent.IsEventLead(userManager.CurrentUser.Id);

        if (isAttending || isLead)
        {
            existingMetrics = await eventAttendeeMetricsRestService.GetMyMetricsAsync(mobEvent.Id);

            if (existingMetrics != null)
            {
                MyBagsCollected = existingMetrics.BagsCollected ?? 0;
                MyWeightDisplay = FormatWeight(existingMetrics.PickedWeight, existingMetrics.PickedWeightUnitId);
                MyDurationMinutes = existingMetrics.DurationMinutes ?? 0;
                MyMetricsStatus = existingMetrics.Status ?? "Pending";
                ShowMyContribution = true;
                ShowLogImpactButton = false;
            }
            else
            {
                ShowLogImpactButton = true;
                ShowMyContribution = false;
            }
        }

        if (isLead)
        {
            var publicMetrics = await eventAttendeeMetricsRestService.GetPublicMetricsAsync(mobEvent.Id);
            PendingMetricsCount = publicMetrics.Contributors.Count(c => c.Status == "Pending");
            ShowLeadMetricsActions = PendingMetricsCount > 0;
        }
    }

    private static string FormatWeight(decimal? weight, int? unitId)
    {
        if (!weight.HasValue || weight.Value == 0)
        {
            return "0 lbs";
        }

        var unit = unitId == 2 ? "kg" : "lbs";
        return $"{weight.Value:0.##} {unit}";
    }

    [RelayCommand]
    private async Task LogImpact()
    {
        await ExecuteAsync(async () =>
        {
            var popup = new Controls.LogImpactPopup(existingMetrics);
            var result = await Shell.Current.CurrentPage.ShowPopupAsync<string>(popup);
            var json = result?.Result;

            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            var impactResult = System.Text.Json.JsonSerializer.Deserialize<Controls.LogImpactPopup.LogImpactResult>(json);
            if (impactResult == null)
            {
                return;
            }

            var metrics = new EventAttendeeMetrics
            {
                EventId = mobEvent.Id,
                UserId = userManager.CurrentUser.Id,
                BagsCollected = impactResult.BagsCollected,
                PickedWeight = impactResult.PickedWeight,
                PickedWeightUnitId = impactResult.PickedWeightUnitId,
                DurationMinutes = impactResult.DurationMinutes,
                Notes = impactResult.Notes,
            };

            await eventAttendeeMetricsRestService.SubmitMyMetricsAsync(mobEvent.Id, metrics);
            await LoadMyMetrics();

            await NotificationService.Notify("Your impact has been logged!");
        }, "An error occurred while logging your impact. Please try again.");
    }

    [RelayCommand]
    private async Task ApproveAllMetrics()
    {
        var popup = new Controls.ConfirmPopup(
            "Approve All Metrics",
            "Are you sure you want to approve all pending attendee metrics?",
            "Approve All");
        var confirmResult = await Shell.Current.CurrentPage.ShowPopupAsync<string>(popup);

        if (confirmResult?.Result != Controls.ConfirmPopup.Confirmed)
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            var count = await eventAttendeeMetricsRestService.ApproveAllPendingAsync(mobEvent.Id);
            ShowLeadMetricsActions = false;
            PendingMetricsCount = 0;

            await NotificationService.Notify($"{count} metric submission(s) approved.");
        }, "An error occurred while approving metrics. Please try again.");
    }
}