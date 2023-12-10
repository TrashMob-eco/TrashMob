namespace TrashMobMobile.Pages;

public partial class MyDashboardPage : ContentPage
{
    private readonly MyDashboardViewModel _viewModel;

    public MyDashboardPage(MyDashboardViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Navigation = Navigation;
        BindingContext = _viewModel;
    }
    
    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.Init();
    }
}