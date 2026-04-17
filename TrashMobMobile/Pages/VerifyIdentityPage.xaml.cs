namespace TrashMobMobile.Pages
{
    using TrashMobMobile.ViewModels;

    public partial class VerifyIdentityPage : ContentPage
    {
        private readonly VerifyIdentityViewModel viewModel;

        public VerifyIdentityPage(VerifyIdentityViewModel viewModel)
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
}
