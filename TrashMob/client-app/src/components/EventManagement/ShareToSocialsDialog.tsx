import { Dialog, DialogPlainContent, DialogFooter, DialogHeader, DialogTitle } from '@/components/ui/dialog';
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from '@/components/ui/tooltip';
import compact from 'lodash/compact';
import { Button } from '@/components/ui/button';
import { Copy, Clock, MapPin } from 'lucide-react';
import React from 'react';
import moment from 'moment';
import {
    FacebookShareButton,
    FacebookIcon,
    TwitterShareButton,
    TwitterIcon,
    LinkedinShareButton,
    LinkedinIcon,
    WhatsappShareButton,
    WhatsappIcon,
    EmailShareButton,
    EmailIcon,
} from 'react-share';

interface ShareToSocialDialogProps {
    eventToShare?: any;
    show: boolean;
    handleShow: (value: boolean) => void;
    modalTitle: string;
    eventLink?: string;
    message: string;
    emailSubject?: string;
}

export const ShareToSocialsDialog = (props: ShareToSocialDialogProps) => {
    const { handleShow } = props;
    const eventLink = props.eventLink ?? `${window.location.origin}/eventdetails/${props.eventToShare.id}`;

    const parseShareMessage = React.useCallback(
        (provider: string) => {
            const TrashMobTag = provider === 'twitter' ? '@TrashMobEco' : 'TrashMob.eco';
            return props.message.replace('{{TrashMob}}', TrashMobTag);
        },
        [props.message],
    );

    /* Copy link */
    const [copiedLink, setCopied] = React.useState(false);
    const handleCopyLink = React.useCallback(() => {
        if (copiedLink) {
            return;
        }

        navigator.clipboard.writeText(eventLink);
        setCopied(true);
        setTimeout(() => {
            setCopied(false);
        }, 2000);
    }, [copiedLink, eventLink, setCopied]);

    return (
        <Dialog open={props.show}>
            <DialogPlainContent className='sm:max-w-[500px]'>
                <DialogHeader>
                    <DialogTitle>{props.modalTitle ?? 'Share Event'}</DialogTitle>
                </DialogHeader>
                <div className='grid gap-4 py-4'>
                    {props.eventToShare ? (
                        <div className='flex flex-col !mb-4'>
                            <h6>{props.eventToShare.name}</h6>
                            <div className='flex flex-row align-items-center mb-2'>
                                <MapPin className='!mr-2' />
                                {compact([
                                    props.eventToShare.streetAddress,
                                    props.eventToShare.city,
                                    props.eventToShare.region,
                                ]).join(', ')}
                            </div>

                            <div className='flex flex-row align-items-center'>
                                <Clock className='mr-2' />
                                {moment(props.eventToShare).format('L [at] LT')}
                            </div>
                        </div>
                    ) : null}
                    <hr />
                    <div>
                        Share a link
                        <div className='flex flex-row !gap-2 !my-4'>
                            <FacebookShareButton className='socials-modal-icon' url={eventLink} hashtag='#litter'>
                                <FacebookIcon size={32} round />
                            </FacebookShareButton>
                            <TwitterShareButton
                                className='socials-modal-icon'
                                title={parseShareMessage('twitter')}
                                hashtags={['litter']}
                                url={eventLink}
                                via='TrashMobEco'
                            >
                                <TwitterIcon size={32} round />
                            </TwitterShareButton>
                            <LinkedinShareButton className='socials-modal-icon' url={eventLink}>
                                <LinkedinIcon size={32} round />
                            </LinkedinShareButton>
                            <WhatsappShareButton
                                className='socials-modal-icon'
                                url={eventLink}
                                title={parseShareMessage('whatsapp')}
                            >
                                <WhatsappIcon size={32} round />
                            </WhatsappShareButton>
                            <EmailShareButton
                                className='socials-modal-icon'
                                url={eventLink}
                                subject={
                                    props.emailSubject ??
                                    `Join me at this TrashMob.eco event in ${props.eventToShare.city}!`
                                }
                                body={parseShareMessage('email')}
                            >
                                <EmailIcon size={32} round />
                            </EmailShareButton>
                        </div>
                        <div className='bg-[#f0f0f1] p-2 flex flex-row items-center gap-2 max-w-[450px]'>
                            <div className='grow overflow-hidden'>
                                <div className='text-xs text-muted'>Event link</div>
                                <a href={eventLink} className='text-sm truncate block'>
                                    {eventLink}
                                </a>
                            </div>
                            <TooltipProvider delayDuration={0}>
                                <Tooltip open={copiedLink}>
                                    <TooltipTrigger asChild>
                                        <Button className='basis-9 shrink-0' size='icon' onClick={handleCopyLink}>
                                            <Copy />
                                        </Button>
                                    </TooltipTrigger>
                                    <TooltipContent>Copied to clipboard!</TooltipContent>
                                </Tooltip>
                            </TooltipProvider>
                        </div>
                    </div>
                </div>
                <DialogFooter>
                    <Button onClick={() => handleShow(false)}>
                        Close
                    </Button>
                </DialogFooter>
            </DialogPlainContent>
        </Dialog>
    );
};
