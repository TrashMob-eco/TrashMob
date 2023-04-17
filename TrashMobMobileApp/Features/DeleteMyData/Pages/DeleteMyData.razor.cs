namespace TrashMobMobileApp.Features.DeleteMyData.Pages
{
    using TrashMobMobileApp.Authentication;

    public partial class DeleteMyData
    {
        protected override void OnInitialized()
        {
            TitleContainer.Title = "Delete My Data";
        }

        private async Task OnDeleteMyDataAsync()
        {
            UserState.IsDeleting = true;
            await Shell.Current.Navigation.PopToRootAsync();
        }
    }
}
