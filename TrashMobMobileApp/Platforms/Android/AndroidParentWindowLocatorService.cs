using Plugin.CurrentActivity;
using TrashMobMobileApp.Authentication;

namespace TrashMobMobileApp.Platforms.Android
{
    public class AndroidParentWindowLocatorService : IParentWindowLocatorService
    {
        public object GetCurrentParentWindow()
        {
            return CrossCurrentActivity.Current.Activity;
        }
    }
}
