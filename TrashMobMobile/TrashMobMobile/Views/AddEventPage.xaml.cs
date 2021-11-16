namespace TrashMobMobile.Views
{
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class AddEventPage : ContentPage
    {
        public AddEventPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<AddEventViewModel>();
        }
    }
}