namespace TrashMobMobile.Pages;

[QueryProperty(nameof(TeamId), nameof(TeamId))]
public partial class ViewTeamPage : ContentPage
{
    private readonly ViewTeamViewModel viewModel;

    public ViewTeamPage(ViewTeamViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = this.viewModel;
    }

    public string TeamId { get; set; } = string.Empty;

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (!Guid.TryParse(TeamId, out var teamId) || teamId == Guid.Empty)
        {
            await Shell.Current.GoToAsync("..");
            return;
        }

        await viewModel.Init(teamId);
    }
}
