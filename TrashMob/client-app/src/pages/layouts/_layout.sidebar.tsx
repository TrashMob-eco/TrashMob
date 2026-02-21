import { Card, CardContent } from '@/components/ui/card';
import { PageHelp } from '@/components/ui/custom/PageHelp';
import { ReactNode } from 'react';

interface SidebarLayoutProps {
    title: string;
    description: ReactNode;
    children: ReactNode;
    useDefaultCard?: boolean;
}

export const SidebarLayout = (props: SidebarLayoutProps) => {
    const { title, description, children, useDefaultCard = true } = props;
    return (
        <div className='container mx-auto my-8 space-y-4'>
            <div className='flex items-center justify-between'>
                <h2 className='font-semibold tracking-tight text-primary text-2xl'>{title}</h2>
                <PageHelp>{description}</PageHelp>
            </div>
            {useDefaultCard ? (
                <Card>
                    <CardContent className='pt-6'>{children}</CardContent>
                </Card>
            ) : (
                children
            )}
        </div>
    );
};
