import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ReactNode } from 'react';

interface SidebarLayoutProps {
    title: string;
    description: string;
    children: ReactNode;
}

export const SidebarLayout = (props: SidebarLayoutProps) => {
    const { title, description, children } = props;
    return (
        <div className='container mx-auto'>
            <div className='grid grid-cols-12 !gap-8 my-8'>
                <div className='col-span-12 lg:col-span-4'>
                    <Card>
                        <CardHeader>
                            <CardTitle className='font-semibold tracking-tight text-primary text-2xl'>
                                {title}
                            </CardTitle>
                        </CardHeader>
                        <CardContent>{description}</CardContent>
                    </Card>
                </div>
                <div className='col-span-12 lg:col-span-8'>
                    <Card>
                        <CardContent className='pt-6'>{children}</CardContent>
                    </Card>
                </div>
            </div>
        </div>
    );
};
