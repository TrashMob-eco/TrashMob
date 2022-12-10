namespace TrashMob.Shared.Extensions
{
    using System;
    using TrashMob.Models;

    public static class PartnerRequestExtensions
    {
        public static Partner ToPartner(this PartnerRequest originalPartnerRequest)
        {
            var partnerContact = new PartnerContact();
            partnerContact.Id = Guid.NewGuid();
            partnerContact.Name = originalPartnerRequest.Name;
            partnerContact.Phone = originalPartnerRequest.Phone;
            partnerContact.Email = originalPartnerRequest.Email;

            var partner = new Partner
            {
                Id = Guid.NewGuid(),
                Name = originalPartnerRequest.Name,
                PrivateNotes = originalPartnerRequest.Notes,
                PartnerStatusId = (int)PartnerStatusEnum.Inactive,
                CreatedByUserId = originalPartnerRequest.CreatedByUserId,
                CreatedDate = originalPartnerRequest.CreatedDate,
                LastUpdatedByUserId = originalPartnerRequest.LastUpdatedByUserId,
                LastUpdatedDate = originalPartnerRequest.LastUpdatedDate,
            };

            partner.PartnerContacts.Add(partnerContact);

            return partner;
        }
    }
}
