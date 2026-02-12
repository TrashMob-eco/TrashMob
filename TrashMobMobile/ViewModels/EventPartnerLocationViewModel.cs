namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class EventPartnerLocationViewModel : ObservableObject
{
    [ObservableProperty]
    private Guid partnerId;

    [ObservableProperty]
    private Guid partnerLocationId;

    [ObservableProperty]
    private string partnerLocationName = string.Empty;

    [ObservableProperty]
    private string partnerLocationNotes = string.Empty;

    [ObservableProperty]
    private string partnerServicesEngaged = string.Empty;
}