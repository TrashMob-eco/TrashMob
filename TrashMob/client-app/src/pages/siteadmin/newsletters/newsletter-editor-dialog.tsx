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
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import {
    Newsletter,
    GetNewsletterCategories,
    GetNewsletterTemplates,
    CreateNewsletter,
    UpdateNewsletter,
} from '@/services/newsletters';
import { GetTeamsILead } from '@/services/teams';
import { GetMyPartners } from '@/services/partners';

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
        targetId: undefined as string | undefined,
    });

    const [activeTab, setActiveTab] = useState<'edit' | 'preview'>('edit');

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

    const { data: teamsILead } = useQuery({
        queryKey: GetTeamsILead().key,
        queryFn: GetTeamsILead().service,
        select: (res) => res.data,
        enabled: formData.targetType === 'Team',
    });

    const { data: myPartners } = useQuery({
        queryKey: GetMyPartners().key,
        queryFn: GetMyPartners().service,
        select: (res) => res.data,
        enabled: formData.targetType === 'Community',
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
                targetId: newsletter.targetId,
            });
        } else {
            setFormData({
                categoryId: 1,
                subject: '',
                previewText: '',
                htmlContent: '',
                textContent: '',
                targetType: 'All',
                targetId: undefined,
            });
        }
        setActiveTab('edit');
    }, [newsletter, open]);

    const handleTargetTypeChange = (value: string) => {
        setFormData((prev) => ({
            ...prev,
            targetType: value,
            targetId: undefined, // Clear targetId when type changes
        }));
    };

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

    // Determine if targetId is required but not set
    const isTargetIdRequired = formData.targetType !== 'All';
    const isTargetIdMissing = isTargetIdRequired && !formData.targetId;

    // Get target name for display
    const getTargetName = () => {
        if (formData.targetType === 'Team' && formData.targetId && teamsILead) {
            const team = teamsILead.find((t) => t.id === formData.targetId);
            return team?.name;
        }
        if (formData.targetType === 'Community' && formData.targetId && myPartners) {
            const partner = myPartners.find((p) => p.id === formData.targetId);
            return partner?.name;
        }
        return null;
    };

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

                <Tabs value={activeTab} onValueChange={(v) => setActiveTab(v as 'edit' | 'preview')}>
                    <TabsList className='mb-4'>
                        <TabsTrigger value='edit'>Edit</TabsTrigger>
                        <TabsTrigger value='preview'>Preview</TabsTrigger>
                    </TabsList>

                    <TabsContent value='edit'>
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
                                        onValueChange={handleTargetTypeChange}
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

                            {formData.targetType === 'Team' ? (
                                <div className='space-y-2'>
                                    <Label htmlFor='targetId'>Select Team *</Label>
                                    <Select
                                        value={formData.targetId || ''}
                                        onValueChange={(value) =>
                                            setFormData((prev) => ({ ...prev, targetId: value }))
                                        }
                                        disabled={isReadOnly}
                                    >
                                        <SelectTrigger>
                                            <SelectValue placeholder='Select a team you lead' />
                                        </SelectTrigger>
                                        <SelectContent>
                                            {teamsILead && teamsILead.length > 0 ? (
                                                teamsILead.map((team) => (
                                                    <SelectItem key={team.id} value={team.id}>
                                                        {team.name}
                                                    </SelectItem>
                                                ))
                                            ) : (
                                                <SelectItem value='' disabled>
                                                    No teams available
                                                </SelectItem>
                                            )}
                                        </SelectContent>
                                    </Select>
                                    {teamsILead && teamsILead.length === 0 ? (
                                        <p className='text-sm text-muted-foreground'>
                                            You don't lead any teams. Only team leads can send team newsletters.
                                        </p>
                                    ) : null}
                                </div>
                            ) : null}

                            {formData.targetType === 'Community' ? (
                                <div className='space-y-2'>
                                    <Label htmlFor='targetId'>Select Community *</Label>
                                    <Select
                                        value={formData.targetId || ''}
                                        onValueChange={(value) =>
                                            setFormData((prev) => ({ ...prev, targetId: value }))
                                        }
                                        disabled={isReadOnly}
                                    >
                                        <SelectTrigger>
                                            <SelectValue placeholder='Select a community you admin' />
                                        </SelectTrigger>
                                        <SelectContent>
                                            {myPartners && myPartners.length > 0 ? (
                                                myPartners.map((partner) => (
                                                    <SelectItem key={partner.id} value={partner.id}>
                                                        {partner.name}
                                                    </SelectItem>
                                                ))
                                            ) : (
                                                <SelectItem value='' disabled>
                                                    No communities available
                                                </SelectItem>
                                            )}
                                        </SelectContent>
                                    </Select>
                                    {myPartners && myPartners.length === 0 ? (
                                        <p className='text-sm text-muted-foreground'>
                                            You don't admin any communities. Only community admins can send community
                                            newsletters.
                                        </p>
                                    ) : null}
                                </div>
                            ) : null}

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
                    </TabsContent>

                    <TabsContent value='preview'>
                        <div className='space-y-4 py-4'>
                            <div className='rounded-lg border p-4 bg-muted/50'>
                                <div className='mb-2'>
                                    <span className='text-sm font-medium text-muted-foreground'>Subject: </span>
                                    <span className='text-sm'>{formData.subject || '(no subject)'}</span>
                                </div>
                                {formData.previewText ? (
                                    <div className='mb-2'>
                                        <span className='text-sm font-medium text-muted-foreground'>Preview: </span>
                                        <span className='text-sm text-muted-foreground'>{formData.previewText}</span>
                                    </div>
                                ) : null}
                                <div className='mb-2'>
                                    <span className='text-sm font-medium text-muted-foreground'>Target: </span>
                                    <span className='text-sm'>
                                        {formData.targetType === 'All'
                                            ? 'All Users'
                                            : `${formData.targetType}: ${getTargetName() || '(not selected)'}`}
                                    </span>
                                </div>
                            </div>

                            <div className='rounded-lg border'>
                                <div className='border-b bg-muted/50 px-4 py-2'>
                                    <span className='text-sm font-medium'>Email Preview</span>
                                </div>
                                {formData.htmlContent ? (
                                    <iframe
                                        srcDoc={formData.htmlContent}
                                        title='Newsletter Preview'
                                        className='w-full h-96 border-0'
                                        sandbox='allow-same-origin'
                                    />
                                ) : (
                                    <div className='p-8 text-center text-muted-foreground'>
                                        No HTML content to preview
                                    </div>
                                )}
                            </div>
                        </div>
                    </TabsContent>
                </Tabs>

                <DialogFooter>
                    <Button variant='outline' onClick={() => onOpenChange(false)}>
                        {isReadOnly ? 'Close' : 'Cancel'}
                    </Button>
                    {!isReadOnly && (
                        <Button
                            onClick={handleSubmit}
                            disabled={isPending || !formData.subject || isTargetIdMissing}
                        >
                            {isPending ? 'Saving...' : isEditing ? 'Save Changes' : 'Create Draft'}
                        </Button>
                    )}
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
