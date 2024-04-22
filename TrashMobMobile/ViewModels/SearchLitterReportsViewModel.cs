namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class SearchLitterReportsViewModel : BaseViewModel
{
    private readonly ILitterReportRestService litterReportRestService;
    private LitterReportViewModel? selectedLitterReport;

    public SearchLitterReportsViewModel(ILitterReportRestService litterReportRestService)
    {
        this.litterReportRestService = litterReportRestService;
    }

    public ObservableCollection<LitterReportViewModel> LitterReports { get; set; } = [];

    public LitterReportViewModel SelectedLitterReport
    {
        get { return selectedLitterReport; }
        set
        {
            if (selectedLitterReport != value)
            {
                selectedLitterReport = value;
                OnPropertyChanged(nameof(selectedLitterReport));

                if (selectedLitterReport != null)
                {
                    PerformNavigation(selectedLitterReport);
                }
            }
        }
    }

    public async Task Init()
    {
        await RefreshLitterReports();
    }

    private async void PerformNavigation(LitterReportViewModel litterReportViewModel)
    {
        await Shell.Current.GoToAsync($"{nameof(ViewLitterReportPage)}?LitterReportId={litterReportViewModel.Id}");
    }

    private async Task RefreshLitterReports()
    {
        IsBusy = true;

        LitterReports.Clear();
        var litterReports = await litterReportRestService.GetAllLitterReportsAsync();

        foreach (var litterReport in litterReports)
        {
            var vm = litterReport.ToLitterReportViewModel();
            LitterReports.Add(vm);
        }

        IsBusy = false;

        await Notify("Litter Report list has been refreshed.");
    }
}
