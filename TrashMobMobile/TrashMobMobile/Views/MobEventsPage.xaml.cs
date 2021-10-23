namespace TrashMobMobile.Views
{
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    using TrashMobMobile.ViewModels;
    
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MobEventsPage : ContentPage
    {
        private readonly MobEventsViewModel _viewModel;

        public MobEventsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MobEventsViewModel();
        }
    }
}