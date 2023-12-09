namespace TrashMobMobile.Pages;

public partial class SearchEventsPage : ContentPage
{
    private readonly SearchEventsViewModel _viewModel;

    public SearchEventsPage(SearchEventsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}