
namespace TrashMob.Shared.Managers
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    public class PartnerManager : IPartnerManager
    {
        private readonly IEmailManager emailManager;
        private readonly IPartnerRepository partnerRepository;
        private readonly IPartnerUserRepository partnerUserRepository;

        public PartnerManager(IEmailManager emailManager, IPartnerRepository partnerRepository, IPartnerUserRepository partnerUserRepository)
        {
            this.emailManager = emailManager;
            this.partnerRepository = partnerRepository;
            this.partnerUserRepository = partnerUserRepository;
        }

        public async Task CreatePartner(PartnerRequest partnerRequest)
        {
            // Convert the partner request to a new partner
            var partner = partnerRequest.ToPartner();

            // Add the partner record
            var newPartner = await partnerRepository.AddPartner(partner).ConfigureAwait(false);

            // Make the creator of the partner request a registered user for the partner
            var partnerUser = new PartnerUser
            {
                PartnerId = newPartner.Id,
                UserId = partnerRequest.CreatedByUserId,
                CreatedByUserId = partnerRequest.CreatedByUserId,
                LastUpdatedByUserId = partnerRequest.LastUpdatedByUserId,
            };

            await partnerUserRepository.AddPartnerUser(partnerUser).ConfigureAwait(false);
            
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
