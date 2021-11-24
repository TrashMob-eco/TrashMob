namespace TrashMobMobile
{
    using TrashMobMobile.Services;
    using TrashMobMobile.Features.LogOn;
    using Xamarin.Forms;
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using TrashMobMobile.Models;
    using TrashMobMobile.ViewModels;
    using Microsoft.AppCenter;
    using Microsoft.AppCenter.Analytics;
    using Microsoft.AppCenter.Crashes;
    public partial class App : Application
    {
        protected static IServiceProvider ServiceProvider { get; set; }

        public static User CurrentUser { get; set; }

        public App(Action<IServiceCollection> addPlatformServices = null)
        {
            InitializeComponent();

            SetupServices(addPlatformServices);

            MainPage = new AppShell();
        }

        void SetupServices(Action<IServiceCollection> addPlatformServices = null)
        {
            var services = new ServiceCollection();

            // Add platform specific services
            addPlatformServices?.Invoke(services);

            // Add View Models
            services.AddTransient<AboutViewModel>();
            services.AddTransient<AddEventViewModel>();
            services.AddTransient<ContactUsViewModel>();
            services.AddTransient<EventDetailViewModel>();
            services.AddTransient<EventsMapViewModel>();
            services.AddTransient<LoginViewModel>();
            services.AddTransient<MobEventsViewModel>();
            services.AddTransient<TermsAndConditionsViewModel>();
            services.AddTransient<UserProfileViewModel>();

            // Add Services
            _ = services.AddSingleton<IB2CAuthenticationService, B2CAuthenticationService>();
            _ = services.AddSingleton<IContactRequestManager, ContactRequestManager>();
            _ = services.AddSingleton<IContactRequestRestService, ContactRequestRestService>();
            _ = services.AddSingleton<IDataStore<Item>, MockDataStore>();
            _ = services.AddSingleton<IEventAttendeeRestService, EventAttendeeRestService>();
            _ = services.AddSingleton<IEventTypeRestService, EventTypeRestService>();
            _ = services.AddSingleton<IMapRestService, MapRestService>();
            _ = services.AddSingleton<IMobEventManager, MobEventManager>();
            _ = services.AddSingleton<IMobEventRestService, MobEventRestService>();
            _ = services.AddSingleton<IUserManager, UserManager>();
            _ = services.AddSingleton<IUserRestService, UserRestService>();

            ServiceProvider = services.BuildServiceProvider();
        }
        public static BaseViewModel GetViewModel<TViewModel>() where TViewModel : BaseViewModel
                => ServiceProvider.GetService<TViewModel>();

        protected override void OnStart()
        {
            AppCenter.Start("android=5cb8f69e-d5af-4f5c-9b7b-80fd9cc12a7d;" +
                  "ios=9b6b51bd-d13e-47d7-b5f6-bf019340391a;",
                  typeof(Analytics), typeof(Crashes));
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
