namespace TrashMobMobile.Pages;

using TrashMobMobile.ViewModels;

[QueryProperty(nameof(TrackRoute), nameof(TrackRoute))]
public partial class InstantEventPage : ContentPage
{
    private readonly InstantEventViewModel viewModel;

    public InstantEventPage(InstantEventViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    /// <summary>
    /// Query parameter from the Dashboard: "true" (or "True") when the user opted in
    /// to route tracking via the toggle. Any other value (missing, "false") means no
    /// route recording. Query params always arrive as strings; parse manually.
    /// </summary>
    public string TrackRoute { get; set; } = string.Empty;

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        viewModel.RequestedTrackRoute = bool.TryParse(TrackRoute, out var t) && t;
        await viewModel.Init();
    }
}
