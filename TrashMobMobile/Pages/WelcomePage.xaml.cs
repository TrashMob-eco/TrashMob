namespace TrashMobMobile.Pages;

public partial class WelcomePage : ContentPage
{
    private readonly WelcomeViewModel viewModel;

    public WelcomePage(WelcomeViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.Init();
    }
}