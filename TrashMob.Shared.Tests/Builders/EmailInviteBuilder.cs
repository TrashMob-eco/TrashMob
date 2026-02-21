namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating EmailInvite test data with sensible defaults.
    /// </summary>
    public class EmailInviteBuilder
    {
        private readonly EmailInvite _invite;

        public EmailInviteBuilder()
        {
            var id = Guid.NewGuid();
            _invite = new EmailInvite
            {
                Id = id,
                BatchId = Guid.NewGuid(),
                Email = $"invite_{id:N}@example.com".Substring(0, 40),
                Status = "Pending",
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = Guid.NewGuid(),
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };
        }

        public EmailInviteBuilder WithId(Guid id)
        {
            _invite.Id = id;
            return this;
        }

        public EmailInviteBuilder WithBatchId(Guid batchId)
        {
            _invite.BatchId = batchId;
            return this;
        }

        public EmailInviteBuilder WithBatch(EmailInviteBatch batch)
        {
            _invite.BatchId = batch.Id;
            _invite.Batch = batch;
            return this;
        }

        public EmailInviteBuilder WithEmail(string email)
        {
            _invite.Email = email;
            return this;
        }

        public EmailInviteBuilder AsPending()
        {
            _invite.Status = "Pending";
            _invite.SentDate = null;
            return this;
        }

        public EmailInviteBuilder AsSent()
        {
            _invite.Status = "Sent";
            _invite.SentDate = DateTimeOffset.UtcNow;
            return this;
        }

        public EmailInviteBuilder AsDelivered()
        {
            _invite.Status = "Delivered";
            _invite.SentDate = DateTimeOffset.UtcNow.AddMinutes(-5);
            _invite.DeliveredDate = DateTimeOffset.UtcNow;
            return this;
        }

        public EmailInviteBuilder AsBounced()
        {
            _invite.Status = "Bounced";
            _invite.SentDate = DateTimeOffset.UtcNow.AddMinutes(-5);
            return this;
        }

        public EmailInviteBuilder AsFailed()
        {
            _invite.Status = "Failed";
            return this;
        }

        public EmailInviteBuilder WithError(string errorMessage)
        {
            _invite.Status = "Failed";
            _invite.ErrorMessage = errorMessage;
            return this;
        }

        public EmailInviteBuilder WithSignup(Guid userId)
        {
            _invite.SignedUpUserId = userId;
            _invite.SignedUpDate = DateTimeOffset.UtcNow;
            return this;
        }

        public EmailInviteBuilder CreatedBy(Guid userId)
        {
            _invite.CreatedByUserId = userId;
            _invite.LastUpdatedByUserId = userId;
            return this;
        }

        public EmailInvite Build() => _invite;
    }
}
