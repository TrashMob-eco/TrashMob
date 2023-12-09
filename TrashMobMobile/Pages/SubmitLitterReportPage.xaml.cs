namespace TrashMobMobile.Pages;

public partial class SubmitLitterReportPage : ContentPage
{
    private readonly SubmitLitterReportViewModel _viewModel;

    public SubmitLitterReportPage(SubmitLitterReportViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}