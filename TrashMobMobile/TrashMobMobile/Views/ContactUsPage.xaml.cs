namespace TrashMobMobile.Views
{
    using TrashMobMobile.Models;
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class ContactUsPage : ContentPage
    {
        public Item Item { get; set; }

        public ContactUsPage()
        {
            InitializeComponent();
            BindingContext = new ContactUsViewModel();
        }
    }
}