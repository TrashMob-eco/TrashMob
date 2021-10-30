namespace TrashMobMobile.Views
{
    using TrashMobMobile.Models;
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;
 
    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<NewItemViewModel>();
        }
    }
}