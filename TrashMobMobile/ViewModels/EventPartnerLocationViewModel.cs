namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class EventPartnerLocationViewModel : ObservableObject
{
    [ObservableProperty]
    private Guid partnerId;

    [ObservableProperty]
    private Guid partnerLocationId;

    [ObservableProperty]
    private string partnerLocationName;

    [ObservableProperty]
    private string partnerLocationNotes;

    [ObservableProperty]
    private string partnerServicesEngaged;
}