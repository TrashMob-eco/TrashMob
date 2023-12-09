namespace TrashMobMobile.Pages;

public partial class EditEventPage : ContentPage
{
    private readonly EditEventViewModel _viewModel;

    public EditEventPage(EditEventViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }
}