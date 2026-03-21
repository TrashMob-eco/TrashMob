namespace TrashMobMobile.UITests.Tests;

using TrashMobMobile.UITests.Setup;
using Xunit;

/// <summary>
/// Tests for the Quick Action tab and sign-out flow (authenticated).
/// </summary>
[Collection(TestCollection.Name)]
[Trait("Category", "Authenticated")]
public class QuickActionTests : BaseTest
{
    public QuickActionTests(AppiumFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void QuickAction_TabIsAccessible()
    {
        TapElement("QuickActionTab");

        // The quick action tab shows a popup — give it time to appear
        Thread.Sleep(2000);

        // Dismiss by navigating back (popup auto-dismisses or we go back)
        Driver.Navigate().Back();
        Thread.Sleep(500);

        // Verify we can get back to home
        TapElement("HomeTab");
        var welcomeMessage = WaitForElement("WelcomeMessage");
        Assert.True(welcomeMessage.Displayed);
    }
}
