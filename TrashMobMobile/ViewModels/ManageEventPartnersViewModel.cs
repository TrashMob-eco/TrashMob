namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class ManageEventPartnersViewModel(IMobEventManager mobEventManager,
                                                  IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService,
                                                  INotificationService notificationService,
                                                  IUserManager userManager) 
    : BaseViewModel(notificationService)
{
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
    private readonly IUserManager userManager = userManager;
    private readonly IMobEventManager mobEventManager = mobEventManager;

    [ObservableProperty]
    private EventViewModel eventViewModel = new();

    private EventPartnerLocationViewModel selectedEventPartnerLocation = new();

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

    private Event MobEvent { get; set; } = new();

    private async void PerformNavigation(EventPartnerLocationViewModel eventPartnerLocationViewModel)
    {
        await Shell.Current.GoToAsync(
            $"{nameof(EditEventPartnerLocationServicesPage)}?EventId={MobEvent.Id}&PartnerLocationId={eventPartnerLocationViewModel.PartnerLocationId}");
    }

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

        try
        {
            var eventPartnerLocations = await eventPartnerLocationServiceRestService.GetEventPartnerLocationsAsync(eventId);

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

            MobEvent = await mobEventManager.GetEventAsync(eventId);

            EventViewModel = MobEvent.ToEventViewModel(userManager.CurrentUser.Id);

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error has occurred while loading the event partners. Please wait and try again in a moment.");
        }
    }
}