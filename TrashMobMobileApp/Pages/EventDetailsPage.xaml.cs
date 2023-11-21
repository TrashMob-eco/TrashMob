namespace TrashMobMobileApp.Pages;

public partial class EventDetailsPage : ContentPage
{
    private readonly EventDetailsViewModel _viewModel;

    public EventDetailsPage(EventDetailsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}