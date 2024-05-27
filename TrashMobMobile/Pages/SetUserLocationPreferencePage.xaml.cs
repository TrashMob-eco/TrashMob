namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class SetUserLocationPreferencePage : ContentPage
{
    private readonly UserLocationPreferenceViewModel viewModel;

    public SetUserLocationPreferencePage(UserLocationPreferenceViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        this.viewModel.Notify = Notify;
        BindingContext = this.viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        viewModel.Init();

        if (viewModel?.Address?.Latitude != null && viewModel?.Address?.Longitude != null)
        {
            var mapSpan =
                new MapSpan(new Location(viewModel.Address.Latitude.Value, viewModel.Address.Longitude.Value), 0.01,
                    0.01);
            userLocationMap.MoveToRegion(mapSpan);
        }
    }

    private async void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        await viewModel.ChangeLocation(e.Location);
    }

    private async Task Notify(string message)
    {
        var cancellationTokenSource = new CancellationTokenSource();

        var duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(message, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);
    }
}