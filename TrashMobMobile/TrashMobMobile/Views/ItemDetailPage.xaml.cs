namespace TrashMobMobile.Views
{
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<ItemDetailViewModel>();
        }
    }
}