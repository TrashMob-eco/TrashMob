import { ReactNode, Children, isValidElement, cloneElement } from 'react';

import { Form } from '@/components/ui/form';
import { Button } from '@/components/ui/button';

interface FormWrapperProps {
    form: any;
    onSubmit: (data: any) => void;
    onCancel?: () => void;
    children: ReactNode;
}

function FormWrapper({ form, onSubmit, onCancel, children }: FormWrapperProps) {
    return (
        <Form {...form}>
            <form onSubmit={form.handleSubmit(onSubmit)} className='w-2/3 space-y-6'>
                {Children.map(children, (child) =>
                    isValidElement(child) ? cloneElement(child, { control: form.control }) : child,
                )}
                {onCancel && <Button>Discard</Button>}
                <Button type='submit'>Save</Button>
            </form>
        </Form>
    );
}

export default FormWrapper;
