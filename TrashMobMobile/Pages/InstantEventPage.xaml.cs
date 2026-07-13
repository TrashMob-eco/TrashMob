namespace TrashMobMobile.Pages;

using TrashMobMobile.ViewModels;

public partial class InstantEventPage : ContentPage
{
    private readonly InstantEventViewModel viewModel;

    public InstantEventPage(InstantEventViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init();
    }
}
