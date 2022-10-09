namespace TrashMob.Shared.Managers.Partners
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerRequestManager : KeyedManager<PartnerRequest>, IPartnerRequestManager
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

        public override async Task<PartnerRequest> AddAsync(PartnerRequest instance, CancellationToken cancellationToken = default)
        {
            var result = await Repository.AddAsync(instance, cancellationToken);

            // If this is a become a partner request, the mail gets routed to the TrashMobAdmin for approval
            if (instance.isBecomeAPartnerRequest)
            {
                var message = $"From Email: {instance.Email}\nFrom Name:{instance.Name}\nMessage:\n{instance.Notes}";
                var subject = "Partner Request";

                var recipients = new List<EmailAddress>
                {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
                };

                var dynamicTemplateData = new
                {
                    username = Constants.TrashMobEmailName,
                    emailCopy = message,
                    subject = subject,
                };

                await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                // If this is a send a partner request, the mail gets routed to the targeted email address. 
                string welcomeMessage;
                string welcomeSubject;
                if (instance.PartnerTypeId == (int)PartnerTypeEnum.Business)
                {
                    welcomeMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteBusinessPartner.ToString());
                    welcomeSubject = "Someone in your community wants you to become a TrashMob Partner!";
                }
                else
                {
                    welcomeMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteGovernmentPartner.ToString());
                    welcomeSubject = "Someone in your community wants your community to become a TrashMob Community!";
                }

                var userDynamicTemplateData = new
                {
                    partnerName = instance.Name,
                    emailCopy = welcomeMessage,
                    subject = welcomeSubject,
                };

                var welcomeRecipients = new List<EmailAddress>
                    {
                        new EmailAddress { Name = instance.Name, Email = instance.Email }
                    };

                await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients, CancellationToken.None).ConfigureAwait(false);

                // Also gets sent to the TrashMob Admin for informational purposes
                var message = $"Sent to Email: {instance.Email}\nTo Partner Name:{instance.Name}\nCity: {instance.City}\nRegion: {instance.Region}\nMessage:\n{instance.Notes}";
                var subject = "Partner Request Sent";

                var recipients = new List<EmailAddress>
                {
                new EmailAddress { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress }
                };

                var dynamicTemplateData = new
                {
                    username = Constants.TrashMobEmailName,
                    emailCopy = message,
                    subject = subject,
                };

                await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, dynamicTemplateData, recipients, CancellationToken.None).ConfigureAwait(false);
            }

            return result;
        }

        public async Task<PartnerRequest> ApproveBecomeAPartnerAsync(Guid partnerRequestId, CancellationToken cancellationToken)
        {
            var partnerRequest = await Repo.GetAsync(partnerRequestId, cancellationToken).ConfigureAwait(false);
            partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Approved;

            var result = await Repo.UpdateAsync(partnerRequest, cancellationToken).ConfigureAwait(false);

            await CreatePartner(partnerRequest, cancellationToken).ConfigureAwait(false);

            var partnerMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.PartnerRequestAccepted.ToString());
            partnerMessage = partnerMessage.Replace("{PartnerName}", partnerRequest.Name);
            var partnerSubject = "Your request to become a TrashMob.eco Partner has been accepted!";

            var dynamicTemplateData = new
            {
                username = partnerRequest.Name,
                emailCopy = partnerMessage,
                subject = partnerSubject,
            };

            var partnerRecipients = new List<EmailAddress>
            {
                new EmailAddress { Name = partnerRequest.Name, Email = partnerRequest.Email },
            };

            await emailManager.SendTemplatedEmailAsync(partnerSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, dynamicTemplateData, partnerRecipients, CancellationToken.None).ConfigureAwait(false);

            return result;
        }

        public async Task<PartnerRequest> DenyBecomeAPartnerAsync(Guid partnerRequestId, CancellationToken cancellationToken)
        {
            var partnerRequest = await Repo.GetAsync(partnerRequestId, cancellationToken).ConfigureAwait(false);
            partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Denied;

            var result = await Repo.UpdateAsync(partnerRequest, cancellationToken).ConfigureAwait(false);

            var partnerMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.PartnerRequestDeclined.ToString());
            partnerMessage = partnerMessage.Replace("{PartnerName}", partnerRequest.Name);
            var partnerSubject = "Your request to become a TrashMob.eco Partner has been declined";

            var dynamicTemplateData = new
            {
                username = partnerRequest.Name,
                emailCopy = partnerMessage,
                subject = partnerSubject,
            };

            var partnerRecipients = new List<EmailAddress>
            {
                new EmailAddress { Name = partnerRequest.Name, Email = partnerRequest.Email },
            };

            await emailManager.SendTemplatedEmailAsync(partnerSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, dynamicTemplateData, partnerRecipients, CancellationToken.None).ConfigureAwait(false);

            return result;
        }

        private async Task CreatePartner(PartnerRequest partnerRequest, CancellationToken cancellationToken = default)
        {
            // Convert the partner request to a new partner
            var partner = partnerRequest.ToPartner();

            // Add the partner record
            var newPartner = await partnerRepository.AddAsync(partner, cancellationToken).ConfigureAwait(false);

            // Make the creator of the partner request a registered user for the partner
            var partnerUser = new PartnerUser
            {
                PartnerId = newPartner.Id,
                UserId = partnerRequest.CreatedByUserId,
                CreatedByUserId = partnerRequest.CreatedByUserId,
                LastUpdatedByUserId = partnerRequest.LastUpdatedByUserId,
            };

            await partnerUserRepository.AddAsync(partnerUser, cancellationToken).ConfigureAwait(false);
        }
    }
}
