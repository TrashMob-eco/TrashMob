namespace TrashMobMobile.UITests.Tests;

using TrashMobMobile.UITests.Setup;
using Xunit;

/// <summary>
/// Captures screenshots of key app screens for store listings.
/// Run via: dotnet test --filter "Category=Screenshots"
/// Screenshots are saved to TestResults/Screenshots/.
/// </summary>
[Collection(TestCollection.Name)]
[Trait("Category", "Screenshots")]
public class ScreenshotTests : BaseTest
{
    public ScreenshotTests(AppiumFixture fixture) : base(fixture)
    {
    }

    [Fact]
    public void Screenshot_01_WelcomePage()
    {
        WaitForElement("WelcomeLogo", TimeSpan.FromSeconds(30));
        Thread.Sleep(1000);
        CaptureScreenshot("01_Welcome");
    }

    [Fact]
    public void Screenshot_02_SignIn()
    {
        WaitForElement("SignInButton", TimeSpan.FromSeconds(15));
        Thread.Sleep(500);
        CaptureScreenshot("02_SignIn");
    }

    [Fact]
    [Trait("Category", "Authenticated")]
    public void Screenshot_03_HomeFeed()
    {
        try
        {
            TapElement("HomeTab");
            WaitForElement("WelcomeMessage", TimeSpan.FromSeconds(15));
            Thread.Sleep(1500);
            CaptureScreenshot("03_HomeFeed");
        }
        catch
        {
            CaptureScreenshot("03_HomeFeed_NoAuth");
        }
    }

    [Fact]
    [Trait("Category", "Authenticated")]
    public void Screenshot_04_ExploreMap()
    {
        try
        {
            TapElement("ExploreTab");
            WaitForElement("EventsMap", TimeSpan.FromSeconds(15));
            Thread.Sleep(2000);
            CaptureScreenshot("04_ExploreMap");

            TapElement("HomeTab");
        }
        catch
        {
            CaptureScreenshot("04_ExploreMap_NoAuth");
        }
    }

    [Fact]
    [Trait("Category", "Authenticated")]
    public void Screenshot_05_Impact()
    {
        try
        {
            TapElement("ImpactTab");
            Thread.Sleep(1500);
            CaptureScreenshot("05_Impact");

            TapElement("HomeTab");
        }
        catch
        {
            CaptureScreenshot("05_Impact_NoAuth");
        }
    }

    [Fact]
    [Trait("Category", "Authenticated")]
    public void Screenshot_06_Profile()
    {
        try
        {
            TapElement("ProfileTab");
            WaitForElement("UserNameLabel", TimeSpan.FromSeconds(15));
            Thread.Sleep(1000);
            CaptureScreenshot("06_Profile");

            TapElement("HomeTab");
        }
        catch
        {
            CaptureScreenshot("06_Profile_NoAuth");
        }
    }
}
