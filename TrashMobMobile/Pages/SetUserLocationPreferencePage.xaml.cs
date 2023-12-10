namespace TrashMobMobile.Pages;

public partial class SetUserLocationPreferencePage : ContentPage
{
    private readonly UserLocationPreferenceViewModel _viewModel;

    public SetUserLocationPreferencePage(UserLocationPreferenceViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        _viewModel.Init();
        base.OnAppearing();
    }
}