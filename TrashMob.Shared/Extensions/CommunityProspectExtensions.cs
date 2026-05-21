namespace TrashMob.Shared.Extensions
{
    using System;
    using System.Linq;
    using TrashMob.Models;

    /// <summary>
    /// Extension methods for the CommunityProspect class.
    /// </summary>
    public static class CommunityProspectExtensions
    {
        /// <summary>
        /// Converts a CommunityProspect to a new Partner entity, copying the primary
        /// ProspectContact (if one exists) into a PartnerContact.
        /// </summary>
        /// <param name="prospect">The prospect to convert. Caller is responsible for loading <see cref="CommunityProspect.Contacts"/>.</param>
        /// <param name="partnerTypeId">The partner type ID to assign.</param>
        /// <returns>A new Partner entity with the prospect data.</returns>
        public static Partner ToPartner(this CommunityProspect prospect, int partnerTypeId)
        {
            var primaryContact = prospect.Contacts?.FirstOrDefault(c => c.IsPrimary)
                ?? prospect.Contacts?.FirstOrDefault();

            var partner = new Partner
            {
                Id = Guid.NewGuid(),
                Name = prospect.Name,
                Website = prospect.Website,
                ContactEmail = primaryContact?.Email,
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

            if (primaryContact != null && !string.IsNullOrWhiteSpace(primaryContact.Name))
            {
                partner.PartnerContacts.Add(new PartnerContact
                {
                    Id = Guid.NewGuid(),
                    Name = primaryContact.Name,
                    Email = primaryContact.Email,
                });
            }

            return partner;
        }
    }
}
