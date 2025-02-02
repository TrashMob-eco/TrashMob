namespace TrashMobMobile.Pages;

public partial class MyDashboardPage : ContentPage
{
    private readonly MyDashboardViewModel viewModel;

    public MyDashboardPage(MyDashboardViewModel viewModel)
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
        Switcher.SelectedIndex = 0;
    }
}