namespace TrashMob.Shared.Persistence
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface ICommunityRequestRepository
    {
        Task<CommunityRequest> AddCommunityRequest(CommunityRequest communityRequest);
    }
}
