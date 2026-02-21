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
        // Welcome page is the first screen for unauthenticated users
        WaitForElement("WelcomeLogo", TimeSpan.FromSeconds(30));

        // Wait for animations to settle
        Thread.Sleep(1000);

        CaptureScreenshot("01_Welcome");
    }

    [Fact]
    public void Screenshot_02_GetStarted()
    {
        // Show the Get Started button prominently
        WaitForElement("GetStartedButton", TimeSpan.FromSeconds(15));
        Thread.Sleep(500);

        CaptureScreenshot("02_GetStarted");
    }

    // The following tests require authentication (Category=Authenticated).
    // They will be skipped if the app is not logged in.
    // To capture these, set up a test account and log in before running.

    [Fact]
    [Trait("Category", "Authenticated")]
    public void Screenshot_03_EventsMap()
    {
        // Main events map showing nearby events
        try
        {
            WaitForElement("EventsMap", TimeSpan.FromSeconds(15));
            Thread.Sleep(2000); // Wait for map tiles to load

            CaptureScreenshot("03_EventsMap");
        }
        catch
        {
            // If not authenticated, capture whatever is on screen
            CaptureScreenshot("03_EventsMap_NoAuth");
        }
    }

    [Fact]
    [Trait("Category", "Authenticated")]
    public void Screenshot_04_Dashboard()
    {
        try
        {
            // Navigate to Dashboard tab
            TapElement("DashboardTab");
            Thread.Sleep(1500);

            CaptureScreenshot("04_Dashboard");
        }
        catch
        {
            CaptureScreenshot("04_Dashboard_NoAuth");
        }
    }

    [Fact]
    [Trait("Category", "Authenticated")]
    public void Screenshot_05_Teams()
    {
        try
        {
            // Navigate to Teams tab
            TapElement("TeamsTab");
            Thread.Sleep(1500);

            CaptureScreenshot("05_Teams");
        }
        catch
        {
            CaptureScreenshot("05_Teams_NoAuth");
        }
    }

    [Fact]
    [Trait("Category", "Authenticated")]
    public void Screenshot_06_CreateEvent()
    {
        try
        {
            // Navigate to Create Event
            TapElement("CreateEventButton");
            Thread.Sleep(1500);

            CaptureScreenshot("06_CreateEvent");

            // Go back
            Driver.Navigate().Back();
        }
        catch
        {
            CaptureScreenshot("06_CreateEvent_NoAuth");
        }
    }
}
