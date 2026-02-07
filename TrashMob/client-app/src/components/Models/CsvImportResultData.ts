export interface CsvImportError {
    rowNumber: number;
    message: string;
}

class CsvImportResultData {
    createdCount: number = 0;
    skippedDuplicateCount: number = 0;
    errorCount: number = 0;
    errors: CsvImportError[] = [];
    totalRowsProcessed: number = 0;
}

export default CsvImportResultData;
