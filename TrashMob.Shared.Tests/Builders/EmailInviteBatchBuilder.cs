namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using System.Collections.Generic;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating EmailInviteBatch test data with sensible defaults.
    /// </summary>
    public class EmailInviteBatchBuilder
    {
        private readonly EmailInviteBatch _batch;

        public EmailInviteBatchBuilder()
        {
            var id = Guid.NewGuid();
            var senderId = Guid.NewGuid();
            _batch = new EmailInviteBatch
            {
                Id = id,
                SenderUserId = senderId,
                BatchType = "Admin",
                TotalCount = 0,
                SentCount = 0,
                DeliveredCount = 0,
                BouncedCount = 0,
                FailedCount = 0,
                Status = "Pending",
                Invites = new List<EmailInvite>(),
                CreatedByUserId = senderId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = senderId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };
        }

        public EmailInviteBatchBuilder WithId(Guid id)
        {
            _batch.Id = id;
            return this;
        }

        public EmailInviteBatchBuilder WithSender(Guid senderUserId)
        {
            _batch.SenderUserId = senderUserId;
            return this;
        }

        public EmailInviteBatchBuilder WithSenderUser(User sender)
        {
            _batch.SenderUserId = sender.Id;
            _batch.SenderUser = sender;
            return this;
        }

        public EmailInviteBatchBuilder AsAdminBatch()
        {
            _batch.BatchType = "Admin";
            _batch.CommunityId = null;
            _batch.TeamId = null;
            return this;
        }

        public EmailInviteBatchBuilder AsCommunityBatch(Guid communityId)
        {
            _batch.BatchType = "Community";
            _batch.CommunityId = communityId;
            return this;
        }

        public EmailInviteBatchBuilder AsTeamBatch(Guid teamId)
        {
            _batch.BatchType = "Team";
            _batch.TeamId = teamId;
            return this;
        }

        public EmailInviteBatchBuilder AsUserBatch()
        {
            _batch.BatchType = "User";
            return this;
        }

        public EmailInviteBatchBuilder WithCounts(int total, int sent = 0, int delivered = 0, int bounced = 0, int failed = 0)
        {
            _batch.TotalCount = total;
            _batch.SentCount = sent;
            _batch.DeliveredCount = delivered;
            _batch.BouncedCount = bounced;
            _batch.FailedCount = failed;
            return this;
        }

        public EmailInviteBatchBuilder AsPending()
        {
            _batch.Status = "Pending";
            _batch.CompletedDate = null;
            return this;
        }

        public EmailInviteBatchBuilder AsProcessing()
        {
            _batch.Status = "Processing";
            _batch.CompletedDate = null;
            return this;
        }

        public EmailInviteBatchBuilder AsComplete()
        {
            _batch.Status = "Complete";
            _batch.CompletedDate = DateTimeOffset.UtcNow;
            return this;
        }

        public EmailInviteBatchBuilder AsFailed()
        {
            _batch.Status = "Failed";
            _batch.CompletedDate = DateTimeOffset.UtcNow;
            return this;
        }

        public EmailInviteBatchBuilder WithInvites(ICollection<EmailInvite> invites)
        {
            _batch.Invites = invites;
            _batch.TotalCount = invites.Count;
            return this;
        }

        public EmailInviteBatchBuilder CreatedBy(Guid userId)
        {
            _batch.CreatedByUserId = userId;
            _batch.LastUpdatedByUserId = userId;
            return this;
        }

        public EmailInviteBatch Build() => _batch;
    }
}
