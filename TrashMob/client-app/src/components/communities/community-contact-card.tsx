import { Mail, Phone, MapPin, Globe } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';

interface CommunityContactCardProps {
    contactEmail?: string;
    contactPhone?: string;
    physicalAddress?: string;
    website?: string;
}

export const CommunityContactCard = ({
    contactEmail,
    contactPhone,
    physicalAddress,
    website,
}: CommunityContactCardProps) => {
    const hasContactInfo = contactEmail || contactPhone || physicalAddress || website;

    if (!hasContactInfo) {
        return null;
    }

    return (
        <Card>
            <CardHeader className='pb-3'>
                <CardTitle className='text-lg'>Contact</CardTitle>
            </CardHeader>
            <CardContent className='space-y-3'>
                {contactEmail ? <a
                        href={`mailto:${contactEmail}`}
                        className='flex items-center gap-2 text-sm text-muted-foreground hover:text-primary transition-colors'
                    >
                        <Mail className='h-4 w-4 shrink-0' />
                        <span className='truncate'>{contactEmail}</span>
                    </a> : null}
                {contactPhone ? <a
                        href={`tel:${contactPhone}`}
                        className='flex items-center gap-2 text-sm text-muted-foreground hover:text-primary transition-colors'
                    >
                        <Phone className='h-4 w-4 shrink-0' />
                        <span>{contactPhone}</span>
                    </a> : null}
                {physicalAddress ? <div className='flex items-start gap-2 text-sm text-muted-foreground'>
                        <MapPin className='h-4 w-4 shrink-0 mt-0.5' />
                        <span>{physicalAddress}</span>
                    </div> : null}
                {website ? <a
                        href={website.startsWith('http') ? website : `https://${website}`}
                        target='_blank'
                        rel='noopener noreferrer'
                        className='flex items-center gap-2 text-sm text-muted-foreground hover:text-primary transition-colors'
                    >
                        <Globe className='h-4 w-4 shrink-0' />
                        <span className='truncate'>{website.replace(/^https?:\/\//, '')}</span>
                    </a> : null}
            </CardContent>
        </Card>
    );
};
