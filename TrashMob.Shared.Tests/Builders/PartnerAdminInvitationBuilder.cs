namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating PartnerAdminInvitation test data with sensible defaults.
    /// </summary>
    public class PartnerAdminInvitationBuilder
    {
        private readonly PartnerAdminInvitation _invitation;

        public PartnerAdminInvitationBuilder()
        {
            var id = Guid.NewGuid();
            _invitation = new PartnerAdminInvitation
            {
                Id = id,
                PartnerId = Guid.NewGuid(),
                Email = $"admin_{id:N}@example.com".Substring(0, 40),
                InvitationStatusId = (int)InvitationStatusEnum.New,
                DateInvited = DateTimeOffset.UtcNow,
                CreatedByUserId = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = Guid.NewGuid(),
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };
        }

        public PartnerAdminInvitationBuilder WithId(Guid id)
        {
            _invitation.Id = id;
            return this;
        }

        public PartnerAdminInvitationBuilder WithPartnerId(Guid partnerId)
        {
            _invitation.PartnerId = partnerId;
            return this;
        }

        public PartnerAdminInvitationBuilder WithPartner(Partner partner)
        {
            _invitation.PartnerId = partner.Id;
            _invitation.Partner = partner;
            return this;
        }

        public PartnerAdminInvitationBuilder WithEmail(string email)
        {
            _invitation.Email = email;
            return this;
        }

        public PartnerAdminInvitationBuilder AsNew()
        {
            _invitation.InvitationStatusId = (int)InvitationStatusEnum.New;
            return this;
        }

        public PartnerAdminInvitationBuilder AsSent()
        {
            _invitation.InvitationStatusId = (int)InvitationStatusEnum.Sent;
            return this;
        }

        public PartnerAdminInvitationBuilder AsAccepted()
        {
            _invitation.InvitationStatusId = (int)InvitationStatusEnum.Accepted;
            return this;
        }

        public PartnerAdminInvitationBuilder AsDeclined()
        {
            _invitation.InvitationStatusId = (int)InvitationStatusEnum.Declined;
            return this;
        }

        public PartnerAdminInvitationBuilder AsCanceled()
        {
            _invitation.InvitationStatusId = (int)InvitationStatusEnum.Canceled;
            return this;
        }

        public PartnerAdminInvitationBuilder InvitedOn(DateTimeOffset dateInvited)
        {
            _invitation.DateInvited = dateInvited;
            return this;
        }

        public PartnerAdminInvitationBuilder CreatedBy(Guid userId)
        {
            _invitation.CreatedByUserId = userId;
            _invitation.LastUpdatedByUserId = userId;
            return this;
        }

        public PartnerAdminInvitation Build() => _invitation;
    }
}
