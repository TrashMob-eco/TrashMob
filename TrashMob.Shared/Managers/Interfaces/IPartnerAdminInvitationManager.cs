namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface IPartnerAdminInvitationManager : IKeyedManager<PartnerAdminInvitation>
    {
        Task<Partner> GetPartnerForInvitation(Guid partnerAdminInvitationId, CancellationToken cancellationToken);

        Task<IEnumerable<DisplayPartnerAdminInvitation>> GetInvitationsForUser(Guid userId, CancellationToken cancellationToken);

        Task AcceptInvitation(Guid partnerAdminInvitationId, Guid UserId, CancellationToken cancellationToken);

        Task DeclineInvitation(Guid partnerAdminInvitationId, Guid UserId, CancellationToken cancellationToken);

        Task<PartnerAdminInvitation> ResendPartnerAdminInvitation(Guid partnerAdminInvitationId, Guid UserId, CancellationToken cancellationToken);
    }
}
