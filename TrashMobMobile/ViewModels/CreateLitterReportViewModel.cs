namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class CreateLitterReportViewModel : BaseViewModel
{
    [ObservableProperty]
    LitterReportViewModel litterReportViewModel;

    private readonly ILitterReportRestService litterReportRestService;
    private const int NewLitterReportStatus = 1;

    public string DefaultLitterReportName { get; } = "New Litter Report";

    public CreateLitterReportViewModel(ILitterReportRestService litterReportRestService)
    {
        SaveLitterReportCommand = new Command(async () => await SaveLitterReport());
        this.litterReportRestService = litterReportRestService;

        LitterReportViewModel = new LitterReportViewModel
        {
            Name = DefaultLitterReportName,
            LitterReportStatusId = NewLitterReportStatus
        };
    }

    public ICommand SaveLitterReportCommand { get; set; }

    private async Task SaveLitterReport()
    {
        IsBusy = true;

        if (!await Validate())
        {
            IsBusy = false;
            return;
        }

        var litterReport = LitterReportViewModel.ToLitterReport();

        var updatedLitterReport = await litterReportRestService.AddLitterReportAsync(litterReport);

        IsBusy = false;

        await Notify("Litter Report has been saved.");
    }

    private async Task<bool> Validate()
    {
        return true;
    }
}
