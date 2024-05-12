namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;

public partial class SearchEventsPage : ContentPage
{
    private readonly SearchEventsViewModel _viewModel;

    public SearchEventsPage(SearchEventsViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Navigation = Navigation;
        _viewModel.Notify = Notify;
        BindingContext = _viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.Init();

        if (_viewModel?.UserLocation?.Location != null)
        {
            var mapSpan = new MapSpan(new Location(_viewModel.UserLocation.Location.Latitude, _viewModel.UserLocation.Location.Longitude), 0.05, 0.05);
            eventsMap.MoveToRegion(mapSpan);
        }
    }

    private async Task Notify(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(message, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);
    }

    private async void OnEventStatusRadioButtonCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked)
        {
            _viewModel.EventStatus = (string)radioButton.Content;
            await _viewModel.Init();
        }
    }

    private async void Pin_InfoWindowClicked(object sender, PinClickedEventArgs e)
    {
        Pin p = (Pin)sender;

        var eventId = p.AutomationId;
        await Shell.Current.GoToAsync($"{nameof(ViewEventPage)}?EventId={eventId}");
    }
}