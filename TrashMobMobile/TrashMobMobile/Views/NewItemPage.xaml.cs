namespace TrashMobMobile.Views
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using TrashMobMobile.Models;
    using TrashMobMobile.ViewModels;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;

    public partial class NewItemPage : ContentPage
    {
        public Item Item { get; set; }

        public NewItemPage()
        {
            InitializeComponent();
            BindingContext = new NewItemViewModel();
        }
    }
}