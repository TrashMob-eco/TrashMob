﻿namespace TrashMobMobile.ViewModels;

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
    INotificationService notificationService) : BaseViewModel(notificationService)
{
    private readonly IEventAttendeeRestService eventAttendeeRestService = eventAttendeeRestService;
    private readonly IEventTypeRestService eventTypeRestService = eventTypeRestService;
    private readonly IMobEventManager mobEventManager = mobEventManager;
    private readonly IWaiverManager waiverManager = waiverManager;

    [ObservableProperty]
    private string attendeeCount;

    [ObservableProperty]
    private string displayDuration;

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
    private EventViewModel eventViewModel;

    private Event mobEvent;

    [ObservableProperty]
    private string selectedEventType;

    [ObservableProperty]
    private string spotsLeft;

    [ObservableProperty]
    private string whatToExpect;

    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        try
        {
            mobEvent = await mobEventManager.GetEventAsync(eventId);

            EventViewModel = mobEvent.ToEventViewModel();

            var eventTypes = (await eventTypeRestService.GetEventTypesAsync()).ToList();
            SelectedEventType = eventTypes.First(et => et.Id == mobEvent.EventTypeId).Name;
            DisplayDuration = mobEvent.GetFormattedDuration();

            Events.Clear();
            Events.Add(EventViewModel);

            EnableEditEvent = mobEvent.IsEventLead();
            EnableViewEventSummary = mobEvent.IsCompleted();

            EnableStartTrackEventRoute = mobEvent.IsEventLead();
            EnableStopTrackEventRoute = false;

            WhatToExpect =
                "What to Expect: \n\tCleanup supplies provided\n\tMeet fellow community members\n\tContribute to a cleaner environment.";

            await SetRegistrationOptions();
            await GetAttendeeCount();

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while loading the event. Please try again.");
        }
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
                UserId = App.CurrentUser.Id,
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
                UserId = App.CurrentUser.Id,
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
                UserId = App.CurrentUser.Id,
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
        var isAttending = await mobEventManager.IsUserAttendingAsync(mobEvent.Id, App.CurrentUser.Id);

        EnableRegister = !mobEvent.IsEventLead() && !isAttending && mobEvent.AreNewRegistrationsAllowed();
        EnableUnregister = !mobEvent.IsEventLead() && isAttending && mobEvent.AreUnregistrationsAllowed();
    }
}