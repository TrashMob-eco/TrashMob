namespace TrashMobMobileApp.Features.Home.Pages
{
    using CommunityToolkit.Maui.Views;
    using Microsoft.AspNetCore.Components;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Features.Map;

    public partial class Home
    {
        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Inject]
        public IUserManager UserManager { get; set; }

        protected override void OnInitialized() => TitleContainer.Title = "Home";

        public async void OnUserLocationPreference_Click()
        {
            await App.Current.MainPage.ShowPopupAsync(new UserLocationPreferencePopup(MapRestService, UserManager));
            StateHasChanged();
        }
    }
}
