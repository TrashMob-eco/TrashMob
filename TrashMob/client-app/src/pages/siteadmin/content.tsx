import { ExternalLink } from 'lucide-react';
import { useQuery } from '@tanstack/react-query';
import { Card, CardHeader, CardTitle, CardContent, CardDescription } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { GetCmsAdminUrl } from '@/services/cms';

export const SiteAdminContent = () => {
    const { data: adminUrlData, isLoading, isError } = useQuery({
        queryKey: GetCmsAdminUrl().key,
        queryFn: GetCmsAdminUrl().service,
    });

    const editableContent = [
        {
            name: 'Hero Section',
            description: 'Main banner at the top of the home page including tagline, buttons, and app store links.',
        },
        {
            name: 'What is TrashMob',
            description: 'Introduction section with heading, description, video, and call-to-action buttons.',
        },
        {
            name: 'Getting Started',
            description: 'Section showing requirements for joining a cleanup event.',
        },
    ];

    return (
        <Card>
            <CardHeader>
                <CardTitle>Content Management</CardTitle>
                <CardDescription>
                    Manage website content using the Strapi CMS. Changes made in the CMS will be reflected on the
                    website.
                </CardDescription>
            </CardHeader>
            <CardContent className='space-y-6'>
                <div>
                    <h3 className='text-lg font-semibold mb-2'>Editable Content Areas</h3>
                    <p className='text-sm text-muted-foreground mb-4'>
                        The following sections of the home page can be edited through the CMS:
                    </p>
                    <ul className='list-disc list-inside space-y-2'>
                        {editableContent.map((content) => (
                            <li key={content.name}>
                                <span className='font-medium'>{content.name}</span>
                                <span className='text-muted-foreground'> - {content.description}</span>
                            </li>
                        ))}
                    </ul>
                </div>

                <div className='pt-4 border-t'>
                    {isError ? (
                        <p className='text-sm text-muted-foreground'>
                            CMS is not configured. Please contact the development team to set up the CMS.
                        </p>
                    ) : (
                        <Button asChild disabled={isLoading || !adminUrlData?.adminUrl}>
                            <a
                                href={adminUrlData?.adminUrl}
                                target='_blank'
                                rel='noopener noreferrer'
                                className='inline-flex items-center gap-2'
                            >
                                Open CMS Admin Panel
                                <ExternalLink className='h-4 w-4' />
                            </a>
                        </Button>
                    )}
                </div>
            </CardContent>
        </Card>
    );
};
