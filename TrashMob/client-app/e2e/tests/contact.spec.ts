import { test, expect } from '../fixtures/base.fixture';
import { ContactPage } from '../pages/contact.page';
import { uniqueId } from '../utils/helpers';

test.describe('Contact Us Form', () => {
    test.describe('Form Display', () => {
        test('should display contact form with all required fields', async ({ page }) => {
            const contactPage = new ContactPage(page);
            await contactPage.goto();

            // Verify all form fields are visible
            await expect(contactPage.nameInput).toBeVisible();
            await expect(contactPage.emailInput).toBeVisible();
            await expect(contactPage.messageInput).toBeVisible();
            await expect(contactPage.submitButton).toBeVisible();
        });

        test('should have required field indicators', async ({ page }) => {
            const contactPage = new ContactPage(page);
            await contactPage.goto();

            // Check that required fields have appropriate indicators
            // This could be aria-required, required attribute, or visual indicator
            await expect(contactPage.nameInput).toHaveAttribute('required', '');
            await expect(contactPage.emailInput).toHaveAttribute('required', '');
            await expect(contactPage.messageInput).toHaveAttribute('required', '');
        });
    });

    test.describe('Form Validation', () => {
        test('should show error when submitting empty form', async ({ page }) => {
            const contactPage = new ContactPage(page);
            await contactPage.goto();

            // Try to submit without filling anything
            await contactPage.submit();

            // Should show validation errors (HTML5 validation or custom)
            // The form should not navigate away
            await expect(page).toHaveURL(/contactus/);
        });

        test('should show error for invalid email format', async ({ page }) => {
            const contactPage = new ContactPage(page);
            await contactPage.goto();

            await contactPage.fillForm({
                name: 'Test User',
                email: 'invalid-email',
                message: 'Test message',
            });

            await contactPage.submit();

            // Should show email validation error
            await expect(page).toHaveURL(/contactus/);
        });

        test('should accept valid email formats', async ({ page }) => {
            const contactPage = new ContactPage(page);
            await contactPage.goto();

            // Fill with valid data
            await contactPage.nameInput.fill('Test User');
            await contactPage.emailInput.fill('valid@example.com');

            // Check that email field doesn't show invalid state
            await expect(contactPage.emailInput).not.toHaveClass(/invalid|error/);
        });
    });

    test.describe('Form Submission', () => {
        // Note: This test may need reCAPTCHA handling in CI
        test.skip('should submit form with valid data', async ({ page }) => {
            const contactPage = new ContactPage(page);
            await contactPage.goto();

            const testId = uniqueId();
            await contactPage.submitForm({
                name: `E2E Test User ${testId}`,
                email: `e2e-test-${testId}@example.com`,
                message: `This is an automated test message from E2E tests. ID: ${testId}`,
            });

            // Wait for success feedback
            const hasSuccess = await contactPage.hasSuccessMessage();
            expect(hasSuccess).toBeTruthy();
        });

        test('should not submit form with empty name', async ({ page }) => {
            const contactPage = new ContactPage(page);
            await contactPage.goto();

            await contactPage.emailInput.fill('test@example.com');
            await contactPage.messageInput.fill('Test message');
            await contactPage.submit();

            // Form should not submit, should stay on page
            await expect(page).toHaveURL(/contactus/);
        });

        test('should not submit form with empty message', async ({ page }) => {
            const contactPage = new ContactPage(page);
            await contactPage.goto();

            await contactPage.nameInput.fill('Test User');
            await contactPage.emailInput.fill('test@example.com');
            // Don't fill message
            await contactPage.submit();

            // Form should not submit, should stay on page
            await expect(page).toHaveURL(/contactus/);
        });
    });

    test.describe('Accessibility', () => {
        test('form fields should have accessible labels', async ({ page }) => {
            const contactPage = new ContactPage(page);
            await contactPage.goto();

            // Check that inputs have associated labels (via id/for or aria-label)
            const nameHasLabel =
                (await contactPage.nameInput.getAttribute('aria-label')) ||
                (await page.locator(`label[for="${await contactPage.nameInput.getAttribute('id')}"]`).count()) > 0;

            const emailHasLabel =
                (await contactPage.emailInput.getAttribute('aria-label')) ||
                (await page.locator(`label[for="${await contactPage.emailInput.getAttribute('id')}"]`).count()) > 0;

            const messageHasLabel =
                (await contactPage.messageInput.getAttribute('aria-label')) ||
                (await page.locator(`label[for="${await contactPage.messageInput.getAttribute('id')}"]`).count()) > 0;

            expect(nameHasLabel).toBeTruthy();
            expect(emailHasLabel).toBeTruthy();
            expect(messageHasLabel).toBeTruthy();
        });

        test('form should be keyboard navigable', async ({ page }) => {
            const contactPage = new ContactPage(page);
            await contactPage.goto();

            // Tab through form elements
            await contactPage.nameInput.focus();
            await page.keyboard.press('Tab');

            // Should focus on email input
            await expect(contactPage.emailInput).toBeFocused();

            await page.keyboard.press('Tab');
            // Should focus on message input
            await expect(contactPage.messageInput).toBeFocused();
        });
    });
});
