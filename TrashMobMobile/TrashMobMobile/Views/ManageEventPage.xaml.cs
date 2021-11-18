namespace TrashMobMobile.Views
{
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class ManageEventPage : ContentPage
    {
        public ManageEventPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<EventDetailViewModel>();
        }
    }
}