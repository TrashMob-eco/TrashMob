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

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (MobEventsViewModel)BindingContext;
            if (vm.Events.Count == 0)
                await vm.RefreshCommand.ExecuteAsync();
        }
    }
}