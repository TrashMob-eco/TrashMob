namespace TrashMobMobile.UITests.Tests;

using TrashMobMobile.UITests.Setup;
using Xunit;

[Collection(TestCollection.Name)]
public class AppLaunchTests : BaseTest
{
    public AppLaunchTests(AppiumFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void AppLaunches_WelcomePageDisplayed()
    {
        // The app should launch and show either the WelcomePage (unauthenticated)
        // or the MainTabsPage (authenticated). Either means the app launched successfully.
        var welcomeLogo = WaitForElement("WelcomeLogo", TimeSpan.FromSeconds(30));
        Assert.True(welcomeLogo.Displayed);
    }

    [Fact]
    public void WelcomePage_SignInButtonVisible()
    {
        var signInButton = WaitForElement("SignInButton", TimeSpan.FromSeconds(30));
        Assert.True(signInButton.Displayed);
    }
}
