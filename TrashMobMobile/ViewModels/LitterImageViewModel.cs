namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using TrashMobMobile.Services;

public partial class LitterImageViewModel : BaseViewModel
{
    [ObservableProperty]
    private AddressViewModel address;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayImageSource))]
    private string azureBlobUrl;

    [ObservableProperty]
    private Guid createdByUserId;

    [ObservableProperty]
    private DateTimeOffset? createdDate;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(DisplayImageSource))]
    private string filePath;

    /// <summary>
    /// Returns the local file path if available (new photo), otherwise the server URL.
    /// </summary>
    public string DisplayImageSource => !string.IsNullOrEmpty(FilePath) ? FilePath : AzureBlobUrl;

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
        FilePath = string.Empty;
        AzureBlobUrl = string.Empty;
        Address = new AddressViewModel();
    }
}