namespace TrashMobMobile.Pages;

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;

[QueryProperty(nameof(EventId), nameof(EventId))]
public partial class ViewEventPage : ContentPage
{
    private readonly ViewEventViewModel _viewModel;

    private string eventId;

    public string EventId
    {
        get
        {
            return eventId;
        }

        set
        {
            eventId = value;
            _viewModel.EventId = value;
        }
    }
    
    public ViewEventPage(ViewEventViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        _viewModel.Notify = Notify;

        if (!string.IsNullOrEmpty(EventId))
        {
            _viewModel.EventId = EventId;
        }

        BindingContext = _viewModel;
    }

    private async Task Notify(string message)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();

        ToastDuration duration = ToastDuration.Short;
        double fontSize = 14;

        var toast = Toast.Make(message, duration, fontSize);
        await toast.Show(cancellationTokenSource.Token);
    }
}