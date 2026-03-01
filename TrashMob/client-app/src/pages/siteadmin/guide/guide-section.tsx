import { useState } from 'react';
import { Link } from 'react-router';
import { ChevronRight, ExternalLink } from 'lucide-react';
import { Collapsible, CollapsibleTrigger, CollapsibleContent } from '@/components/ui/collapsible';
import { Button } from '@/components/ui/button';
import { MarkdownContent } from '@/components/ui/custom/markdown-content';
import { cn } from '@/lib/utils';
import type { GuideEntry } from './guide-data';

interface GuideEntryCollapsibleProps {
    entry: GuideEntry;
    defaultOpen?: boolean;
}

export const GuideEntryCollapsible = ({ entry, defaultOpen = false }: GuideEntryCollapsibleProps) => {
    const [isOpen, setIsOpen] = useState(defaultOpen);

    return (
        <Collapsible open={isOpen} onOpenChange={setIsOpen}>
            <div className='rounded-lg border px-4 py-3'>
                <div className='flex items-center justify-between'>
                    <CollapsibleTrigger className='flex items-center gap-2 text-sm font-medium hover:underline'>
                        <ChevronRight
                            className={cn('h-4 w-4 transition-transform', isOpen && 'rotate-90')}
                        />
                        {entry.title}
                    </CollapsibleTrigger>
                    {entry.adminPath && (
                        <Button variant='ghost' size='sm' asChild>
                            <Link to={entry.adminPath}>
                                <ExternalLink className='mr-1.5 h-3.5 w-3.5' />
                                Go to page
                            </Link>
                        </Button>
                    )}
                </div>
                <CollapsibleContent className='pt-3 pl-6'>
                    <MarkdownContent>{entry.content}</MarkdownContent>
                </CollapsibleContent>
            </div>
        </Collapsible>
    );
};
