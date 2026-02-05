import * as React from 'react';
import { useState } from 'react';
import ReactMarkdown from 'react-markdown';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { cn } from '@/lib/utils';
import { Eye, Pencil } from 'lucide-react';

interface MarkdownEditorProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
    value?: string;
    onChange?: (e: React.ChangeEvent<HTMLTextAreaElement>) => void;
}

const MarkdownEditor = React.forwardRef<HTMLTextAreaElement, MarkdownEditorProps>(
    ({ className, value, onChange, ...props }, ref) => {
        const [isPreview, setIsPreview] = useState(false);

        return (
            <div className='space-y-2'>
                <div className='flex items-center justify-between'>
                    <div className='flex gap-1'>
                        <Button
                            type='button'
                            variant={!isPreview ? 'default' : 'outline'}
                            size='sm'
                            onClick={() => setIsPreview(false)}
                        >
                            <Pencil className='h-4 w-4 mr-1' />
                            Edit
                        </Button>
                        <Button
                            type='button'
                            variant={isPreview ? 'default' : 'outline'}
                            size='sm'
                            onClick={() => setIsPreview(true)}
                        >
                            <Eye className='h-4 w-4 mr-1' />
                            Preview
                        </Button>
                    </div>
                    <span className='text-xs text-muted-foreground'>Supports Markdown formatting</span>
                </div>

                {isPreview ? (
                    <div
                        className={cn(
                            'min-h-[200px] rounded-md border border-input bg-background px-3 py-2 prose prose-sm max-w-none dark:prose-invert',
                            className,
                        )}
                    >
                        {value ? (
                            <ReactMarkdown>{value}</ReactMarkdown>
                        ) : (
                            <p className='text-muted-foreground italic'>Nothing to preview</p>
                        )}
                    </div>
                ) : (
                    <>
                        <Textarea
                            ref={ref}
                            className={cn('min-h-[200px] font-mono text-sm', className)}
                            value={value}
                            onChange={onChange}
                            {...props}
                        />
                        <div className='text-xs text-muted-foreground space-y-1'>
                            <p className='font-medium'>Markdown tips:</p>
                            <ul className='list-disc list-inside space-y-0.5'>
                                <li>
                                    <code className='bg-muted px-1 rounded'>**bold**</code> for{' '}
                                    <strong>bold text</strong>
                                </li>
                                <li>
                                    <code className='bg-muted px-1 rounded'>*italic*</code> for <em>italic text</em>
                                </li>
                                <li>
                                    <code className='bg-muted px-1 rounded'>- item</code> for bullet lists
                                </li>
                                <li>
                                    <code className='bg-muted px-1 rounded'>1. item</code> for numbered lists
                                </li>
                                <li>
                                    <code className='bg-muted px-1 rounded'>## Heading</code> for headings
                                </li>
                            </ul>
                        </div>
                    </>
                )}
            </div>
        );
    },
);
MarkdownEditor.displayName = 'MarkdownEditor';

export { MarkdownEditor };
