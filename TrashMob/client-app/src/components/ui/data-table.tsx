import { useState } from 'react';

import {
    Table as TanstackTable,
    SortingState,
    ColumnFiltersState,
    Column,
    ColumnDef,
    flexRender,
    getCoreRowModel,
    getFilteredRowModel,
    getPaginationRowModel,
    getSortedRowModel,
    useReactTable,
} from '@tanstack/react-table';

import {
    ArrowDown,
    ArrowUp,
    ChevronsUpDown,
    ChevronLeft,
    ChevronRight,
    ChevronsLeft,
    ChevronsRight,
    Search,
    X,
} from 'lucide-react';

import { Button } from '@/components/ui/button';
import {
    DropdownMenu,
    DropdownMenuContent,
    DropdownMenuItem,
    DropdownMenuTrigger,
} from '@/components/ui/dropdown-menu';
import { Input } from '@/components/ui/input';

import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';

import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';

import { cn } from '@/lib/utils';

interface DataTableProps<TData, TValue> {
    columns: ColumnDef<TData, TValue>[];
    data: TData[];
    /** Enable global search filter */
    enableSearch?: boolean;
    /** Placeholder text for search input */
    searchPlaceholder?: string;
    /** Columns to search in (by accessorKey). If not provided, searches all columns. */
    searchColumns?: string[];
}

export const DataTable = <TData, TValue>({
    columns,
    data,
    enableSearch = false,
    searchPlaceholder = 'Search...',
    searchColumns,
}: DataTableProps<TData, TValue>) => {
    const [sorting, setSorting] = useState<SortingState>([]);
    const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([]);
    const [globalFilter, setGlobalFilter] = useState('');

    const table = useReactTable({
        data,
        columns,
        getCoreRowModel: getCoreRowModel(),
        getPaginationRowModel: getPaginationRowModel(),
        onSortingChange: setSorting,
        getSortedRowModel: getSortedRowModel(),
        onColumnFiltersChange: setColumnFilters,
        onGlobalFilterChange: setGlobalFilter,
        getFilteredRowModel: getFilteredRowModel(),
        globalFilterFn: searchColumns
            ? (row, _columnId, filterValue) => {
                  const search = filterValue.toLowerCase();
                  return searchColumns.some((col) => {
                      const value = row.getValue(col);
                      return value?.toString().toLowerCase().includes(search);
                  });
              }
            : 'includesString',
        state: {
            sorting,
            columnFilters,
            globalFilter,
        },
    });

    return (
        <div>
            {enableSearch ? (
                <div className='flex items-center gap-2 pb-4'>
                    <div className='relative flex-1 max-w-sm'>
                        <Search className='absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground' />
                        <Input
                            placeholder={searchPlaceholder}
                            value={globalFilter}
                            onChange={(e) => setGlobalFilter(e.target.value)}
                            className='pl-8 pr-8'
                        />
                        {globalFilter ? (
                            <Button
                                variant='ghost'
                                size='sm'
                                className='absolute right-0 top-0 h-full px-2 hover:bg-transparent'
                                onClick={() => setGlobalFilter('')}
                            >
                                <X className='h-4 w-4' />
                                <span className='sr-only'>Clear search</span>
                            </Button>
                        ) : null}
                    </div>
                    {globalFilter ? (
                        <span className='text-sm text-muted-foreground'>
                            {table.getFilteredRowModel().rows.length} results
                        </span>
                    ) : null}
                </div>
            ) : null}
            <Table>
                <TableHeader>
                    {table.getHeaderGroups().map((headerGroup) => (
                        <TableRow key={headerGroup.id}>
                            {headerGroup.headers.map((header) => {
                                return (
                                    <TableHead key={header.id}>
                                        {header.isPlaceholder
                                            ? null
                                            : flexRender(header.column.columnDef.header, header.getContext())}
                                    </TableHead>
                                );
                            })}
                        </TableRow>
                    ))}
                </TableHeader>
                <TableBody>
                    {table.getRowModel().rows?.length ? (
                        table.getRowModel().rows.map((row) => (
                            <TableRow key={row.id} data-state={row.getIsSelected() && 'selected'}>
                                {row.getVisibleCells().map((cell) => (
                                    <TableCell key={cell.id}>
                                        {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                    </TableCell>
                                ))}
                            </TableRow>
                        ))
                    ) : (
                        <TableRow>
                            <TableCell colSpan={columns.length} className='h-24 text-center'>
                                No results.
                            </TableCell>
                        </TableRow>
                    )}
                </TableBody>
            </Table>
            {data.length > table.getRowModel().rows.length ? <DataTablePagination table={table} /> : null}
        </div>
    );
};

/** Pagination */

interface DataTablePaginationProps<TData> {
    table: TanstackTable<TData>;
}

export function DataTablePagination<TData>({ table }: DataTablePaginationProps<TData>) {
    const { pageIndex, pageSize } = table.getState().pagination;
    const totalFiltered = table.getFilteredRowModel().rows.length;
    const currentPageRows = table.getRowModel().rows.length;
    const firstItemNumber = pageIndex * pageSize + 1;
    const lastItemNumber = Math.min(firstItemNumber + currentPageRows - 1, totalFiltered);

    return (
        <div className='flex items-center justify-between px-2'>
            <div className='flex-1 text-sm text-muted-foreground'>
                {firstItemNumber} - {lastItemNumber} of {totalFiltered}
            </div>
            <div className='flex items-center space-x-6 lg:space-x-8'>
                <div className='flex items-center space-x-2'>
                    <p className='text-sm font-medium'>Rows per page</p>
                    <Select
                        value={`${table.getState().pagination.pageSize}`}
                        onValueChange={(value) => {
                            table.setPageSize(Number(value));
                        }}
                    >
                        <SelectTrigger className='h-8 w-[70px]'>
                            <SelectValue placeholder={table.getState().pagination.pageSize} />
                        </SelectTrigger>
                        <SelectContent side='top'>
                            {[10, 20, 30, 40, 50].map((pageSize) => (
                                <SelectItem key={pageSize} value={`${pageSize}`}>
                                    {pageSize}
                                </SelectItem>
                            ))}
                        </SelectContent>
                    </Select>
                </div>
                <div className='flex w-[100px] items-center justify-center text-sm font-medium'>
                    Page {table.getState().pagination.pageIndex + 1} of {table.getPageCount()}
                </div>
                <div className='flex items-center space-x-2'>
                    <Button
                        variant='outline'
                        className='hidden h-8 w-8 p-0 lg:flex'
                        onClick={() => table.setPageIndex(0)}
                        disabled={!table.getCanPreviousPage()}
                    >
                        <span className='sr-only'>Go to first page</span>
                        <ChevronsLeft />
                    </Button>
                    <Button
                        variant='outline'
                        className='h-8 w-8 p-0'
                        onClick={() => table.previousPage()}
                        disabled={!table.getCanPreviousPage()}
                    >
                        <span className='sr-only'>Go to previous page</span>
                        <ChevronLeft />
                    </Button>
                    <Button
                        variant='outline'
                        className='h-8 w-8 p-0'
                        onClick={() => table.nextPage()}
                        disabled={!table.getCanNextPage()}
                    >
                        <span className='sr-only'>Go to next page</span>
                        <ChevronRight />
                    </Button>
                    <Button
                        variant='outline'
                        className='hidden h-8 w-8 p-0 lg:flex'
                        onClick={() => table.setPageIndex(table.getPageCount() - 1)}
                        disabled={!table.getCanNextPage()}
                    >
                        <span className='sr-only'>Go to last page</span>
                        <ChevronsRight />
                    </Button>
                </div>
            </div>
        </div>
    );
}

/** Column Header */
interface DataTableColumnHeaderProps<TData, TValue> extends React.HTMLAttributes<HTMLDivElement> {
    column: Column<TData, TValue>;
    title: string;
}

export function DataTableColumnHeader<TData, TValue>({
    column,
    title,
    className,
}: DataTableColumnHeaderProps<TData, TValue>) {
    if (!column.getCanSort()) {
        return <div className={cn(className)}>{title}</div>;
    }

    return (
        <div className={cn('flex items-center space-x-2', className)}>
            <DropdownMenu>
                <DropdownMenuTrigger asChild>
                    <Button variant='ghost' size='sm' className='-ml-3 h-8 data-[state=open]:bg-accent'>
                        <span>{title}</span>
                        {column.getIsSorted() === 'desc' ? (
                            <ArrowDown />
                        ) : column.getIsSorted() === 'asc' ? (
                            <ArrowUp />
                        ) : (
                            <ChevronsUpDown />
                        )}
                    </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent align='start'>
                    <DropdownMenuItem onClick={() => column.toggleSorting(false)}>
                        <ArrowUp className='h-3.5 w-3.5 text-muted-foreground/70' />
                        Asc
                    </DropdownMenuItem>
                    <DropdownMenuItem onClick={() => column.toggleSorting(true)}>
                        <ArrowDown className='h-3.5 w-3.5 text-muted-foreground/70' />
                        Desc
                    </DropdownMenuItem>
                </DropdownMenuContent>
            </DropdownMenu>
        </div>
    );
}
