namespace TrashMob.Shared.Extensions
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Extension methods for the PartnerRequest class.
    /// </summary>
    public static class PartnerRequestExtensions
    {
        /// <summary>
        /// Converts a PartnerRequest to a new Partner entity with an associated PartnerContact.
        /// </summary>
        /// <param name="originalPartnerRequest">The partner request to convert.</param>
        /// <returns>A new Partner entity with the request data.</returns>
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
                City = originalPartnerRequest.City,
                Region = originalPartnerRequest.Region,
                Country = originalPartnerRequest.Country,
                Latitude = originalPartnerRequest.Latitude,
                Longitude = originalPartnerRequest.Longitude,
                Website = originalPartnerRequest.Website,
                PartnerTypeId = originalPartnerRequest.PartnerTypeId,
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