namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows.Input;
using TrashMob.Models;
using TrashMobMobile.Data;

public partial class LitterImageViewModel : BaseViewModel
{
    public LitterImageViewModel(ILitterReportRestService litterReportRestService, ILitterImageRestService litterImageRestService)
    {
        AzureBlobUrl = string.Empty;
        Address = new AddressViewModel();
        this.litterReportRestService = litterReportRestService;
        this.litterImageRestService = litterImageRestService;
    }

    public LitterImage LitterImage { get; set; }

    private LitterReport litterReport;

    public async Task Init(Guid litterReportId)
    {
        IsBusy = true;
        litterReport = await litterReportRestService.GetLitterReportAsync(litterReportId);

        //CanDeleteLitterImage = litterReport.IsEventLead();
        //CanEditLitterImage = litterReport.IsEventLead();

        IsBusy = false;
    }

    public ICommand DeleteLitterImageCommand { get; set; }
    public ICommand EditLitterImageCommand { get; set; }

    [ObservableProperty]
    Guid litterReportId;

    [ObservableProperty]
    Guid id;

    [ObservableProperty]
    string azureBlobUrl;

    [ObservableProperty]
    AddressViewModel address;

    [ObservableProperty]
    bool isCancelled;
    private readonly ILitterReportRestService litterReportRestService;
    private readonly ILitterImageRestService litterImageRestService;

    private async Task DeleteLitterImage()
    {
        await litterImageRestService.DeleteLitterImageAsync(LitterImage.Id);

        await Notify("Litter Image has been removed.");

        await Navigation.PopAsync();
    }

    private async Task EditLitterImage()
    {
        await Shell.Current.GoToAsync($"{nameof(EditLitterImagePage)}?LitterReportId={litterReport.Id}&LitterImageId={LitterImage.Id}");
    }
}
