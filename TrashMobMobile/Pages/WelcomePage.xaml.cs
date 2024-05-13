namespace TrashMobMobile.Pages;

public partial class WelcomePage : ContentPage
{
    private readonly WelcomeViewModel _viewModel;

    public WelcomePage(WelcomeViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Navigation = Navigation;
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _viewModel.Init();
    }
}