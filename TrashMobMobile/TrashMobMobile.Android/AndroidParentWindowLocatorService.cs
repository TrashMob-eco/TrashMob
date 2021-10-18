namespace TrashMobMobile.Droid
{
    using Plugin.CurrentActivity;
    using TrashMobMobile.Features.LogOn;

    class AndroidParentWindowLocatorService : IParentWindowLocatorService
    {
        public object GetCurrentParentWindow()
        {
            return CrossCurrentActivity.Current.Activity;
        }
    }
}