import { Form } from '@/components/ui/form';
import { ReactNode } from 'react';

interface FormWrapperProps {
    form: any;
    onSubmit: (data: any) => void;
    children: ReactNode;
}

function FormWrapper({ form, onSubmit, children }: FormWrapperProps) {
    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='w-2/3 space-y-6'>
                {children}
            </form>
        </Form>
    );
}

export default FormWrapper;
