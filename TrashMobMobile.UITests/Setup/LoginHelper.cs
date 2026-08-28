namespace TrashMobMobile.UITests.Setup;

using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;

/// <summary>
/// Automates Entra External ID (CIAM) login via MSAL browser flow.
/// Uses native UiAutomator2 selectors to interact with the browser login page
/// (works with any browser — Chrome, AOSP Browser, WebView).
/// Requires APPIUM_TEST_EMAIL and APPIUM_TEST_PASSWORD environment variables.
/// </summary>
public static class LoginHelper
{
    private static readonly string DiagFile = Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "TestResults", "login-diagnostics.txt");

    public static bool HasCredentials =>
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPIUM_TEST_EMAIL")) &&
        !string.IsNullOrEmpty(Environment.GetEnvironmentVariable("APPIUM_TEST_PASSWORD"));

    public static void Login(AndroidDriver driver)
    {
        var email = Environment.GetEnvironmentVariable("APPIUM_TEST_EMAIL")!;
        var password = Environment.GetEnvironmentVariable("APPIUM_TEST_PASSWORD")!;

        try
        {
            LoginInternal(driver, email, password);
        }
        catch (Exception ex)
        {
            Log($"Login failed: {ex.Message}");
            Log("Tests will run in unauthenticated mode.");

            // Try to get back to the app
            try { driver.Context = "NATIVE_APP"; } catch { /* already native */ }
            try { driver.ActivateApp("eco.trashmob.trashmobmobileapp"); } catch { /* best effort */ }
        }
    }

    private static void LoginInternal(AndroidDriver driver, string email, string password)
    {
        Log("Starting MSAL login flow...");

        // Poll for app state — WelcomePage or already authenticated
        Log("Waiting for app to be ready...");
        var deadline = DateTime.UtcNow.AddMinutes(2);
        var appState = "unknown";
        var attempt = 0;
        while (DateTime.UtcNow < deadline)
        {
            attempt++;
            if (IsElementPresent(driver, MobileBy.AccessibilityId("HomeTab")))
            {
                appState = "authenticated";
                break;
            }
            if (IsElementPresent(driver, MobileBy.AccessibilityId("WelcomeLogo"))
                || IsElementPresent(driver, MobileBy.AccessibilityId("SignInButton")))
            {
                appState = "welcome";
                break;
            }

            // Dump page source on first and every 10th attempt for diagnostics
            if (attempt == 1 || attempt % 10 == 0)
            {
                try
                {
                    var source = driver.PageSource;
                    // Log first 500 chars of page source
                    Log($"Attempt {attempt} page source ({source.Length} chars): {source[..Math.Min(2000, source.Length)]}");
                }
                catch (Exception ex)
                {
                    Log($"Attempt {attempt} page source error: {ex.Message}");
                }
            }

            Thread.Sleep(2000);
        }

        Log($"App state after {attempt} attempts: {appState}");

        if (appState == "authenticated")
        {
            Log("Already logged in, skipping.");
            return;
        }

        if (appState == "unknown")
        {
            Log("App did not reach a known state within 2 minutes.");
            return;
        }

        // Find Sign In button — may need to scroll
        AppiumElement? signInButton = null;
        for (var scrollAttempt = 0; scrollAttempt < 3; scrollAttempt++)
        {
            if (IsElementPresent(driver, MobileBy.AccessibilityId("SignInButton")))
            {
                signInButton = (AppiumElement)driver.FindElement(MobileBy.AccessibilityId("SignInButton"));
                if (signInButton.Displayed)
                    break;
            }

            Log($"SignInButton not visible, scrolling down (attempt {scrollAttempt + 1})...");
            try
            {
                driver.ExecuteScript("mobile: scrollGesture", new Dictionary<string, object>
                {
                    { "left", 100 }, { "top", 500 }, { "width", 400 }, { "height", 800 },
                    { "direction", "down" }, { "percent", 0.75 }
                });
            }
            catch { /* scroll failed — try without */ }
            Thread.Sleep(500);
        }

        if (signInButton == null || !signInButton.Displayed)
        {
            Log("SignInButton not found after scrolling.");
            return;
        }

        signInButton.Click();
        Log("Tapped Sign In button.");

        // Dump available contexts and packages for diagnostics
        Thread.Sleep(3000);
        try
        {
            var contexts = driver.Contexts;
            Log($"Available contexts: {string.Join(", ", contexts)}");
            var currentActivity = driver.CurrentActivity;
            Log($"Current activity: {currentActivity}");
        }
        catch (Exception ex)
        {
            Log($"Diagnostics error: {ex.Message}");
        }

        // Strategy 1: Try WEBVIEW context (works if Chrome is installed)
        if (TryWebViewLogin(driver, email, password))
        {
            return;
        }

        // Strategy 2: Use native UiAutomator2 selectors (works with any browser)
        Log("Trying native UiAutomator2 login...");
        TryNativeLogin(driver, email, password);

        // Check if login succeeded
        driver.Context = "NATIVE_APP";
        Thread.Sleep(3000);

        // Try to activate our app in case browser is still in foreground
        try { driver.ActivateApp("eco.trashmob.trashmobmobileapp"); } catch { /* best effort */ }
        Thread.Sleep(2000);

        if (IsElementPresent(driver, MobileBy.AccessibilityId("HomeTab")))
        {
            Log("Login successful — MainTabsPage visible.");
        }
        else
        {
            Log("MainTabsPage not visible after login attempt.");
        }
    }

    private static bool TryWebViewLogin(AndroidDriver driver, string email, string password)
    {
        var webviewContext = WaitForContext(driver, "WEBVIEW", TimeSpan.FromSeconds(10));
        if (webviewContext == null)
        {
            Log("No WEBVIEW context available.");
            return false;
        }

        try
        {
            driver.Context = webviewContext;
            Log($"Switched to {webviewContext}");

            // Enter email
            var emailField = WaitForWebElement(driver, By.CssSelector(
                "input[type='email'], input[name='loginfmt'], #email, input[name='email']"));
            emailField.Clear();
            emailField.SendKeys(email);
            Log("Entered email (WEBVIEW).");

            ClickWebSubmitButton(driver);
            Thread.Sleep(2000);

            // Enter password
            var passwordField = WaitForWebElement(driver, By.CssSelector(
                "input[type='password'], input[name='passwd'], #password"));
            passwordField.Clear();
            passwordField.SendKeys(password);
            Log("Entered password (WEBVIEW).");

            ClickWebSubmitButton(driver);
            Thread.Sleep(5000);

            driver.Context = "NATIVE_APP";
            return true;
        }
        catch (Exception ex)
        {
            Log($"WEBVIEW login failed: {ex.Message}");
            try { driver.Context = "NATIVE_APP"; } catch { /* already native */ }
            return false;
        }
    }

    private static void TryNativeLogin(AndroidDriver driver, string email, string password)
    {
        // Ensure we're in native context
        try { driver.Context = "NATIVE_APP"; } catch { /* already native */ }

        try
        {
            // Find any EditText on screen (the email field in the browser login form)
            var editTexts = driver.FindElements(By.ClassName("android.widget.EditText"));
            Log($"Found {editTexts.Count} EditText elements on screen.");

            if (editTexts.Count == 0)
            {
                Log("No EditText found — browser login form may not be visible.");
                return;
            }

            // Type email into the first visible EditText
            var emailField = editTexts.FirstOrDefault(e => e.Displayed);
            if (emailField == null)
            {
                Log("No visible EditText found.");
                return;
            }

            emailField.Click();
            emailField.Clear();
            emailField.SendKeys(email);
            Log("Entered email (native).");

            // Click any visible Button (Next/Submit)
            ClickNativeButton(driver);
            Thread.Sleep(3000);

            // Find password field
            editTexts = driver.FindElements(By.ClassName("android.widget.EditText"));
            var passwordField = editTexts.FirstOrDefault(e => e.Displayed);
            if (passwordField != null)
            {
                passwordField.Click();
                passwordField.Clear();
                passwordField.SendKeys(password);
                Log("Entered password (native).");

                ClickNativeButton(driver);
                Thread.Sleep(5000);
            }
            else
            {
                Log("No password field found.");
            }
        }
        catch (Exception ex)
        {
            Log($"Native login error: {ex.Message}");
        }
    }

    private static void ClickNativeButton(AndroidDriver driver)
    {
        var buttons = driver.FindElements(By.ClassName("android.widget.Button"));
        var submitButton = buttons.LastOrDefault(b => b.Displayed);
        if (submitButton != null)
        {
            Log($"Clicking button: '{submitButton.Text}'");
            submitButton.Click();
        }
        else
        {
            // Fallback: press Enter key
            Log("No button found, pressing Enter.");
            driver.PressKeyCode(66); // KEYCODE_ENTER
        }
    }

    private static string? WaitForContext(AndroidDriver driver, string contextPrefix, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            foreach (var context in driver.Contexts)
            {
                if (context.Contains(contextPrefix))
                    return context;
            }
            Thread.Sleep(1000);
        }
        return null;
    }

    private static IWebElement WaitForWebElement(AndroidDriver driver, By locator)
    {
        var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(15));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        return wait.Until(d => d.FindElement(locator));
    }

    private static void ClickWebSubmitButton(AndroidDriver driver)
    {
        var selectors = new[]
        {
            By.CssSelector("input[type='submit']"),
            By.CssSelector("button[type='submit']"),
            By.CssSelector("#idSIButton9"),
            By.CssSelector("#idSIButton4"),
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
            catch (NoSuchElementException) { continue; }
        }

        // Fallback: Enter key
        try { driver.FindElement(By.CssSelector("input:focus")).SendKeys(Keys.Enter); } catch { /* best effort */ }
    }

    private static bool IsElementPresent(AndroidDriver driver, By locator)
    {
        try
        {
            // Temporarily disable implicit wait for fast polling
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.Zero;
            driver.FindElement(locator);
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }
    }

    private static void Log(string message)
    {
        var line = $"[LoginHelper {DateTime.UtcNow:HH:mm:ss}] {message}";
        Console.WriteLine(line);

        // Also write to file — xUnit swallows Console.WriteLine from fixtures
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(DiagFile)!);
            File.AppendAllText(DiagFile, line + Environment.NewLine);
        }
        catch { /* best effort */ }
    }
}
