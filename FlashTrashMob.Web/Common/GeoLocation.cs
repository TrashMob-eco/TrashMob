namespace FlashTrashMob.Web.Common
{
    using System.Linq;
    using System.Xml.Linq;
    using FlashTrashMob.Web.Models;
    
    public static class GeoLocation
    {
        public static void SearchByPlaceNameOrZip(CleanupEvent cleanupEvent)
        {
            string url = "http://ws.geonames.org/postalCodeSearch?{0}={1}&maxRows=1&style=SHORT&username=cleanupEvent";
            bool isNumeric = int.TryParse(cleanupEvent.Address, out int n);
            url = string.Format(url, isNumeric ? "postalcode" : "placename", cleanupEvent.Address);

            var result = XDocument.Load(url);

            if (result.Descendants("code").Any())
            {
                var latLong = (from x in result.Descendants("code")
                               select new LatLong
                               {
                                   Lat = (float)x.Element("lat"),
                                   Long = (float)x.Element("lng")
                               }).First();

                cleanupEvent.Latitude = latLong.Lat;
                cleanupEvent.Longitude = latLong.Long;
            }
        }
    }

    public class LatLong
    {
        public float Lat { get; set; }
        public float Long { get; set; }
    }
}