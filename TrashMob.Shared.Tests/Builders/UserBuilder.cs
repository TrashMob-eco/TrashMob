namespace TrashMob.Shared.Tests.Builders
{
    using System;
    using TrashMob.Models;

    /// <summary>
    /// Builder for creating User test data with sensible defaults.
    /// </summary>
    public class UserBuilder
    {
        private readonly User _user;

        public UserBuilder()
        {
            var id = Guid.NewGuid();
            _user = new User
            {
                Id = id,
                ObjectId = Guid.NewGuid(),
                UserName = $"testuser_{id:N}".Substring(0, 20),
                Email = $"test_{id:N}@example.com".Substring(0, 40),
                NameIdentifier = $"nameid_{id:N}",
                SourceSystemUserName = $"source_{id:N}",
                City = "Seattle",
                Region = "WA",
                Country = "United States",
                PostalCode = "98101",
                Latitude = 47.6062,
                Longitude = -122.3321,
                TravelLimitForLocalEvents = 25,
                IsSiteAdmin = false,
                PrefersMetric = false,
                ShowOnLeaderboards = true,
                AchievementNotificationsEnabled = true,
                MemberSince = DateTimeOffset.UtcNow.AddMonths(-6),
                CreatedByUserId = id,
                CreatedDate = DateTimeOffset.UtcNow.AddMonths(-6),
                LastUpdatedByUserId = id,
                LastUpdatedDate = DateTimeOffset.UtcNow
            };
        }

        public UserBuilder WithId(Guid id)
        {
            _user.Id = id;
            return this;
        }

        public UserBuilder WithUserName(string userName)
        {
            _user.UserName = userName;
            return this;
        }

        public UserBuilder WithEmail(string email)
        {
            _user.Email = email;
            return this;
        }

        public UserBuilder WithCity(string city)
        {
            _user.City = city;
            return this;
        }

        public UserBuilder WithRegion(string region)
        {
            _user.Region = region;
            return this;
        }

        public UserBuilder WithCountry(string country)
        {
            _user.Country = country;
            return this;
        }

        public UserBuilder WithLocation(double latitude, double longitude)
        {
            _user.Latitude = latitude;
            _user.Longitude = longitude;
            return this;
        }

        public UserBuilder AsSiteAdmin()
        {
            _user.IsSiteAdmin = true;
            return this;
        }

        public UserBuilder WithWaiverSigned(string version = "1.0")
        {
            _user.DateAgreedToTrashMobWaiver = DateTimeOffset.UtcNow.AddDays(-30);
            _user.TrashMobWaiverVersion = version;
            return this;
        }

        public UserBuilder WithTravelLimit(int miles)
        {
            _user.TravelLimitForLocalEvents = miles;
            return this;
        }

        public UserBuilder HiddenFromLeaderboards()
        {
            _user.ShowOnLeaderboards = false;
            return this;
        }

        public UserBuilder WithMemberSince(DateTimeOffset date)
        {
            _user.MemberSince = date;
            return this;
        }

        public UserBuilder CreatedBy(Guid userId)
        {
            _user.CreatedByUserId = userId;
            _user.LastUpdatedByUserId = userId;
            return this;
        }

        public User Build() => _user;
    }
}
