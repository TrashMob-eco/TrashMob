namespace TrashMobMobile.Views
{
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class ItemsPage : ContentPage
    {
        private readonly ItemsViewModel _viewModel;

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = App.GetViewModel<ItemsViewModel>();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}