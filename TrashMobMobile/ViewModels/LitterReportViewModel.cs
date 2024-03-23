namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

public partial class LitterReportViewModel : ObservableObject
{
    public LitterReportViewModel()
    {
        Description = string.Empty;
        Name = string.Empty;
    }

    [ObservableProperty]
    Guid id;

    [ObservableProperty]
    string name;

    [ObservableProperty]
    string description;

    [ObservableProperty]
    int litterReportStatusId;

    public ObservableCollection<LitterImageViewModel> LitterImages { get; set; } = [];
}
