namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating Team test data with sensible defaults.
    /// </summary>
    public class TeamBuilder
    {
        private readonly Team _team;

        public TeamBuilder()
        {
            var creatorId = Guid.NewGuid();
            _team = new Team
            {
                Id = Guid.NewGuid(),
                Name = "Test Team",
                Description = "A test team for unit testing",
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                IsActive = true,
                CreatedByUserId = creatorId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = creatorId,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };
        }

        public TeamBuilder WithId(Guid id)
        {
            _team.Id = id;
            return this;
        }

        public TeamBuilder WithName(string name)
        {
            _team.Name = name;
            return this;
        }

        public TeamBuilder WithDescription(string description)
        {
            _team.Description = description;
            return this;
        }

        public TeamBuilder WithLocation(string city, string region, string country)
        {
            _team.City = city;
            _team.Region = region;
            _team.Country = country;
            return this;
        }

        public TeamBuilder AsInactive()
        {
            _team.IsActive = false;
            return this;
        }

        public TeamBuilder AsActive()
        {
            _team.IsActive = true;
            return this;
        }

        public TeamBuilder AsPublic()
        {
            _team.IsPublic = true;
            return this;
        }

        public TeamBuilder AsPrivate()
        {
            _team.IsPublic = false;
            return this;
        }

        public TeamBuilder WithCoordinates(double latitude, double longitude)
        {
            _team.Latitude = latitude;
            _team.Longitude = longitude;
            return this;
        }

        public TeamBuilder CreatedBy(Guid userId)
        {
            _team.CreatedByUserId = userId;
            _team.LastUpdatedByUserId = userId;
            return this;
        }

        public Team Build() => _team;
    }
}
