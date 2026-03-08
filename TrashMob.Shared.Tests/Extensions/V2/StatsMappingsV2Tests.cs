namespace TrashMob.Shared.Tests.Extensions.V2
{
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco;
    using Xunit;

    public class StatsMappingsV2Tests
    {
        [Fact]
        public void ToV2Dto_MapsAllProperties()
        {
            var stats = new Stats
            {
                TotalBags = 100,
                TotalHours = 200,
                TotalEvents = 50,
                TotalWeightInPounds = 1500.5m,
                TotalWeightInKilograms = 680.4m,
                TotalParticipants = 300,
                TotalLitterReportsSubmitted = 75,
                TotalLitterReportsClosed = 60,
            };

            var dto = stats.ToV2Dto();

            Assert.Equal(stats.TotalBags, dto.TotalBags);
            Assert.Equal(stats.TotalHours, dto.TotalHours);
            Assert.Equal(stats.TotalEvents, dto.TotalEvents);
            Assert.Equal(stats.TotalWeightInPounds, dto.TotalWeightInPounds);
            Assert.Equal(stats.TotalWeightInKilograms, dto.TotalWeightInKilograms);
            Assert.Equal(stats.TotalParticipants, dto.TotalParticipants);
            Assert.Equal(stats.TotalLitterReportsSubmitted, dto.TotalLitterReportsSubmitted);
            Assert.Equal(stats.TotalLitterReportsClosed, dto.TotalLitterReportsClosed);
        }

        [Fact]
        public void ToV2Dto_HandlesDefaultValues()
        {
            var stats = new Stats();

            var dto = stats.ToV2Dto();

            Assert.Equal(0, dto.TotalBags);
            Assert.Equal(0, dto.TotalHours);
            Assert.Equal(0, dto.TotalEvents);
            Assert.Equal(0m, dto.TotalWeightInPounds);
            Assert.Equal(0m, dto.TotalWeightInKilograms);
            Assert.Equal(0, dto.TotalParticipants);
            Assert.Equal(0, dto.TotalLitterReportsSubmitted);
            Assert.Equal(0, dto.TotalLitterReportsClosed);
        }
    }
}
