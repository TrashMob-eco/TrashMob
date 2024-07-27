namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class ManageEventPartnersViewModel(IMobEventManager mobEventManager,
    IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService) : BaseViewModel
{
    private readonly IEventPartnerLocationServiceRestService eventPartnerLocationServiceRestService = eventPartnerLocationServiceRestService;
    private readonly IMobEventManager mobEventManager = mobEventManager;

    [ObservableProperty]
    private EventViewModel eventViewModel;

    private EventPartnerLocationViewModel selectedEventPartnerLocation;

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

    private Event MobEvent { get; set; }

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

            EventViewModel = MobEvent.ToEventViewModel();

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotifyError("An error has occured while loading the event partners. Please wait and try again in a moment.");
        }
    }
}