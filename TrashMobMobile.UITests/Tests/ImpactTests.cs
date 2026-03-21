namespace TrashMobMobile.UITests.Tests;

using TrashMobMobile.UITests.Setup;
using Xunit;

/// <summary>
/// Tests for the Impact page (authenticated).
/// Verifies personal stats and navigation buttons load correctly.
/// </summary>
[Collection(TestCollection.Name)]
[Trait("Category", "Authenticated")]
public class ImpactTests : BaseTest
{
    public ImpactTests(AppiumFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void Impact_DisplaysStatsSection()
    {
        TapElement("ImpactTab");
        var statsSection = WaitForElement("ImpactStatsSection", TimeSpan.FromSeconds(15));
        Assert.True(statsSection.Displayed);

        TapElement("HomeTab");
    }

    [Fact]
    public void Impact_LeaderboardsButtonVisible()
    {
        TapElement("ImpactTab");
        var leaderboardsBtn = WaitForElement("LeaderboardsButton", TimeSpan.FromSeconds(15));
        Assert.True(leaderboardsBtn.Displayed);

        TapElement("HomeTab");
    }

    [Fact]
    public void Impact_AchievementsButtonVisible()
    {
        TapElement("ImpactTab");
        var achievementsBtn = WaitForElement("AchievementsButton", TimeSpan.FromSeconds(15));
        Assert.True(achievementsBtn.Displayed);

        TapElement("HomeTab");
    }
}
