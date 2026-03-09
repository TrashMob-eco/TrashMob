namespace TrashMob.Shared.Tests.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using TrashMob.Controllers.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;
    using Xunit;

    public class LeaderboardsV2ControllerTests
    {
        private readonly Mock<ILeaderboardManager> leaderboardManager = new();
        private readonly Mock<ILogger<LeaderboardsV2Controller>> logger = new();
        private readonly LeaderboardsV2Controller controller;

        public LeaderboardsV2ControllerTests()
        {
            controller = new LeaderboardsV2Controller(leaderboardManager.Object, logger.Object);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext(),
            };
        }

        [Fact]
        public async Task GetLeaderboard_ReturnsOkWithEntries()
        {
            var response = new LeaderboardResponse
            {
                LeaderboardType = "Events",
                TimeRange = "Month",
                LocationScope = "Global",
                ComputedDate = DateTimeOffset.UtcNow,
                TotalEntries = 2,
                Entries = new List<LeaderboardEntry>
                {
                    new()
                    {
                        EntityId = Guid.NewGuid(),
                        EntityName = "TopUser",
                        EntityType = "User",
                        Rank = 1,
                        Score = 10,
                        FormattedScore = "10 events",
                    },
                    new()
                    {
                        EntityId = Guid.NewGuid(),
                        EntityName = "RunnerUp",
                        EntityType = "User",
                        Rank = 2,
                        Score = 8,
                        FormattedScore = "8 events",
                    },
                },
            };

            leaderboardManager
                .Setup(m => m.GetLeaderboardAsync("Events", "Month", "Global", null, 50, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await controller.GetLeaderboard(cancellationToken: CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<LeaderboardResponseDto>(okResult.Value);
            Assert.Equal("Events", dto.LeaderboardType);
            Assert.Equal(2, dto.Entries.Count);
            Assert.Equal("TopUser", dto.Entries[0].EntityName);
            Assert.Equal(1, dto.Entries[0].Rank);
        }

        [Fact]
        public async Task GetTeamLeaderboard_ReturnsOkWithEntries()
        {
            var response = new LeaderboardResponse
            {
                LeaderboardType = "Bags",
                TimeRange = "Year",
                LocationScope = "Global",
                TotalEntries = 1,
                Entries = new List<LeaderboardEntry>
                {
                    new()
                    {
                        EntityId = Guid.NewGuid(),
                        EntityName = "Eco Warriors",
                        EntityType = "Team",
                        Rank = 1,
                        Score = 500,
                        FormattedScore = "500 bags",
                    },
                },
            };

            leaderboardManager
                .Setup(m => m.GetTeamLeaderboardAsync("Bags", "Year", "Global", null, 50, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await controller.GetTeamLeaderboard(type: "Bags", timeRange: "Year", cancellationToken: CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<LeaderboardResponseDto>(okResult.Value);
            Assert.Equal("Bags", dto.LeaderboardType);
            Assert.Single(dto.Entries);
            Assert.Equal("Team", dto.Entries[0].EntityType);
        }

        [Fact]
        public async Task GetTeamRank_ReturnsOk_WhenTeamFound()
        {
            var teamId = Guid.NewGuid();
            var response = new TeamRankResponse
            {
                TeamId = teamId,
                TeamName = "Green Team",
                LeaderboardType = "Events",
                TimeRange = "AllTime",
                Rank = 3,
                Score = 25,
                FormattedScore = "25 events",
                TotalRanked = 50,
                IsEligible = true,
            };

            leaderboardManager
                .Setup(m => m.GetTeamRankAsync(teamId, "Events", "AllTime", It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await controller.GetTeamRank(teamId, cancellationToken: CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<TeamRankDto>(okResult.Value);
            Assert.Equal(teamId, dto.TeamId);
            Assert.Equal("Green Team", dto.TeamName);
            Assert.Equal(3, dto.Rank);
        }

        [Fact]
        public async Task GetTeamRank_ReturnsNotFound_WhenTeamNotFound()
        {
            var teamId = Guid.NewGuid();
            var response = new TeamRankResponse
            {
                TeamId = teamId,
                IsEligible = false,
                IneligibleReason = "Team not found.",
            };

            leaderboardManager
                .Setup(m => m.GetTeamRankAsync(teamId, "Events", "AllTime", It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await controller.GetTeamRank(teamId, cancellationToken: CancellationToken.None);

            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task GetMyRank_ReturnsOkWithUserRank()
        {
            var userId = Guid.NewGuid();
            controller.HttpContext.Items["UserId"] = userId.ToString();

            var response = new UserRankResponse
            {
                LeaderboardType = "Events",
                TimeRange = "AllTime",
                Rank = 7,
                Score = 15,
                FormattedScore = "15 events",
                TotalRanked = 200,
                IsEligible = true,
            };

            leaderboardManager
                .Setup(m => m.GetUserRankAsync(userId, "Events", "AllTime", It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await controller.GetMyRank(cancellationToken: CancellationToken.None);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<UserRankDto>(okResult.Value);
            Assert.Equal(7, dto.Rank);
            Assert.Equal(15, dto.Score);
            Assert.True(dto.IsEligible);
        }

        [Fact]
        public void GetOptions_ReturnsOkWithOptions()
        {
            leaderboardManager
                .Setup(m => m.GetAvailableLeaderboardTypes())
                .Returns(new[] { "Events", "Bags", "Weight", "Hours" });
            leaderboardManager
                .Setup(m => m.GetAvailableTimeRanges())
                .Returns(new[] { "Week", "Month", "Year", "AllTime" });
            leaderboardManager
                .Setup(m => m.GetAvailableLocationScopes())
                .Returns(new[] { "Global", "Region", "City" });

            var result = controller.GetOptions();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<LeaderboardOptionsDto>(okResult.Value);
            Assert.Equal(4, dto.Types.Length);
            Assert.Equal(4, dto.TimeRanges.Length);
            Assert.Equal(3, dto.LocationScopes.Length);
        }
    }
}
