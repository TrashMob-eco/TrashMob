# Project 25 — Automated UI/E2E Testing

| Attribute | Value |
|-----------|-------|
| **Status** | Not Started |
| **Priority** | Medium |
| **Risk** | Low |
| **Size** | Medium |
| **Dependencies** | Project 5 (CI/CD Infrastructure) |

---

## Business Rationale

Reduce regression risk, enable confident releases, and replace manual test scenarios with executable, maintainable test suites for both web and mobile platforms. Currently, testing relies on manual scenarios documented in TestScenarios.md.

---

## Objectives

### Primary Goals
- **Playwright test framework** for web UI with CI integration
- **UI testing for mobile** app (Appium or MAUI test framework)
- **Convert manual test scenarios** (TestScenarios.md) to automated tests
- **GitHub Actions integration** with test reports
- **Parallel test execution** for faster feedback

### Secondary Goals
- Visual regression testing
- Performance/load testing
- API contract testing
- Accessibility testing automation

---

## Scope

### Web (Playwright)
- ✅ User registration and sign-in flows
- ✅ Event creation, editing, and cancellation
- ✅ Event registration and waiver signing
- ✅ Litter report creation and management
- ✅ Partner request and management flows
- ✅ Site administration access controls
- ✅ Contact form submission

### Mobile (Appium/.NET MAUI Testing)
- ✅ Authentication flows
- ✅ Event discovery and details
- ✅ Event registration
- ✅ Litter report creation with camera
- ✅ Dashboard and stats viewing
- ✅ Map interactions

---

## Out-of-Scope

- ❌ Visual regression testing (future phase)
- ❌ Performance/load testing (separate project)
- ❌ API-only tests (covered by unit tests)
- ❌ Third-party service integration tests

---

## Success Metrics

### Quantitative
- **Test coverage of critical user flows:** ≥ 80%
- **CI pipeline runs all E2E tests on PRs:** 100%
- **Test execution time:** < 10 minutes
- **Flaky test rate:** < 5%
- **Manual regression testing time:** Reduced by 50%

### Qualitative
- Developers confident in automated test coverage
- Faster release cycles
- Fewer production regressions

---

## Dependencies

### Blockers
- **Project 5 (CI/CD Infrastructure):** Stable pipeline for test execution

### Enables
- Faster development velocity
- Confident refactoring
- Better code quality

---

## Risks & Mitigations

| Risk | Likelihood | Impact | Mitigation |
|------|------------|--------|------------|
| **Flaky tests** | High | Medium | Retry logic; stable test data; proper waits |
| **Slow test execution** | Medium | Medium | Parallelization; test sharding; selective runs |
| **Test maintenance burden** | Medium | Medium | Page object pattern; shared fixtures; good abstraction |
| **Mobile emulator issues in CI** | Medium | High | Android-first; stable emulator images; fallback to device farm |

---

## Implementation Plan

### Web Testing - Playwright Setup

**Project Structure:**
```
TrashMob/client-app/
├── e2e/
│   ├── fixtures/
│   │   ├── auth.fixture.ts      # Authentication helpers
│   │   └── test-data.fixture.ts # Test data management
│   ├── pages/
│   │   ├── home.page.ts         # Home page object
│   │   ├── events.page.ts       # Events page object
│   │   ├── login.page.ts        # Login page object
│   │   └── dashboard.page.ts    # Dashboard page object
│   ├── tests/
│   │   ├── auth.spec.ts         # Authentication tests
│   │   ├── events.spec.ts       # Event management tests
│   │   ├── litter-reports.spec.ts
│   │   ├── partners.spec.ts
│   │   └── admin.spec.ts
│   └── playwright.config.ts
```

**Playwright Configuration:**
```typescript
// playwright.config.ts
import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e/tests',
  fullyParallel: true,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 2 : 0,
  workers: process.env.CI ? 2 : undefined,
  reporter: [
    ['html', { open: 'never' }],
    ['junit', { outputFile: 'test-results/junit.xml' }],
  ],
  use: {
    baseURL: process.env.BASE_URL || 'https://localhost:44332',
    trace: 'on-first-retry',
    screenshot: 'only-on-failure',
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
    {
      name: 'firefox',
      use: { ...devices['Desktop Firefox'] },
    },
    {
      name: 'mobile',
      use: { ...devices['iPhone 13'] },
    },
  ],
});
```

**Example Test (Authentication):**
```typescript
// e2e/tests/auth.spec.ts
import { test, expect } from '@playwright/test';
import { LoginPage } from '../pages/login.page';

test.describe('User Authentication', () => {
  test('user can sign up with valid information', async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.goto();
    await loginPage.clickSignUp();

    // Fill registration form
    await page.fill('[name="username"]', 'testuser_' + Date.now());
    await page.fill('[name="email"]', `test${Date.now()}@example.com`);
    await page.check('[name="agreeToPrivacyPolicy"]');
    await page.check('[name="agreeToTermsOfService"]');
    await page.click('button[type="submit"]');

    // Verify success
    await expect(page).toHaveURL(/dashboard/);
  });

  test('user cannot sign up without agreeing to terms', async ({ page }) => {
    const loginPage = new LoginPage(page);
    await loginPage.goto();
    await loginPage.clickSignUp();

    await page.fill('[name="username"]', 'testuser');
    await page.fill('[name="email"]', 'test@example.com');
    // Don't check terms
    await page.click('button[type="submit"]');

    // Verify error
    await expect(page.locator('.error')).toBeVisible();
  });
});
```

**Page Object Example:**
```typescript
// e2e/pages/events.page.ts
import { Page, Locator } from '@playwright/test';

export class EventsPage {
  readonly page: Page;
  readonly createEventButton: Locator;
  readonly eventNameInput: Locator;
  readonly eventDateInput: Locator;
  readonly eventLocationInput: Locator;
  readonly submitButton: Locator;

  constructor(page: Page) {
    this.page = page;
    this.createEventButton = page.getByRole('button', { name: 'Create Event' });
    this.eventNameInput = page.locator('[name="eventName"]');
    this.eventDateInput = page.locator('[name="eventDate"]');
    this.eventLocationInput = page.locator('[name="location"]');
    this.submitButton = page.getByRole('button', { name: 'Submit' });
  }

  async goto() {
    await this.page.goto('/events');
  }

  async createEvent(name: string, date: string, location: string) {
    await this.createEventButton.click();
    await this.eventNameInput.fill(name);
    await this.eventDateInput.fill(date);
    await this.eventLocationInput.fill(location);
    await this.submitButton.click();
  }
}
```

### Mobile Testing Setup

**Project Structure:**
```
TrashMobMobile.Tests.UI/
├── PageObjects/
│   ├── LoginPage.cs
│   ├── EventsPage.cs
│   └── DashboardPage.cs
├── Tests/
│   ├── AuthenticationTests.cs
│   ├── EventTests.cs
│   └── LitterReportTests.cs
├── Fixtures/
│   └── AppFixture.cs
└── TrashMobMobile.Tests.UI.csproj
```

**MAUI Test Example:**
```csharp
// Tests/AuthenticationTests.cs
[TestClass]
public class AuthenticationTests : BaseTest
{
    [TestMethod]
    public async Task User_Can_Login_Successfully()
    {
        // Arrange
        var loginPage = new LoginPage(App);

        // Act
        await loginPage.EnterUsername("testuser@example.com");
        await loginPage.EnterPassword("TestPassword123!");
        await loginPage.TapLoginButton();

        // Assert
        var dashboardPage = new DashboardPage(App);
        Assert.IsTrue(await dashboardPage.IsDisplayed());
    }

    [TestMethod]
    public async Task User_Sees_Error_With_Invalid_Credentials()
    {
        // Arrange
        var loginPage = new LoginPage(App);

        // Act
        await loginPage.EnterUsername("invalid@example.com");
        await loginPage.EnterPassword("wrongpassword");
        await loginPage.TapLoginButton();

        // Assert
        Assert.IsTrue(await loginPage.IsErrorDisplayed());
    }
}
```

### GitHub Actions Workflow

```yaml
# .github/workflows/e2e-tests.yml
name: E2E Tests

on:
  pull_request:
    branches: [main]
  push:
    branches: [main]

jobs:
  web-e2e:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup Node.js
        uses: actions/setup-node@v4
        with:
          node-version: '20'

      - name: Install dependencies
        run: |
          cd TrashMob/client-app
          npm ci

      - name: Install Playwright browsers
        run: npx playwright install --with-deps

      - name: Run Playwright tests
        run: npx playwright test
        env:
          BASE_URL: ${{ secrets.DEV_URL }}

      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: playwright-report
          path: TrashMob/client-app/playwright-report/

  mobile-e2e:
    runs-on: macos-latest
    steps:
      - uses: actions/checkout@v4

      - name: Setup .NET
        uses: actions/setup-dotnet@v4
        with:
          dotnet-version: '10.0.x'

      - name: Start Android Emulator
        uses: reactivecircus/android-emulator-runner@v2
        with:
          api-level: 33
          script: |
            dotnet test TrashMobMobile.Tests.UI/TrashMobMobile.Tests.UI.csproj

      - name: Upload test results
        if: always()
        uses: actions/upload-artifact@v4
        with:
          name: mobile-test-results
          path: '**/TestResults/**'
```

---

## Test Scenarios to Automate

### From TestScenarios.md

| Scenario | Test File | Priority |
|----------|-----------|----------|
| Sign up for site | auth.spec.ts | Critical |
| Sign in to site | auth.spec.ts | Critical |
| Contact Us | contact.spec.ts | Medium |
| Update User Location Preference | user-settings.spec.ts | Medium |
| Create Event | events.spec.ts | Critical |
| Update event | events.spec.ts | High |
| Sign Waiver | waiver.spec.ts | High |
| Register for event | events.spec.ts | Critical |
| Send Invite to potential partner | partners.spec.ts | Medium |
| Request to become a partner | partners.spec.ts | Medium |
| Update Partner | partners.spec.ts | Medium |
| Site Administration | admin.spec.ts | High |

---

## Implementation Phases

### Phase 1: Web Foundation
- Set up Playwright in client-app
- Configure test fixtures
- Implement authentication tests
- Add GitHub Actions workflow

### Phase 2: Core Web Flows
- Event CRUD tests
- Litter report tests
- Partner management tests
- Dashboard tests

### Phase 3: Mobile Foundation
- Evaluate Appium vs MAUI testing
- Set up mobile test project
- Implement auth flow tests
- Configure CI (Android first)

### Phase 4: Completion
- Remaining web tests
- Remaining mobile tests
- Performance baseline
- Delete TestScenarios.md

---

## Open Questions

1. **Test data management strategy?**
   **Recommendation:** Seed data before tests; cleanup after; isolated test users
   **Owner:** Engineering
   **Due:** Before Phase 1

2. **Test environment?**
   **Recommendation:** Use dev environment with test tenant; consider ephemeral environments
   **Owner:** Engineering
   **Due:** Before Phase 1

3. **Required vs optional PR checks?**
   **Recommendation:** Start optional; make required after 2 weeks stability
   **Owner:** Engineering Lead
   **Due:** After Phase 2

4. **Browser/device matrix?**
   **Recommendation:** Chrome + Firefox for web; Android for mobile; expand later
   **Owner:** Engineering
   **Due:** Before Phase 1

---

## Related Documents

- **[Project 5 - CI/CD](./Project_05_Deployment_Pipelines.md)** - Pipeline infrastructure
- **[TestScenarios.md](../../TestScenarios.md)** - Manual scenarios to automate
- **[Playwright Documentation](https://playwright.dev)** - Framework docs

---

**Last Updated:** January 24, 2026
**Owner:** Engineering Team
**Status:** Not Started
**Next Review:** When CI/CD stabilized
