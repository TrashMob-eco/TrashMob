namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Engine;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages bulk email invitation operations including creating batches and sending emails.
    /// </summary>
    public class EmailInviteManager : KeyedManager<EmailInviteBatch>, IEmailInviteManager
    {
        private readonly IKeyedRepository<EmailInvite> emailInviteRepository;
        private readonly IEmailManager emailManager;
        private readonly IUserManager userManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly ITeamManager teamManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmailInviteManager"/> class.
        /// </summary>
        /// <param name="emailInviteBatchRepository">The repository for email invite batch data access.</param>
        /// <param name="emailInviteRepository">The repository for individual email invite data access.</param>
        /// <param name="emailManager">The email manager for sending notifications.</param>
        /// <param name="userManager">The manager for user operations.</param>
        /// <param name="partnerManager">The manager for partner/community operations.</param>
        /// <param name="teamManager">The manager for team operations.</param>
        public EmailInviteManager(
            IKeyedRepository<EmailInviteBatch> emailInviteBatchRepository,
            IKeyedRepository<EmailInvite> emailInviteRepository,
            IEmailManager emailManager,
            IUserManager userManager,
            IKeyedManager<Partner> partnerManager,
            ITeamManager teamManager)
            : base(emailInviteBatchRepository)
        {
            this.emailInviteRepository = emailInviteRepository;
            this.emailManager = emailManager;
            this.userManager = userManager;
            this.partnerManager = partnerManager;
            this.teamManager = teamManager;
        }

        /// <inheritdoc />
        public async Task<EmailInviteBatch> CreateBatchAsync(
            IEnumerable<string> emails,
            Guid senderUserId,
            string batchType,
            Guid? communityId = null,
            Guid? teamId = null,
            CancellationToken cancellationToken = default)
        {
            // Normalize and deduplicate emails
            var emailList = emails
                .Select(e => e?.Trim().ToLowerInvariant())
                .Where(e => !string.IsNullOrWhiteSpace(e) && e.Contains('@'))
                .Distinct()
                .ToList();

            if (!emailList.Any())
            {
                throw new ArgumentException("No valid email addresses provided.", nameof(emails));
            }

            // Create the batch
            var batch = new EmailInviteBatch
            {
                Id = Guid.NewGuid(),
                SenderUserId = senderUserId,
                BatchType = batchType,
                CommunityId = communityId,
                TeamId = teamId,
                TotalCount = emailList.Count,
                SentCount = 0,
                DeliveredCount = 0,
                BouncedCount = 0,
                FailedCount = 0,
                Status = "Pending",
            };

            await base.AddAsync(batch, senderUserId, cancellationToken);

            // Create individual invite records
            foreach (var email in emailList)
            {
                var invite = new EmailInvite
                {
                    Id = Guid.NewGuid(),
                    BatchId = batch.Id,
                    Email = email,
                    Status = "Pending",
                };

                await emailInviteRepository.AddAsync(invite);
            }

            return batch;
        }

        /// <inheritdoc />
        public async Task<EmailInviteBatch> ProcessBatchAsync(Guid batchId, CancellationToken cancellationToken = default)
        {
            var batch = await Repository.Get(b => b.Id == batchId)
                .Include(b => b.SenderUser)
                .Include(b => b.Invites)
                .FirstOrDefaultAsync(cancellationToken);

            if (batch == null)
            {
                throw new ArgumentException("Batch not found.", nameof(batchId));
            }

            // Update batch status to processing
            batch.Status = "Processing";
            await Repository.UpdateAsync(batch);

            var senderName = batch.SenderUser?.UserName ?? "TrashMob.eco";
            var pendingInvites = batch.Invites.Where(i => i.Status == "Pending").ToList();

            // Get community name if this is a community batch
            string communityName = null;
            if (batch.CommunityId.HasValue)
            {
                var community = await partnerManager.GetAsync(batch.CommunityId.Value, cancellationToken);
                communityName = community?.Name;
            }

            // Get team name if this is a team batch
            string teamName = null;
            if (batch.TeamId.HasValue)
            {
                var team = await teamManager.GetAsync(batch.TeamId.Value, cancellationToken);
                teamName = team?.Name;
            }

            foreach (var invite in pendingInvites)
            {
                try
                {
                    await SendInviteEmailAsync(invite, senderName, communityName, teamName, cancellationToken);
                    invite.Status = "Sent";
                    invite.SentDate = DateTimeOffset.UtcNow;
                    batch.SentCount++;
                }
                catch (Exception ex)
                {
                    invite.Status = "Failed";
                    invite.ErrorMessage = ex.Message.Length > 500 ? ex.Message[..500] : ex.Message;
                    batch.FailedCount++;
                }

                await emailInviteRepository.UpdateAsync(invite);
            }

            // Update batch completion status
            batch.Status = "Complete";
            batch.CompletedDate = DateTimeOffset.UtcNow;
            await Repository.UpdateAsync(batch);

            return batch;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EmailInviteBatch>> GetAdminBatchesAsync(CancellationToken cancellationToken = default)
        {
            return await Repository.Get()
                .Include(b => b.SenderUser)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<EmailInviteBatch> GetBatchDetailsAsync(Guid batchId, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(b => b.Id == batchId)
                .Include(b => b.SenderUser)
                .Include(b => b.Invites)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EmailInviteBatch>> GetCommunityBatchesAsync(Guid communityId, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(b => b.CommunityId == communityId)
                .Include(b => b.SenderUser)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<EmailInviteBatch>> GetTeamBatchesAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(b => b.TeamId == teamId)
                .Include(b => b.SenderUser)
                .OrderByDescending(b => b.CreatedDate)
                .ToListAsync(cancellationToken);
        }

        private async Task SendInviteEmailAsync(EmailInvite invite, string senderName, string communityName, string teamName, CancellationToken cancellationToken)
        {
            string emailCopy;
            string subject;

            if (!string.IsNullOrEmpty(teamName))
            {
                // Use team-specific template
                emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteToJoinTeam.ToString());
                emailCopy = emailCopy.Replace("{teamName}", teamName);
                subject = $"You're Invited to Join {teamName} on TrashMob.eco!";
            }
            else if (!string.IsNullOrEmpty(communityName))
            {
                // Use community-specific template
                emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteToJoinCommunity.ToString());
                emailCopy = emailCopy.Replace("{communityName}", communityName);
                subject = $"You're Invited to Join {communityName} on TrashMob.eco!";
            }
            else
            {
                // Use generic TrashMob template
                emailCopy = emailManager.GetHtmlEmailCopy(NotificationTypeEnum.InviteToJoinTrashMob.ToString());
                subject = "You're Invited to Join TrashMob.eco!";
            }

            var dynamicTemplateData = new
            {
                subject,
                emailCopy,
            };

            var recipients = new List<EmailAddress>
            {
                new() { Name = invite.Email, Email = invite.Email },
            };

            await emailManager.SendTemplatedEmailAsync(
                subject,
                SendGridEmailTemplateId.GenericEmail,
                SendGridEmailGroupId.General,
                dynamicTemplateData,
                recipients,
                cancellationToken);
        }
    }
}
