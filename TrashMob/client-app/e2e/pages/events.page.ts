import { Page, Locator } from '@playwright/test';

/**
 * Page Object for the Event Creation page (/events/create)
 */
export class EventCreatePage {
    readonly page: Page;

    // Form fields
    readonly nameInput: Locator;
    readonly descriptionInput: Locator;
    readonly eventDateInput: Locator;
    readonly startTimeInput: Locator;
    readonly endTimeInput: Locator;
    readonly eventTypeSelect: Locator;
    readonly maxParticipantsInput: Locator;
    readonly isPublicCheckbox: Locator;

    // Location fields
    readonly locationSearchInput: Locator;
    readonly streetAddressInput: Locator;
    readonly cityInput: Locator;
    readonly stateInput: Locator;
    readonly postalCodeInput: Locator;
    readonly countryInput: Locator;

    // Buttons
    readonly submitButton: Locator;
    readonly cancelButton: Locator;

    // Validation messages
    readonly errorMessages: Locator;

    constructor(page: Page) {
        this.page = page;

        // Form fields - adjust selectors based on actual form implementation
        this.nameInput = page.locator('input[name="name"], #name');
        this.descriptionInput = page.locator('textarea[name="description"], #description');
        this.eventDateInput = page.locator('input[name="eventDate"], #eventDate, [data-testid="event-date"]');
        this.startTimeInput = page.locator('input[name="startTime"], #startTime');
        this.endTimeInput = page.locator('input[name="endTime"], #endTime');
        this.eventTypeSelect = page.locator('select[name="eventTypeId"], [data-testid="event-type"]');
        this.maxParticipantsInput = page.locator('input[name="maxNumberOfParticipants"]');
        this.isPublicCheckbox = page.locator('input[name="isEventPublic"], #isEventPublic');

        // Location
        this.locationSearchInput = page.getByPlaceholder(/search.*location|enter.*address/i);
        this.streetAddressInput = page.locator('input[name="streetAddress"]');
        this.cityInput = page.locator('input[name="city"]');
        this.stateInput = page.locator('input[name="region"], input[name="state"]');
        this.postalCodeInput = page.locator('input[name="postalCode"]');
        this.countryInput = page.locator('input[name="country"]');

        // Buttons
        this.submitButton = page.getByRole('button', { name: /create|submit|save/i });
        this.cancelButton = page.getByRole('button', { name: /cancel/i });

        // Errors
        this.errorMessages = page.locator('[role="alert"], .error-message, [data-testid="error"]');
    }

    async goto() {
        await this.page.goto('/events/create');
    }

    async isLoaded() {
        await this.nameInput.waitFor({ state: 'visible' });
    }

    async fillEventDetails(data: {
        name: string;
        description?: string;
        date: string;
        startTime?: string;
        endTime?: string;
        maxParticipants?: number;
        isPublic?: boolean;
    }) {
        await this.nameInput.fill(data.name);

        if (data.description) {
            await this.descriptionInput.fill(data.description);
        }

        await this.eventDateInput.fill(data.date);

        if (data.startTime) {
            await this.startTimeInput.fill(data.startTime);
        }

        if (data.endTime) {
            await this.endTimeInput.fill(data.endTime);
        }

        if (data.maxParticipants !== undefined) {
            await this.maxParticipantsInput.fill(String(data.maxParticipants));
        }

        if (data.isPublic !== undefined) {
            if (data.isPublic) {
                await this.isPublicCheckbox.check();
            } else {
                await this.isPublicCheckbox.uncheck();
            }
        }
    }

    async fillLocation(data: {
        streetAddress: string;
        city: string;
        state: string;
        postalCode: string;
        country?: string;
    }) {
        await this.streetAddressInput.fill(data.streetAddress);
        await this.cityInput.fill(data.city);
        await this.stateInput.fill(data.state);
        await this.postalCodeInput.fill(data.postalCode);

        if (data.country) {
            await this.countryInput.fill(data.country);
        }
    }

    async submit() {
        await this.submitButton.click();
    }

    async cancel() {
        await this.cancelButton.click();
    }

    async getErrorMessages(): Promise<string[]> {
        const errors = await this.errorMessages.allTextContents();
        return errors.filter((e) => e.trim().length > 0);
    }
}

/**
 * Page Object for the Event Details page (/eventdetails/:eventId)
 */
export class EventDetailsPage {
    readonly page: Page;

    // Event info
    readonly eventTitle: Locator;
    readonly eventDate: Locator;
    readonly eventLocation: Locator;
    readonly eventDescription: Locator;
    readonly attendeeCount: Locator;

    // Action buttons
    readonly registerButton: Locator;
    readonly unregisterButton: Locator;
    readonly editButton: Locator;
    readonly cancelEventButton: Locator;

    // Attendees section
    readonly attendeesSection: Locator;
    readonly attendeeList: Locator;

    constructor(page: Page) {
        this.page = page;

        // Event info
        this.eventTitle = page.locator('h1, [data-testid="event-title"]');
        this.eventDate = page.locator('[data-testid="event-date"], .event-date');
        this.eventLocation = page.locator('[data-testid="event-location"], .event-location');
        this.eventDescription = page.locator('[data-testid="event-description"], .event-description');
        this.attendeeCount = page.locator('[data-testid="attendee-count"]');

        // Buttons
        this.registerButton = page.getByRole('button', { name: /register|attend|join/i });
        this.unregisterButton = page.getByRole('button', { name: /unregister|cancel registration/i });
        this.editButton = page.getByRole('link', { name: /edit/i });
        this.cancelEventButton = page.getByRole('button', { name: /cancel event/i });

        // Attendees
        this.attendeesSection = page.locator('[data-testid="attendees-section"]');
        this.attendeeList = page.locator('[data-testid="attendee-list"] li, .attendee-item');
    }

    async goto(eventId: string) {
        await this.page.goto(`/eventdetails/${eventId}`);
    }

    async isLoaded() {
        await this.eventTitle.waitFor({ state: 'visible' });
    }

    async register() {
        await this.registerButton.click();
    }

    async unregister() {
        await this.unregisterButton.click();
    }

    async getAttendeeCount(): Promise<number> {
        const countText = await this.attendeeCount.textContent();
        const match = countText?.match(/\d+/);
        return match ? parseInt(match[0], 10) : 0;
    }
}
