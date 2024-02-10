namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class EventPartnerLocationViewModel : ObservableObject
{
    public EventPartnerLocationViewModel()
    {
    }

    [ObservableProperty]
    Guid partnerId;

    [ObservableProperty]
    Guid partnerLocationId;

    [ObservableProperty]
    string partnerLocationName;

    [ObservableProperty]
    string partnerLocationNotes;

    [ObservableProperty]
    string partnerServicesEngaged;
}
