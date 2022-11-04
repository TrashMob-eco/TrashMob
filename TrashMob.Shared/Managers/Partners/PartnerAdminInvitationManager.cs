namespace TrashMob.Shared.Managers.Partners
{
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class PartnerAdminInvitationManager : KeyedManager<PartnerAdminInvitation>, IPartnerAdminInvitationManager
    {
        private readonly IBaseManager<PartnerAdmin> partnerAdminManager;
        private readonly IKeyedRepository<User> userRepository;

        public PartnerAdminInvitationManager(IKeyedRepository<PartnerAdminInvitation> partnerAdminInvitationRepository,
                                             IBaseManager<PartnerAdmin> partnerAdminManager,
                                             IKeyedRepository<User> userRepository) 
            : base(partnerAdminInvitationRepository)
        {
            this.partnerAdminManager = partnerAdminManager;
            this.userRepository = userRepository;
        }

        public async Task<Partner> GetPartnerForInvitation(Guid partnerAdminInvitationId, CancellationToken cancellationToken)
        {
            var partnerInvitation = await Repository.Get(p => p.Id == partnerAdminInvitationId, false)
                                        .Include(p => p.Partner)
                                        .FirstOrDefaultAsync(cancellationToken);
            return partnerInvitation.Partner;
        }

        public override async Task<PartnerAdminInvitation> AddAsync(PartnerAdminInvitation instance, Guid userId, CancellationToken cancellationToken = default)
        {
            // Check to see if this user already exists in the system. If so, immediately add them as an admin
            var existingUser = await userRepository.Get(u => u.Email == instance.Email).ToListAsync(cancellationToken);
            var newInvitation = await base.AddAsync(instance, userId, cancellationToken);

            if (!existingUser.Any())
            {
                // Todo - Send Email

            }
            else
            {
                AcceptInvitation(newInvitation.Id, existingUser.)
            }

            return newInvitation;
        }

        public async Task<bool> AcceptInvitation(Guid partnerAdminInviationId, Guid userId, CancellationToken cancellationToken)
        {
            var partnerAdminInvitation = await Repository.Get(pa => pa.Id == partnerAdminInviationId).FirstOrDefaultAsync(cancellationToken);

            if (partnerAdminInvitation == null)
            {
                return false;
            }

            var partnerAdmin = new PartnerAdmin
            {
                PartnerId = partnerAdminInvitation.PartnerId,
                UserId = userId
            };

            await partnerAdminManager.AddAsync(partnerAdmin, cancellationToken);

            return true;
        }
    }
}
