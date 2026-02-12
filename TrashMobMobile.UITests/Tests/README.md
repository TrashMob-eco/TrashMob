# TrashMobMobile UI Tests

Automated UI tests for the TrashMob mobile app using Appium + xUnit.

## Prerequisites

1. **Appium 2.x** server installed globally:
   ```bash
   npm install -g appium
   ```

2. **UIAutomator2 driver** for Android:
   ```bash
   appium driver install uiautomator2
   ```

3. **Android emulator** running (or a physical device connected via ADB):
   ```bash
   # Start an emulator (example using existing AVD)
   emulator -avd pixel_8_api_35
   ```

4. **APK** built from the TrashMobMobile project:
   ```bash
   dotnet build TrashMobMobile -f net10.0-android -p:AndroidSdkDirectory=D:\Android\android-sdk
   ```

## Environment Variables

| Variable | Default | Description |
|----------|---------|-------------|
| `APPIUM_SERVER` | `http://127.0.0.1:4723` | Appium server URL |
| `TRASHMOB_APK_PATH` | *(none)* | Path to the APK file. If not set, the app must already be installed on the device. |
| `ANDROID_DEVICE` | `emulator-5554` | Device name or serial number |

## Running Tests

### 1. Start the Appium server

```bash
appium
```

### 2. Start the Android emulator

```bash
emulator -avd pixel_8_api_35
```

### 3. Run all tests

```bash
dotnet test TrashMobMobile.UITests
```

### 4. Run only unauthenticated tests (no login required)

```bash
dotnet test TrashMobMobile.UITests --filter "Category!=Authenticated"
```

### 5. Run authenticated tests

These tests require the app to have an active authenticated session. Either:
- Log in manually once before running tests (with `noReset=true`, the session persists), or
- Set `TRASHMOB_APK_PATH` to install a fresh APK and log in as part of a custom test setup.

```bash
dotnet test TrashMobMobile.UITests --filter "Category=Authenticated"
```

## Test Structure

```
TrashMobMobile.UITests/
  Setup/
    AppiumFixture.cs      # Appium driver lifecycle (shared across tests)
    BaseTest.cs           # Base class with helper methods
    TestCollection.cs     # xUnit collection definition
  Tests/
    AppLaunchTests.cs     # App launch smoke tests (no auth)
    NavigationTests.cs    # Tab navigation tests (auth required)
    README.md             # This file
```

## Adding New Tests

1. Create a new test class in `Tests/` inheriting from `BaseTest`
2. Add `[Collection(TestCollection.Name)]` to share the Appium driver
3. Use `AutomationId` in XAML to make elements findable:
   ```xml
   <Button AutomationId="MyButton" Text="Click Me" />
   ```
4. Find elements in tests using:
   ```csharp
   var element = WaitForElement("MyButton");
   element.Click();
   ```

## Known Limitations

- **Authentication**: Tests cannot automate the Azure B2C / Entra login flow since it opens an external browser. Log in manually first, then run tests.
- **Network dependency**: Tests require the device to have network access for API calls.
- **Screenshots**: Failed test screenshots are saved to `TestResults/Screenshots/` in the build output directory.
