namespace TrashMobMobileApp.Pages;

public partial class ViewLitterReportPage : ContentPage
{
    private readonly LitterReportViewModel _viewModel;

    public ViewLitterReportPage(LitterReportViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}