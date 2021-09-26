namespace TrashMob.Shared.Tests
{
    using AzureMapsToolkit.Timezone;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Xunit;

    public class MapRepositoryTest  
    {
        //[Fact]
        //public async Task GetTimeForPoint_Succeeds()
        //{
        //    var azureMaps = new AzureMapsToolkit.AzureMapsServices("8B4iW2WzR4uIsN5H4klEXzSBbt3Ufqp7gfJ85ykoQ7U");

        //    var checkDate = DateTimeOffset.Now;
        //    var date = checkDate.AddHours(6).UtcDateTime;

        //    Tuple<double, double> pointA = new(47.54435319655616, -122.02045895718422);
        //    var timezoneRequest = new TimeZoneRequest
        //    {
        //        Query = $"{pointA.Item1},{pointA.Item2}",
        //        TimeStamp = date.ToString("O")
        //    };

        //    var response = await azureMaps.GetTimezoneByCoordinates(timezoneRequest).ConfigureAwait(false);

        //    if (response.HttpResponseCode != (int)HttpStatusCode.OK && response.HttpResponseCode != 0)
        //    {
        //        throw new Exception($"Error getting timezonebycoordinates: {response}");
        //    }

        //    Assert.Equal(checkDate.ToString("O"), response?.Result?.TimeZones[0]?.ReferenceTime?.WallTime);
        //}
    }
}

