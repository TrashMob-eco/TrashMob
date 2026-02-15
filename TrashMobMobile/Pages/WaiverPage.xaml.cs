namespace TrashMobMobile.Pages;

[QueryProperty(nameof(WaiverVersionId), nameof(WaiverVersionId))]
public partial class WaiverPage : ContentPage
{
    private readonly WaiverViewModel viewModel;

    public WaiverPage(WaiverViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = this.viewModel;
    }

    public string WaiverVersionId { get; set; } = string.Empty;

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (Guid.TryParse(WaiverVersionId, out var versionId) && versionId != Guid.Empty)
        {
            await viewModel.Init(versionId);
        }
    }
}
