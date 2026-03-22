namespace TrashMobMobile.UITests.Setup;

using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;

/// <summary>
/// Automates Entra External ID (CIAM) login via MSAL's Chrome Custom Tab.
/// Used by AppiumFixture to authenticate before running tests in CI.
/// Requires APPIUM_TEST_EMAIL and APPIUM_TEST_PASSWORD environment variables.
/// </summary>
public static class LoginHelper
{
    private static readonly TimeSpan WebViewTimeout = TimeSpan.FromSeconds(30);
    private static readonly TimeSpan ElementTimeout = TimeSpan.FromSeconds(15);

    public static bool HasCredentials =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPIUM_TEST_EMAIL")) &&
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPIUM_TEST_PASSWORD"));

    public static void Login(AndroidDriver driver)
    {
        var email = Environment.GetEnvironmentVariable("APPIUM_TEST_EMAIL")!;
        var password = Environment.GetEnvironmentVariable("APPIUM_TEST_PASSWORD")!;

        Console.WriteLine("LoginHelper: Starting MSAL login flow...");

        try
        {
            LoginInternal(driver, email, password);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LoginHelper: Login failed — {ex.Message}");
            Console.WriteLine("LoginHelper: Tests will run in unauthenticated mode.");

            // Ensure we're back in native context
            try { driver.Context = "NATIVE_APP"; } catch { /* already native */ }
        }
    }

    private static void LoginInternal(AndroidDriver driver, string email, string password)
    {
        // Check if we're already logged in (MainTabsPage visible)
        try
        {
            driver.FindElement(MobileBy.AccessibilityId("HomeTab"));
            Console.WriteLine("LoginHelper: Already logged in, skipping.");
            return;
        }
        catch (NoSuchElementException)
        {
            // Not logged in — proceed with login
        }

        // Tap Sign In button on WelcomePage
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(30));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        var signInButton = (AppiumElement)wait.Until(d =>
            d.FindElement(MobileBy.AccessibilityId("SignInButton")));
        signInButton.Click();
        Console.WriteLine("LoginHelper: Tapped Sign In button.");

        // Wait for MSAL browser to appear and switch to WEBVIEW context
        Thread.Sleep(3000); // Allow Chrome Custom Tab to load

        var webviewContext = WaitForWebViewContext(driver);
        if (webviewContext == null)
        {
            Console.WriteLine("LoginHelper: No WEBVIEW context found. MSAL may not have a browser available.");
            // Switch back to native just in case
            driver.Context = "NATIVE_APP";
            return;
        }

        driver.Context = webviewContext;
        Console.WriteLine($"LoginHelper: Switched to context: {webviewContext}");

        try
        {
            // Entra External ID login form — enter email
            var emailField = WaitForWebElement(driver, By.CssSelector("input[type='email'], input[name='loginfmt'], #email"));
            emailField.Clear();
            emailField.SendKeys(email);
            Console.WriteLine("LoginHelper: Entered email.");

            // Click Next/Submit after email
            ClickSubmitButton(driver);
            Console.WriteLine("LoginHelper: Clicked Next after email.");

            Thread.Sleep(2000); // Wait for password page to load

            // Enter password
            var passwordField = WaitForWebElement(driver, By.CssSelector("input[type='password'], input[name='passwd'], #password"));
            passwordField.Clear();
            passwordField.SendKeys(password);
            Console.WriteLine("LoginHelper: Entered password.");

            // Click Sign In
            ClickSubmitButton(driver);
            Console.WriteLine("LoginHelper: Clicked Sign In.");

            // Wait for redirect back to app (MSAL callback closes the browser)
            Thread.Sleep(5000);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"LoginHelper: Error during web login: {ex.Message}");
        }
        finally
        {
            // Always switch back to native app context
            try
            {
                driver.Context = "NATIVE_APP";
            }
            catch
            {
                // Context may already be native if browser closed
            }
        }

        // Wait for MainTabsPage to appear (confirms successful login)
        try
        {
            Console.WriteLine("LoginHelper: Waiting for MainTabsPage...");
            var homeTab = (AppiumElement)wait.Until(d =>
                d.FindElement(MobileBy.AccessibilityId("HomeTab")));
            Console.WriteLine("LoginHelper: Login successful — MainTabsPage visible.");
        }
        catch (WebDriverTimeoutException)
        {
            Console.WriteLine("LoginHelper: MainTabsPage did not appear after login. Tests may fail.");
        }
    }

    private static string? WaitForWebViewContext(AndroidDriver driver)
    {
        var deadline = DateTime.UtcNow + WebViewTimeout;
        while (DateTime.UtcNow < deadline)
        {
            var contexts = driver.Contexts;
            foreach (var context in contexts)
            {
                if (context.Contains("WEBVIEW"))
                {
                    return context;
                }
            }

            Thread.Sleep(1000);
        }

        return null;
    }

    private static IWebElement WaitForWebElement(AndroidDriver driver, By locator)
    {
        var wait = new WebDriverWait(driver, ElementTimeout);
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait.Until(d => d.FindElement(locator));
    }

    private static void ClickSubmitButton(AndroidDriver driver)
    {
        // Entra login forms use various submit button patterns
        var selectors = new[]
        {
            By.CssSelector("input[type='submit']"),
            By.CssSelector("button[type='submit']"),
            By.CssSelector("#idSIButton9"),          // Microsoft "Next" button
            By.CssSelector("#idSIButton4"),           // Microsoft "Sign in" button
            By.CssSelector(".win-button.button_primary"),
        };

        foreach (var selector in selectors)
        {
            try
            {
                var button = driver.FindElement(selector);
                if (button.Displayed)
                {
                    button.Click();
                    return;
                }
            }
            catch (NoSuchElementException)
            {
                continue;
            }
        }

        // Fallback: try pressing Enter
        driver.FindElement(By.CssSelector("input:focus")).SendKeys(Keys.Enter);
    }
}
