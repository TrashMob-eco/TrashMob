namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ViewLitterReportViewModel : BaseViewModel
{
    public ViewLitterReportViewModel(ILitterReportRestService litterReportRestService)
    {
        this.litterReportRestService = litterReportRestService;
    }

    [ObservableProperty]
    public LitterReportViewModel litterReportViewModel;
    private readonly ILitterReportRestService litterReportRestService;

    public async Task Init(Guid litterReportId)
    {
        IsBusy = true;

        var litterReport = await litterReportRestService.GetLitterReportAsync(litterReportId);

        LitterReportViewModel = litterReport.ToLitterReportViewModel();

        IsBusy = false;
    }
}
