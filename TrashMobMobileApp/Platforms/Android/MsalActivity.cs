namespace TrashMobMobileApp.Platforms.Android
{
    using global::Android.App;
    using global::Android.Content;
    using Microsoft.Identity.Client;

    namespace TrashMobMobileApp.Platforms.Android.Resources
    {
        [Activity(Exported = true)]
        [IntentFilter(new[] { Intent.ActionView },
            Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
            DataHost = "auth",
            DataScheme = "msal31cb1c9a-eaa6-4fd0-b59f-0bd0099845ee")]
        public class MsalActivity : BrowserTabActivity
        {
        }
    }
}