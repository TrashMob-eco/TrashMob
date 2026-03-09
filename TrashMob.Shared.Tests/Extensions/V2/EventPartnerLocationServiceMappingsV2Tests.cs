namespace TrashMob.Shared.Tests.Extensions.V2
{
    using System;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using Xunit;

    public class EventPartnerLocationServiceMappingsV2Tests
    {
        [Fact]
        public void EventPartnerLocationServiceRequestDto_ToEntity_MapsAllProperties()
        {
            var eventId = Guid.NewGuid();
            var partnerLocationId = Guid.NewGuid();

            var dto = new EventPartnerLocationServiceRequestDto
            {
                EventId = eventId,
                PartnerLocationId = partnerLocationId,
                ServiceTypeId = 3,
                EventPartnerLocationServiceStatusId = 1,
            };

            var entity = dto.ToEntity();

            Assert.Equal(eventId, entity.EventId);
            Assert.Equal(partnerLocationId, entity.PartnerLocationId);
            Assert.Equal(3, entity.ServiceTypeId);
            Assert.Equal(1, entity.EventPartnerLocationServiceStatusId);
        }
    }
}
