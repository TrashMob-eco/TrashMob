import { useState, useRef } from 'react';
import { useMutation } from '@tanstack/react-query';
import { Upload, Loader2, CheckCircle, AlertCircle } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { DataTable } from '@/components/ui/data-table';
import { useToast } from '@/hooks/use-toast';
import { ImportProspectsCsv } from '@/services/community-prospects';
import type { CsvImportError } from '@/components/Models/CsvImportResultData';
import type { ColumnDef } from '@tanstack/react-table';

const errorColumns: ColumnDef<CsvImportError>[] = [
    { accessorKey: 'rowNumber', header: 'Row' },
    { accessorKey: 'message', header: 'Error' },
];

export const SiteAdminProspectImport = () => {
    const { toast } = useToast();
    const fileInputRef = useRef<HTMLInputElement>(null);
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [previewRows, setPreviewRows] = useState<string[][]>([]);

    const importCsv = useMutation({
        mutationKey: ImportProspectsCsv().key,
        mutationFn: ImportProspectsCsv().service,
        onSuccess: () => toast({ variant: 'default', title: 'Import complete' }),
        onError: () => toast({ variant: 'destructive', title: 'Import failed' }),
    });

    function handleFileSelect(e: React.ChangeEvent<HTMLInputElement>) {
        const file = e.target.files?.[0];
        if (!file) return;

        if (!file.name.endsWith('.csv')) {
            toast({ variant: 'destructive', title: 'Only .csv files are supported' });
            return;
        }

        setSelectedFile(file);
        importCsv.reset();

        // Read first 6 lines for preview (header + 5 rows)
        const reader = new FileReader();
        reader.onload = (evt) => {
            const text = evt.target?.result as string;
            const lines = text.split('\n').filter((l) => l.trim().length > 0);
            const preview = lines.slice(0, 6).map((line) => line.split(',').map((f) => f.trim().replace(/^"|"$/g, '')));
            setPreviewRows(preview);
        };
        reader.readAsText(file);
    }

    function handleImport() {
        if (!selectedFile) return;
        importCsv.mutate(selectedFile);
    }

    const result = importCsv.data?.data;

    return (
        <div className='space-y-6'>
            {/* File Upload */}
            <Card>
                <CardHeader>
                    <CardTitle className='flex items-center gap-2'>
                        <Upload className='h-5 w-5' /> Import Prospects from CSV
                    </CardTitle>
                </CardHeader>
                <CardContent className='space-y-4'>
                    <p className='text-sm text-muted-foreground'>
                        Expected columns: Name, Type, City, Region, Country, Population, Website, ContactEmail,
                        ContactName, ContactTitle
                    </p>
                    <div className='flex items-center gap-4'>
                        <input
                            ref={fileInputRef}
                            type='file'
                            accept='.csv'
                            onChange={handleFileSelect}
                            className='text-sm'
                        />
                        <Button onClick={handleImport} disabled={!selectedFile || importCsv.isPending}>
                            {importCsv.isPending ? (
                                <>
                                    <Loader2 className='mr-2 h-4 w-4 animate-spin' /> Importing...
                                </>
                            ) : (
                                'Import'
                            )}
                        </Button>
                    </div>

                    {/* Preview */}
                    {previewRows.length > 0 && !result ? (
                        <div>
                            <h4 className='text-sm font-medium mb-2'>Preview (first 5 rows)</h4>
                            <div className='overflow-x-auto rounded border'>
                                <table className='text-xs w-full'>
                                    <thead>
                                        <tr className='bg-muted'>
                                            {previewRows[0]?.map((header, i) => (
                                                <th key={i} className='px-2 py-1 text-left font-medium'>
                                                    {header}
                                                </th>
                                            ))}
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {previewRows.slice(1).map((row, ri) => (
                                            <tr key={ri} className='border-t'>
                                                {row.map((cell, ci) => (
                                                    <td key={ci} className='px-2 py-1'>
                                                        {cell}
                                                    </td>
                                                ))}
                                            </tr>
                                        ))}
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    ) : null}
                </CardContent>
            </Card>

            {/* Results */}
            {result ? (
                <Card>
                    <CardHeader>
                        <CardTitle className='flex items-center gap-2'>
                            {result.errorCount === 0 ? (
                                <CheckCircle className='h-5 w-5 text-green-600' />
                            ) : (
                                <AlertCircle className='h-5 w-5 text-yellow-600' />
                            )}
                            Import Results
                        </CardTitle>
                    </CardHeader>
                    <CardContent className='space-y-4'>
                        <div className='grid grid-cols-4 gap-4'>
                            <div className='rounded border p-3 text-center'>
                                <p className='text-2xl font-bold'>{result.totalRowsProcessed}</p>
                                <p className='text-xs text-muted-foreground'>Rows Processed</p>
                            </div>
                            <div className='rounded border p-3 text-center'>
                                <p className='text-2xl font-bold text-green-600'>{result.createdCount}</p>
                                <p className='text-xs text-muted-foreground'>Created</p>
                            </div>
                            <div className='rounded border p-3 text-center'>
                                <p className='text-2xl font-bold text-yellow-600'>{result.skippedDuplicateCount}</p>
                                <p className='text-xs text-muted-foreground'>Skipped (Duplicate)</p>
                            </div>
                            <div className='rounded border p-3 text-center'>
                                <p className='text-2xl font-bold text-red-600'>{result.errorCount}</p>
                                <p className='text-xs text-muted-foreground'>Errors</p>
                            </div>
                        </div>

                        {result.errors.length > 0 ? (
                            <div>
                                <h4 className='text-sm font-medium mb-2'>Errors</h4>
                                <DataTable columns={errorColumns} data={result.errors} />
                            </div>
                        ) : null}
                    </CardContent>
                </Card>
            ) : null}
        </div>
    );
};
