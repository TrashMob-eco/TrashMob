namespace TrashMobMobile.Views
{
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using TrashMobMobile.ViewModels;
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MobEventsPage : ContentPage
    {
        public MobEventsPage()
        {
            InitializeComponent();
            BindingContext = App.GetViewModel<MobEventsViewModel>();
        }
    }
}