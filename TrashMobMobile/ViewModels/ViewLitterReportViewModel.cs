namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ViewLitterReportViewModel : BaseViewModel
{
    public ViewLitterReportViewModel(ILitterReportManager litterReportManager)
    {
        this.litterReportManager = litterReportManager;
    }

    [ObservableProperty]
    public LitterReportViewModel? litterReportViewModel;

    [ObservableProperty]
    string litterReportStatus;

    [ObservableProperty]
    double overlayOpacity;

    public ObservableCollection<LitterImageViewModel> LitterImageViewModels { get; init; } = [];

    public LitterImageViewModel? SelectedLitterImageViewModel { get; set; }

    private readonly ILitterReportManager litterReportManager;

    public async Task Init(Guid litterReportId)
    {
        IsBusy = true;
        OverlayOpacity = 0.25; // Workaround for: https://github.com/dotnet/maui/issues/18234

        var litterReport = await litterReportManager.GetLitterReportAsync(litterReportId);

        LitterReportViewModel = litterReport.ToLitterReportViewModel();
        LitterReportStatus = LitterReportExtensions.GetLitterStatusFromId(LitterReportViewModel?.LitterReportStatusId);

        LitterImageViewModels.Clear();
        foreach (var litterImage in litterReport.LitterImages)
        {
            var litterImageViewModel = litterImage.ToLitterImageViewModel();

            if (litterImageViewModel != null)
            {
                LitterImageViewModels.Add(litterImageViewModel);
            }
        }

        IsBusy = false;
    }
}
