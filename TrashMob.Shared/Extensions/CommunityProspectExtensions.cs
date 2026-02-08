namespace TrashMob.Shared.Extensions
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Extension methods for the CommunityProspect class.
    /// </summary>
    public static class CommunityProspectExtensions
    {
        /// <summary>
        /// Converts a CommunityProspect to a new Partner entity with an associated PartnerContact.
        /// </summary>
        /// <param name="prospect">The prospect to convert.</param>
        /// <param name="partnerTypeId">The partner type ID to assign.</param>
        /// <returns>A new Partner entity with the prospect data.</returns>
        public static Partner ToPartner(this CommunityProspect prospect, int partnerTypeId)
        {
            var partner = new Partner
            {
                Id = Guid.NewGuid(),
                Name = prospect.Name,
                Website = prospect.Website,
                ContactEmail = prospect.ContactEmail,
                City = prospect.City,
                Region = prospect.Region,
                Country = prospect.Country,
                Latitude = prospect.Latitude,
                Longitude = prospect.Longitude,
                PartnerStatusId = (int)PartnerStatusEnum.Inactive,
                PartnerTypeId = partnerTypeId,
                PrivateNotes = $"Converted from prospect pipeline. Original notes: {prospect.Notes}",
                CreatedByUserId = prospect.CreatedByUserId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = prospect.LastUpdatedByUserId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };

            if (!string.IsNullOrWhiteSpace(prospect.ContactName))
            {
                var partnerContact = new PartnerContact
                {
                    Id = Guid.NewGuid(),
                    Name = prospect.ContactName,
                    Email = prospect.ContactEmail,
                };

                partner.PartnerContacts.Add(partnerContact);
            }

            return partner;
        }
    }
}
