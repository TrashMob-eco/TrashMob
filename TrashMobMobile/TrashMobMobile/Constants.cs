using Xamarin.Essentials;
using Xamarin.Forms;


namespace TrashMobMobile
{
    public static class Constants
    {
        public static string MobEventsDataUrl = DeviceInfo.Platform == DevicePlatform.Android ? "https://www.trashmob.eco/api/events" : null;
    }
}
