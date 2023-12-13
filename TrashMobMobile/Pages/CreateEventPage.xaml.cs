namespace TrashMobMobile.Pages;

public partial class CreateEventPage : ContentPage
{
    private readonly CreateEventViewModel _viewModel;

    public CreateEventPage(CreateEventViewModel viewModel)
	{
		InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await _viewModel.Init();
    }
}