namespace TrashMobMobileApp.Shared
{
    using Microsoft.AspNetCore.Components;
    using Microsoft.AspNetCore.Components.Routing;
    using Microsoft.Extensions.Logging;
    using MudBlazor;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Data;

    public partial class MainLayout
    {
        private bool _drawerOpen;
        private MudTheme _theme;
        private string _pageTitle;
        private string _userInitials;

        [Inject]
        public IB2CAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public IUserManager UserManager { get; set; }

        [Inject]
        ILogger<NavigationManager> NavigationLogger { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Navigator.LocationChanged += Navigator_LocationChanged;
            TitleContainer.OnTitleChange += (title) => SetPageTitle(title);
            await base.OnInitializedAsync();
            await PerformAuthenticationAsync();
            SetTheme();
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

        private async Task OnSignOutAsync()
        {
            await AuthenticationService.SignOutAsync();
            await PerformAuthenticationAsync();
        }
    }
}
