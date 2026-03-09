namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using System.Collections.Generic;
    using TrashMob.Shared.Extensions.V2;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class LeaderboardMappingsV2Tests
    {
        [Fact]
        public void LeaderboardResponse_ToV2Dto_MapsAllProperties()
        {
            var response = new LeaderboardResponse
            {
                LeaderboardType = "Events",
                TimeRange = "Month",
                LocationScope = "Global",
                LocationValue = null,
                ComputedDate = new DateTimeOffset(2026, 3, 1, 0, 0, 0, TimeSpan.Zero),
                TotalEntries = 100,
                Entries = new List<LeaderboardEntry>
                {
                    new()
                    {
                        EntityId = Guid.NewGuid(),
                        EntityName = "CleanupKing",
                        EntityType = "User",
                        Rank = 1,
                        Score = 42,
                        FormattedScore = "42 events",
                        Region = "Washington",
                        City = "Seattle",
                        ProfilePhotoUrl = "https://example.com/photo.jpg",
                    },
                },
            };

            var dto = response.ToV2Dto();

            Assert.Equal("Events", dto.LeaderboardType);
            Assert.Equal("Month", dto.TimeRange);
            Assert.Equal("Global", dto.LocationScope);
            Assert.Equal(string.Empty, dto.LocationValue);
            Assert.Equal(response.ComputedDate, dto.ComputedDate);
            Assert.Equal(100, dto.TotalEntries);
            Assert.Single(dto.Entries);
            Assert.Equal("CleanupKing", dto.Entries[0].EntityName);
            Assert.Equal(1, dto.Entries[0].Rank);
            Assert.Equal(42, dto.Entries[0].Score);
            Assert.Equal("42 events", dto.Entries[0].FormattedScore);
        }

        [Fact]
        public void LeaderboardEntry_ToV2Dto_MapsAllProperties()
        {
            var entityId = Guid.NewGuid();
            var entry = new LeaderboardEntry
            {
                EntityId = entityId,
                EntityName = "TrashTeam",
                EntityType = "Team",
                Rank = 3,
                Score = 150.5m,
                FormattedScore = "150 lbs",
                Region = "Oregon",
                City = "Portland",
                ProfilePhotoUrl = "https://example.com/team.jpg",
            };

            var dto = entry.ToV2Dto();

            Assert.Equal(entityId, dto.EntityId);
            Assert.Equal("TrashTeam", dto.EntityName);
            Assert.Equal("Team", dto.EntityType);
            Assert.Equal(3, dto.Rank);
            Assert.Equal(150.5m, dto.Score);
            Assert.Equal("150 lbs", dto.FormattedScore);
            Assert.Equal("Oregon", dto.Region);
            Assert.Equal("Portland", dto.City);
            Assert.Equal("https://example.com/team.jpg", dto.ProfilePhotoUrl);
        }

        [Fact]
        public void UserRankResponse_ToV2Dto_MapsAllProperties()
        {
            var response = new UserRankResponse
            {
                LeaderboardType = "Bags",
                TimeRange = "AllTime",
                Rank = 5,
                Score = 200m,
                FormattedScore = "200 bags",
                TotalRanked = 500,
                IsEligible = true,
                IneligibleReason = null,
            };

            var dto = response.ToV2Dto();

            Assert.Equal("Bags", dto.LeaderboardType);
            Assert.Equal("AllTime", dto.TimeRange);
            Assert.Equal(5, dto.Rank);
            Assert.Equal(200m, dto.Score);
            Assert.Equal("200 bags", dto.FormattedScore);
            Assert.Equal(500, dto.TotalRanked);
            Assert.True(dto.IsEligible);
            Assert.Equal(string.Empty, dto.IneligibleReason);
        }

        [Fact]
        public void UserRankResponse_ToV2Dto_HandlesIneligibleUser()
        {
            var response = new UserRankResponse
            {
                LeaderboardType = "Events",
                TimeRange = "Month",
                Rank = null,
                Score = null,
                FormattedScore = null,
                TotalRanked = 100,
                IsEligible = false,
                IneligibleReason = "Fewer than 3 events",
            };

            var dto = response.ToV2Dto();

            Assert.Null(dto.Rank);
            Assert.Null(dto.Score);
            Assert.Equal(string.Empty, dto.FormattedScore);
            Assert.False(dto.IsEligible);
            Assert.Equal("Fewer than 3 events", dto.IneligibleReason);
        }

        [Fact]
        public void TeamRankResponse_ToV2Dto_MapsAllProperties()
        {
            var teamId = Guid.NewGuid();
            var response = new TeamRankResponse
            {
                TeamId = teamId,
                TeamName = "Eco Warriors",
                LeaderboardType = "Weight",
                TimeRange = "Year",
                Rank = 2,
                Score = 500m,
                FormattedScore = "500 lbs",
                TotalRanked = 50,
                IsEligible = true,
                IneligibleReason = null,
            };

            var dto = response.ToV2Dto();

            Assert.Equal(teamId, dto.TeamId);
            Assert.Equal("Eco Warriors", dto.TeamName);
            Assert.Equal("Weight", dto.LeaderboardType);
            Assert.Equal("Year", dto.TimeRange);
            Assert.Equal(2, dto.Rank);
            Assert.Equal(500m, dto.Score);
            Assert.Equal("500 lbs", dto.FormattedScore);
            Assert.Equal(50, dto.TotalRanked);
            Assert.True(dto.IsEligible);
            Assert.Equal(string.Empty, dto.IneligibleReason);
        }

        [Fact]
        public void LeaderboardResponse_ToV2Dto_HandlesNullEntries()
        {
            var response = new LeaderboardResponse
            {
                LeaderboardType = "Events",
                TimeRange = "Week",
                LocationScope = "Global",
                Entries = null,
            };

            var dto = response.ToV2Dto();

            Assert.Empty(dto.Entries);
        }
    }
}
