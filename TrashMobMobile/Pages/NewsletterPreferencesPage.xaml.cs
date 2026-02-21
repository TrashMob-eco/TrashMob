namespace TrashMobMobile.Pages;

public partial class NewsletterPreferencesPage : ContentPage
{
    private readonly NewsletterPreferencesViewModel viewModel;

    public NewsletterPreferencesPage(NewsletterPreferencesViewModel viewModel)
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
