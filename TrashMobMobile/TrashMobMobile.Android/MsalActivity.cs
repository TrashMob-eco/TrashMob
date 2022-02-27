namespace TrashMobMobile.Droid
{
    using Android.App;
    using Android.Content;
    using Microsoft.Identity.Client;

    [Activity]
    [IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
    DataHost = "auth",
    DataScheme = "msal90c0fe63-bcf2-44d5-8fb7-b8bbc0b29dc6")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}