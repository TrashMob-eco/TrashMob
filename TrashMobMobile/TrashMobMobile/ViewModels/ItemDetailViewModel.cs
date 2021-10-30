namespace TrashMobMobile.ViewModels
{
    using System;
    using System.Diagnostics;
    using TrashMobMobile.Models;
    using TrashMobMobile.Services;
    using Xamarin.Forms;

    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class ItemDetailViewModel : BaseViewModel
    {
        private string itemId;
        private string text;
        private string description;
        private readonly IDataStore<Item> dataStore;

        public string Id { get; set; }

        public ItemDetailViewModel(IDataStore<Item> dataStore)
        {
            this.dataStore = dataStore;
        }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            try
            {
                var item = await dataStore.GetItemAsync(itemId);
                Id = item.Id;
                Text = item.Text;
                Description = item.Description;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
