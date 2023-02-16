namespace TrashMobMobileApp.Shared
{
    using CommunityToolkit.Maui.Views;
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Routing;
    using Microsoft.Extensions.Logging;
    using MudBlazor;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Data;
    using TrashMobMobileApp.Features.Map;
    using TrashMobMobileApp.StateContainers;

    public partial class MainLayout
    {
        private bool _drawerOpen;
        private MudTheme _theme;
        private string _pageTitle;
        private string _userInitials;

        private List<string> _menuList { get; } = new List<string>
        {
            Routes.Home,
            Routes.Events,
            Routes.ContactUs,
        };

        [Inject]
        public IB2CAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public IUserManager UserManager { get; set; }

        [Inject]
        public IMapRestService MapRestService { get; set; }

        [Inject]
        public ILogger<NavigationManager> NavigationLogger { get; set; }

        [Inject]
        public UserStateInformation UserContainer { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            UserContainer.OnSignOut += async () => await OnSignOutAsync();
            Navigator.LocationChanged += Navigator_LocationChanged;
            TitleContainer.OnTitleChange += (title) => SetPageTitle(title);
            await PerformAuthenticationAsync();
            SetTheme();
        }

        public async void OnUserLocationPreference_Click()
        {
            await App.Current.MainPage.ShowPopupAsync(new UserLocationPreferencePopup(UserManager, MapRestService));
            StateHasChanged();
        }

        private void Navigator_LocationChanged(object sender, LocationChangedEventArgs e)
        {
            //TODO: handle location change logging/interception functionality
            NavigationLogger.LogTrace($"Navigating to {e.Location}");
        }

        private void DrawerToggle() => _drawerOpen = !_drawerOpen;

        private async Task PerformAuthenticationAsync()
        {
            try
            {
                await AuthenticationService.SignInAsync(UserManager);
                _userInitials = App.CurrentUser.UserName
                    .ToUpperInvariant().First().ToString();
                Navigator.NavigateTo(Routes.Home);
            }
            catch
            {
                await OnSignOutAsync();
            }
        }

        private void SetTheme()
        {
            _theme = new MudTheme()
            {
                Palette = new Palette()
                {
                    Primary = Colors.Green.Default,
                    Secondary = Colors.Green.Lighten1,
                    Dark = Colors.Green.Darken1,
                    AppbarBackground = Colors.Green.Default,
                },
                PaletteDark = new Palette()
                {
                    Primary = Colors.Green.Darken1
                }
            };
        }

        private void SetPageTitle(string title)
        {
            _pageTitle = title;
            StateHasChanged();
        }

        private void DetermineBarIcon()
        {

        }

        private async Task OnSignOutAsync()
        {
            await AuthenticationService.SignOutAsync();
            await PerformAuthenticationAsync();
        }
    }
}
