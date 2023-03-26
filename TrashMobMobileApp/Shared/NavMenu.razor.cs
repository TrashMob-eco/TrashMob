namespace TrashMobMobileApp.Shared
{
    using CommunityToolkit.Maui.Views;
    using Microsoft.AspNetCore.Components;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Features.Map;

    public partial class NavMenu
    {
        [Inject]
        public IUserManager UserManager { get; set; }

        [Inject]
        public IMapRestService MapRestService { get; set; }

        public async void OnUserLocationPreference_Click()
        {
            await App.Current.MainPage.ShowPopupAsync(new UserLocationPreferencePopup(UserManager, MapRestService));
        }
    }
}