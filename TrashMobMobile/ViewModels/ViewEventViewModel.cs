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
using TrashMobMobile.Services.Offline;

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
    IEventAttendeeMetricsRestService eventAttendeeMetricsRestService,
    RoutePointWriter routePointWriter,
    SyncQueue syncQueue,
    IDependentRestService dependentRestService) : BaseViewModel(notificationService)
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

    private bool skipDefaultTrim;

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
    private bool hasDensityData;

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

    public ObservableCollection<EventAttendeeMetrics> PendingMetricsList { get; set; } = [];

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

            // Check for crashed/interrupted route sessions for this event
            await CheckForInterruptedSessionsAsync(eventId);

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
        await ExecuteAsync(async () =>
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
        }, "Failed to load tab data. Please try again.");
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

        var depCount = await dependentRestService.GetEventDependentCountAsync(mobEvent.Id);
        var totalHeadcount = currentAttendeeCount + depCount;

        if (totalHeadcount == 1)
        {
            AttendeeCount = "1 person is going!";
        }
        else
        {
            AttendeeCount = $"{totalHeadcount} people are going!";
        }

        if (depCount > 0)
        {
            AttendeeCount += $" ({currentAttendeeCount} adult{(currentAttendeeCount != 1 ? "s" : "")}, {depCount} dependent{(depCount != 1 ? "s" : "")})";
        }

        if (mobEvent.MaxNumberOfParticipants > 0)
        {
            if (mobEvent.MaxNumberOfParticipants - currentAttendeeCount > 0)
            {
                SpotsLeft = $"{mobEvent.MaxNumberOfParticipants - currentAttendeeCount} spot left!";
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
            // Show prominent disclosure the first time the user starts route tracking (Google Play policy)
            const string disclosureKey = "RouteTrackingDisclosureShown";
            if (!Preferences.Default.Get(disclosureKey, false))
            {
                var disclosureMessage =
                    "TrashMob needs access to your location to record your cleanup route on a map. " +
                    "Your location will be tracked while the route recording is active and a notification is shown. " +
                    "You can stop recording at any time.";
                var accepted = await Shell.Current.CurrentPage.DisplayAlertAsync(
                    "Location Permission Required", disclosureMessage, "Continue", "Cancel");

                if (!accepted)
                {
                    return;
                }

                Preferences.Default.Set(disclosureKey, true);
            }

            // Ask user whether to hide start/end location for privacy
            var privacyMessage =
                "Would you like to hide the first and last 100 meters of your route? " +
                "This helps protect your home location if you're starting from home.";
            var hideStartEnd = await Shell.Current.CurrentPage.DisplayAlertAsync(
                "Privacy Option", privacyMessage, "Yes, hide", "No, keep full route");

            skipDefaultTrim = !hideStartEnd;

            RouteStartTime = DateTimeOffset.Now;
            RouteEndTime = DateTimeOffset.Now;
            EnableStopTrackEventRoute = true;
            EnableStartTrackEventRoute = false;
            IsRecordingRoute = true;

            // Create SQLite session for crash-safe persistence
            var session = await syncQueue.CreateRouteSessionAsync(
                mobEvent.Id, userManager.CurrentUser.Id, RouteStartTime, skipDefaultTrim);
            routeTrackingSessionManager.TryStartSession(mobEvent.Id, mobEvent.Name, session.SessionId);
            routePointWriter.StartSession(session.SessionId);
        }

        if (DeviceInfo.DeviceType == DeviceType.Virtual)
        {
            // On emulators, wait for Stop to be pressed, then simulate
            var tcs = new TaskCompletionSource();
            cancellationToken.Register(async () =>
            {
                IsRecordingRoute = false;
                RouteEndTime = DateTimeOffset.Now;
                await routePointWriter.StopAndFlushAsync();

                var sessionId = routeTrackingSessionManager.ActiveSessionId;
                routeTrackingSessionManager.EndSession();

                // Discard the empty SQLite session since emulator uses server-side simulation
                if (sessionId != null)
                {
                    await syncQueue.DiscardSessionAsync(sessionId);
                }

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

            // Flush remaining GPS points to SQLite
            await routePointWriter.StopAndFlushAsync();

            // Mark session ready for upload
            var sessionId = routeTrackingSessionManager.ActiveSessionId;
            routeTrackingSessionManager.EndSession();

            if (sessionId != null)
            {
                await syncQueue.MarkSessionPendingUploadAsync(sessionId, RouteEndTime);
            }

            await SaveRoute(sessionId);
            Locations.Clear();
            EnableStopTrackEventRoute = false;
            EnableStartTrackEventRoute = true;
        });

        var progress = new Progress<Microsoft.Maui.Devices.Sensors.Location>(location =>
        {
            location.Timestamp = DateTimeOffset.Now;
            Locations.Add(location);

            // Persist to SQLite for crash recovery
            routePointWriter.AddPoint(location.Latitude, location.Longitude, location.Altitude, location.Timestamp);
        });

        await Geolocator.Default.StartListening(progress, cancellationToken);
    }

    private async Task SaveRoute(string? sessionId)
    {
        // If there are no locations, discard the empty session
        if (Locations.Count == 0)
        {
            if (sessionId != null)
            {
                await syncQueue.DiscardSessionAsync(sessionId);
            }

            return;
        }

        try
        {
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
                SkipDefaultTrim = skipDefaultTrim,
                SessionId = sessionId != null ? Guid.Parse(sessionId) : null,
            });

            // Upload succeeded — clean up SQLite data
            if (sessionId != null)
            {
                await syncQueue.MarkSessionUploadedAsync(sessionId);
            }
        }
        catch (Exception ex)
        {
            // Upload failed — data is safe in SQLite, will retry via SyncService
            if (sessionId != null)
            {
                await syncQueue.MarkSessionFailedAsync(sessionId, ex.Message);
                SentrySdk.AddBreadcrumb(
                    $"Route queued offline: event={mobEvent.Id}, session={sessionId}",
                    "sync",
                    level: BreadcrumbLevel.Info);
                _ = NotificationService.Notify(
                    "Route saved locally and will upload when connection improves.");
            }
            else
            {
                _ = NotificationService.Notify(
                    "An error occurred while saving your route.");
            }
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

    /// <summary>
    /// Checks for route sessions that were interrupted (app crash/kill during recording).
    /// Offers the user to upload the partial route or discard it.
    /// </summary>
    private async Task CheckForInterruptedSessionsAsync(Guid eventId)
    {
        try
        {
            var interruptedSessions = await syncQueue.GetInterruptedSessionsAsync();
            var eventSession = interruptedSessions
                .FirstOrDefault(s => s.EventId == eventId.ToString());

            if (eventSession == null)
            {
                return;
            }

            var points = await syncQueue.GetRoutePointsAsync(eventSession.SessionId);
            if (points.Count == 0)
            {
                await syncQueue.DiscardSessionAsync(eventSession.SessionId);
                return;
            }

            var recoveryMessage =
                $"A route recording with {points.Count} GPS points was interrupted. " +
                "Would you like to upload the partial route?";
            var recover = await Shell.Current.CurrentPage.DisplayAlertAsync(
                "Route Recovery", recoveryMessage, "Upload", "Discard");

            if (recover)
            {
                // Mark as pending upload with the last point's timestamp as end time
                var lastTimestamp = DateTimeOffset.TryParse(points.Last().Timestamp, out var ts)
                    ? ts
                    : DateTimeOffset.UtcNow;
                await syncQueue.MarkSessionPendingUploadAsync(eventSession.SessionId, lastTimestamp);

                // Attempt immediate upload via SyncService
                _ = NotificationService.Notify("Uploading recovered route...");
            }
            else
            {
                await syncQueue.DiscardSessionAsync(eventSession.SessionId);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Crash recovery check failed: {ex.Message}");
        }
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

            // Offer to register dependents if user has any
            var dependents = await dependentRestService.GetDependentsAsync(userManager.CurrentUser.Id);
            if (dependents.Count > 0)
            {
                var dependentPopup = new SelectDependentsPopup(dependents);
                var popupResult = await Shell.Current.CurrentPage.ShowPopupAsync<List<Guid>>(dependentPopup);
                var selectedIds = popupResult?.Result;

                if (selectedIds is { Count: > 0 })
                {
                    await dependentRestService.RegisterDependentsForEventAsync(
                        EventViewModel.Id, selectedIds);

                    var countPopup = new ConfirmPopup("Dependents Registered!",
                        $"{selectedIds.Count} dependent(s) registered for this event.", "OK");
                    await Shell.Current.CurrentPage.ShowPopupAsync<string>(countPopup);
                }
            }
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

            // Copy to cache and compress
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

            // Check storage cap before queueing
            var pendingDir = Path.Combine(FileSystem.AppDataDirectory, "pending_photos");
            Directory.CreateDirectory(pendingDir);

            var totalSize = new DirectoryInfo(pendingDir).EnumerateFiles().Sum(f => f.Length);
            if (totalSize >= 500L * 1024 * 1024)
            {
                // Clean up the cached copy since we won't persist it
                if (File.Exists(cachedPath))
                {
                    File.Delete(cachedPath);
                }

                await NotificationService.Notify("Photo storage is full. Please sync pending photos first.");
                return;
            }

            // Move from CacheDirectory to AppDataDirectory so OS won't delete before upload
            var persistedPath = Path.Combine(pendingDir, $"{Guid.NewGuid()}.jpg");
            File.Move(cachedPath, persistedPath);

            // Save to SQLite first so data survives upload failure
            var pending = await syncQueue.EnqueuePhotoAsync(
                EventViewModel.Id, userManager.CurrentUser.Id,
                persistedPath, (int)eventPhotoType);

            try
            {
                await eventPhotoManager.UploadPhotoAsync(
                    EventViewModel.Id, persistedPath, eventPhotoType, string.Empty);
                await syncQueue.MarkPhotoUploadedAsync(pending.Id);
                await LoadPhotos();
                await NotificationService.Notify("Photo uploaded successfully.");
            }
            catch (Exception)
            {
                await syncQueue.MarkPhotoFailedAsync(pending.Id, "Upload failed, will retry.");
                SentrySdk.AddBreadcrumb(
                    $"Photo queued offline: event={EventViewModel.Id}, id={pending.Id}",
                    "sync",
                    level: BreadcrumbLevel.Info);
                await NotificationService.Notify("Photo saved offline. It will upload when connectivity returns.");
            }
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

        HasDensityData = EventAttendeeRoutes.Any(r => r.DensityGramsPerMeter is not null and > 0);
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
            var pendingMetrics = await eventAttendeeMetricsRestService.GetPendingMetricsAsync(mobEvent.Id);
            PendingMetricsList.Clear();
            foreach (var m in pendingMetrics)
            {
                PendingMetricsList.Add(m);
            }

            PendingMetricsCount = PendingMetricsList.Count;
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

            // Save to SQLite first so data survives upload failure
            var pending = await syncQueue.EnqueueMetricsAsync(
                mobEvent.Id, userManager.CurrentUser.Id,
                impactResult.BagsCollected, impactResult.PickedWeight,
                impactResult.PickedWeightUnitId, impactResult.DurationMinutes,
                impactResult.Notes);

            try
            {
                await eventAttendeeMetricsRestService.SubmitMyMetricsAsync(mobEvent.Id, metrics);
                await syncQueue.MarkMetricsUploadedAsync(pending.Id);
                await LoadMyMetrics();
                await NotificationService.Notify("Your impact has been logged!");
            }
            catch (Exception)
            {
                await syncQueue.MarkMetricsFailedAsync(pending.Id, "Upload failed, will retry.");
                SentrySdk.AddBreadcrumb(
                    $"Metrics queued offline: event={mobEvent.Id}, id={pending.Id}",
                    "sync",
                    level: BreadcrumbLevel.Info);
                await NotificationService.Notify("Impact saved offline. It will upload when connectivity returns.");
            }
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
            PendingMetricsList.Clear();
            ShowLeadMetricsActions = false;
            PendingMetricsCount = 0;

            await NotificationService.Notify($"{count} metric submission(s) approved.");
        }, "An error occurred while approving metrics. Please try again.");
    }

    [RelayCommand]
    private async Task ReviewMetrics(EventAttendeeMetrics metrics)
    {
        var popup = new Controls.ReviewMetricsPopup(metrics);
        var result = await Shell.Current.CurrentPage.ShowPopupAsync<string>(popup);
        var value = result?.Result;

        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            if (value == Controls.ReviewMetricsPopup.ApprovedResult)
            {
                await eventAttendeeMetricsRestService.ApproveMetricsAsync(mobEvent.Id, metrics.Id);
                await NotificationService.Notify("Submission approved.");
            }
            else if (value.StartsWith(Controls.ReviewMetricsPopup.RejectedPrefix))
            {
                var reason = value[Controls.ReviewMetricsPopup.RejectedPrefix.Length..];
                await eventAttendeeMetricsRestService.RejectMetricsAsync(mobEvent.Id, metrics.Id, reason);
                await NotificationService.Notify("Submission rejected.");
            }

            await LoadMyMetrics();
        }, "An error occurred while reviewing the submission. Please try again.");
    }
}