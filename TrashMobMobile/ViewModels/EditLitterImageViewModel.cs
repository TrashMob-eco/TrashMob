namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class EditLitterImageViewModel : BaseViewModel
{
    [ObservableProperty]
    LitterImageViewModel litterImageViewModel;

    [ObservableProperty]
    LitterReportViewModel litterReportViewModel;

    private LitterImage litterImage;

    public EditLitterImageViewModel(ILitterReportRestService litterReportRestService, ILitterImageRestService litterImageRestService)
    {
        SaveLitterImageCommand = new Command(async () => await SaveLitterImage());
        this.litterReportRestService = litterReportRestService;
        this.litterImageRestService = litterImageRestService;
    }

    public async Task Init(Guid litterReportId, Guid litterImageId)
    {
        IsBusy = true;

        var litterReport = await litterReportRestService.GetLitterReportAsync(litterReportId);

        LitterReportViewModel = litterReport.ToLitterReportViewModel();

        litterImage = await litterImageRestService.GetLitterImageAsync(litterImageId);

        LitterImageViewModel = new LitterImageViewModel(litterReportRestService, litterImageRestService)
        {
        };

        await LitterImageViewModel.Init(litterReportId);

        IsBusy = false;
    }

    public ICommand SaveLitterImageCommand { get; set; }

    private readonly ILitterReportRestService litterReportRestService;
    private readonly ILitterImageRestService litterImageRestService;

    private async Task SaveLitterImage()
    {
        IsBusy = true;

        var updatedLitterImage = await litterImageRestService.UpdateLitterImageAsync(litterImage);

        IsBusy = false;

        await Notify("Litter Image has been saved.");
        await Navigation.PopAsync();
    }
}
