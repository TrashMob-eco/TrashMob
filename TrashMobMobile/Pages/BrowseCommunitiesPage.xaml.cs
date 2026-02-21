namespace TrashMobMobile.Pages;

public partial class BrowseCommunitiesPage : ContentPage
{
    private readonly BrowseCommunitiesViewModel viewModel;

    public BrowseCommunitiesPage(BrowseCommunitiesViewModel viewModel)
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
