import { Link, useParams } from 'react-router';
import orderBy from 'lodash/orderBy';
import { useQuery } from '@tanstack/react-query';
import { ArrowLeft } from 'lucide-react';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { GetAdminEmailTemplates } from '@/services/admin';

export const SiteAdminEmailTemplateDetail = () => {
    const { templateName } = useParams<{ templateName: string }>() as { templateName: string };
    const decodedName = decodeURIComponent(templateName);

    const { data: templates } = useQuery({
        queryKey: GetAdminEmailTemplates().key,
        queryFn: GetAdminEmailTemplates().service,
        select: (res) => orderBy(res.data, ['name'], ['asc']),
    });

    const template = templates?.find((t) => t.name === decodedName);

    if (!templates) {
        return <p>Loading...</p>;
    }

    if (!template) {
        return (
            <Card>
                <CardContent className='py-8 text-center'>
                    <p className='mb-4'>Template not found: {decodedName}</p>
                    <Button asChild>
                        <Link to='/siteadmin/email-templates'>
                            <ArrowLeft className='h-4 w-4 mr-2' /> Back to Templates
                        </Link>
                    </Button>
                </CardContent>
            </Card>
        );
    }

    return (
        <div className='space-y-6'>
            <Card>
                <CardHeader className='flex flex-row items-center gap-3'>
                    <Button variant='ghost' size='icon' asChild>
                        <Link to='/siteadmin/email-templates'>
                            <ArrowLeft className='h-4 w-4' />
                        </Link>
                    </Button>
                    <CardTitle>{template.name}</CardTitle>
                </CardHeader>
                <CardContent>
                    <div className='border rounded-md p-4' dangerouslySetInnerHTML={{ __html: template.content }} />
                </CardContent>
            </Card>
        </div>
    );
};
