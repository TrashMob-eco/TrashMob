import { render, screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ColumnDef } from '@tanstack/react-table';
import { DataTable, DataTableColumnHeader, features } from './data-table';

interface Row {
    id: string;
    name: string;
    age: number;
}

const rows: Row[] = [
    { id: '1', name: 'Charlie', age: 40 },
    { id: '2', name: 'Alice', age: 30 },
    { id: '3', name: 'Bob', age: 20 },
];

const columns: ColumnDef<typeof features, Row>[] = [
    {
        accessorKey: 'name',
        header: ({ column }) => <DataTableColumnHeader column={column} title='Name' />,
    },
    {
        accessorKey: 'age',
        header: 'Age',
    },
];

// Radix DropdownMenu (used by DataTableColumnHeader) applies pointer-events: none
// during open/close transitions; JSDOM's synchronous clicks race with that. These
// tests check business logic (sort order, filtered results), not interactability,
// so skipping the check is safe — same rationale as AgeGateDialog.test.tsx.
const setupUser = () => userEvent.setup({ pointerEventsCheck: 0 });

describe('DataTable', () => {
    it('renders a header and a row per data item', () => {
        render(<DataTable columns={columns} data={rows} />);

        expect(screen.getByText('Name')).toBeInTheDocument();
        expect(screen.getByText('Age')).toBeInTheDocument();
        expect(screen.getByText('Charlie')).toBeInTheDocument();
        expect(screen.getByText('Alice')).toBeInTheDocument();
        expect(screen.getByText('Bob')).toBeInTheDocument();
    });

    it('shows "No results." when data is empty', () => {
        render(<DataTable columns={columns} data={[]} />);

        expect(screen.getByText('No results.')).toBeInTheDocument();
    });

    it('renders rows in the given order by default', () => {
        render(<DataTable columns={columns} data={rows} />);

        const cells = screen.getAllByRole('cell').map((cell) => cell.textContent);
        // First cell of each row is the name column — default order is insertion order.
        expect(cells[0]).toBe('Charlie');
        expect(cells[2]).toBe('Alice');
        expect(cells[4]).toBe('Bob');
    });

    it('applies an initial sort when initialSorting is provided', () => {
        render(<DataTable columns={columns} data={rows} initialSorting={[{ id: 'name', desc: false }]} />);

        const cells = screen.getAllByRole('cell').map((cell) => cell.textContent);
        expect(cells[0]).toBe('Alice');
        expect(cells[2]).toBe('Bob');
        expect(cells[4]).toBe('Charlie');
    });

    it('sorts rows when a sortable column header is clicked', async () => {
        const user = setupUser();
        render(<DataTable columns={columns} data={rows} />);

        await user.click(screen.getByRole('button', { name: /Name/i }));
        await user.click(screen.getByRole('menuitem', { name: /Asc/i }));

        const cells = screen.getAllByRole('cell').map((cell) => cell.textContent);
        expect(cells[0]).toBe('Alice');
        expect(cells[2]).toBe('Bob');
        expect(cells[4]).toBe('Charlie');
    });

    it('filters rows by the global search box when enableSearch is set', async () => {
        const user = setupUser();
        render(<DataTable columns={columns} data={rows} enableSearch searchPlaceholder='Search...' />);

        await user.type(screen.getByPlaceholderText('Search...'), 'ali');

        expect(screen.getByText('Alice')).toBeInTheDocument();
        expect(screen.queryByText('Charlie')).not.toBeInTheDocument();
        expect(screen.queryByText('Bob')).not.toBeInTheDocument();
        expect(screen.getByText('1 results')).toBeInTheDocument();
    });

    it('restricts global search to searchColumns when provided', async () => {
        const user = setupUser();
        render(
            <DataTable
                columns={columns}
                data={rows}
                enableSearch
                searchPlaceholder='Search...'
                searchColumns={['name']}
            />,
        );

        // "20" only matches Bob's age, not his name — searchColumns=['name'] should exclude it.
        await user.type(screen.getByPlaceholderText('Search...'), '20');

        expect(screen.getByText('No results.')).toBeInTheDocument();
    });

    it('clears the global search when the clear button is clicked', async () => {
        const user = setupUser();
        render(<DataTable columns={columns} data={rows} enableSearch searchPlaceholder='Search...' />);

        const input = screen.getByPlaceholderText('Search...');
        await user.type(input, 'ali');
        expect(screen.queryByText('Charlie')).not.toBeInTheDocument();

        await user.click(screen.getByRole('button', { name: /clear search/i }));

        expect(input).toHaveValue('');
        expect(screen.getByText('Charlie')).toBeInTheDocument();
    });

    it('does not render pagination controls when all rows fit on one page', () => {
        render(<DataTable columns={columns} data={rows} />);

        expect(screen.queryByText(/Page \d+ of \d+/)).not.toBeInTheDocument();
    });

    it('renders pagination controls and paginates when data exceeds the default page size', async () => {
        const user = setupUser();
        const manyRows: Row[] = Array.from({ length: 15 }, (_, i) => ({
            id: String(i),
            name: `Row ${i}`,
            age: i,
        }));

        render(<DataTable columns={columns} data={manyRows} />);

        // Default page size is 10 (see the [10, 20, 30, 40, 50] options in DataTablePagination).
        expect(screen.getByText('Page 1 of 2')).toBeInTheDocument();
        expect(screen.getByText('Row 0')).toBeInTheDocument();
        expect(screen.queryByText('Row 10')).not.toBeInTheDocument();

        await user.click(screen.getByRole('button', { name: /go to next page/i }));

        expect(screen.getByText('Page 2 of 2')).toBeInTheDocument();
        expect(screen.getByText('Row 10')).toBeInTheDocument();
        expect(screen.queryByText('Row 0')).not.toBeInTheDocument();
    });
});
