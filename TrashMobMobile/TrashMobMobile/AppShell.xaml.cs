namespace TrashMobMobile
{
    using TrashMobMobile.Views;
    using Xamarin.Forms;
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            //Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(AddEventPage), typeof(AddEventPage));
            Routing.RegisterRoute(nameof(EditEventPage), typeof(EditEventPage));
            Routing.RegisterRoute(nameof(EventDetailPage), typeof(EventDetailPage));
            Routing.RegisterRoute(nameof(TermsAndConditionsPage), typeof(TermsAndConditionsPage));
        }
    }
}
