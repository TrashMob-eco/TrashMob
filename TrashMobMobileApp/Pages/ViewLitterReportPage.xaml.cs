namespace TrashMobMobileApp.Pages;

public partial class ViewLitterReportPage : ContentPage
{
    private readonly ViewLitterReportViewModel _viewModel;

    public ViewLitterReportPage(ViewLitterReportViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}