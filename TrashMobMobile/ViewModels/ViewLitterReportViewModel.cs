namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMob.Models;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class ViewLitterReportViewModel(ILitterReportManager litterReportManager, INotificationService notificationService) : BaseViewModel(notificationService)
{
    private const int NewLitterReportStatus = 1;
    private const int AssignedLitterReportStatus = 2;
    private const int CleanedLitterReportStatus = 3;

    private readonly ILitterReportManager litterReportManager = litterReportManager;

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

    private LitterReport LitterReport { get; set; }

    public ObservableCollection<LitterImageViewModel> LitterImageViewModels { get; init; } = [];

    public LitterImageViewModel? SelectedLitterImageViewModel { get; set; }

    public async Task Init(Guid litterReportId)
    {
        IsBusy = true;

        try
        {
            LitterReport = await litterReportManager.GetLitterReportAsync(litterReportId, ImageSizeEnum.Reduced);

            LitterReportViewModel = LitterReport.ToLitterReportViewModel(NotificationService);
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
                var litterImageViewModel = litterImage.ToLitterImageViewModel(NotificationService);

                if (litterImageViewModel != null)
                {
                    litterImageViewModel.Address.DisplayName = LitterReport.Name;
                    litterImageViewModel.Address.ParentId = LitterReport.Id;
                    LitterImageViewModels.Add(litterImageViewModel);
                }
            }

            IsBusy = false;
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while loading this litter report. Please try again.");
        }
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
        IsBusy = true;

        try
        {
            LitterReport.LitterReportStatusId = CleanedLitterReportStatus;
            var tempLitterReport = LitterReport;
            tempLitterReport.LitterImages.Clear();
            await litterReportManager.UpdateLitterReportAsync(tempLitterReport);
            IsBusy = false;
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            SentrySdk.CaptureException(ex);
            IsBusy = false;
            await NotificationService.NotifyError("An error occurred while updating the status of this litter report. Please try again.");
        }
    }
}