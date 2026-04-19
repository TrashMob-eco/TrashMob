namespace TrashMobMobile.Pages;

using TrashMobMobile.ViewModels;

[QueryProperty(nameof(DateOfBirth), nameof(DateOfBirth))]
public partial class ChildSignupPage : ContentPage
{
    private readonly ChildSignupViewModel viewModel;

    public ChildSignupPage(ChildSignupViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    public string DateOfBirth { get; set; } = string.Empty;

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        viewModel.Init(DateOfBirth);
    }
}
