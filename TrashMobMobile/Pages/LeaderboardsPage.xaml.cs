namespace TrashMobMobile.Pages;

public partial class LeaderboardsPage : ContentPage
{
    private readonly LeaderboardsViewModel viewModel;

    public LeaderboardsPage(LeaderboardsViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = this.viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init();
    }
}
