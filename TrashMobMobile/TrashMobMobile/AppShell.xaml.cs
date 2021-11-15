namespace TrashMobMobile
{
    using TrashMobMobile.Views;
    using Xamarin.Forms;
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
            Routing.RegisterRoute(nameof(ContactUsPage), typeof(ContactUsPage));
            Routing.RegisterRoute(nameof(EventDetailPage), typeof(EventDetailPage));
            Routing.RegisterRoute(nameof(EventsMapPage), typeof(EventsMapPage));
            Routing.RegisterRoute(nameof(LogoutPage), typeof(LogoutPage));
            Routing.RegisterRoute(nameof(ManageEventPage), typeof(ManageEventPage));
            Routing.RegisterRoute(nameof(MobEventsPage), typeof(MobEventsPage));
            Routing.RegisterRoute(nameof(TermsAndConditionsPage), typeof(TermsAndConditionsPage));
            Routing.RegisterRoute(nameof(UserProfilePage), typeof(UserProfilePage));
        }
    }
}
