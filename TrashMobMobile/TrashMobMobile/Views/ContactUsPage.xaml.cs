namespace TrashMobMobile.Views
{
    using TrashMobMobile.Models;
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class ContactUsPage : ContentPage
    {
        public ContactUsPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<ContactUsViewModel>();
        }
    }
}