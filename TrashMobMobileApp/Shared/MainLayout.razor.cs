namespace TrashMobMobileApp.Shared
{
    using Microsoft.AspNetCore.Components;
    using MudBlazor;
    using TrashMobMobileApp.Authentication;
    using TrashMobMobileApp.Data;

    public partial class MainLayout
    {
        private bool _drawerOpen;
        private MudTheme _theme;
        private string _pageTitle;

        [Inject]
        public IB2CAuthenticationService AuthenticationService { get; set; }

        [Inject]
        public IUserManager UserManager { get; set; }

        protected override async Task OnInitializedAsync()
        {
            TitleContainer.OnTitleChange += (title) => SetPageTitle(title);
            await base.OnInitializedAsync();
            await PerformAuthenticationAsync();
            SetTheme();
            TitleContainer.Title = "Home";
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private async Task PerformAuthenticationAsync()
        {
            //await AuthenticationService.SignOutAsync(); //sign out on initialize to kick off login
            await AuthenticationService.SignInAsync(UserManager);
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
    }
}
