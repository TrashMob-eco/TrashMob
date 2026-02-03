import React, { useState, useRef } from 'react';
import { useMutation, useQuery } from '@tanstack/react-query';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Checkbox } from '@/components/ui/checkbox';
import { Label } from '@/components/ui/label';
import {
    Select,
    SelectContent,
    SelectItem,
    SelectTrigger,
    SelectValue,
} from '@/components/ui/select';
import { UploadPaperWaiver, GetRequiredWaivers } from '@/services/user-waivers';
import { PaperWaiverUploadRequest } from '@/components/Models/PaperWaiverUpload';
import { Upload } from 'lucide-react';

const ACCEPTED_FILE_TYPES = '.pdf,.jpg,.jpeg,.png,.webp';
const MAX_FILE_SIZE_MB = 10;
const MAX_FILE_SIZE_BYTES = MAX_FILE_SIZE_MB * 1024 * 1024;

interface PaperWaiverUploadDialogProps {
    /** Whether the dialog is open. */
    open: boolean;
    /** Called when the dialog should close. */
    onClose: () => void;
    /** Called when the waiver is successfully uploaded. */
    onUploaded: () => void;
    /** The user ID to upload the waiver for. */
    userId: string;
    /** The user's display name (for pre-filling signer name). */
    userName?: string;
    /** Optional event ID if uploading for a specific event. */
    eventId?: string;
}

export const PaperWaiverUploadDialog: React.FC<PaperWaiverUploadDialogProps> = ({
    open,
    onClose,
    onUploaded,
    userId,
    userName,
    eventId,
}) => {
    const [selectedFile, setSelectedFile] = useState<File | null>(null);
    const [signerName, setSignerName] = useState(userName ?? '');
    const [dateSigned, setDateSigned] = useState(new Date().toISOString().split('T')[0]);
    const [waiverVersionId, setWaiverVersionId] = useState<string>('');
    const [isMinor, setIsMinor] = useState(false);
    const [guardianName, setGuardianName] = useState('');
    const [guardianRelationship, setGuardianRelationship] = useState('');
    const [fileError, setFileError] = useState<string | null>(null);
    const fileInputRef = useRef<HTMLInputElement>(null);

    // Get available waiver versions
    const waiversQuery = useQuery({
        queryKey: GetRequiredWaivers().key,
        queryFn: GetRequiredWaivers().service,
        enabled: open,
    });

    const uploadMutation = useMutation({
        mutationKey: UploadPaperWaiver().key,
        mutationFn: UploadPaperWaiver().service,
        onSuccess: () => {
            onUploaded();
            resetForm();
        },
    });

    const resetForm = () => {
        setSelectedFile(null);
        setSignerName(userName ?? '');
        setDateSigned(new Date().toISOString().split('T')[0]);
        setWaiverVersionId('');
        setIsMinor(false);
        setGuardianName('');
        setGuardianRelationship('');
        setFileError(null);
        if (fileInputRef.current) {
            fileInputRef.current.value = '';
        }
    };

    const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const file = e.target.files?.[0];
        setFileError(null);

        if (!file) {
            setSelectedFile(null);
            return;
        }

        if (file.size > MAX_FILE_SIZE_BYTES) {
            setFileError(`File size exceeds ${MAX_FILE_SIZE_MB}MB limit.`);
            setSelectedFile(null);
            return;
        }

        const allowedTypes = ['application/pdf', 'image/jpeg', 'image/png', 'image/webp'];
        if (!allowedTypes.includes(file.type)) {
            setFileError('Invalid file type. Allowed: PDF, JPEG, PNG, WebP.');
            setSelectedFile(null);
            return;
        }

        setSelectedFile(file);
    };

    const handleUpload = async () => {
        if (!selectedFile || !waiverVersionId) return;

        const request: PaperWaiverUploadRequest = {
            formFile: selectedFile,
            userId,
            waiverVersionId,
            signerName: signerName.trim(),
            dateSigned: new Date(dateSigned).toISOString(),
            eventId,
            isMinor,
            guardianName: isMinor ? guardianName.trim() : undefined,
            guardianRelationship: isMinor ? guardianRelationship.trim() : undefined,
        };

        await uploadMutation.mutateAsync(request);
    };

    const canUpload =
        selectedFile &&
        waiverVersionId &&
        signerName.trim().length >= 2 &&
        dateSigned &&
        (!isMinor || (guardianName.trim().length >= 2 && guardianRelationship.trim().length >= 2));

    const waiverVersions = waiversQuery.data ?? [];

    return (
        <Dialog
            open={open}
            onOpenChange={(isOpen) => {
                if (!isOpen) {
                    resetForm();
                    onClose();
                }
            }}
        >
            <DialogContent className='sm:max-w-[500px]'>
                <DialogHeader>
                    <DialogTitle>Upload Paper Waiver</DialogTitle>
                    <DialogDescription>
                        Upload a signed paper waiver on behalf of an attendee.
                    </DialogDescription>
                </DialogHeader>

                <div className='space-y-4'>
                    {/* Waiver Version Selection */}
                    <div className='space-y-2'>
                        <Label htmlFor='waiver-version'>Waiver Version</Label>
                        <Select value={waiverVersionId} onValueChange={setWaiverVersionId}>
                            <SelectTrigger id='waiver-version'>
                                <SelectValue placeholder='Select a waiver version' />
                            </SelectTrigger>
                            <SelectContent>
                                {waiverVersions.map((w) => (
                                    <SelectItem key={w.id} value={w.id}>
                                        {w.name}
                                    </SelectItem>
                                ))}
                            </SelectContent>
                        </Select>
                    </div>

                    {/* File Upload */}
                    <div className='space-y-2'>
                        <Label htmlFor='file-upload'>Signed Waiver Document</Label>
                        <div className='flex items-center gap-2'>
                            <Input
                                ref={fileInputRef}
                                id='file-upload'
                                type='file'
                                accept={ACCEPTED_FILE_TYPES}
                                onChange={handleFileChange}
                                className='cursor-pointer'
                            />
                        </div>
                        {selectedFile ? (
                            <p className='text-sm text-muted-foreground flex items-center gap-1'>
                                <Upload className='h-4 w-4' />
                                {selectedFile.name} ({(selectedFile.size / 1024 / 1024).toFixed(2)} MB)
                            </p>
                        ) : null}
                        {fileError ? <p className='text-sm text-destructive'>{fileError}</p> : null}
                        <p className='text-xs text-muted-foreground'>
                            Accepted formats: PDF, JPEG, PNG, WebP. Max size: {MAX_FILE_SIZE_MB}MB.
                        </p>
                    </div>

                    {/* Signer Name */}
                    <div className='space-y-2'>
                        <Label htmlFor='signer-name'>Signer Name (as written on waiver)</Label>
                        <Input
                            id='signer-name'
                            placeholder='Enter the name as signed'
                            value={signerName}
                            onChange={(e) => setSignerName(e.target.value)}
                        />
                    </div>

                    {/* Date Signed */}
                    <div className='space-y-2'>
                        <Label htmlFor='date-signed'>Date Signed</Label>
                        <Input
                            id='date-signed'
                            type='date'
                            value={dateSigned}
                            onChange={(e) => setDateSigned(e.target.value)}
                            max={new Date().toISOString().split('T')[0]}
                        />
                    </div>

                    {/* Minor Checkbox */}
                    <div className='flex items-center space-x-2'>
                        <Checkbox
                            id='is-minor'
                            checked={isMinor}
                            onCheckedChange={(checked) => setIsMinor(checked === true)}
                        />
                        <Label htmlFor='is-minor' className='text-sm'>
                            Signer is a minor (under 18)
                        </Label>
                    </div>

                    {/* Guardian Info (if minor) */}
                    {isMinor ? (
                        <div className='space-y-4 pl-6 border-l-2 border-muted'>
                            <div className='space-y-2'>
                                <Label htmlFor='guardian-name'>Guardian Name</Label>
                                <Input
                                    id='guardian-name'
                                    placeholder='Enter guardian name'
                                    value={guardianName}
                                    onChange={(e) => setGuardianName(e.target.value)}
                                />
                            </div>
                            <div className='space-y-2'>
                                <Label htmlFor='guardian-relationship'>Relationship to Minor</Label>
                                <Input
                                    id='guardian-relationship'
                                    placeholder='e.g., Parent, Legal Guardian'
                                    value={guardianRelationship}
                                    onChange={(e) => setGuardianRelationship(e.target.value)}
                                />
                            </div>
                        </div>
                    ) : null}
                </div>

                <DialogFooter>
                    <Button variant='outline' onClick={onClose} disabled={uploadMutation.isPending}>
                        Cancel
                    </Button>
                    <Button onClick={handleUpload} disabled={!canUpload || uploadMutation.isPending}>
                        {uploadMutation.isPending ? 'Uploading...' : 'Upload Waiver'}
                    </Button>
                </DialogFooter>

                {uploadMutation.isError ? (
                    <p className='text-sm text-destructive text-center'>
                        Failed to upload waiver. Please try again.
                    </p>
                ) : null}
            </DialogContent>
        </Dialog>
    );
};

export default PaperWaiverUploadDialog;
