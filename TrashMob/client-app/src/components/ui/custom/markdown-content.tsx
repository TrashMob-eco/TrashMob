import * as React from 'react';
import ReactMarkdown from 'react-markdown';
import { cn } from '@/lib/utils';

interface MarkdownContentProps {
    children: string;
    className?: string;
}

export const MarkdownContent: React.FC<MarkdownContentProps> = ({ children, className }) => {
    return (
        <div className={cn('prose prose-sm max-w-none dark:prose-invert', className)}>
            <ReactMarkdown>{children}</ReactMarkdown>
        </div>
    );
};
