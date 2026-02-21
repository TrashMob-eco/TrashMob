'use client';

import * as React from 'react';
import { useState } from 'react';
import { MessageSquarePlus, Send, CheckCircle, AlertCircle } from 'lucide-react';
import { Button } from '@/components/ui/button';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from '@/components/ui/dialog';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Textarea } from '@/components/ui/textarea';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from '@/components/ui/select';
import { SubmitFeedback, SubmitFeedback_Body } from '@/services/feedback';

type FeedbackCategory = 'Bug' | 'FeatureRequest' | 'General' | 'Praise';

interface FeedbackState {
    category: FeedbackCategory | '';
    description: string;
    email: string;
}

const categoryLabels: Record<FeedbackCategory, string> = {
    Bug: 'Report a Bug',
    FeatureRequest: 'Feature Request',
    General: 'General Feedback',
    Praise: 'Share Praise',
};

export function FeedbackWidget() {
    const [isOpen, setIsOpen] = useState(false);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [submitStatus, setSubmitStatus] = useState<'idle' | 'success' | 'error'>('idle');
    const [errorMessage, setErrorMessage] = useState('');
    const [feedback, setFeedback] = useState<FeedbackState>({
        category: '',
        description: '',
        email: '',
    });

    const resetForm = () => {
        setFeedback({ category: '', description: '', email: '' });
        setSubmitStatus('idle');
        setErrorMessage('');
    };

    const handleOpen = () => {
        resetForm();
        setIsOpen(true);
    };

    const handleClose = () => {
        setIsOpen(false);
        // Reset after animation
        setTimeout(resetForm, 300);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();

        if (!feedback.category || !feedback.description.trim()) {
            setSubmitStatus('error');
            setErrorMessage('Please select a category and provide a description.');
            return;
        }

        setIsSubmitting(true);
        setSubmitStatus('idle');

        try {
            const body: SubmitFeedback_Body = {
                category: feedback.category as FeedbackCategory,
                description: feedback.description.trim(),
                email: feedback.email.trim() || undefined,
                pageUrl: window.location.href,
                userAgent: navigator.userAgent,
            };

            await SubmitFeedback().service(body);
            setSubmitStatus('success');

            // Auto-close after success
            setTimeout(() => {
                handleClose();
            }, 2000);
        } catch (error) {
            setSubmitStatus('error');
            setErrorMessage('Failed to submit feedback. Please try again.');
            console.error('Feedback submission error:', error);
        } finally {
            setIsSubmitting(false);
        }
    };

    return (
        <>
            {/* Floating feedback button */}
            <Button
                onClick={handleOpen}
                className='fixed bottom-6 right-6 z-50 h-14 w-14 rounded-full shadow-lg hover:shadow-xl transition-shadow'
                size='icon'
                aria-label='Send feedback'
            >
                <MessageSquarePlus className='h-6 w-6' />
            </Button>

            {/* Feedback dialog */}
            <Dialog open={isOpen} onOpenChange={setIsOpen}>
                <DialogContent className='sm:max-w-[425px]'>
                    <DialogHeader>
                        <DialogTitle>Send Feedback</DialogTitle>
                        <DialogDescription>Help us improve TrashMob.eco! Your feedback is valuable.</DialogDescription>
                    </DialogHeader>

                    {submitStatus === 'success' ? (
                        <div className='flex flex-col items-center justify-center py-8 text-center'>
                            <CheckCircle className='h-16 w-16 text-green-500 mb-4' />
                            <h3 className='text-lg font-semibold'>Thank you!</h3>
                            <p className='text-muted-foreground'>Your feedback has been submitted.</p>
                        </div>
                    ) : (
                        <form onSubmit={handleSubmit} className='space-y-4'>
                            <div className='space-y-2'>
                                <Label htmlFor='category'>Category *</Label>
                                <Select
                                    value={feedback.category}
                                    onValueChange={(value) =>
                                        setFeedback({ ...feedback, category: value as FeedbackCategory })
                                    }
                                >
                                    <SelectTrigger>
                                        <SelectValue placeholder='Select a category' />
                                    </SelectTrigger>
                                    <SelectContent>
                                        {(Object.keys(categoryLabels) as FeedbackCategory[]).map((cat) => (
                                            <SelectItem key={cat} value={cat}>
                                                {categoryLabels[cat]}
                                            </SelectItem>
                                        ))}
                                    </SelectContent>
                                </Select>
                            </div>

                            <div className='space-y-2'>
                                <Label htmlFor='description'>Description *</Label>
                                <Textarea
                                    id='description'
                                    placeholder="Tell us what's on your mind..."
                                    value={feedback.description}
                                    onChange={(e) => setFeedback({ ...feedback, description: e.target.value })}
                                    className='min-h-[120px]'
                                />
                            </div>

                            <div className='space-y-2'>
                                <Label htmlFor='email'>Email (optional)</Label>
                                <Input
                                    id='email'
                                    type='email'
                                    placeholder='your@email.com'
                                    value={feedback.email}
                                    onChange={(e) => setFeedback({ ...feedback, email: e.target.value })}
                                />
                                <p className='text-xs text-muted-foreground'>
                                    Provide your email if you'd like us to follow up.
                                </p>
                            </div>

                            {submitStatus === 'error' && (
                                <div className='flex items-center gap-2 text-destructive text-sm'>
                                    <AlertCircle className='h-4 w-4' />
                                    {errorMessage}
                                </div>
                            )}

                            <DialogFooter>
                                <Button type='button' variant='outline' onClick={handleClose}>
                                    Cancel
                                </Button>
                                <Button type='submit' disabled={isSubmitting}>
                                    {isSubmitting ? (
                                        'Submitting...'
                                    ) : (
                                        <>
                                            <Send className='h-4 w-4 mr-2' />
                                            Submit
                                        </>
                                    )}
                                </Button>
                            </DialogFooter>
                        </form>
                    )}
                </DialogContent>
            </Dialog>
        </>
    );
}

export default FeedbackWidget;
