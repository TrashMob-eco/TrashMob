namespace TrashMobMobile.Pages;

public partial class AchievementsPage : ContentPage
{
    private readonly AchievementsViewModel viewModel;

    public AchievementsPage(AchievementsViewModel viewModel)
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
