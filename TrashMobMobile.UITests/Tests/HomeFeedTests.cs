namespace TrashMobMobile.UITests.Tests;

using TrashMobMobile.UITests.Setup;
using Xunit;

/// <summary>
/// Tests for the Home Feed page (authenticated).
/// Verifies personal stats, upcoming events, and litter reports load correctly.
/// </summary>
[Collection(TestCollection.Name)]
[Trait("Category", "Authenticated")]
public class HomeFeedTests : BaseTest
{
    public HomeFeedTests(AppiumFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void HomeFeed_DisplaysPersonalStatsBar()
    {
        TapElement("HomeTab");
        var statsBar = WaitForElement("PersonalStatsBar");
        Assert.True(statsBar.Displayed);
    }

    [Fact]
    public void HomeFeed_ShowsUpcomingEventsOrEmptyState()
    {
        TapElement("HomeTab");

        // Should show either the events list or the "no events" message
        try
        {
            var eventsList = WaitForElement("UpcomingEventsList", TimeSpan.FromSeconds(10));
            Assert.True(eventsList.Displayed);
        }
        catch
        {
            var noEvents = WaitForElement("NoEventsMessage", TimeSpan.FromSeconds(5));
            Assert.True(noEvents.Displayed);
        }
    }

    [Fact]
    public void HomeFeed_ShowsLitterReportsOrEmptyState()
    {
        TapElement("HomeTab");

        // Should show either the litter reports list or the "no reports" message
        try
        {
            var reportsList = WaitForElement("NearbyLitterReportsList", TimeSpan.FromSeconds(10));
            Assert.True(reportsList.Displayed);
        }
        catch
        {
            var noReports = WaitForElement("NoLitterReportsMessage", TimeSpan.FromSeconds(5));
            Assert.True(noReports.Displayed);
        }
    }

    [Fact]
    public void HomeFeed_WelcomeMessageContainsText()
    {
        TapElement("HomeTab");
        var welcomeMessage = WaitForElement("WelcomeMessage");
        Assert.True(welcomeMessage.Displayed);
        Assert.StartsWith("Hi,", welcomeMessage.Text);
    }
}
