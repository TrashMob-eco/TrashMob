import { useEffect, useState } from 'react';
import { useMutation, useQuery, useQueryClient } from '@tanstack/react-query';
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogFooter,
    DialogHeader,
    DialogTitle,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Label } from '@/components/ui/label';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import {
    Newsletter,
    GetNewsletterCategories,
    GetNewsletterTemplates,
    CreateNewsletter,
    UpdateNewsletter,
} from '@/services/newsletters';

interface NewsletterEditorDialogProps {
    open: boolean;
    onOpenChange: (open: boolean) => void;
    newsletter: Newsletter | null;
}

export const NewsletterEditorDialog = ({ open, onOpenChange, newsletter }: NewsletterEditorDialogProps) => {
    const queryClient = useQueryClient();
    const isEditing = newsletter !== null;
    const isReadOnly = newsletter !== null && newsletter.status !== 'Draft';

    const [formData, setFormData] = useState({
        categoryId: 1,
        subject: '',
        previewText: '',
        htmlContent: '',
        textContent: '',
        targetType: 'All',
    });

    const { data: categories } = useQuery({
        queryKey: GetNewsletterCategories().key,
        queryFn: GetNewsletterCategories().service,
        select: (res) => res.data,
    });

    const { data: templates } = useQuery({
        queryKey: GetNewsletterTemplates().key,
        queryFn: GetNewsletterTemplates().service,
        select: (res) => res.data,
        enabled: !isReadOnly,
    });

    const createMutation = useMutation({
        mutationKey: CreateNewsletter().key,
        mutationFn: CreateNewsletter().service,
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/admin/newsletters'] });
            onOpenChange(false);
        },
    });

    const updateMutation = useMutation({
        mutationKey: UpdateNewsletter().key,
        mutationFn: ({ id, data }: { id: string; data: typeof formData }) => UpdateNewsletter().service({ id }, data),
        onSuccess: () => {
            queryClient.invalidateQueries({ queryKey: ['/admin/newsletters'] });
            onOpenChange(false);
        },
    });

    useEffect(() => {
        if (newsletter) {
            setFormData({
                categoryId: newsletter.categoryId,
                subject: newsletter.subject,
                previewText: newsletter.previewText || '',
                htmlContent: newsletter.htmlContent || '',
                textContent: newsletter.textContent || '',
                targetType: newsletter.targetType,
            });
        } else {
            setFormData({
                categoryId: 1,
                subject: '',
                previewText: '',
                htmlContent: '',
                textContent: '',
                targetType: 'All',
            });
        }
    }, [newsletter, open]);

    const handleApplyTemplate = (templateId: string) => {
        const template = templates?.find((t) => t.id === parseInt(templateId));
        if (template) {
            setFormData((prev) => ({
                ...prev,
                htmlContent: template.htmlContent || '',
                textContent: template.textContent || '',
            }));
        }
    };

    const handleSubmit = () => {
        if (isEditing && newsletter) {
            updateMutation.mutate({ id: newsletter.id, data: formData });
        } else {
            createMutation.mutate(formData);
        }
    };

    const isPending = createMutation.isPending || updateMutation.isPending;

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className='max-w-4xl max-h-[90vh] overflow-y-auto'>
                <DialogHeader>
                    <DialogTitle>
                        {isReadOnly ? 'View Newsletter' : isEditing ? 'Edit Newsletter' : 'Create Newsletter'}
                    </DialogTitle>
                    <DialogDescription>
                        {isReadOnly
                            ? 'This newsletter has been sent and cannot be edited.'
                            : 'Fill in the details for your newsletter.'}
                    </DialogDescription>
                </DialogHeader>

                <div className='grid gap-4 py-4'>
                    <div className='grid grid-cols-2 gap-4'>
                        <div className='space-y-2'>
                            <Label htmlFor='category'>Category</Label>
                            <Select
                                value={formData.categoryId.toString()}
                                onValueChange={(value) =>
                                    setFormData((prev) => ({ ...prev, categoryId: parseInt(value) }))
                                }
                                disabled={isReadOnly}
                            >
                                <SelectTrigger>
                                    <SelectValue placeholder='Select category' />
                                </SelectTrigger>
                                <SelectContent>
                                    {categories?.map((cat) => (
                                        <SelectItem key={cat.id} value={cat.id.toString()}>
                                            {cat.name}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                        </div>

                        <div className='space-y-2'>
                            <Label htmlFor='targetType'>Target Audience</Label>
                            <Select
                                value={formData.targetType}
                                onValueChange={(value) => setFormData((prev) => ({ ...prev, targetType: value }))}
                                disabled={isReadOnly}
                            >
                                <SelectTrigger>
                                    <SelectValue placeholder='Select target' />
                                </SelectTrigger>
                                <SelectContent>
                                    <SelectItem value='All'>All Users</SelectItem>
                                    <SelectItem value='Community'>Specific Community</SelectItem>
                                    <SelectItem value='Team'>Specific Team</SelectItem>
                                </SelectContent>
                            </Select>
                        </div>
                    </div>

                    <div className='space-y-2'>
                        <Label htmlFor='subject'>Subject *</Label>
                        <Input
                            id='subject'
                            value={formData.subject}
                            onChange={(e) => setFormData((prev) => ({ ...prev, subject: e.target.value }))}
                            placeholder='Newsletter subject line'
                            disabled={isReadOnly}
                        />
                    </div>

                    <div className='space-y-2'>
                        <Label htmlFor='previewText'>Preview Text</Label>
                        <Input
                            id='previewText'
                            value={formData.previewText}
                            onChange={(e) => setFormData((prev) => ({ ...prev, previewText: e.target.value }))}
                            placeholder='Text shown in email preview'
                            disabled={isReadOnly}
                        />
                    </div>

                    {!isReadOnly && templates && templates.length > 0 ? (
                        <div className='space-y-2'>
                            <Label>Apply Template</Label>
                            <Select onValueChange={handleApplyTemplate}>
                                <SelectTrigger>
                                    <SelectValue placeholder='Choose a template' />
                                </SelectTrigger>
                                <SelectContent>
                                    {templates.map((template) => (
                                        <SelectItem key={template.id} value={template.id.toString()}>
                                            {template.name}
                                        </SelectItem>
                                    ))}
                                </SelectContent>
                            </Select>
                        </div>
                    ) : null}

                    <div className='space-y-2'>
                        <Label htmlFor='htmlContent'>HTML Content</Label>
                        <Textarea
                            id='htmlContent'
                            value={formData.htmlContent}
                            onChange={(e) => setFormData((prev) => ({ ...prev, htmlContent: e.target.value }))}
                            placeholder='HTML email content'
                            rows={10}
                            className='font-mono text-sm'
                            disabled={isReadOnly}
                        />
                    </div>

                    <div className='space-y-2'>
                        <Label htmlFor='textContent'>Plain Text Content</Label>
                        <Textarea
                            id='textContent'
                            value={formData.textContent}
                            onChange={(e) => setFormData((prev) => ({ ...prev, textContent: e.target.value }))}
                            placeholder='Plain text fallback content'
                            rows={5}
                            disabled={isReadOnly}
                        />
                    </div>
                </div>

                <DialogFooter>
                    <Button variant='outline' onClick={() => onOpenChange(false)}>
                        {isReadOnly ? 'Close' : 'Cancel'}
                    </Button>
                    {!isReadOnly && (
                        <Button onClick={handleSubmit} disabled={isPending || !formData.subject}>
                            {isPending ? 'Saving...' : isEditing ? 'Save Changes' : 'Create Draft'}
                        </Button>
                    )}
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
