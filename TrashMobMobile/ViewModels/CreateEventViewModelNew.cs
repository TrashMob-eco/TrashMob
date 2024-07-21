﻿using System.ComponentModel;
using System.Windows.Input;
using TrashMobMobile.Pages.CreateEvent;

namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;
using Color = Microsoft.Maui.Graphics.Color;

public partial class CreateEventViewModelNew : BaseViewModel
{
    private const int ActiveEventStatus = 1;
    private readonly IEventTypeRestService eventTypeRestService;
    private readonly IMapRestService mapRestService;

    private readonly IMobEventManager mobEventManager;
    private readonly IWaiverManager waiverManager;

    public ICommand PreviousCommand { get; set; }
    public ICommand NextCommand { get; set; }

    public ICommand CloseCommand { get; set; }

    [ObservableProperty] private Color stepOneColor;

    [ObservableProperty] private Color stepTwoColor;

    [ObservableProperty] private Color stepThreeColor;

    [ObservableProperty] private Color stepFourColor;

    [ObservableProperty] private Color stepFiveColor;

    [ObservableProperty] private Color stepSixColor;

    [ObservableProperty] private EventViewModel eventViewModel;

    [ObservableProperty] private bool isManageEventPartnersEnabled;

    [ObservableProperty] private string selectedEventType;

    [ObservableProperty] private AddressViewModel userLocation;

    [ObservableProperty] private bool isStepValid;

    public CreateEventViewModelNew(IMobEventManager mobEventManager,
        IEventTypeRestService eventTypeRestService,
        IMapRestService mapRestService,
        IWaiverManager waiverManager)
    {
        this.mobEventManager = mobEventManager;
        this.eventTypeRestService = eventTypeRestService;
        this.mapRestService = mapRestService;
        this.waiverManager = waiverManager;

        NextCommand = new Command(async () =>
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await Task.Delay(200);
            SetCurrentStep(StepType.Forward);

            IsBusy = false;
        });

        PreviousCommand = new Command(async
            () =>
        {
            if (IsBusy)
                return;

            IsBusy = true;

            await Task.Delay(200);
            SetCurrentStep(StepType.Backward);

            IsBusy = false;
        });

        CloseCommand = new Command(() => { Shell.Current.GoToAsync(".."); });
    }

    public string DefaultEventName { get; } = "New Event";

    public int CurrentStep { get; set; }

    [ObservableProperty] private IContentView currentView;

    public IContentView[] Steps { get; set; }

    public enum StepType
    {
        Forward,
        Backward
    }

    private void ValidateCurrentStep(object sender, PropertyChangedEventArgs e)
    {
        if (EventViewModel == null)
            return;

        switch (CurrentStep)
        {
            //Step 1 validation 
            case 0:
                if (!string.IsNullOrEmpty(EventViewModel.Name) &&
                    EventViewModel.EventDateOnly != default && 
                   !string.IsNullOrEmpty(SelectedEventType))
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
    }

    private void SetCurrentView()
    {
        CurrentView = Steps[CurrentStep];

        if (CurrentView is BaseStepClass current)
            current.OnNavigated();

        //TODO reference this colors from the app styles
        StepOneColor = CurrentStep == 0 ? Color.Parse("#005C4B") : Color.Parse("#CCDEDA");
        StepTwoColor = CurrentStep == 1 ? Color.Parse("#005C4B") : Color.Parse("#CCDEDA");
        StepThreeColor = CurrentStep == 2 ? Color.Parse("#005C4B") : Color.Parse("#CCDEDA");
        StepFourColor = CurrentStep == 3 ? Color.Parse("#005C4B") : Color.Parse("#CCDEDA");
        StepFiveColor = CurrentStep == 4 ? Color.Parse("#005C4B") : Color.Parse("#CCDEDA");
        StepSixColor = CurrentStep == 5 ? Color.Parse("#005C4B") : Color.Parse("#CCDEDA");
    }

    public void SetCurrentStep(StepType step)
    {
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
            if (CurrentStep < Steps.Length - 1)
            {
                CurrentStep++;
                SetCurrentView();
            }
        }
    }


    // This is only for the map point
    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    private List<EventType> EventTypes { get; set; } = [];

    public ObservableCollection<string> ETypes { get; set; } = [];

    public async Task Init()
    {
        IsBusy = true;

        SetCurrentView();

        foreach (var item in Steps)
        {
            if (item is BaseStepClass step)
                step.ViewModel = this;
        }

        if (!await waiverManager.HasUserSignedTrashMobWaiverAsync())
        {
            await Shell.Current.GoToAsync($"{nameof(WaiverPage)}");
        }

        IsManageEventPartnersEnabled = false;

        UserLocation = App.CurrentUser.GetAddress();
        EventTypes = (await eventTypeRestService.GetEventTypesAsync()).ToList();

        // Set defaults
        EventViewModel = new EventViewModel
        {
            Name = DefaultEventName,
            EventDate = DateTime.Now.AddDays(1),
            IsEventPublic = true,
            MaxNumberOfParticipants = 0,
            DurationHours = 2,
            DurationMinutes = 0,
            Address = UserLocation,
            EventTypeId = EventTypes.OrderBy(e => e.DisplayOrder).First().Id,
            EventStatusId = ActiveEventStatus,
        };
        
        SelectedEventType = EventTypes.OrderBy(e => e.DisplayOrder).First().Name;

        Events.Add(EventViewModel);

        foreach (var eventType in EventTypes)
        {
            ETypes.Add(eventType.Name);
        }

        IsBusy = false;
        
        //We need to subscribe to both eventViewmodel and creatEventViewmodel propertyChanged to validate step
        eventViewModel.PropertyChanged += ValidateCurrentStep;
        PropertyChanged += ValidateCurrentStep;
    }

    [RelayCommand]
    private async Task SaveEvent()
    {
        IsBusy = true;

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

        var mobEvent = EventViewModel.ToEvent();

        var updatedEvent = await mobEventManager.AddEventAsync(mobEvent);

        EventViewModel = updatedEvent.ToEventViewModel();
        Events.Clear();
        Events.Add(EventViewModel);

        IsManageEventPartnersEnabled = true;
        IsBusy = false;

        await Notify("Event has been saved.");
    }

    [RelayCommand]
    private async Task ManageEventPartners()
    {
        await Shell.Current.GoToAsync($"{nameof(ManageEventPartnersPage)}?EventId={EventViewModel.Id}");
    }

    public async Task ChangeLocation(Location location)
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
    }

    private async Task<bool> Validate()
    {
        if (EventViewModel.IsEventPublic && EventViewModel.EventDate < DateTimeOffset.Now)
        {
            await NotifyError("Event Dates for new public events must be in the future.");
            return false;
        }

        return true;
    }
}