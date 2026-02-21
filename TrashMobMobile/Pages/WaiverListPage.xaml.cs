namespace TrashMobMobile.Pages;

using TrashMobMobile.ViewModels;

public partial class WaiverListPage : ContentPage
{
    private readonly WaiverListViewModel viewModel;

    public WaiverListPage(WaiverListViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        BindingContext = this.viewModel;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await viewModel.Init();
    }

    private void OnCancelClicked(object? sender, EventArgs e)
    {
        Shell.Current.SendBackButtonPressed();
    }
}
