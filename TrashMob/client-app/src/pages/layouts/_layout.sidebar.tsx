import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { ReactNode } from 'react';

interface SidebarLayoutProps {
    readonly title: string;
    readonly description: string;
    readonly children: ReactNode;
    readonly useDefaultCard?: boolean;
}

export const SidebarLayout = (props: SidebarLayoutProps) => {
    const { title, description, children, useDefaultCard = true } = props;
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
                <div className='col-span-12 lg:col-span-8 space-y-8'>
                    {useDefaultCard ? (
                        <Card>
                            <CardContent className='pt-6'>{children}</CardContent>
                        </Card>
                    ) : (
                        children
                    )}
                </div>
            </div>
        </div>
    );
};
