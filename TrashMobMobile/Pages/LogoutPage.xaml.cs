namespace TrashMobMobile.Pages;

public partial class LogoutPage : ContentPage
{
    private readonly LogoutViewModel viewModel;

    public LogoutPage(LogoutViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await viewModel.Init();
    }
}