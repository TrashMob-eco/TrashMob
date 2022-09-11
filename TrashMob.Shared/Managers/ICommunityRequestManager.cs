namespace TrashMob.Shared.Managers
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface ICommunityRequestManager
    {
        Task AddCommunityRequest(CommunityRequest communityRequest);
    }
}
