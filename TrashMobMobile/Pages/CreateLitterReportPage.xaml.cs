namespace TrashMobMobile.Pages;

public partial class CreateLitterReportPage : ContentPage
{
    private readonly CreateLitterReportViewModel viewModel;

    public CreateLitterReportPage(CreateLitterReportViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }
}
