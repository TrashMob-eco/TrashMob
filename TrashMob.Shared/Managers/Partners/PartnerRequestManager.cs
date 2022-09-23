namespace TrashMob.Shared.Managers.Partners
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerRequestManager : KeyedManager<PartnerRequest>, IKeyedManager<PartnerRequest>
    {
        private readonly IEmailManager emailManager;
        private readonly IKeyedRepository<Partner> partnerRepository;
        private readonly IBaseRepository<PartnerUser> partnerUserRepository;

        public PartnerRequestManager(IKeyedRepository<PartnerRequest> partnerRequestRepository,
                                     IKeyedRepository<Partner> partnerRepository,
                                     IBaseRepository<PartnerUser> partnerUserRepository,
                                     IEmailManager emailManager) : base(partnerRequestRepository)
        {
            this.partnerRepository = partnerRepository;
            this.partnerUserRepository = partnerUserRepository;
            this.emailManager = emailManager;
        }

        public async Task CreatePartner(PartnerRequest partnerRequest)
        {
            // Convert the partner request to a new partner
            var partner = partnerRequest.ToPartner();

            // Add the partner record
            var newPartner = await partnerRepository.Add(partner).ConfigureAwait(false);

            // Make the creator of the partner request a registered user for the partner
            var partnerUser = new PartnerUser
            {
                PartnerId = newPartner.Id,
                UserId = partnerRequest.CreatedByUserId,
                CreatedByUserId = partnerRequest.CreatedByUserId,
                LastUpdatedByUserId = partnerRequest.LastUpdatedByUserId,
            };

            await partnerUserRepository.Add(partnerUser).ConfigureAwait(false);

            // Notify user when their request has been approved and what to do next
            // Need a template for this
            //var email = new Email
            //{
            //    Message = $"From Email: {partnerRequest.PrimaryEmail}\nFrom Name:{partnerRequest.Name}\nMessage:\n{partnerRequest.Notes}",
            //    Subject = "Partner Request"
            //};
            //email.Addresses.Add(new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress });

            //await emailManager.SendSystemEmail(email, CancellationToken.None).ConfigureAwait(false);
        }
    }
}
