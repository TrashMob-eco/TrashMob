namespace TrashMobMobile.Views
{
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class EventSummaryPage : ContentPage
    {
        public EventSummaryPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<EventSummaryViewModel>();
        }
    }
}