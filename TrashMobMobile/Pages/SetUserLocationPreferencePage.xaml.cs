namespace TrashMobMobile.Pages;

using Microsoft.Maui.Controls.Maps;

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

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        await _viewModel.ChangeLocation(e.Location);
    }
}