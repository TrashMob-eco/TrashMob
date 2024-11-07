import { useFormContext } from 'react-hook-form';

import { Checkbox } from '@/components/ui/base/checkbox';
import { FormControl, FormDescription, FormField, FormItem, FormLabel } from '@/components/ui/base/form';

type FormCheckboxProps = {
    name: string;
    label?: string;
    description?: string;
};

function FormCheckbox({ name, description, label }: FormCheckboxProps) {
    const methods = useFormContext();
    return (
        <FormField
            control={methods.control}
            name={name}
            render={({ field }) => (
                <FormItem className='flex flex-row items-start space-x-3 space-y-0 rounded-md border p-4'>
                    <FormControl>
                        <Checkbox checked={field.value} onCheckedChange={field.onChange} />
                    </FormControl>
                    <div className='space-y-1 leading-none'>
                        {label && <FormLabel>{label}</FormLabel>}
                        {description && <FormDescription>{description}</FormDescription>}
                    </div>
                </FormItem>
            )}
        />
    );
}

export default FormCheckbox;
