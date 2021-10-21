using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TrashMobMobile.ViewModels;

namespace TrashMobMobile.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MobEventsPage : ContentPage
    {
        MobEventsViewModel _viewModel;

        public MobEventsPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new MobEventsViewModel();
        }
    }
}