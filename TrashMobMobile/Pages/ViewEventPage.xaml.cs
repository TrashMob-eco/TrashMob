namespace TrashMobMobile.Pages;

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

        if (!string.IsNullOrEmpty(EventId))
        {
            _viewModel.EventId = EventId;
        }

        BindingContext = _viewModel;
    }
}