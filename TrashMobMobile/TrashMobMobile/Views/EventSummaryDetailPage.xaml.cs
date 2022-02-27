namespace TrashMobMobile.Views
{
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;

    public partial class EventSummaryDetailPage : ContentPage
    {
        public EventSummaryDetailPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<EventSummaryViewModel>();
        }
    }
}