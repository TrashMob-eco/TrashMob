namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Color = Microsoft.Maui.Graphics.Color;
using Sentry;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;
using TrashMobMobile.Pages.CreateEvent;
using TrashMob.Models.Poco;

public partial class CreateEventViewModel : BaseViewModel
{
    private const int ActiveEventStatus = 1;
    private const int NewLitterReportStatus = 1;
    private readonly IEventTypeRestService eventTypeRestService;
    private readonly IMapRestService mapRestService;

    private readonly IMobEventManager mobEventManager;
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService;
    private readonly ILitterReportManager litterReportManager;
    private readonly IEventLitterReportManager eventLitterReportManager;
    private readonly IUserManager userManager;
    private readonly ITeamManager teamManager;
    private readonly INotificationService notificationService;
    private IEnumerable<LitterReport> RawLitterReports { get; set; } = [];

    public ICommand PreviousCommand { get; set; } = null!;
    public ICommand NextCommand { get; set; } = null!;

    public ICommand CloseCommand { get; set; } = null!;

    [ObservableProperty] private EventViewModel eventViewModel = null!;

    [ObservableProperty] private AddressViewModel userLocation = null!;

    [ObservableProperty] private bool isStepValid;

    [ObservableProperty] private bool canGoBack;

    [ObservableProperty] private bool arePartnersAvailable;

    [ObservableProperty] private bool areNoPartnersAvailable;

    [ObservableProperty] private bool areLitterReportsAvailable;

    [ObservableProperty] private bool areNoLitterReportsAvailable;

    [ObservableProperty]
    private bool isLitterReportMapSelected;

    [ObservableProperty]
    private bool isLitterReportListSelected;

    [ObservableProperty]
    private bool isTeamPickerVisible;

    public ObservableCollection<string> VisibilityOptions { get; set; } = ["Public", "Team Only", "Private"];

    public ObservableCollection<string> TeamNames { get; set; } = [];

    private List<Team> UserTeams { get; set; } = [];

    private string selectedVisibility = "Public";

    public string SelectedVisibility
    {
        get => selectedVisibility;
        set
        {
            if (value == null)
                return;

            if (selectedVisibility != value)
            {
                selectedVisibility = value;
                OnPropertyChanged();

                IsTeamPickerVisible = value == "Team Only";

                EventViewModel.EventVisibilityId = value switch
                {
                    "Team Only" => (int)EventVisibilityEnum.TeamOnly,
                    "Private" => (int)EventVisibilityEnum.Private,
                    _ => (int)EventVisibilityEnum.Public,
                };

                if (value != "Team Only")
                {
                    EventViewModel.TeamId = null;
                    SelectedTeam = string.Empty;
                }
            }
        }
    }

    private string selectedTeam = string.Empty;

    public string SelectedTeam
    {
        get => selectedTeam;
        set
        {
            if (value == null)
                return;

            if (selectedTeam != value)
            {
                selectedTeam = value;
                OnPropertyChanged();

                var team = UserTeams.FirstOrDefault(t => t.Name == value);
                EventViewModel.TeamId = team?.Id;
            }
        }
    }

    private string selectedEventType = string.Empty;

    public string SelectedEventType
    {
        get => selectedEventType;
        set
        {
            if (value == null)
                return;

            if (selectedEventType != value)
            {
                selectedEventType = value;

                OnPropertyChanged();
            }
        }
    }

    private bool validating;

    public CreateEventViewModel(IMobEventManager mobEventManager,
        IEventTypeRestService eventTypeRestService,
        IMapRestService mapRestService,
        INotificationService notificationService,
        IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService,
        ILitterReportManager litterReportManager,
        IEventLitterReportManager eventLitterReportRestService,
        IUserManager userManager,
        ITeamManager teamManager)
        : base(notificationService)
    {
        this.mobEventManager = mobEventManager;
        this.eventTypeRestService = eventTypeRestService;
        this.mapRestService = mapRestService;
        this.notificationService = notificationService;
        this.eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
        this.litterReportManager = litterReportManager;
        this.eventLitterReportManager = eventLitterReportRestService;
        this.userManager = userManager;
        this.teamManager = teamManager;

        NextCommand = new Command(async () =>
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await Task.Delay(200);
            await SetCurrentStep(StepType.Forward);
            IsBusy = false;
        });

        PreviousCommand = new Command(async
            () =>
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await Task.Delay(200);
            await SetCurrentStep(StepType.Backward);

            IsBusy = false;
        });

        CloseCommand = new Command(async () => { await Navigation.PopAsync(); });
    }

    public string DefaultEventName { get; } = "New Event";

    public int CurrentStep { get; set; }

    [ObservableProperty] private IContentView currentView = null!;

    public IContentView[] Steps { get; set; } = [];

    public enum StepType
    {
        Forward,
        Backward
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

    private void ValidateViewModel()
    {
        EventDurationError = string.Empty;
        DescriptionRequiredError = string.Empty;

        var totalHours = EventViewModel.DurationHours + (EventViewModel.DurationMinutes / 60.0);

        if (totalHours > 10)
        {
            EventDurationError = "Event maximum duration can only be 10 hours";
        }

        if (totalHours < 1)
        {
            EventDurationError = "Event minimum duration must be at least 1 hour";
        }

        if (string.IsNullOrEmpty(EventViewModel.Description))
        {
            DescriptionRequiredError = "Event description is required";
        }
    }

    private void ValidateCurrentStep(object? sender, PropertyChangedEventArgs? e)
    {
        if (EventViewModel == null)
            return;

        if (validating)
            return;

        validating = true;

        ValidateViewModel();

        switch (CurrentStep)
        {
            //Step 1 validation 
            case 0:
                if (!string.IsNullOrEmpty(EventViewModel.Name) &&
                    EventViewModel.EventDateOnly != default &&
                    !string.IsNullOrEmpty(SelectedEventType) && string.IsNullOrEmpty(EventDurationError) &&
                    !string.IsNullOrEmpty(EventViewModel.Description))
                {
                    IsStepValid = true;
                }
                else
                {
                    IsStepValid = false;
                }

                break;
            //Step 2 validation 
            case 1:
                if (!string.IsNullOrEmpty(EventViewModel.Address.City) && !string.IsNullOrEmpty(EventViewModel.Address.Region))
                {
                    IsStepValid = true;
                }
                else
                {
                    IsStepValid = false;
                }

                break;
            default:
                break;
        }

        validating = false;
    }

    private void SetCurrentView()
    {
        StepTitle = stepTitles[CurrentStep];

        CurrentView = Steps[CurrentStep];

        switch (CurrentStep)
        {
            case 0:
            case 1:
            case 3:
                NextStepText = "Next";
                break;
            case 2:
                NextStepText = "Save Event";
                break;
            case 4:
                NextStepText = "Finish";
                break;
            default:
                break;
        }

        if (CurrentView is BaseStepClass current)
            current.OnNavigated();

        var activeColor = GetThemeColor("Primary", "PrimaryDark");
        var inactiveColor = GetThemeColor("BorderLight", "BorderDark");

        StepOneColor = CurrentStep == 0 ? activeColor : inactiveColor;
        StepTwoColor = CurrentStep == 1 ? activeColor : inactiveColor;
        StepFourColor = CurrentStep == 2 ? activeColor : inactiveColor;
        StepFiveColor = CurrentStep == 3 ? activeColor : inactiveColor;
        StepSixColor = CurrentStep == 4 ? activeColor : inactiveColor;
    }

    public async Task SetCurrentStep(StepType step)
    {
        /*
         * Step 1 Main details                  CurrentStep = 0
         * Step 2 Map Location                  CurrentStep = 1
         * Step 4 Event Summary and Save        CurrentStep = 2
         * Step 5 Add Partners                  CurrentStep = 3
         * Step 6 Add Litter Reports            CurrentStep = 4
         */

        if (step == StepType.Backward)
        {
            if (CurrentStep > 0)
            {
                CurrentStep--;
                SetCurrentView();
            }
        }
        else
        {
            if (CurrentStep < Steps.Length)
            {
                switch (CurrentStep)
                {
                    case 2:
                    {
                        if (await SaveEvent() == false)
                        {
                            return;
                        }

                        await LoadPartners();
                        await LoadLitterReports();

                        break;
                    }
                    case 4:
                        //Evaluate displaying a confirmation text
                        CloseCommand.Execute(null);
                        return;
                }

                CurrentStep++;

                SetCurrentView();
            }
        }

        CanGoBack = CurrentStep != 0 && CurrentStep != 4 && CurrentStep != 4;
    }

    // This is only for the map point
    public ObservableCollection<EventViewModel> Events { get; set; } = [];
    public ObservableCollection<LitterImageViewModel> LitterImages { get; set; } = [];
    public ObservableCollection<EventLitterReportViewModel> EventLitterReports { get; set; } = [];

    private List<EventType> EventTypes { get; set; } = [];
    private EventPartnerLocationViewModel selectedEventPartnerLocation = null!;

    public ObservableCollection<string> ETypes { get; set; } = [];

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

    private async void PerformNavigation(EventPartnerLocationViewModel eventPartnerLocationViewModel)
    {
        try
        {
            await Shell.Current.GoToAsync(
                $"{nameof(EditEventPartnerLocationServicesPage)}?EventId={EventViewModel.Id}&PartnerLocationId={eventPartnerLocationViewModel.PartnerLocationId}");
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
        }
    }

    private Guid? initialLitterReport = null;

    public async Task Init(Guid? litterReportId)
    {
        initialLitterReport = litterReportId;

        SetCurrentView();

        foreach (var item in Steps)
        {
            if (item is BaseStepClass step)
                step.ViewModel = this;
        }

        // Load event types first (anonymous endpoint, no auth required)
        await ExecuteAsync(async () =>
        {
            EventTypes = (await eventTypeRestService.GetEventTypesAsync()).ToList();
        }, "Failed to load event types. Please check your connection and try again.");

        if (EventTypes.Count == 0)
        {
            return;
        }

        await ExecuteAsync(async () =>
        {
            UserLocation = userManager.CurrentUser.GetAddress();

            // Load teams (requires auth) — non-fatal if it fails
            try
            {
                UserTeams = (await teamManager.GetMyTeamsAsync()).ToList();
                TeamNames.Clear();
                foreach (var team in UserTeams)
                {
                    TeamNames.Add(team.Name);
                }
            }
            catch (Exception ex)
            {
                SentrySdk.CaptureException(ex);
            }

            // Set defaults
            EventViewModel = new EventViewModel
            {
                Name = DefaultEventName,
                EventDate = DateTime.Now.AddDays(1),
                EventVisibilityId = (int)EventVisibilityEnum.Public,
                MaxNumberOfParticipants = 0,
                DurationHours = 2,
                DurationMinutes = 0,
                EventTypeId = EventTypes.OrderBy(e => e.DisplayOrder).First().Id,
                EventStatusId = ActiveEventStatus,
            };

            // If the LitterReportId is passed in, we need to load the litter report and set the address
            if (litterReportId != null && litterReportId != Guid.Empty)
            {
                var litterReport = await litterReportManager.GetLitterReportAsync(litterReportId.Value, ImageSizeEnum.Thumb);

                if (litterReport != null)
                {
                    var address = litterReport.LitterImages.FirstOrDefault();

                    if (address != null)
                    {
                        EventViewModel.Address.City = address.City;
                        EventViewModel.Address.Country = address.Country;
                        EventViewModel.Address.Latitude = address.Latitude;
                        EventViewModel.Address.Longitude = address.Longitude;
                        EventViewModel.Address.PostalCode = address.PostalCode;
                        EventViewModel.Address.StreetAddress = address.StreetAddress;
                        EventViewModel.Address.AddressType = AddressType.Event;
                        EventViewModel.Address.Location = new Microsoft.Maui.Devices.Sensors.Location(address.Latitude!.Value, address.Longitude!.Value);
                    }
                }
            }
            else
            {
                EventViewModel.Address = UserLocation;
            }

            EventViewModel.Address.IconFile = EventExtensions.GetMapIcon(false);

            await LoadPartners();

            EventViewModel.EventTime = TimeSpan.FromHours(9);

            foreach (var eventType in EventTypes)
            {
                ETypes.Add(eventType.Name);
            }

            SelectedEventType = EventTypes.OrderBy(e => e.DisplayOrder).First().Name;

            Events.Add(EventViewModel);

            // We need to subscribe to both eventViewmodel and creatEventViewmodel propertyChanged to validate step
            EventViewModel.PropertyChanged += ValidateCurrentStep;
            PropertyChanged += ValidateCurrentStep;
        }, "An error has occurred while loading the page. Please wait and try again in a moment.");
    }

    [RelayCommand]
    private async Task<bool> SaveEvent()
    {
        var result = await ExecuteAsync<bool>(async () =>
        {
            if (!await Validate())
            {
                return false;
            }

            if (!string.IsNullOrEmpty(SelectedEventType))
            {
                var eventType = EventTypes.FirstOrDefault(e => e.Name == SelectedEventType);
                if (eventType != null)
                {
                    EventViewModel.EventTypeId = eventType.Id;
                }
            }

            var mobEvent = EventViewModel.ToEvent();

            var updatedEvent = await mobEventManager.AddEventAsync(mobEvent);

            EventViewModel = updatedEvent.ToEventViewModel(userManager.CurrentUser.Id);
            Events.Clear();
            Events.Add(EventViewModel);

            // Assign the litter report to the event if it was passed in
            if (initialLitterReport != null && initialLitterReport != Guid.Empty)
            {
                await eventLitterReportManager.AddLitterReportAsync(new EventLitterReport
                {
                    EventId = EventViewModel.Id,
                    LitterReportId = initialLitterReport.Value,
                });
            }

            await notificationService.Notify("Event has been saved.");

            return true;
        }, "An error has occurred while saving the event. Please wait and try again in a moment.");

        return result;
    }

    private async Task LoadPartners()
    {
        ArePartnersAvailable = false;
        AreNoPartnersAvailable = true;

        if (EventViewModel == null || EventViewModel.Id == Guid.Empty)
        {
            return;
        }

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

        var filter = new LitterReportFilter()
        {
            City = EventViewModel.Address.City,
            Country = EventViewModel.Address.Country,
            LitterReportStatusId = NewLitterReportStatus,
        };

        RawLitterReports = await litterReportManager.GetLitterReportsAsync(filter, ImageSizeEnum.Thumb);
        var assignedLitterReports = await eventLitterReportManager.GetEventLitterReportsAsync(EventViewModel.Id, ImageSizeEnum.Thumb, true);

        UpdateLitterReportViewModels(assignedLitterReports);

        AreLitterReportsAvailable = EventLitterReports.Any();
        AreNoLitterReportsAvailable = !AreLitterReportsAvailable;
    }

    public async Task ChangeLocation(Microsoft.Maui.Devices.Sensors.Location location)
    {
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

        ValidateCurrentStep(null, null);
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

    private async Task<bool> Validate()
    {
        if (EventViewModel.EventVisibilityId == (int)EventVisibilityEnum.Public && EventViewModel.EventDate < DateTimeOffset.Now)
        {
            await NotificationService.NotifyError("Event Dates for new public events must be in the future.");
            return false;
        }

        if (EventViewModel.EventVisibilityId == (int)EventVisibilityEnum.TeamOnly && EventViewModel.TeamId == null)
        {
            await NotificationService.NotifyError("A team must be selected for Team Only events.");
            return false;
        }

        return true;
    }

    private readonly string[] stepTitles =
    [
        "Set Event Details", "Set Event Location", "Review Event", "Event Partners", "Litter Reports"
    ];

    #region UI Related properties

    [ObservableProperty] private string stepTitle = string.Empty;

    [ObservableProperty] private string nextStepText = "Next";

    //Event current step control

    [ObservableProperty] private Color stepOneColor = null!;

    [ObservableProperty] private Color stepTwoColor = null!;

    [ObservableProperty] private Color stepThreeColor = null!;

    [ObservableProperty] private Color stepFourColor = null!;

    [ObservableProperty] private Color stepFiveColor = null!;

    [ObservableProperty] private Color stepSixColor = null!;

    //Validation Errors

    [ObservableProperty] private string eventDurationError = string.Empty;

    [ObservableProperty] private string descriptionRequiredError = string.Empty;

    #endregion

    private static Color GetThemeColor(string lightKey, string darkKey)
    {
        var key = Application.Current?.RequestedTheme == AppTheme.Dark ? darkKey : lightKey;
        if (Application.Current?.Resources.TryGetValue(key, out var colorObj) == true && colorObj is Color color)
            return color;
        return Colors.Gray;
    }
}