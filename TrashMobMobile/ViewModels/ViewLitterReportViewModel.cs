namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Data;
using TrashMobMobile.Extensions;

public partial class ViewLitterReportViewModel : BaseViewModel
{
    private const int NewLitterReportStatus = 1;
    private const int AssignedLitterReportStatus = 2;
    private const int CleanedLitterReportStatus = 3;

    private readonly ILitterReportManager litterReportManager;

    [ObservableProperty]
    private bool canDeleteLitterReport;

    [ObservableProperty]
    private bool canEditLitterReport;

    [ObservableProperty]
    private bool canMarkLitterReportCleaned;

    [ObservableProperty]
    private string litterReportStatus;

    [ObservableProperty]
    public LitterReportViewModel? litterReportViewModel;

    public ViewLitterReportViewModel(ILitterReportManager litterReportManager)
    {
        this.litterReportManager = litterReportManager;
    }

    private LitterReport LitterReport { get; set; }

    public ObservableCollection<LitterImageViewModel> LitterImageViewModels { get; init; } = [];

    public LitterImageViewModel? SelectedLitterImageViewModel { get; set; }

    public async Task Init(Guid litterReportId)
    {
        IsBusy = true;

        LitterReport = await litterReportManager.GetLitterReportAsync(litterReportId, ImageSizeEnum.Reduced);

        LitterReportViewModel = LitterReport.ToLitterReportViewModel();
        LitterReportStatus = LitterReportExtensions.GetLitterStatusFromId(LitterReportViewModel?.LitterReportStatusId);

        if (LitterReport.CreatedByUserId == App.CurrentUser.Id &&
            LitterReport.LitterReportStatusId == NewLitterReportStatus)
        {
            CanDeleteLitterReport = true;
        }
        else
        {
            CanDeleteLitterReport = false;
        }

        if (LitterReport.CreatedByUserId == App.CurrentUser.Id &&
            (LitterReport.LitterReportStatusId == NewLitterReportStatus ||
             LitterReport.LitterReportStatusId == AssignedLitterReportStatus))
        {
            CanEditLitterReport = true;
        }
        else
        {
            CanEditLitterReport = false;
        }

        if (LitterReport.CreatedByUserId == App.CurrentUser.Id &&
            (LitterReport.LitterReportStatusId == NewLitterReportStatus ||
             LitterReport.LitterReportStatusId == AssignedLitterReportStatus))
        {
            CanMarkLitterReportCleaned = true;
        }
        else
        {
            CanMarkLitterReportCleaned = false;
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

    [RelayCommand]
    private async Task EditLitterReport()
    {
        await Shell.Current.GoToAsync($"{nameof(EditLitterReportPage)}?LitterReportId={LitterReportViewModel.Id}");
    }

    [RelayCommand]
    private async Task DeleteLitterReport()
    {
        await litterReportManager.DeleteLitterReportAsync(LitterReport.Id);
        await Navigation.PopAsync();
    }

    [RelayCommand]
    private async Task MarkLitterReportCleaned()
    {
        LitterReport.LitterReportStatusId = CleanedLitterReportStatus;
        var tempLitterReport = LitterReport;
        tempLitterReport.LitterImages.Clear();
        await litterReportManager.UpdateLitterReportAsync(tempLitterReport);
        await Navigation.PopAsync();
    }
}