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
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages partner requests including creation, approval, denial, and email notifications for partnership applications.
    /// </summary>
    public class PartnerRequestManager(
        IKeyedRepository<PartnerRequest> partnerRequestRepository,
        IKeyedManager<Partner> partnerManager,
        IBaseManager<PartnerAdmin> partnerUserManager,
        IEmailManager emailManager)
        : KeyedManager<PartnerRequest>(partnerRequestRepository), IPartnerRequestManager
    {

        /// <inheritdoc />
        public override async Task<PartnerRequest> AddAsync(PartnerRequest instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            if (instance.isBecomeAPartnerRequest)
            {
                instance.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Pending;
            }
            else
            {
                instance.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Sent;
            }

            var result = await base.AddAsync(instance, userId, cancellationToken);

            // If this is a become a partner request, the mail gets routed to the TrashMobAdmin for approval
            if (instance.isBecomeAPartnerRequest)
            {
                var message = $"From Email: {instance.Email}\nFrom Name:{instance.Name}\nMessage:\n{instance.Notes}";
                var subject = "Partner Request";

                var recipients = new List<EmailAddress>
                {
                    new() { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress },
                };

                var dynamicTemplateData = new
                {
                    username = Constants.TrashMobEmailName,
                    emailCopy = message,
                    subject,
                };

                await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail,
                        SendGridEmailGroupId.General, dynamicTemplateData, recipients, CancellationToken.None);
            }
            else
            {
                // If this is a send a partner request, the mail gets routed to the targeted email address. 
                string welcomeMessage;
                string welcomeSubject;
                if (instance.PartnerTypeId == (int)PartnerTypeEnum.Business)
                {
                    welcomeMessage =
                        emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteBusinessPartner.ToString());
                    welcomeSubject = "Someone in your community wants you to become a TrashMob Partner!";
                }
                else
                {
                    welcomeMessage =
                        emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteGovernmentPartner.ToString());
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
                    new() { Name = instance.Name, Email = instance.Email },
                };

                await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail,
                        SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients,
                        CancellationToken.None);

                // Also gets sent to the TrashMob Admin for informational purposes
                var message =
                    $"Sent to Email: {instance.Email}\nTo Partner Name:{instance.Name}\nCity: {instance.City}\nRegion: {instance.Region}\nMessage:\n{instance.Notes}";
                var subject = "Partner Request Sent";

                var recipients = new List<EmailAddress>
                {
                    new() { Name = Constants.TrashMobEmailName, Email = Constants.TrashMobEmailAddress },
                };

                var dynamicTemplateData = new
                {
                    username = Constants.TrashMobEmailName,
                    emailCopy = message,
                    subject,
                };

                await emailManager.SendTemplatedEmailAsync(subject, SendGridEmailTemplateId.GenericEmail,
                        SendGridEmailGroupId.General, dynamicTemplateData, recipients, CancellationToken.None);
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<PartnerRequest> ApproveBecomeAPartnerAsync(Guid partnerRequestId, Guid userId,
            CancellationToken cancellationToken)
        {
            var partnerRequest = await Repo.GetAsync(partnerRequestId, cancellationToken);
            partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Approved;

            var result = await base.UpdateAsync(partnerRequest, userId, cancellationToken);

            await CreatePartnerAsync(partnerRequest, cancellationToken);

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
                new() { Name = partnerRequest.Name, Email = partnerRequest.Email },
            };

            await emailManager.SendTemplatedEmailAsync(partnerSubject, SendGridEmailTemplateId.GenericEmail,
                    SendGridEmailGroupId.General, dynamicTemplateData, partnerRecipients, CancellationToken.None);

            return result;
        }

        /// <inheritdoc />
        public async Task<PartnerRequest> DenyBecomeAPartnerAsync(Guid partnerRequestId, Guid userId,
            CancellationToken cancellationToken)
        {
            var partnerRequest = await Repo.GetAsync(partnerRequestId, cancellationToken);
            partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Denied;

            var result = await base.UpdateAsync(partnerRequest, userId, cancellationToken);

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
                new() { Name = partnerRequest.Name, Email = partnerRequest.Email },
            };

            await emailManager.SendTemplatedEmailAsync(partnerSubject, SendGridEmailTemplateId.GenericEmail,
                    SendGridEmailGroupId.General, dynamicTemplateData, partnerRecipients, CancellationToken.None);

            return result;
        }

        /// <summary>
        /// Creates a new partner from an approved partner request.
        /// </summary>
        /// <param name="partnerRequest">The partner request to convert to a partner.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        private async Task CreatePartnerAsync(PartnerRequest partnerRequest,
            CancellationToken cancellationToken = default)
        {
            // Convert the partner request to a new partner
            var partner = partnerRequest.ToPartner();

            // Add the partner record
            var newPartner = await partnerManager.AddAsync(partner, partnerRequest.CreatedByUserId, cancellationToken);

            // Make the creator of the partner request a registered user for the partner
            var partnerUser = new PartnerAdmin
            {
                PartnerId = newPartner.Id,
                UserId = partnerRequest.CreatedByUserId,
            };

            await partnerUserManager.AddAsync(partnerUser, partnerRequest.CreatedByUserId, cancellationToken);
        }
    }
}