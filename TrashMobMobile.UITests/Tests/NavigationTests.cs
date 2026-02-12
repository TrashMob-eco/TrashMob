namespace TrashMobMobile.UITests.Tests;

using TrashMobMobile.UITests.Setup;
using Xunit;

/// <summary>
/// Navigation tests that require an authenticated session.
/// Run these after logging in at least once with noReset=true so the session persists.
/// </summary>
[Collection(TestCollection.Name)]
[Trait("Category", "Authenticated")]
public class NavigationTests : BaseTest
{
    public NavigationTests(AppiumFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void HomeTab_DisplaysWelcomeMessage()
    {
        TapElement("HomeTab");
        var welcomeMessage = WaitForElement("WelcomeMessage");
        Assert.True(welcomeMessage.Displayed);
    }

    [Fact]
    public void TabNavigation_ExploreTabAccessible()
    {
        TapElement("ExploreTab");

        // Give the page time to load; verify we didn't crash
        Thread.Sleep(2000);

        // Navigate back to home to reset state
        TapElement("HomeTab");
        var welcomeMessage = WaitForElement("WelcomeMessage");
        Assert.True(welcomeMessage.Displayed);
    }

    [Fact]
    public void TabNavigation_ImpactTabAccessible()
    {
        TapElement("ImpactTab");

        // Give the page time to load; verify we didn't crash
        Thread.Sleep(2000);

        // Navigate back to home to reset state
        TapElement("HomeTab");
        var welcomeMessage = WaitForElement("WelcomeMessage");
        Assert.True(welcomeMessage.Displayed);
    }

    [Fact]
    public void ProfileTab_ShowsUserName()
    {
        TapElement("ProfileTab");
        var userNameLabel = WaitForElement("UserNameLabel");
        Assert.True(userNameLabel.Displayed);
        Assert.False(string.IsNullOrWhiteSpace(userNameLabel.Text));
    }

    [Fact]
    public void ProfileTab_ShowsSignOutButton()
    {
        TapElement("ProfileTab");
        var signOutButton = WaitForElement("SignOutButton");
        Assert.True(signOutButton.Displayed);
    }
}
