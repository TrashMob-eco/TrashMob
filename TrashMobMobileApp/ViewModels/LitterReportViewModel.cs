namespace TrashMobMobileApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

public partial class LitterReportViewModel : ObservableObject
{
    public LitterReportViewModel()
    {
    }

    [ObservableProperty]
    Guid id;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string description;

    [ObservableProperty]
    DateTimeOffset reportDate;

    public ObservableCollection<AddressViewModel> Addresses { get; set; } = new ObservableCollection<AddressViewModel>();
}
