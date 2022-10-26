namespace TrashMob.Shared.Managers.Partners
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerManager : KeyedManager<Partner>, IKeyedManager<Partner>
    {
        private readonly IBaseManager<PartnerUser> partnerUserManager;

        public PartnerManager(IKeyedRepository<Partner> partnerRepository,
                              IBaseManager<PartnerUser> partnerUserManager) : base(partnerRepository)
        {
            this.partnerUserManager = partnerUserManager;
        }

        public async Task CreatePartner(PartnerRequest partnerRequest, CancellationToken cancellationToken = default)
        {
            // Convert the partner request to a new partner
            var partner = partnerRequest.ToPartner();

            // Add the partner record
            var newPartner = await base.AddAsync(partner, partnerRequest.CreatedByUserId, cancellationToken).ConfigureAwait(false);

            // Make the creator of the partner request a registered user for the partner
            var partnerUser = new PartnerUser
            {
                PartnerId = newPartner.Id,
                UserId = partnerRequest.CreatedByUserId,
                CreatedByUserId = partnerRequest.CreatedByUserId,
                LastUpdatedByUserId = partnerRequest.LastUpdatedByUserId,
            };

            await partnerUserManager.AddAsync(partnerUser, partnerRequest.CreatedByUserId, cancellationToken).ConfigureAwait(false);

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
