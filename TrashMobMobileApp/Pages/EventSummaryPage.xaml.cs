namespace TrashMobMobileApp.Pages;

public partial class EventSummaryPage : ContentPage
{
    private readonly EventSummaryViewModel _viewModel;

    public EventSummaryPage(EventSummaryViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}