namespace TrashMobMobile.Pages;

using TrashMobMobile.ViewModels;

[QueryProperty(nameof(DependentId), nameof(DependentId))]
public partial class EditDependentPage : ContentPage
{
    private readonly EditDependentViewModel viewModel;

    public EditDependentPage(EditDependentViewModel viewModel)
    {
        InitializeComponent();
        this.viewModel = viewModel;
        this.viewModel.Navigation = Navigation;
        BindingContext = this.viewModel;
    }

    public string DependentId { get; set; } = string.Empty;

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (Guid.TryParse(DependentId, out var id) && id != Guid.Empty)
        {
            await viewModel.Init(id);
        }
    }
}
