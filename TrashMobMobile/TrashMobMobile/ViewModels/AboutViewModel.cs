namespace TrashMobMobile.ViewModels
{
    using System.Windows.Input;
    using Xamarin.Essentials;
    using Xamarin.Forms;

    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
            Title = "About TrashMob.eco";
            OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://www.trashmob.eco"));
        }

        public ICommand OpenWebCommand { get; }
    }
}