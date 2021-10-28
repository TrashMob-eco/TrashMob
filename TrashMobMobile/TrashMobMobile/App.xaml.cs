namespace TrashMobMobile
{
    using TrashMobMobile.Services;
    using TrashMobMobile.Views;
    using TrashMobMobile.Features.LogOn;
    using Xamarin.Forms;

    public partial class App : Application
    {
        public static string ApiEndpoint = "https://www.trashmob.eco/api/";

        public App()
        {
            InitializeComponent();

            DependencyService.Register<MockDataStore>();
            DependencyService.Register<B2CAuthenticationService>();
            DependencyService.Register<ContactRequestManager>();
            DependencyService.Register<MobEventManager>();

            MainPage = new NavigationPage(new WelcomePage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
