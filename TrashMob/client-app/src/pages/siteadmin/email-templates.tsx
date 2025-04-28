import orderBy from 'lodash/orderBy';
import { Card, CardHeader, CardTitle, CardContent } from '@/components/ui/card';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import { GetAdminEmailTemplates } from '@/services/admin';
import { useQuery } from '@tanstack/react-query';

export const SiteAdminEmailTemplates = () => {
    const { data: emailTemplates } = useQuery({
        queryKey: GetAdminEmailTemplates().key,
        queryFn: GetAdminEmailTemplates().service,
        select: (res) => orderBy(res.data, ['name'], ['asc']),
    });

    return (
        <Card>
            <CardHeader>
                <CardTitle>Email Templates</CardTitle>
            </CardHeader>
            <CardContent>
                <Table>
                    <TableHeader>
                        <TableRow>
                            <TableHead>Name</TableHead>
                            <TableHead>Content</TableHead>
                        </TableRow>
                    </TableHeader>
                    <TableBody>
                        {(emailTemplates || []).map((template) => (
                            <TableRow key={template.name}>
                                <TableCell>{template.name}</TableCell>
                                <TableCell>
                                    <div
                                        dangerouslySetInnerHTML={{
                                            __html: template.content,
                                        }}
                                    />
                                </TableCell>
                            </TableRow>
                        ))}
                    </TableBody>
                </Table>
            </CardContent>
        </Card>
    );
};
