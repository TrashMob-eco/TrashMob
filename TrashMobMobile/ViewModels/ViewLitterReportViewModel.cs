namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ViewLitterReportViewModel : BaseViewModel
{
    public ViewLitterReportViewModel(ILitterReportRestService litterReportRestService)
    {
        this.litterReportRestService = litterReportRestService;
    }

    [ObservableProperty]
    public LitterReportViewModel? litterReportViewModel;

    [ObservableProperty]
    private string litterReportStatus;

    public ObservableCollection<LitterImageViewModel> LitterImageViewModels { get; init; } = [];

    public LitterImageViewModel? SelectedLitterImageViewModel { get; set; }

    private readonly ILitterReportRestService litterReportRestService;

    public async Task Init(Guid litterReportId)
    {
        IsBusy = true;

        var litterReport = await litterReportRestService.GetLitterReportAsync(litterReportId);

        LitterReportViewModel = litterReport.ToLitterReportViewModel();
        LitterReportStatus = LitterReportExtensions.GetLitterStatusFromId(LitterReportViewModel?.LitterReportStatusId);

        LitterImageViewModels.Clear();
        foreach (var litterImage in litterReport.LitterImages)
        {
            LitterImageViewModels.Add(litterImage.ToLitterImageViewModel());
        }

        IsBusy = false;
    }
}
