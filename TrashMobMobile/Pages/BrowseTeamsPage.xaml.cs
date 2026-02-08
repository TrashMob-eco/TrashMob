namespace TrashMobMobile.Pages;

public partial class BrowseTeamsPage : ContentPage
{
    private readonly BrowseTeamsViewModel viewModel;

    public BrowseTeamsPage(BrowseTeamsViewModel viewModel)
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
