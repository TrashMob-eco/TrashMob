namespace TrashMobMobile.Views
{
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class CancelEventPage : ContentPage
    {
        public CancelEventPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<CancelEventViewModel>();
        }
    }
}