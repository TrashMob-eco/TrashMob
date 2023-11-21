namespace TrashMobMobileApp.Pages;

public partial class CreateEventPage : ContentPage
{
    private readonly CreateEventViewModel _viewModel;

    public CreateEventPage(CreateEventViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}