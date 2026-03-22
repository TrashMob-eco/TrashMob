namespace TrashMobMobile.UITests.Setup;

using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using Xunit;

public class AppiumFixture : IAsyncLifetime
{
    private const string DefaultAppiumServer = "http://127.0.0.1:4723";
    private const string DefaultDeviceName = "emulator-5554";
    private const string AppPackage = "eco.trashmob.trashmobmobileapp";
    private const string AppActivity = $"{AppPackage}.MainActivity";

    public AndroidDriver Driver { get; private set; } = null!;

    public Task InitializeAsync()
    {
        var serverUrl = Environment.GetEnvironmentVariable("APPIUM_SERVER") ?? DefaultAppiumServer;
        var apkPath = Environment.GetEnvironmentVariable("TRASHMOB_APK_PATH");
        var deviceName = Environment.GetEnvironmentVariable("ANDROID_DEVICE") ?? DefaultDeviceName;

        var options = new AppiumOptions
        {
            PlatformName = "Android",
            AutomationName = "UiAutomator2",
            DeviceName = deviceName,
        };

        options.AddAdditionalAppiumOption("appPackage", AppPackage);
        options.AddAdditionalAppiumOption("appActivity", AppActivity);
        options.AddAdditionalAppiumOption("noReset", true);
        options.AddAdditionalAppiumOption("uiautomator2ServerInstallTimeout", 120000);
        options.AddAdditionalAppiumOption("uiautomator2ServerLaunchTimeout", 120000);
        options.AddAdditionalAppiumOption("appWaitForLaunch", false);
        options.AddAdditionalAppiumOption("adbExecTimeout", 180000);
        options.AddAdditionalAppiumOption("androidInstallTimeout", 180000);
        options.AddAdditionalAppiumOption("newCommandTimeout", 300);

        if (!string.IsNullOrEmpty(apkPath))
        {
            options.App = apkPath;
        }

        Driver = new AndroidDriver(new Uri(serverUrl), options, TimeSpan.FromMinutes(5));
        Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        Driver?.Quit();
        return Task.CompletedTask;
    }
}
