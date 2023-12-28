namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ViewEventViewModel : BaseViewModel
{   
    public ViewEventViewModel(IMobEventManager mobEventManager)
    {
        RegisterCommand = new Command(async () => await Register());
        UnregisterCommand = new Command(async () => await Unregister());
        this.mobEventManager = mobEventManager;
    }

    private readonly IMobEventManager mobEventManager;

    private Guid eventId;

    [ObservableProperty]
    EventViewModel eventViewModel;

    [ObservableProperty]
    bool enableRegister;

    [ObservableProperty]
    bool enableUnregister;

    public ObservableCollection<EventViewModel> Events { get; set; } = [];

    public string EventId
    {
        get
        {
            return eventId.ToString();
        }

        set
        {
            if (eventId.ToString() != value)
            {
                if (value != null)
                {
                    eventId = new Guid(value);
                }

                OnPropertyChanged(nameof(eventId));
                Refresh();
            }
        }
    }

    public ICommand RegisterCommand { get; set; }
    public ICommand UnregisterCommand { get; set; }

    private async Task Refresh()
    {
        var mobEvent = await mobEventManager.GetEventAsync(eventId);

        EventViewModel = mobEvent.ToEventViewModel();
        Events.Clear();
        Events.Add(EventViewModel);
        var isAttending = await mobEventManager.IsUserAttendingAsync(eventId, App.CurrentUser.Id);

        EnableRegister = !mobEvent.IsEventLead() && !isAttending && mobEvent.AreNewRegistrationsAllowed();
        EnableUnregister = !mobEvent.IsEventLead() && isAttending && mobEvent.AreUnregistrationsAllowed();
    }

    private async Task Register()
    {
        var eventAttendee = new EventAttendee()
        {
            EventId = EventViewModel.Id,
            UserId = App.CurrentUser.Id
        };

        await mobEventManager.AddEventAttendeeAsync(eventAttendee);

        await Notify("You have been registered for this event.");

        await Refresh();
    }

    private async Task Unregister()
    {
        var eventAttendee = new EventAttendee()
        {
            EventId = EventViewModel.Id,
            UserId = App.CurrentUser.Id
        };

        await mobEventManager.RemoveEventAttendeeAsync(eventAttendee);

        await Notify("You have been unregistered for this event.");

        await Refresh();
    }
}
