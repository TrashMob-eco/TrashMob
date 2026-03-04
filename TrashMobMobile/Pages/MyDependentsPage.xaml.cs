namespace TrashMobMobile.Pages;

using TrashMobMobile.ViewModels;

public partial class MyDependentsPage : ContentPage
{
    private readonly MyDependentsViewModel viewModel;

    public MyDependentsPage(MyDependentsViewModel viewModel)
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
