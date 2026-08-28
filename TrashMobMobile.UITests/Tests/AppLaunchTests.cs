namespace TrashMobMobile.UITests.Tests;

using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using TrashMobMobile.UITests.Setup;
using Xunit;

[Collection(TestCollection.Name)]
public class AppLaunchTests : BaseTest
{
    public AppLaunchTests(AppiumFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void AppLaunches_PageDisplayed()
    {
        // The app should show either the WelcomePage (unauthenticated)
        // or the MainTabsPage (authenticated). Either means the app launched successfully.
        try
        {
            var homeTab = WaitForElement("HomeTab", TimeSpan.FromSeconds(5));
            Assert.True(homeTab.Displayed);
        }
        catch
        {
            var welcomeLogo = WaitForElement("WelcomeLogo", TimeSpan.FromSeconds(30));
            Assert.True(welcomeLogo.Displayed);
        }
    }

    [Fact]
    public void WelcomePage_SignInButtonVisible()
    {
        // Skip if already authenticated (no WelcomePage to test)
        try
        {
            Driver.FindElement(MobileBy.AccessibilityId("HomeTab"));
            // Already logged in — test is not applicable
            return;
        }
        catch (NoSuchElementException)
        {
            // Not logged in — proceed with test
        }

        var signInButton = WaitForElement("SignInButton", TimeSpan.FromSeconds(30));
        Assert.True(signInButton.Displayed);
    }
}
