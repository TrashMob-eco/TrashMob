namespace TrashMobMobile.Pages;

public partial class HomeFeedPage : ContentPage
{
    private readonly HomeFeedViewModel viewModel;

    public HomeFeedPage(HomeFeedViewModel viewModel)
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
