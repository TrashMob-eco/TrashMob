namespace TrashMobMobile.Pages;

public partial class MyDashboardPage : ContentPage
{
    private readonly MyDashboardViewModel _viewModel;

    public MyDashboardPage(MyDashboardViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}