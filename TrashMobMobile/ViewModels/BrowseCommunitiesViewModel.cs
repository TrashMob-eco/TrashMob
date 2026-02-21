namespace TrashMobMobile.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TrashMobMobile.Extensions;
using TrashMobMobile.Services;

public partial class BrowseCommunitiesViewModel(
    ICommunityManager communityManager,
    INotificationService notificationService,
    IUserManager userManager) : BaseViewModel(notificationService)
{
    private readonly ICommunityManager communityManager = communityManager;
    private readonly IUserManager userManager = userManager;

    [ObservableProperty]
    private bool areCommunitiesFound;

    [ObservableProperty]
    private bool areNoCommunitiesFound = true;

    public ObservableCollection<CommunityViewModel> Communities { get; } = [];

    public async Task Init()
    {
        await ExecuteAsync(async () =>
        {
            var user = userManager.CurrentUser;
            var address = user.GetAddress();

            var communities = await communityManager.GetCommunitiesAsync(
                address.Latitude, address.Longitude, 100);

            Communities.Clear();
            foreach (var community in communities)
            {
                Communities.Add(community.ToCommunityViewModel());
            }

            AreCommunitiesFound = Communities.Count > 0;
            AreNoCommunitiesFound = !AreCommunitiesFound;
        }, "Failed to load communities. Please try again.");
    }

    [RelayCommand]
    private async Task Refresh()
    {
        await Init();
    }

    [RelayCommand]
    private async Task ViewCommunity(CommunityViewModel? communityVm)
    {
        if (communityVm == null)
        {
            return;
        }

        await Shell.Current.GoToAsync(
            $"{nameof(ViewCommunityPage)}?Slug={communityVm.Slug}");
    }
}
