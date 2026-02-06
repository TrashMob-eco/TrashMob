namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating PartnerRequest test data with sensible defaults.
    /// </summary>
    public class PartnerRequestBuilder
    {
        private readonly PartnerRequest _partnerRequest;

        public PartnerRequestBuilder()
        {
            var userId = Guid.NewGuid();
            _partnerRequest = new PartnerRequest
            {
                Id = Guid.NewGuid(),
                Name = "Test Partner Organization",
                Email = "partner@test.com",
                Website = "https://www.testpartner.com",
                Phone = "555-0100",
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                PostalCode = "98101",
                Latitude = 47.6062,
                Longitude = -122.3321,
                Notes = "Test partner request notes",
                PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Pending,
                PartnerTypeId = (int)PartnerTypeEnum.Business,
                isBecomeAPartnerRequest = true,
                CreatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = userId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };
        }

        public PartnerRequestBuilder WithId(Guid id)
        {
            _partnerRequest.Id = id;
            return this;
        }

        public PartnerRequestBuilder WithName(string name)
        {
            _partnerRequest.Name = name;
            return this;
        }

        public PartnerRequestBuilder WithEmail(string email)
        {
            _partnerRequest.Email = email;
            return this;
        }

        public PartnerRequestBuilder WithWebsite(string website)
        {
            _partnerRequest.Website = website;
            return this;
        }

        public PartnerRequestBuilder WithPhone(string phone)
        {
            _partnerRequest.Phone = phone;
            return this;
        }

        public PartnerRequestBuilder WithCity(string city)
        {
            _partnerRequest.City = city;
            return this;
        }

        public PartnerRequestBuilder WithRegion(string region)
        {
            _partnerRequest.Region = region;
            return this;
        }

        public PartnerRequestBuilder WithNotes(string notes)
        {
            _partnerRequest.Notes = notes;
            return this;
        }

        public PartnerRequestBuilder WithStatus(PartnerRequestStatusEnum status)
        {
            _partnerRequest.PartnerRequestStatusId = (int)status;
            return this;
        }

        public PartnerRequestBuilder AsPending()
        {
            _partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Pending;
            return this;
        }

        public PartnerRequestBuilder AsSent()
        {
            _partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Sent;
            return this;
        }

        public PartnerRequestBuilder AsApproved()
        {
            _partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Approved;
            return this;
        }

        public PartnerRequestBuilder AsDenied()
        {
            _partnerRequest.PartnerRequestStatusId = (int)PartnerRequestStatusEnum.Denied;
            return this;
        }

        public PartnerRequestBuilder AsBusinessPartner()
        {
            _partnerRequest.PartnerTypeId = (int)PartnerTypeEnum.Business;
            return this;
        }

        public PartnerRequestBuilder AsGovernmentPartner()
        {
            _partnerRequest.PartnerTypeId = (int)PartnerTypeEnum.Government;
            return this;
        }

        public PartnerRequestBuilder AsBecomeAPartnerRequest()
        {
            _partnerRequest.isBecomeAPartnerRequest = true;
            return this;
        }

        public PartnerRequestBuilder AsSendPartnerInviteRequest()
        {
            _partnerRequest.isBecomeAPartnerRequest = false;
            return this;
        }

        public PartnerRequestBuilder CreatedBy(Guid userId)
        {
            _partnerRequest.CreatedByUserId = userId;
            _partnerRequest.LastUpdatedByUserId = userId;
            return this;
        }

        public PartnerRequest Build() => _partnerRequest;
    }
}
