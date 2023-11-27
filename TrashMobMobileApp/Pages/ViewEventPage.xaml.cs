namespace TrashMobMobileApp.Pages;

public partial class ViewEventPage : ContentPage
{
    private readonly ViewEventViewModel _viewModel;

    public ViewEventPage(ViewEventViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}