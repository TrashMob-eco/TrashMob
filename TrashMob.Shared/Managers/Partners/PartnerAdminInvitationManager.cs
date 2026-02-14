namespace TrashMob.Shared.Managers.Partners
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages partner admin invitations including sending, accepting, declining, and resending invitations.
    /// </summary>
    public class PartnerAdminInvitationManager : KeyedManager<PartnerAdminInvitation>, IPartnerAdminInvitationManager
    {
        private readonly IEmailManager emailManager;
        private readonly IPartnerAdminManager partnerAdminManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IUserManager userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerAdminInvitationManager"/> class.
        /// </summary>
        /// <param name="partnerAdminInvitationRepository">The repository for partner admin invitation data access.</param>
        /// <param name="partnerAdminManager">The manager for partner admin operations.</param>
        /// <param name="userManager">The manager for user operations.</param>
        /// <param name="partnerManager">The manager for partner operations.</param>
        /// <param name="emailManager">The email manager for sending notifications.</param>
        public PartnerAdminInvitationManager(IKeyedRepository<PartnerAdminInvitation> partnerAdminInvitationRepository,
            IPartnerAdminManager partnerAdminManager,
            IUserManager userManager,
            IKeyedManager<Partner> partnerManager,
            IEmailManager emailManager)
            : base(partnerAdminInvitationRepository)
        {
            this.partnerAdminManager = partnerAdminManager;
            this.userManager = userManager;
            this.partnerManager = partnerManager;
            this.emailManager = emailManager;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<PartnerAdminInvitation>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Partner> GetPartnerForInvitation(Guid partnerAdminInvitationId,
            CancellationToken cancellationToken)
        {
            var partnerInvitation = await Repository.Get(p => p.Id == partnerAdminInvitationId, false)
                .Include(p => p.Partner)
                .FirstOrDefaultAsync(cancellationToken);
            return partnerInvitation.Partner;
        }

        /// <inheritdoc />
        public override async Task<PartnerAdminInvitation> AddAsync(PartnerAdminInvitation instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            // Check to see if this user already has an invite
            var existingInvitation = await Repository
                .Get(i => i.Email == instance.Email && i.PartnerId == instance.PartnerId)
                .FirstOrDefaultAsync(cancellationToken);
            var existingUser = await userManager.GetUserByEmailAsync(instance.Email, cancellationToken);
            var partner = await partnerManager.GetAsync(instance.PartnerId, cancellationToken);

            if (existingInvitation != null)
            {
                if (existingInvitation.InvitationStatusId == (int)InvitationStatusEnum.Accepted)
                {
                    return existingInvitation;
                }

                existingInvitation.InvitationStatusId = (int)InvitationStatusEnum.New;
                await base.UpdateAsync(existingInvitation, userId, cancellationToken);
            }
            else
            {
                instance.InvitationStatusId = (int)InvitationStatusEnum.New;

                if (existingUser != null)
                {
                    // Check to see if this Admin already exists in the system (without an invite since they were created prior to invites).
                    var partners =
                        await partnerAdminManager.GetPartnersByUserIdAsync(existingUser.Id, cancellationToken);

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
                var welcomeMessage =
                    emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteNewUserToBePartnerAdmin.ToString());
                var welcomeSubject = "You have been invited to be an Administrator for a TrashMob.eco Partner";

                var userDynamicTemplateData = new
                {
                    partnerName = partner.Name,
                    emailCopy = welcomeMessage,
                    subject = welcomeSubject,
                };

                var welcomeRecipients = new List<EmailAddress>
                {
                    new() { Name = instance.Email, Email = instance.Email },
                };

                await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail,
                        SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients,
                        CancellationToken.None);

                instance.InvitationStatusId = (int)InvitationStatusEnum.Sent;
            }
            else
            {
                if (instance.InvitationStatusId != (int)InvitationStatusEnum.Accepted)
                {
                    var welcomeMessage =
                        emailManager.GetHtmlEmailCopy(
                            NotificationTypeEnum.InviteExistingUserToBePartnerAdmin.ToString());
                    var welcomeSubject = "You have been invited to be an Administrator for a TrashMob.eco Partner";

                    var userDynamicTemplateData = new
                    {
                        partnerName = partner.Name,
                        emailCopy = welcomeMessage,
                        subject = welcomeSubject,
                    };

                    var welcomeRecipients = new List<EmailAddress>
                    {
                        new() { Name = existingUser.UserName, Email = instance.Email },
                    };

                    await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail,
                        SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients,
                        CancellationToken.None);

                    instance.InvitationStatusId = (int)InvitationStatusEnum.Sent;
                }
            }

            var newInvitation = await base.UpdateAsync(instance, userId, cancellationToken);
            return newInvitation;
        }

        /// <inheritdoc />
        public async Task AcceptInvitationAsync(Guid partnerAdminInviationId, Guid userId,
            CancellationToken cancellationToken)
        {
            var partnerAdminInvitation = await Repository.Get(pa => pa.Id == partnerAdminInviationId)
                .FirstOrDefaultAsync(cancellationToken);

            if (partnerAdminInvitation == null)
            {
                return;
            }

            var partnerAdmin = new PartnerAdmin
            {
                PartnerId = partnerAdminInvitation.PartnerId,
                UserId = userId,
            };

            await partnerAdminManager.AddAsync(partnerAdmin, cancellationToken);
            partnerAdminInvitation.InvitationStatusId = (int)InvitationStatusEnum.Accepted;
            await base.UpdateAsync(partnerAdminInvitation, userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task DeclineInvitationAsync(Guid partnerAdminInviationId, Guid userId,
            CancellationToken cancellationToken)
        {
            var partnerAdminInvitation = await Repository.Get(pa => pa.Id == partnerAdminInviationId)
                .FirstOrDefaultAsync(cancellationToken);

            if (partnerAdminInvitation == null)
            {
                return;
            }

            partnerAdminInvitation.InvitationStatusId = (int)InvitationStatusEnum.Declined;
            await base.UpdateAsync(partnerAdminInvitation, userId, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<PartnerAdminInvitation> ResendPartnerAdminInvitationAsync(Guid partnerAdminInvitationId,
            Guid UserId, CancellationToken cancellationToken)
        {
            // Check to see if this user already exists in the system.
            var instance = await Repo.GetAsync(partnerAdminInvitationId, cancellationToken);
            var existingUser = await userManager.GetUserByEmailAsync(instance.Email, cancellationToken);
            var partner = await partnerManager.GetAsync(instance.PartnerId, cancellationToken);

            if (existingUser == null)
            {
                var welcomeMessage =
                    emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteNewUserToBePartnerAdmin.ToString());
                var welcomeSubject = "You have been invited to be an Administrator for a TrashMob.eco Partner";

                var userDynamicTemplateData = new
                {
                    partnerName = partner.Name,
                    emailCopy = welcomeMessage,
                    subject = welcomeSubject,
                };

                var welcomeRecipients = new List<EmailAddress>
                {
                    new() { Name = instance.Email, Email = instance.Email },
                };

                await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail,
                        SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients,
                        CancellationToken.None);
            }
            else
            {
                var welcomeMessage =
                    emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteExistingUserToBePartnerAdmin.ToString());
                var welcomeSubject = "You have been invited to be an Administrator for a TrashMob.eco Partner";

                var userDynamicTemplateData = new
                {
                    partnerName = partner.Name,
                    emailCopy = welcomeMessage,
                    subject = welcomeSubject,
                };

                var welcomeRecipients = new List<EmailAddress>
                {
                    new() { Name = existingUser.UserName, Email = instance.Email },
                };

                await emailManager.SendTemplatedEmailAsync(welcomeSubject, SendGridEmailTemplateId.GenericEmail,
                        SendGridEmailGroupId.General, userDynamicTemplateData, welcomeRecipients,
                        CancellationToken.None);
            }

            instance.InvitationStatusId = (int)InvitationStatusEnum.Sent;
            var updatedInvitation = await base.UpdateAsync(instance, UserId, cancellationToken);

            return updatedInvitation;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DisplayPartnerAdminInvitation>> GetInvitationsForUser(Guid userId,
            CancellationToken cancellationToken)
        {
            var user = await userManager.GetAsync(userId, cancellationToken);

            var partnerInvitations = await Repository.Get(p =>
                    p.Email == user.Email && p.InvitationStatusId == (int)InvitationStatusEnum.Sent)
                .Include(p => p.Partner)
                .ToListAsync(cancellationToken);

            var displayInvitations = new List<DisplayPartnerAdminInvitation>();
            foreach (var invitation in partnerInvitations)
            {
                var displayInvitation = new DisplayPartnerAdminInvitation
                {
                    Id = invitation.Id,
                    PartnerName = invitation.Partner.Name,
                };

                displayInvitations.Add(displayInvitation);
            }

            return displayInvitations;
        }
    }
}