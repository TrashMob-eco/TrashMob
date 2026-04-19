namespace TrashMobMobile.Pages;

using TrashMobMobile.ViewModels;

[QueryProperty(nameof(DependentId), nameof(DependentId))]
[QueryProperty(nameof(DependentName), nameof(DependentName))]
[QueryProperty(nameof(EventName), nameof(EventName))]
[QueryProperty(nameof(WaiverVersionIds), nameof(WaiverVersionIds))]
public partial class DependentWaiverPage : ContentPage
{
    private readonly DependentWaiverViewModel viewModel;

    public DependentWaiverPage(DependentWaiverViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    public string DependentId { get; set; } = string.Empty;

    public string DependentName { get; set; } = string.Empty;

    public string EventName { get; set; } = string.Empty;

    public string WaiverVersionIds { get; set; } = string.Empty;

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(
            new Guid(DependentId),
            Uri.UnescapeDataString(DependentName),
            Uri.UnescapeDataString(EventName),
            WaiverVersionIds);
    }
}
