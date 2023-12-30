namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class EventPartnerLocationServiceViewModel : ObservableObject
{
    public EventPartnerLocationServiceViewModel()
    {
    }

    [ObservableProperty]
    string partnerLocationName;

    [ObservableProperty]
    string partnerLocationNotes;

    [ObservableProperty]
    string serviceName;

    [ObservableProperty]
    string serviceStatus;
}
