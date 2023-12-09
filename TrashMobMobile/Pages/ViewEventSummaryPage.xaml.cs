namespace TrashMobMobile.Pages;

public partial class ViewEventSummaryPage : ContentPage
{
    private readonly ViewEventSummaryViewModel _viewModel;

    public ViewEventSummaryPage(ViewEventSummaryViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}