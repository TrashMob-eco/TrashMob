namespace TrashMobMobile.Platforms.Android;

using global::Android.Content;
using global::Android.App;
using Microsoft.Identity.Client;

[Activity(Exported = true)]
[IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = "eco.trashmob.trashmobmobile")]
public class MsalActivity : BrowserTabActivity
{
}