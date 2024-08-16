namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Services;

public partial class EventLitterReportViewModel : LitterReportViewModel
{
    public EventLitterReportViewModel(IEventLitterReportRestService eventLitterReportRestService, Guid eventId)
    {
        Description = string.Empty;
        Name = string.Empty;
        this.eventLitterReportRestService = eventLitterReportRestService;
        this.eventId = eventId;
    }

    [ObservableProperty]
    private bool canAddToEvent;

    [ObservableProperty]
    private bool canRemoveFromEvent;

    private readonly IEventLitterReportRestService eventLitterReportRestService;
    private readonly Guid eventId;

    [RelayCommand]
    private async Task AddToEvent()
    {
        await eventLitterReportRestService.AddLitterReportAsync(new EventLitterReport
        {
            EventId = eventId,
            LitterReportId = Id,
        });

        CanAddToEvent = false;
        CanRemoveFromEvent = true;
    }

    [RelayCommand]
    private async Task RemoveFromEvent()
    {
        await eventLitterReportRestService.RemoveLitterReportAsync(new EventLitterReport
        {
            EventId = eventId,
            LitterReportId = Id,
        });

        CanAddToEvent = true;
        CanRemoveFromEvent = false;
    }
}