class OutreachSendResultData {
    prospectOutreachEmailId: string = '';
    success: boolean = false;
    errorMessage: string = '';
}

class BatchOutreachResultData {
    totalRequested: number = 0;
    sent: number = 0;
    failed: number = 0;
    skipped: number = 0;
    results: OutreachSendResultData[] = [];
}

export { OutreachSendResultData };
export default BatchOutreachResultData;
