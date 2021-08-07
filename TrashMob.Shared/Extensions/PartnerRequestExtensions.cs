namespace TrashMob.Shared.Extensions
{
    using System;
    using TrashMob.Shared.Models;

    public static class PartnerRequestExtensions
    {
        public static Partner ToPartner(this PartnerRequest originalPartnerRequest)
        {
            return new Partner
            {
                Id = Guid.NewGuid(),
                Name = originalPartnerRequest.Name,
                Notes = originalPartnerRequest.Notes,
                PartnerStatusId = (int)PartnerStatusEnum.Inactive,
                PrimaryEmail = originalPartnerRequest.PrimaryEmail,
                SecondaryEmail = originalPartnerRequest.SecondaryEmail,
                PrimaryPhone = originalPartnerRequest.PrimaryPhone,
                SecondaryPhone = originalPartnerRequest.SecondaryPhone,
                CreatedByUserId = originalPartnerRequest.CreatedByUserId,
                CreatedDate = originalPartnerRequest.CreatedDate,
                LastUpdatedByUserId = originalPartnerRequest.LastUpdatedByUserId,
                LastUpdatedDate = originalPartnerRequest.LastUpdatedDate,             
            };
        }
    }
}
