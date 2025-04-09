namespace TrashMobMobile.Pages;

[QueryProperty(nameof(EventId), nameof(EventId))]
public partial class EditEventPage : ContentPage
{
    private readonly EditEventViewModel viewModel;

    public EditEventPage(EditEventViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    public string EventId { get; set; }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init(new Guid(EventId));
        Switcher.SelectedIndex = 0;
    }
}