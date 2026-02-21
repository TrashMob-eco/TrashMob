namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating CommunityProspect test data with sensible defaults.
    /// </summary>
    public class CommunityProspectBuilder
    {
        private readonly CommunityProspect _prospect;

        public CommunityProspectBuilder()
        {
            var creatorId = Guid.NewGuid();
            _prospect = new CommunityProspect
            {
                Id = Guid.NewGuid(),
                Name = "Test Prospect",
                Type = "Municipality",
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                Latitude = 47.6062,
                Longitude = -122.3321,
                Population = 750000,
                PipelineStage = 0,
                FitScore = 0,
                CreatedByUserId = creatorId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = creatorId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };
        }

        public CommunityProspectBuilder WithId(Guid id)
        {
            _prospect.Id = id;
            return this;
        }

        public CommunityProspectBuilder WithName(string name)
        {
            _prospect.Name = name;
            return this;
        }

        public CommunityProspectBuilder WithType(string type)
        {
            _prospect.Type = type;
            return this;
        }

        public CommunityProspectBuilder WithLocation(string city, string region, string country)
        {
            _prospect.City = city;
            _prospect.Region = region;
            _prospect.Country = country;
            return this;
        }

        public CommunityProspectBuilder WithCoordinates(double latitude, double longitude)
        {
            _prospect.Latitude = latitude;
            _prospect.Longitude = longitude;
            return this;
        }

        public CommunityProspectBuilder WithPopulation(int? population)
        {
            _prospect.Population = population;
            return this;
        }

        public CommunityProspectBuilder WithFitScore(int score)
        {
            _prospect.FitScore = score;
            return this;
        }

        public CommunityProspectBuilder WithPipelineStage(int stage)
        {
            _prospect.PipelineStage = stage;
            return this;
        }

        public CommunityProspectBuilder WithContactInfo(string email, string name, string title)
        {
            _prospect.ContactEmail = email;
            _prospect.ContactName = name;
            _prospect.ContactTitle = title;
            return this;
        }

        public CommunityProspectBuilder WithWebsite(string website)
        {
            _prospect.Website = website;
            return this;
        }

        public CommunityProspectBuilder CreatedBy(Guid userId)
        {
            _prospect.CreatedByUserId = userId;
            _prospect.LastUpdatedByUserId = userId;
            return this;
        }

        public CommunityProspect Build() => _prospect;
    }
}
