namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class LitterReportViewModel : ObservableObject
{
    [ObservableProperty]
    private string createdDate = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private string litterReportStatus = string.Empty;

    private int litterReportStatusId;

    [ObservableProperty]
    private string name = string.Empty;

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
                    LitterReportStatus = "Assigned to an Event";
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

    public string? ThumbnailUrl => LitterImageViewModels.FirstOrDefault()?.AzureBlobUrl;

    public bool HasThumbnail => !string.IsNullOrEmpty(ThumbnailUrl);

    public string? FirstImageLocation => LitterImageViewModels.FirstOrDefault()?.Address?.DisplayAddress;

    public double? FirstImageLatitude => LitterImageViewModels.FirstOrDefault()?.Address?.Latitude;

    public double? FirstImageLongitude => LitterImageViewModels.FirstOrDefault()?.Address?.Longitude;
}