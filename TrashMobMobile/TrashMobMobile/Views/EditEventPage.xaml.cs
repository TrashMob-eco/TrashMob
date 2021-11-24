namespace TrashMobMobile.Views
{
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class EditEventPage : ContentPage
    {
        public EditEventPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<EditEventViewModel>();
        }
    }
}