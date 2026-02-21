namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating Partner test data with sensible defaults.
    /// </summary>
    public class PartnerBuilder
    {
        private readonly Partner _partner;

        public PartnerBuilder()
        {
            var creatorId = Guid.NewGuid();
            _partner = new Partner
            {
                Id = Guid.NewGuid(),
                Name = "Test Partner",
                PartnerStatusId = (int)PartnerStatusEnum.Active,
                PartnerTypeId = 1,
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                HomePageEnabled = false,
                CreatedByUserId = creatorId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = creatorId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };
        }

        public PartnerBuilder WithId(Guid id)
        {
            _partner.Id = id;
            return this;
        }

        public PartnerBuilder WithName(string name)
        {
            _partner.Name = name;
            return this;
        }

        public PartnerBuilder WithSlug(string slug)
        {
            _partner.Slug = slug;
            return this;
        }

        public PartnerBuilder WithLocation(string city, string region, string country)
        {
            _partner.City = city;
            _partner.Region = region;
            _partner.Country = country;
            return this;
        }

        public PartnerBuilder WithCoordinates(double latitude, double longitude)
        {
            _partner.Latitude = latitude;
            _partner.Longitude = longitude;
            return this;
        }

        public PartnerBuilder WithStatus(PartnerStatusEnum status)
        {
            _partner.PartnerStatusId = (int)status;
            return this;
        }

        public PartnerBuilder AsActive()
        {
            _partner.PartnerStatusId = (int)PartnerStatusEnum.Active;
            return this;
        }

        public PartnerBuilder AsInactive()
        {
            _partner.PartnerStatusId = (int)PartnerStatusEnum.Inactive;
            return this;
        }

        public PartnerBuilder WithHomePageEnabled()
        {
            _partner.HomePageEnabled = true;
            return this;
        }

        public PartnerBuilder WithHomePageDisabled()
        {
            _partner.HomePageEnabled = false;
            return this;
        }

        public PartnerBuilder WithHomePageDates(DateTimeOffset? startDate, DateTimeOffset? endDate)
        {
            _partner.HomePageStartDate = startDate;
            _partner.HomePageEndDate = endDate;
            return this;
        }

        public PartnerBuilder WithBranding(string primaryColor, string secondaryColor)
        {
            _partner.BrandingPrimaryColor = primaryColor;
            _partner.BrandingSecondaryColor = secondaryColor;
            return this;
        }

        public PartnerBuilder WithContactInfo(string email, string phone)
        {
            _partner.ContactEmail = email;
            _partner.ContactPhone = phone;
            return this;
        }

        public PartnerBuilder WithWebsite(string website)
        {
            _partner.Website = website;
            return this;
        }

        public PartnerBuilder CreatedBy(Guid userId)
        {
            _partner.CreatedByUserId = userId;
            _partner.LastUpdatedByUserId = userId;
            return this;
        }

        public PartnerBuilder AsCountyCommunity(string countyName, string state, double south, double north, double west, double east)
        {
            _partner.RegionType = (int)RegionTypeEnum.County;
            _partner.CountyName = countyName;
            _partner.Region = state;
            _partner.City = null;
            _partner.BoundsSouth = south;
            _partner.BoundsNorth = north;
            _partner.BoundsWest = west;
            _partner.BoundsEast = east;
            return this;
        }

        public PartnerBuilder AsStateCommunity(string state, double south, double north, double west, double east)
        {
            _partner.RegionType = (int)RegionTypeEnum.State;
            _partner.Region = state;
            _partner.City = null;
            _partner.BoundsSouth = south;
            _partner.BoundsNorth = north;
            _partner.BoundsWest = west;
            _partner.BoundsEast = east;
            return this;
        }

        public Partner Build() => _partner;
    }
}
