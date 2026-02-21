import { Page, Locator } from '@playwright/test';

/**
 * Page Object for the Contact Us page (/contactus)
 */
export class ContactPage {
    readonly page: Page;

    // Form fields
    readonly nameInput: Locator;
    readonly emailInput: Locator;
    readonly messageInput: Locator;

    // Buttons
    readonly submitButton: Locator;

    // Feedback
    readonly successMessage: Locator;
    readonly errorMessage: Locator;

    // Validation
    readonly nameError: Locator;
    readonly emailError: Locator;
    readonly messageError: Locator;

    constructor(page: Page) {
        this.page = page;

        // Form fields
        this.nameInput = page.locator('input[name="name"], #name');
        this.emailInput = page.locator('input[name="email"], #email');
        this.messageInput = page.locator('textarea[name="message"], #message');

        // Buttons - use form context to avoid matching the feedback widget
        this.submitButton = page.locator('form').getByRole('button', { name: /send|submit/i });

        // Feedback
        this.successMessage = page.locator('[data-testid="success-message"], .success-message, [role="status"]');
        this.errorMessage = page.locator('[data-testid="error-message"], .error-message, [role="alert"]');

        // Field-level errors
        this.nameError = page.locator('[data-testid="name-error"], #name-error');
        this.emailError = page.locator('[data-testid="email-error"], #email-error');
        this.messageError = page.locator('[data-testid="message-error"], #message-error');
    }

    async goto() {
        await this.page.goto('/contactus');
    }

    async isLoaded() {
        await this.nameInput.waitFor({ state: 'visible' });
    }

    async fillForm(data: { name: string; email: string; message: string }) {
        await this.nameInput.fill(data.name);
        await this.emailInput.fill(data.email);
        await this.messageInput.fill(data.message);
    }

    async submit() {
        await this.submitButton.click();
    }

    async submitForm(data: { name: string; email: string; message: string }) {
        await this.fillForm(data);
        await this.submit();
    }

    async hasSuccessMessage(): Promise<boolean> {
        try {
            await this.successMessage.waitFor({ state: 'visible', timeout: 10000 });
            return true;
        } catch {
            return false;
        }
    }

    async hasError(): Promise<boolean> {
        try {
            await this.errorMessage.waitFor({ state: 'visible', timeout: 5000 });
            return true;
        } catch {
            return false;
        }
    }
}
