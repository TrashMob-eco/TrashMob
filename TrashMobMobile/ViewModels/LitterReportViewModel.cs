namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class LitterReportViewModel : ObservableObject
{
    [ObservableProperty]
    private string createdDate;

    [ObservableProperty]
    private string description;

    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private string litterReportStatus;

    private int litterReportStatusId;

    [ObservableProperty]
    private string name;

    public LitterReportViewModel()
    {
        Description = string.Empty;
        Name = string.Empty;
    }

    public int LitterReportStatusId
    {
        get => litterReportStatusId;
        set
        {
            litterReportStatusId = value;
            OnPropertyChanged();
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
            }
        }
    }

    public ObservableCollection<LitterImageViewModel> LitterImageViewModels { get; set; } = [];
}