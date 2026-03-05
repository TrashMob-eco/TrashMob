namespace TrashMobMobile.Pages;

using TrashMobMobile.Views.ViewEvent;

[QueryProperty(nameof(EventId), nameof(EventId))]
public partial class ViewEventPage : ContentPage
{
    private readonly ViewEventViewModel viewModel;
    private string? loadedEventId;

    public ViewEventPage(ViewEventViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;

        BindingContext = this.viewModel;
    }

    public string EventId { get; set; } = string.Empty;

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // Re-subscribe to tab changes (safe pattern: unsubscribe first to avoid duplicates)
        Switcher.PropertyChanged -= OnSwitcherPropertyChanged;
        Switcher.PropertyChanged += OnSwitcherPropertyChanged;

        // Only re-initialize when loading a different event, not when returning from popups
        if (loadedEventId != EventId)
        {
            loadedEventId = EventId;
            await viewModel.Init(new Guid(EventId), RenderRoutesOnDetailsMap);
            Switcher.SelectedIndex = 0;
        }
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        Switcher.PropertyChanged -= OnSwitcherPropertyChanged;
    }

    private async void OnSwitcherPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Switcher.SelectedIndex))
        {
            await viewModel.OnTabSelected(Switcher.SelectedIndex);
        }
    }

    private void RenderRoutesOnDetailsMap()
    {
        if (tabDetails.Content is TabDetails details)
        {
            details.RenderRoutes(viewModel.EventAttendeeRoutes);
        }
    }
}