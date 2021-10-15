namespace TrashMobMobile.Views
{
    using System.ComponentModel;
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}