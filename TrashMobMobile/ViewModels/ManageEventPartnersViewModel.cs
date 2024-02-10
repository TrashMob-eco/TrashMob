namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ManageEventPartnersViewModel :  BaseViewModel
{
    private readonly IMobEventManager mobEventManager;
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService;

    public ManageEventPartnersViewModel(IMobEventManager mobEventManager, IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService)
    {
        this.mobEventManager = mobEventManager;
        this.eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
    }

    [ObservableProperty]
    EventViewModel eventViewModel;

    public ObservableCollection<EventPartnerLocationViewModel> AvailablePartners { get; set; } = new ObservableCollection<EventPartnerLocationViewModel>();

    private EventPartnerLocationViewModel selectedEventPartnerLocation;

    public EventPartnerLocationViewModel SelectedEventPartnerLocation
    {
        get { return selectedEventPartnerLocation; }
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
        await Shell.Current.GoToAsync($"{nameof(EditEventPartnerLocationServicesPage)}?EventId={MobEvent.Id}&PartnerLocationId={eventPartnerLocationViewModel.PartnerLocationId}");
    }

    private Event MobEvent { get; set; }

    public async Task Init(Guid eventId)
    {
        IsBusy = true;

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

        EventViewModel = MobEvent.ToEventViewModel();

        IsBusy = false;
    }
}
