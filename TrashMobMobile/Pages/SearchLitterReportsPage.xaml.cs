namespace TrashMobMobile.Pages;

public partial class SearchLitterReportsPage : ContentPage
{
    private readonly SearchLitterReportsViewModel _viewModel;

    public SearchLitterReportsPage(SearchLitterReportsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}