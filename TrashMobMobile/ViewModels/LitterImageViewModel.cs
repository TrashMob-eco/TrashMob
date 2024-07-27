namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using TrashMobMobile.Services;

public partial class LitterImageViewModel : BaseViewModel
{
    [ObservableProperty]
    private AddressViewModel address;

    [ObservableProperty]
    private string azureBlobUrl;

    [ObservableProperty]
    private Guid createdByUserId;

    [ObservableProperty]
    private DateTimeOffset? createdDate;

    [ObservableProperty]
    private string filePath;

    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private bool isCancelled;

    [ObservableProperty]
    private Guid lastUpdatedByUserId;

    [ObservableProperty]
    private DateTimeOffset? lastUpdatedDate;

    [ObservableProperty]
    private Guid litterReportId;

    public LitterImageViewModel(INotificationService notificationService) : base(notificationService)
    {
        AzureBlobUrl = string.Empty;
        Address = new AddressViewModel();
    }
}