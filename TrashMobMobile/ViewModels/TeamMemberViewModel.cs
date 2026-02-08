namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class TeamMemberViewModel : ObservableObject
{
    [ObservableProperty]
    private Guid userId;

    [ObservableProperty]
    private string userName = string.Empty;

    [ObservableProperty]
    private bool isTeamLead;

    [ObservableProperty]
    private string role = string.Empty;

    [ObservableProperty]
    private string joinedDate = string.Empty;
}
