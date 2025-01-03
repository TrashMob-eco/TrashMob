namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class EventLitterReportViewModel : LitterReportViewModel
{
    public EventLitterReportViewModel(IEventLitterReportManager eventLitterReportManager, Guid eventId)
    {
        Description = string.Empty;
        Name = string.Empty;
        Status = "Available";
        CanAddToEvent = true;
        this.eventLitterReportManager = eventLitterReportManager;
        this.eventId = eventId;
    }

    [ObservableProperty]
    private bool canAddToEvent;

    [ObservableProperty]
    private bool canRemoveFromEvent;

    [ObservableProperty]
    private string status;

    private readonly IEventLitterReportManager eventLitterReportManager;
    private readonly Guid eventId;

    [RelayCommand]
    private async Task AddToEvent()
    {
        await AddLitterReportToEvent();
    }

    public async Task AddLitterReportToEvent()
    {
        await eventLitterReportManager.AddLitterReportAsync(new EventLitterReport
        {
            EventId = eventId,
            LitterReportId = Id,
        });

        CanAddToEvent = false;
        CanRemoveFromEvent = true;
        Status = "Assigned to this event";
    }

    [RelayCommand]
    private async Task RemoveFromEvent()
    {
        await RemoveLitterReportFromEvent();
    }

    public async Task RemoveLitterReportFromEvent()
    {
        await eventLitterReportManager.RemoveLitterReportAsync(new EventLitterReport
        {
            EventId = eventId,
            LitterReportId = Id,
        });

        CanAddToEvent = true;
        CanRemoveFromEvent = false;
        Status = "Open";
    }
}