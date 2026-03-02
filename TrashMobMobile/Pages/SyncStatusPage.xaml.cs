namespace TrashMobMobile.Pages;

using TrashMobMobile.ViewModels;

public partial class SyncStatusPage : ContentPage
{
    private readonly SyncStatusViewModel viewModel;

    public SyncStatusPage(SyncStatusViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init();
    }
}
