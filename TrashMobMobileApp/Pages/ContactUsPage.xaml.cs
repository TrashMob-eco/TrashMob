namespace TrashMobMobileApp.Pages;

public partial class ContactUsPage : ContentPage
{
    private readonly ContactUsViewModel _viewModel;

    public ContactUsPage(ContactUsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;

    }
}