namespace TrashMobMobile.Views
{
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class EventsMapPage : ContentPage
    {
        public EventsMapPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<EventsMapViewModel>();
        }
    }
}