namespace TrashMobMobile.Pages;

public partial class SearchEventsPage : ContentPage
{
    private readonly SearchEventsViewModel _viewModel;

    public SearchEventsPage(SearchEventsViewModel viewModel)
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