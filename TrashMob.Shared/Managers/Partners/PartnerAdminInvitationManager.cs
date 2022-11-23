namespace TrashMob.Shared.Managers.Partners
{
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Linq;
    using TrashMob.Shared.Poco;
    using TrashMob.Shared.Engine;

    public class PartnerAdminInvitationManager : KeyedManager<PartnerAdminInvitation>, IPartnerAdminInvitationManager
    {
        private readonly IPartnerAdminManager partnerAdminManager;
        private readonly IUserManager userManager;
        private readonly IPartnerManager partnerManager;
        private readonly IEmailManager emailManager;

        public PartnerAdminInvitationManager(IKeyedRepository<PartnerAdminInvitation> partnerAdminInvitationRepository,
                                             IPartnerAdminManager partnerAdminManager,
                                             IUserManager userManager,
                                             IPartnerManager partnerManager,
                                             IEmailManager emailManager)
            : base(partnerAdminInvitationRepository)
        {
            this.partnerAdminManager = partnerAdminManager;
            this.userManager = userManager;
            this.partnerManager = partnerManager;
            this.emailManager = emailManager;
        }

        public override async Task<IEnumerable<PartnerAdminInvitation>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken)).AsEnumerable();
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
            // Check to see if this user already has an invite
            var existingInvitation = await Repository.Get(i => i.Email == instance.Email && i.PartnerId == instance.PartnerId).FirstOrDefaultAsync(cancellationToken);
            var existingUser = await userManager.GetUserByEmailAsync(instance.Email, cancellationToken);
            var partner = await partnerManager.GetAsync(instance.PartnerId, cancellationToken);

            if (existingInvitation != null)
            {
                if (existingInvitation.InvitationStatusId == (int)InvitationStatusEnum.Accepted)
                {
                    return existingInvitation;
                }
                else
                {
                    existingInvitation.InvitationStatusId = (int)InvitationStatusEnum.New;
                    await base.UpdateAsync(existingInvitation, userId, cancellationToken);
                }
            }
            else
            {
                instance.InvitationStatusId = (int)InvitationStatusEnum.New;

                if (existingUser != null)
                {
                    // Check to see if this Admin already exists in the system (without an invite since they were created prior to invites).
                    var partners = await partnerAdminManager.GetPartnersByUserIdAsync(existingUser.Id, cancellationToken);

                    if (partners.Any(p => p.Id == instance.PartnerId))
                    {
                        // Backfill the invitation to remove future confusion
                        instance.InvitationStatusId = (int)InvitationStatusEnum.Accepted;
                    }
                }

                await base.AddAsync(instance, userId, cancellationToken);
            }

            if (existingUser == null)
            {
                var welcomeMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteNewUserToBePartnerAdmin.ToString());
                var welcomeSubject = "You have been invited to be an Administrator for a TrashMob.eco Partner";

                var userDynamicTemplateData = new
                {
                    partnerName = partner.Name,
                    emailCopy = welcomeMessage,
                    subject = welcomeSubject,
                };

                var welcomeRecipients = new List<EmailAddress>
                    {
                        new EmailAddress { Name = instance.Email, Email = instance.Email }
                    };

                await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients, CancellationToken.None).ConfigureAwait(false);

                instance.InvitationStatusId = (int)InvitationStatusEnum.Sent;
            }
            else
            {
                if (instance.InvitationStatusId != (int)InvitationStatusEnum.Accepted)
                {
                    var welcomeMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteExistingUserToBePartnerAdmin.ToString());
                    var welcomeSubject = "You have been invited to be an Administrator for a TrashMob.eco Partner";

                    var userDynamicTemplateData = new
                    {
                        partnerName = partner.Name,
                        emailCopy = welcomeMessage,
                        subject = welcomeSubject,
                    };

                    var welcomeRecipients = new List<EmailAddress>
                    {
                        new EmailAddress { Name = existingUser.UserName, Email = instance.Email }
                    };

                    await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients, CancellationToken.None).ConfigureAwait(false);

                    instance.InvitationStatusId = (int)InvitationStatusEnum.Sent;
                }
            }

            var newInvitation = await base.UpdateAsync(instance, userId, cancellationToken).ConfigureAwait(false);
            return newInvitation;
        }

        public async Task AcceptInvitation(Guid partnerAdminInviationId, Guid userId, CancellationToken cancellationToken)
        {
            var partnerAdminInvitation = await Repository.Get(pa => pa.Id == partnerAdminInviationId).FirstOrDefaultAsync(cancellationToken);

            if (partnerAdminInvitation == null)
            {
                return;
            }

            var partnerAdmin = new PartnerAdmin
            {
                PartnerId = partnerAdminInvitation.PartnerId,
                UserId = userId
            };

            await partnerAdminManager.AddAsync(partnerAdmin, cancellationToken);
            partnerAdminInvitation.InvitationStatusId = (int)InvitationStatusEnum.Accepted;
            await base.UpdateAsync(partnerAdminInvitation, userId, cancellationToken);
        }

        public async Task DeclineInvitation(Guid partnerAdminInviationId, Guid userId, CancellationToken cancellationToken)
        {
            var partnerAdminInvitation = await Repository.Get(pa => pa.Id == partnerAdminInviationId).FirstOrDefaultAsync(cancellationToken);

            if (partnerAdminInvitation == null)
            {
                return;
            }

            partnerAdminInvitation.InvitationStatusId = (int)InvitationStatusEnum.Declined;
            await base.UpdateAsync(partnerAdminInvitation, userId, cancellationToken);
        }

        public async Task<PartnerAdminInvitation> ResendPartnerAdminInvitation(Guid partnerAdminInvitationId, Guid UserId, CancellationToken cancellationToken)
        {
            // Check to see if this user already exists in the system.
            var instance = await Repo.GetAsync(partnerAdminInvitationId, cancellationToken);
            var existingUser = await userManager.GetUserByEmailAsync(instance.Email, cancellationToken);
            var partner = await partnerManager.GetAsync(instance.PartnerId, cancellationToken);

            if (existingUser == null)
            {
                var welcomeMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteNewUserToBePartnerAdmin.ToString());
                var welcomeSubject = "You have been invited to be an Administrator for a TrashMob.eco Partner";

                var userDynamicTemplateData = new
                {
                    partnerName = partner.Name,
                    emailCopy = welcomeMessage,
                    subject = welcomeSubject,
                };

                var welcomeRecipients = new List<EmailAddress>
                    {
                        new EmailAddress { Name = instance.Email, Email = instance.Email }
                    };

                await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients, CancellationToken.None).ConfigureAwait(false);
            }
            else
            {
                var welcomeMessage = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteExistingUserToBePartnerAdmin.ToString());
                var welcomeSubject = "You have been invited to be an Administrator for a TrashMob.eco Partner";

                var userDynamicTemplateData = new
                {
                    partnerName = partner.Name,
                    emailCopy = welcomeMessage,
                    subject = welcomeSubject,
                };

                var welcomeRecipients = new List<EmailAddress>
                    {
                        new EmailAddress { Name = existingUser.UserName, Email = instance.Email }
                    };

                await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail, SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients, CancellationToken.None).ConfigureAwait(false);
            }

            instance.InvitationStatusId = (int)InvitationStatusEnum.Sent;
            var updatedInvitation = await base.UpdateAsync(instance, UserId, cancellationToken);

            return updatedInvitation;
        }

        public async Task<IEnumerable<DisplayPartnerAdminInvitation>> GetInvitationsForUser(Guid userId, CancellationToken cancellationToken)
        {
            var user = await userManager.GetAsync(userId, cancellationToken);

            var partnerInvitations = await Repository.Get(p => p.Email == user.Email && p.InvitationStatusId == (int)InvitationStatusEnum.Sent)
                                                     .Include(p => p.Partner)
                                                     .ToListAsync(cancellationToken);

            var displayInvitations = new List<DisplayPartnerAdminInvitation>();
            foreach (var invitation in partnerInvitations)
            {
                var displayInvitation = new DisplayPartnerAdminInvitation
                {
                    Id = invitation.Id,
                    PartnerName = invitation.Partner.Name
                };

                displayInvitations.Add(displayInvitation);
            }

            return displayInvitations;
        }
    }
}
