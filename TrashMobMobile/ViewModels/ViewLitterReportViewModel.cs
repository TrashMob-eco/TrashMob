namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ViewLitterReportViewModel : BaseViewModel
{
    private const int NewLitterReportStatus = 1;
    private const int AssignedLitterReportStatus = 2;
    private const int ClosedLitterReportStatus = 3;

    public ViewLitterReportViewModel(ILitterReportManager litterReportManager)
    {
        this.litterReportManager = litterReportManager;
    }

    private LitterReport LitterReport { get; set; }

    [ObservableProperty]
    public LitterReportViewModel? litterReportViewModel;

    [ObservableProperty]
    string litterReportStatus;

    public ObservableCollection<LitterImageViewModel> LitterImageViewModels { get; init; } = [];

    public LitterImageViewModel? SelectedLitterImageViewModel { get; set; }

    private readonly ILitterReportManager litterReportManager;

    public async Task Init(Guid litterReportId)
    {
        IsBusy = true;
        
        LitterReport = await litterReportManager.GetLitterReportAsync(litterReportId);

        LitterReportViewModel = LitterReport.ToLitterReportViewModel();
        LitterReportStatus = LitterReportExtensions.GetLitterStatusFromId(LitterReportViewModel?.LitterReportStatusId);

        if (LitterReport.CreatedByUserId == App.CurrentUser.Id && LitterReport.LitterReportStatusId == NewLitterReportStatus)
        {
            CanDeleteLitterReport = true;
        }
        else
        {
            CanDeleteLitterReport = false;
        }

        if (LitterReport.CreatedByUserId == App.CurrentUser.Id && (LitterReport.LitterReportStatusId == NewLitterReportStatus || LitterReport.LitterReportStatusId == AssignedLitterReportStatus))
        {
            CanCloseLitterReport = true;
        }
        else
        {
            CanCloseLitterReport = false;
        }

        LitterImageViewModels.Clear();
        foreach (var litterImage in LitterReport.LitterImages)
        {
            var litterImageViewModel = litterImage.ToLitterImageViewModel();

            if (litterImageViewModel != null)
            {
                LitterImageViewModels.Add(litterImageViewModel);
            }
        }

        IsBusy = false;
    }

    [ObservableProperty]
    private bool canDeleteLitterReport;

    [ObservableProperty]
    private bool canCloseLitterReport;

    [RelayCommand]
    private async Task DeleteLitterReport()
    {
        await litterReportManager.DeleteLitterReportAsync(LitterReport.Id);
        await Navigation.PopAsync();
    }

    [RelayCommand]
    private async Task CloseLitterReport()
    {
        LitterReport.LitterReportStatusId = ClosedLitterReportStatus;
        await litterReportManager.UpdateLitterReportAsync(LitterReport);
        await Navigation.PopAsync();
    }
}
