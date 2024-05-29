namespace TrashMob.Shared.Tests
{
    using System.Text.Json;
    using AzureMapsToolkit.Spatial;
    using Xunit;

    public class JsonTest
    {
        [Fact]
        public void DeserializeJson_Succeeds()
        {
            var json =
                "{ \"HttpResponseCode\":0,\"Result\":{ \"result\":{ \"distanceInMeters\":7717.44},\"summary\":{ \"information\":null,\"udid\":null},\"Udid\":\"00000000-0000-0000-0000-000000000000\",\"ApiVersion\":\"v1\"},\"Error\":null}";

            var response = (AzureMapsToolkit.Response<GreatCircleDistanceResponse>)JsonSerializer.Deserialize(json,
                typeof(AzureMapsToolkit.Response<GreatCircleDistanceResponse>));

            Assert.NotNull(response);
            Assert.NotNull(response.Result);
            Assert.NotNull(response.Result.Result);
            Assert.Equal(7717.44, response.Result.Result.DistanceInMeters);
        }
    }
}