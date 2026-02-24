namespace TrashMobMobile.Pages;

using TrashMobMobile.Views.ViewEvent;

[QueryProperty(nameof(EventId), nameof(EventId))]
public partial class ViewEventPage : ContentPage
{
    private readonly ViewEventViewModel viewModel;
    private bool isInitialized;

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

        // Only initialize on first navigation, not when returning from popups
        if (!isInitialized)
        {
            isInitialized = true;
            Switcher.PropertyChanged += OnSwitcherPropertyChanged;
            await viewModel.Init(new Guid(EventId), RenderRoutesOnDetailsMap);
            Switcher.SelectedIndex = 0;
        }
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        Switcher.PropertyChanged -= OnSwitcherPropertyChanged;
        // Reset so re-entering this page (for a different event) will re-initialize
        isInitialized = false;
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