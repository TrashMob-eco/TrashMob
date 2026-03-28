import { render, screen, within } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { vi } from 'vitest';
import { MemoryRouter } from 'react-router';
import { AgeGateDialog } from './AgeGateDialog';

describe('AgeGateDialog', () => {
    const mockOnConfirm = vi.fn();
    const mockOnOpenChange = vi.fn();

    beforeEach(() => {
        vi.clearAllMocks();
    });

    const renderDialog = () =>
        render(
            <MemoryRouter>
                <AgeGateDialog open={true} onOpenChange={mockOnOpenChange} onConfirm={mockOnConfirm} />
            </MemoryRouter>,
        );

    it('renders the dialog with title and date picker', () => {
        renderDialog();
        expect(screen.getByText('Create Account')).toBeInTheDocument();
        expect(screen.getByText('Select your date of birth')).toBeInTheDocument();
    });

    it('renders month and year dropdown selects for quick navigation', async () => {
        const user = userEvent.setup();
        renderDialog();

        // Click the date picker button to open the calendar popover
        await user.click(screen.getByText('Select your date of birth'));

        // The calendar should have month and year dropdown selects (not just arrow buttons)
        // react-day-picker v9 with captionLayout='dropdown' renders <select> elements
        const selects = document.querySelectorAll('select');

        expect(selects.length).toBeGreaterThanOrEqual(2);

        // Verify one select has month options and one has year options
        const allOptions = Array.from(selects).flatMap((s) => Array.from(s.options).map((o) => o.text));
        const hasMonths = allOptions.some((text) => text.includes('January') || text.includes('March'));
        const hasYears = allOptions.some((text) => text.includes('2000') || text.includes('1990'));

        expect(hasMonths).toBe(true);
        expect(hasYears).toBe(true);
    });

    it('shows blocked message for under-13 date of birth', async () => {
        const user = userEvent.setup();
        renderDialog();

        // Click the date picker to open it
        await user.click(screen.getByText('Select your date of birth'));

        // Select a date that makes the user under 13
        const recentYear = new Date().getFullYear() - 10;

        // Find and change the year dropdown
        const selects = document.querySelectorAll('select');
        const yearSelect = Array.from(selects).find((s) =>
            Array.from(s.options).some((o) => o.value === String(recentYear)),
        );

        if (yearSelect) {
            await user.selectOptions(yearSelect, String(recentYear));
            // Click day 15 — use getAllByRole and pick one that isn't an outside day
            const dayCells = screen.getAllByRole('gridcell', { name: '15' });
            const targetCell = dayCells.find((c) => !c.classList.contains('day-outside')) ?? dayCells[0];
            await user.click(targetCell.querySelector('button') ?? targetCell);

            await user.click(screen.getByRole('button', { name: 'Continue' }));

            expect(screen.getByText('Unable to Create Account')).toBeInTheDocument();
        }
    });

    it('shows parental consent message for 13-17 date of birth', async () => {
        const user = userEvent.setup();
        renderDialog();

        // Click the date picker to open it
        await user.click(screen.getByText('Select your date of birth'));

        // Select a date that makes the user 15 years old
        const minorYear = new Date().getFullYear() - 15;
        const selects = document.querySelectorAll('select');
        const yearSelect = Array.from(selects).find((s) =>
            Array.from(s.options).some((o) => o.value === String(minorYear)),
        );

        if (yearSelect) {
            await user.selectOptions(yearSelect, String(minorYear));
            // Click day 15 — use getAllByRole to handle duplicate gridcells
            const dayCells = screen.getAllByRole('gridcell', { name: '15' });
            const targetCell = dayCells.find((c) => !c.classList.contains('day-outside')) ?? dayCells[0];
            await user.click(targetCell.querySelector('button') ?? targetCell);

            await user.click(screen.getByRole('button', { name: 'Continue' }));

            expect(screen.getByText('Parental Consent Required')).toBeInTheDocument();
            expect(screen.getByRole('button', { name: 'Request Parental Consent' })).toBeInTheDocument();
        }
    });
});
