namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IPartnerAdminInvitationManager : IKeyedManager<PartnerAdminInvitation>
    {
        Task<Partner> GetPartnerForInvitation(Guid partnerAdminInvitationId, CancellationToken cancellationToken);
        
        Task<PartnerAdminInvitation> ResendPartnerAdminInvitation(Guid partnerAdminInvitationId, Guid UserId, CancellationToken cancellationToken);
    }
}
