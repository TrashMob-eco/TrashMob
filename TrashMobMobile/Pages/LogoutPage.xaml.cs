namespace TrashMobMobile.Pages;

public partial class LogoutPage : ContentPage
{
    private readonly LogoutViewModel _viewModel;

    public LogoutPage(LogoutViewModel viewModel)
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