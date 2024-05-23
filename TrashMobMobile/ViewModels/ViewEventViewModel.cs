namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ViewEventViewModel : BaseViewModel
{
    private readonly IEventAttendeeRestService eventAttendeeRestService;
    private readonly IEventTypeRestService eventTypeRestService;
    private readonly IMobEventManager mobEventManager;
    private readonly IWaiverManager waiverManager;

    [ObservableProperty]
    private string attendeeCount;

    [ObservableProperty]
    private bool enableEditEvent;

    [ObservableProperty]
    private bool enableRegister;

    [ObservableProperty]
    private bool enableUnregister;

    [ObservableProperty]
    private bool enableViewEventSummary;

    [ObservableProperty]
    private EventViewModel eventViewModel;

    private Event mobEvent;

    [ObservableProperty]
    private string selectedEventType;

    [ObservableProperty]
    private string displayDuration;

    [ObservableProperty]
    private string spotsLeft;

    [ObservableProperty]
    private string whatToExpect;

    public ViewEventViewModel(IMobEventManager mobEventManager,
        IEventTypeRestService eventTypeRestService,
        IWaiverManager waiverManager,
        IEventAttendeeRestService eventAttendeeRestService)
    {
        this.mobEventManager = mobEventManager;
        this.eventTypeRestService = eventTypeRestService;
        this.waiverManager = waiverManager;
        this.eventAttendeeRestService = eventAttendeeRestService;
    }

    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        mobEvent = await mobEventManager.GetEventAsync(eventId);

        EventViewModel = mobEvent.ToEventViewModel();

        var eventTypes = (await eventTypeRestService.GetEventTypesAsync()).ToList();
        SelectedEventType = eventTypes.First(et => et.Id == mobEvent.EventTypeId).Name;
        DisplayDuration = mobEvent.GetFormattedDuration();

        Events.Clear();
        Events.Add(EventViewModel);

        EnableEditEvent = mobEvent.IsEventLead();
        EnableViewEventSummary = mobEvent.IsCompleted();

        WhatToExpect =
            "What to Expect: \n\tCleanup supplies provided\n\tMeet fellow community members\n\tContribute to a cleaner environment.";

        await SetRegistrationOptions();
        await GetAttendeeCount();

        IsBusy = false;
    }

    private async Task GetAttendeeCount()
    {
        var attendees = await eventAttendeeRestService.GetEventAttendeesAsync(mobEvent.Id);

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

    [RelayCommand]
    private async Task ViewEventSummary()
    {
        await Shell.Current.GoToAsync($"{nameof(ViewEventSummaryPage)}?EventId={EventViewModel.Id}");
    }

    [RelayCommand]
    private async Task Register()
    {
        IsBusy = true;

        if (!await waiverManager.HasUserSignedTrashMobWaiverAsync())
        {
            await Shell.Current.GoToAsync($"{nameof(WaiverPage)}");
        }

        var eventAttendee = new EventAttendee
        {
            EventId = EventViewModel.Id,
            UserId = App.CurrentUser.Id
        };

        await mobEventManager.AddEventAttendeeAsync(eventAttendee);

        await SetRegistrationOptions();
        await GetAttendeeCount();

        IsBusy = false;

        await Notify("You have been registered for this event.");
    }

    [RelayCommand]
    private async Task Unregister()
    {
        IsBusy = true;

        var eventAttendee = new EventAttendee
        {
            EventId = EventViewModel.Id,
            UserId = App.CurrentUser.Id
        };

        await mobEventManager.RemoveEventAttendeeAsync(eventAttendee);

        await SetRegistrationOptions();
        await GetAttendeeCount();

        IsBusy = false;

        await Notify("You have been unregistered for this event.");
    }

    private async Task SetRegistrationOptions()
    {
        var isAttending = await mobEventManager.IsUserAttendingAsync(mobEvent.Id, App.CurrentUser.Id);

        EnableRegister = !mobEvent.IsEventLead() && !isAttending && mobEvent.AreNewRegistrationsAllowed();
        EnableUnregister = !mobEvent.IsEventLead() && isAttending && mobEvent.AreUnregistrationsAllowed();
    }
}