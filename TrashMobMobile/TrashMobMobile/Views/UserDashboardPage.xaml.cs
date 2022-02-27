namespace TrashMobMobile.Views
{
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using TrashMobMobile.ViewModels;
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class UserDashboardPage : ContentPage
    {
        public UserDashboardPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<UserDashboardViewModel>();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (UserDashboardViewModel)BindingContext;
            if (vm.Events.Count == 0)
                await vm.RefreshCommand.ExecuteAsync();
        }
    }
}