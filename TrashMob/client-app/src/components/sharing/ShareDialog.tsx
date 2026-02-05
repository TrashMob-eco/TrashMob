import { useState, useCallback } from 'react';
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogDescription,
    DialogFooter,
} from '@/components/ui/dialog';
import { Button } from '@/components/ui/button';
import { Tabs, TabsContent, TabsList, TabsTrigger } from '@/components/ui/tabs';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import { Copy, Clock, MapPin, Link2, QrCode } from 'lucide-react';
import {
    FacebookShareButton,
    FacebookIcon,
    TwitterShareButton,
    XIcon,
    LinkedinShareButton,
    LinkedinIcon,
    WhatsappShareButton,
    WhatsappIcon,
    EmailShareButton,
    EmailIcon,
    BlueskyShareButton,
    BlueskyIcon,
    RedditShareButton,
    RedditIcon,
} from 'react-share';
import { ShareableContent } from './types';
import { QRCodeDisplay } from './QRCodeDisplay';

interface ShareDialogProps {
    content: ShareableContent;
    open: boolean;
    onOpenChange: (open: boolean) => void;
    message?: string;
    emailSubject?: string;
}

export const ShareDialog = ({ content, open, onOpenChange, message, emailSubject }: ShareDialogProps) => {
    const [copiedLink, setCopied] = useState(false);

    const parseShareMessage = useCallback(
        (provider: string) => {
            if (!message) return content.description;
            const TrashMobTag = provider === 'twitter' ? '@TrashMobEco' : 'TrashMob.eco';
            return message.replace('{{TrashMob}}', TrashMobTag);
        },
        [message, content.description],
    );

    const handleCopyLink = useCallback(() => {
        if (copiedLink) return;

        navigator.clipboard.writeText(content.url);
        setCopied(true);
        setTimeout(() => {
            setCopied(false);
        }, 2000);
    }, [copiedLink, content.url]);

    const getDialogTitle = () => {
        switch (content.type) {
            case 'event':
                return 'Share Event';
            case 'team':
                return 'Share Team';
            case 'community':
                return 'Share Community';
            default:
                return 'Share';
        }
    };

    const getEmailSubject = () => {
        if (emailSubject) return emailSubject;
        switch (content.type) {
            case 'event':
                return `Join me at this TrashMob.eco event${content.location ? ` in ${content.location}` : ''}!`;
            case 'team':
                return `Check out this team on TrashMob.eco: ${content.title}`;
            case 'community':
                return `Check out this community on TrashMob.eco: ${content.title}`;
            default:
                return `Check out this on TrashMob.eco: ${content.title}`;
        }
    };

    return (
        <Dialog open={open} onOpenChange={onOpenChange}>
            <DialogContent className='sm:max-w-[500px]'>
                <DialogHeader>
                    <DialogTitle>{getDialogTitle()}</DialogTitle>
                    <DialogDescription>Share this with your friends and followers</DialogDescription>
                </DialogHeader>

                <Tabs defaultValue='social' className='w-full'>
                    <TabsList className='grid w-full grid-cols-2'>
                        <TabsTrigger value='social'>
                            <Link2 className='h-4 w-4 mr-2' />
                            Social
                        </TabsTrigger>
                        <TabsTrigger value='qr'>
                            <QrCode className='h-4 w-4 mr-2' />
                            QR Code
                        </TabsTrigger>
                    </TabsList>

                    <TabsContent value='social' className='mt-4'>
                        <div className='grid gap-4'>
                            {/* Content Preview */}
                            <div className='flex flex-col mb-2'>
                                <h6 className='font-semibold'>{content.title}</h6>
                                {content.location ? <div className='flex flex-row items-center text-sm text-muted-foreground mt-1'>
                                        <MapPin className='h-4 w-4 mr-2' />
                                        {content.location}
                                    </div> : null}
                                {content.date ? <div className='flex flex-row items-center text-sm text-muted-foreground mt-1'>
                                        <Clock className='h-4 w-4 mr-2' />
                                        {content.date.toLocaleDateString('en-us', {
                                            weekday: 'short',
                                            year: 'numeric',
                                            month: 'short',
                                            day: 'numeric',
                                            hour: 'numeric',
                                            minute: '2-digit',
                                        })}
                                    </div> : null}
                            </div>

                            <hr />

                            {/* Social Share Buttons */}
                            <div>
                                <p className='text-sm font-medium mb-3'>Share a link</p>
                                <div className='flex flex-row flex-wrap gap-2'>
                                    <FacebookShareButton url={content.url} hashtag='#litter'>
                                        <FacebookIcon size={40} round />
                                    </FacebookShareButton>
                                    <TwitterShareButton
                                        title={parseShareMessage('twitter')}
                                        hashtags={['litter', 'TrashMob']}
                                        url={content.url}
                                        via='TrashMobEco'
                                    >
                                        <XIcon size={40} round />
                                    </TwitterShareButton>
                                    <BlueskyShareButton title={parseShareMessage('bluesky')} url={content.url}>
                                        <BlueskyIcon size={40} round />
                                    </BlueskyShareButton>
                                    <LinkedinShareButton url={content.url}>
                                        <LinkedinIcon size={40} round />
                                    </LinkedinShareButton>
                                    <RedditShareButton title={content.title} url={content.url}>
                                        <RedditIcon size={40} round />
                                    </RedditShareButton>
                                    <WhatsappShareButton url={content.url} title={parseShareMessage('whatsapp')}>
                                        <WhatsappIcon size={40} round />
                                    </WhatsappShareButton>
                                    <EmailShareButton
                                        url={content.url}
                                        subject={getEmailSubject()}
                                        body={parseShareMessage('email')}
                                    >
                                        <EmailIcon size={40} round />
                                    </EmailShareButton>
                                </div>
                            </div>

                            {/* Copy Link Section */}
                            <div className='bg-muted p-3 flex flex-row items-center gap-2 rounded-md'>
                                <div className='grow overflow-hidden'>
                                    <div className='text-xs text-muted-foreground'>
                                        {content.type.charAt(0).toUpperCase() + content.type.slice(1)} link
                                    </div>
                                    <a href={content.url} className='text-sm truncate block hover:underline'>
                                        {content.url}
                                    </a>
                                </div>
                                <TooltipProvider delayDuration={0}>
                                    <Tooltip open={copiedLink}>
                                        <TooltipTrigger asChild>
                                            <Button className='shrink-0' size='icon' variant='outline' onClick={handleCopyLink}>
                                                <Copy className='h-4 w-4' />
                                            </Button>
                                        </TooltipTrigger>
                                        <TooltipContent>Copied to clipboard!</TooltipContent>
                                    </Tooltip>
                                </TooltipProvider>
                            </div>
                        </div>
                    </TabsContent>

                    <TabsContent value='qr' className='mt-4'>
                        <QRCodeDisplay url={content.url} title={content.title} />
                    </TabsContent>
                </Tabs>

                <DialogFooter>
                    <Button variant='outline' onClick={() => onOpenChange(false)}>
                        Close
                    </Button>
                </DialogFooter>
            </DialogContent>
        </Dialog>
    );
};
