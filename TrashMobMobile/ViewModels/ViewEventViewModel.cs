namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ViewEventViewModel : BaseViewModel
{   
    public ViewEventViewModel(IMobEventManager mobEventManager)
    {
        RefreshCommand = new Command(async () => await Refresh());
        this.mobEventManager = mobEventManager;
    }

    private readonly IMobEventManager mobEventManager;

    private Guid eventId;

    [ObservableProperty]
    EventViewModel eventViewModel;

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

    public ICommand RefreshCommand { get; set; }

    private async Task Refresh()
    {
        var mobEvent = await mobEventManager.GetEventAsync(eventId);

        EventViewModel = mobEvent.ToEventViewModel();
        Events.Clear();
        Events.Add(EventViewModel);
    }
}
