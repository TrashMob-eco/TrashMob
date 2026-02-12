namespace TrashMobMobile.UITests.Setup;

using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using Xunit;

[Collection(TestCollection.Name)]
public abstract class BaseTest
{
    private static readonly string ScreenshotDir = Path.Combine(
        AppContext.BaseDirectory, "TestResults", "Screenshots");

    protected AndroidDriver Driver { get; }

    protected BaseTest(AppiumFixture fixture)
    {
        Driver = fixture.Driver;
    }

    protected AppiumElement FindByAutomationId(string automationId)
    {
        return Driver.FindElement(MobileBy.AccessibilityId(automationId));
    }

    protected AppiumElement WaitForElement(string automationId, TimeSpan? timeout = null)
    {
        var wait = new WebDriverWait(Driver, timeout ?? TimeSpan.FromSeconds(15));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));

        return (AppiumElement)wait.Until(driver =>
            driver.FindElement(MobileBy.AccessibilityId(automationId)));
    }

    protected void TapElement(string automationId)
    {
        var element = WaitForElement(automationId);
        element.Click();
    }

    protected string CaptureScreenshot(string name)
    {
        Directory.CreateDirectory(ScreenshotDir);
        var filePath = Path.Combine(ScreenshotDir, $"{name}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.png");
        var screenshot = Driver.GetScreenshot();
        screenshot.SaveAsFile(filePath);
        return filePath;
    }
}
