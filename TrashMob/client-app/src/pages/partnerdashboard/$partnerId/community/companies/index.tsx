import { useParams, Link, useNavigate } from 'react-router';
import { useQuery } from '@tanstack/react-query';
import { AxiosResponse } from 'axios';
import { Loader2, Plus, Pencil, Briefcase } from 'lucide-react';

import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle, CardDescription } from '@/components/ui/card';
import { Badge } from '@/components/ui/badge';
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table';
import ProfessionalCompanyData from '@/components/Models/ProfessionalCompanyData';
import { GetProfessionalCompanies } from '@/services/professional-companies';

export const PartnerCommunityCompanies = () => {
    const navigate = useNavigate();
    const { partnerId } = useParams<{ partnerId: string }>() as { partnerId: string };

    const { data: companies, isLoading } = useQuery<
        AxiosResponse<ProfessionalCompanyData[]>,
        unknown,
        ProfessionalCompanyData[]
    >({
        queryKey: GetProfessionalCompanies({ partnerId }).key,
        queryFn: GetProfessionalCompanies({ partnerId }).service,
        select: (res) => res.data,
        enabled: !!partnerId,
    });

    if (isLoading) {
        return (
            <div className='py-8 text-center'>
                <Loader2 className='h-8 w-8 animate-spin mx-auto' />
            </div>
        );
    }

    return (
        <div className='py-8'>
            <Card>
                <CardHeader className='flex flex-row items-center justify-between'>
                    <div>
                        <CardTitle className='flex items-center gap-2'>
                            <Briefcase className='h-5 w-5' />
                            Professional Companies
                        </CardTitle>
                        <CardDescription>
                            Manage companies that perform professional cleanup services for sponsored adoptions.
                        </CardDescription>
                    </div>
                    <Button
                        onClick={() => navigate(`/partnerdashboard/${partnerId}/community/companies/create`)}
                    >
                        <Plus className='h-4 w-4 mr-2' />
                        Add Company
                    </Button>
                </CardHeader>
                <CardContent>
                    {companies && companies.length > 0 ? (
                        <Table>
                            <TableHeader>
                                <TableRow>
                                    <TableHead>Name</TableHead>
                                    <TableHead>Email</TableHead>
                                    <TableHead>Phone</TableHead>
                                    <TableHead>Status</TableHead>
                                    <TableHead className='text-right'>Actions</TableHead>
                                </TableRow>
                            </TableHeader>
                            <TableBody>
                                {companies.map((company) => (
                                    <TableRow key={company.id}>
                                        <TableCell className='font-medium'>{company.name}</TableCell>
                                        <TableCell>{company.contactEmail}</TableCell>
                                        <TableCell>{company.contactPhone}</TableCell>
                                        <TableCell>
                                            <Badge
                                                className={
                                                    company.isActive
                                                        ? 'bg-green-100 text-green-800 dark:bg-green-900 dark:text-green-200'
                                                        : 'bg-red-100 text-red-800 dark:bg-red-900 dark:text-red-200'
                                                }
                                            >
                                                {company.isActive ? 'Active' : 'Inactive'}
                                            </Badge>
                                        </TableCell>
                                        <TableCell className='text-right'>
                                            <Button variant='outline' size='sm' asChild>
                                                <Link
                                                    to={`/partnerdashboard/${partnerId}/community/companies/${company.id}/edit`}
                                                >
                                                    <Pencil className='h-4 w-4' />
                                                </Link>
                                            </Button>
                                        </TableCell>
                                    </TableRow>
                                ))}
                            </TableBody>
                        </Table>
                    ) : (
                        <div className='text-center py-12'>
                            <Briefcase className='h-12 w-12 mx-auto text-muted-foreground mb-4' />
                            <h3 className='text-lg font-medium mb-2'>No professional companies yet</h3>
                            <p className='text-muted-foreground mb-4'>
                                Add your first professional cleanup company to start managing sponsored
                                adoption services.
                            </p>
                            <Button
                                onClick={() =>
                                    navigate(`/partnerdashboard/${partnerId}/community/companies/create`)
                                }
                            >
                                <Plus className='h-4 w-4 mr-2' />
                                Add First Company
                            </Button>
                        </div>
                    )}
                </CardContent>
            </Card>
        </div>
    );
};

export default PartnerCommunityCompanies;
