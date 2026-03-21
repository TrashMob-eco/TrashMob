namespace TrashMobMobile.UITests.Tests;

using TrashMobMobile.UITests.Setup;
using Xunit;

/// <summary>
/// Tests for the Explore page (authenticated).
/// Verifies map and list views load and toggle correctly.
/// </summary>
[Collection(TestCollection.Name)]
[Trait("Category", "Authenticated")]
public class ExploreTests : BaseTest
{
    public ExploreTests(AppiumFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void Explore_MapViewDisplayed()
    {
        TapElement("ExploreTab");
        var map = WaitForElement("EventsMap", TimeSpan.FromSeconds(15));
        Assert.True(map.Displayed);

        TapElement("HomeTab");
    }

    [Fact]
    public void Explore_CanSwitchToListView()
    {
        TapElement("ExploreTab");
        WaitForElement("EventsMap", TimeSpan.FromSeconds(15));

        TapElement("ExploreListToggle");
        Thread.Sleep(1000);

        var listView = WaitForElement("ExploreListView", TimeSpan.FromSeconds(10));
        Assert.True(listView.Displayed);

        // Switch back to map
        TapElement("ExploreMapToggle");
        TapElement("HomeTab");
    }

    [Fact]
    public void Explore_MapAndListToggleButtonsVisible()
    {
        TapElement("ExploreTab");
        WaitForElement("EventsMap", TimeSpan.FromSeconds(15));

        var mapToggle = WaitForElement("ExploreMapToggle");
        var listToggle = WaitForElement("ExploreListToggle");
        Assert.True(mapToggle.Displayed);
        Assert.True(listToggle.Displayed);

        TapElement("HomeTab");
    }
}
