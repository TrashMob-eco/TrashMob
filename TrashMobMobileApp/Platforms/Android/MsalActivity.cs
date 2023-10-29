using Android.App;
using Android.Content;
using Microsoft.Identity.Client;


namespace TrashMobMobileApp.Platforms.Android;

[Activity(Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = "eco.trashmob.trashmobmobileapp")]
public class MsalActivity : BrowserTabActivity
{
}
