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
    string createdDate;

    private int litterReportStatusId;
    
    public int LitterReportStatusId
    {
        get
        {
            return litterReportStatusId;
        }
        set
        {
            litterReportStatusId = value;
            OnPropertyChanged(nameof(LitterReportStatusId));
            switch (litterReportStatusId)
            {
                case 1:
                    LitterReportStatus = "New";
                    break;
                case 2:
                    LitterReportStatus = "Assigned";
                    break;
                case 3:
                    LitterReportStatus = "Cleaned";
                    break;
                case 4:
                    LitterReportStatus = "Cancelled";
                    break;
                default:
                    break;
            }
        }
    }

    [ObservableProperty]
    string litterReportStatus;

    public ObservableCollection<LitterImageViewModel> LitterImageViewModels { get; set; } = [];
}
