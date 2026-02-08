namespace TrashMobMobile.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;

public partial class TeamViewModel : ObservableObject
{
    [ObservableProperty]
    private Guid id;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string location = string.Empty;

    [ObservableProperty]
    private bool isPublic;

    [ObservableProperty]
    private bool requiresApproval;

    [ObservableProperty]
    private int memberCount;

    [ObservableProperty]
    private string memberCountDisplay = string.Empty;

    [ObservableProperty]
    private bool isUserMember;
}
