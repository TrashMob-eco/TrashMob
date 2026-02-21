namespace TrashMobMobile.Pages;

[QueryProperty(nameof(Slug), nameof(Slug))]
public partial class ViewCommunityPage : ContentPage
{
    private readonly ViewCommunityViewModel viewModel;

    public ViewCommunityPage(ViewCommunityViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = this.viewModel;
    }

    public string Slug { get; set; } = string.Empty;

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(Slug);
    }
}
