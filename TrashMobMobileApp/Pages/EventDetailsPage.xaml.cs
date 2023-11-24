namespace TrashMobMobileApp.Pages;

public partial class EventDetailsPage : ContentPage
{
    private readonly ViewEventViewModel _viewModel;

    public EventDetailsPage(ViewEventViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}