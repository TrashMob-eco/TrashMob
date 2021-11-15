namespace TrashMobMobile.Views
{
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogoutPage : ContentPage
    {
        public LogoutPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await Xamarin.Essentials.SecureStorage.SetAsync("isLogged", "0");
            await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
        }
    }
}